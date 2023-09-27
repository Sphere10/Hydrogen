// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SQLite;

namespace Hydrogen.Data;

public class SqliteDAC : DACBase {

	public SqliteDAC(string connectionString, ILogger logger = null)
		: base(connectionString, logger) {
	}


	public override DBMSType DBMSType {
		get { return DBMSType.Sqlite; }
	}

	public override IDbConnection CreateConnection() {
		return new SQLiteConnection() { ConnectionString = ConnectionString };
	}

	protected override IEnumerable<string> GetBatches(ISQLBuilder sqlBuilder) {
		return
			sqlBuilder
				.Statements
				.Select(statement => statement.SQL);
	}

	public override ISQLBuilder CreateSQLBuilder() {
		return new SqliteSQLBuilder();
	}

	public override void EnlistInSystemTransaction(IDbConnection connection, Transaction transaction) {
		var mssqlConnection = connection as SQLiteConnection;
		if (mssqlConnection == null)
			throw new ArgumentException("Not an SqlConnection", "connection");
		mssqlConnection.EnlistTransaction(transaction);
	}


	public override void BulkInsert(DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout) {
		throw new NotImplementedException();
	}

	protected override DataTable GetDenormalizedTableDescriptions() {
		DataTable result = new DataTable();
//TableName		string	NOT NULL,
//ColumnName		string	NOT NULL,
//Position		INT		NOT NULL,
//Type			STRING	NOT NULL,
//Length			INT		NOT NULL,
//Precision		INT		NOT NULL,
//Scale			INT		NOT NULL,
//IsNullable		BIT		NOT NULL,
//UniqueName		STRING	NULLABLE,
//PrimaryKeyName	STRING	NULLABLE,
//IsAutoIncrement	BIT		NOT NULLABLE,
//Sequence		STRING	NULLABLE,
//ForeignKeyName	STRING	NULLABLE,
//ReferenceTableName	STRING NULLABLE,
//ReferenceColumnName	STRING NULLABLE,
//CascadeUpdate	BIT NOT NULLABLE,
//CascadeDelete	BIT NOT NULLABLE
		result.Columns.Add("TableName", typeof(string));
		result.Columns.Add("ColumnName", typeof(string));
		result.Columns.Add("Position", typeof(int));
		result.Columns.Add("Type", typeof(string));
		result.Columns.Add("Length", typeof(int));
		result.Columns.Add("Precision", typeof(int));
		result.Columns.Add("Scale", typeof(int));
		result.Columns.Add("IsNullable", typeof(bool));
		result.Columns.Add("UniqueName", typeof(string));
		result.Columns.Add("PrimaryKeyName", typeof(string));
		result.Columns.Add("IsAutoIncrement", typeof(bool));
		result.Columns.Add("Sequence", typeof(string));
		result.Columns.Add("ForeignKeyName", typeof(string));
		result.Columns.Add("ReferenceTableName", typeof(string));
		result.Columns.Add("ReferenceColumnName", typeof(string));
		result.Columns.Add("CascadeUpdate", typeof(bool));
		result.Columns.Add("CascadeDelete", typeof(bool));
		using (var scope = this.BeginScope()) {
			var sqliteMasterTables = SelectSqliteMaster().Where(x => x.type == "table").ToArray();
			var tables = sqliteMasterTables.Where(x => x.name != "sqlite_sequence").ToArray();
			var tableColumns = tables.ToDictionary(t => t.name, t => PragmaTableInfo(t.name).OrderBy(ti => ti.cid).ToArray());
			var tablePKs = tableColumns.Where(kvp => kvp.Value.Any(c => c.pk)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Where(c => c.pk));
			var sequences =
				sqliteMasterTables.Any(x => x.name == "sqlite_sequence") ? SelectSqliteSequence().ToDictionary(s => s.name) : new Dictionary<string, sqlite_sequence_result>();
			foreach (var table in tables) {
				var columns = tableColumns[table.name];
				var foreignKeys = PragmaForeignKeyList(table.name).OrderBy(fk => fk.seq);
				var indexList = PragmaIndexList(table.name).OrderBy(ix => ix.seq).ToArray();
				var indexInfo = indexList.ToDictionary(ix => ix.name, ix => PragmaIndexInfo(ix.name));

				var primaryKeyByCID = columns.Where(c => c.pk).ToDictionary(c => c.cid, c => "PK " + table.name);

				var uniqueContraintsByCID = (
					from ix in indexList.Where(ix => ix.unique)
					from col in indexInfo[ix.name]
					where !primaryKeyByCID.ContainsKey(col.cid) // explicit index for pk's are not considered unique keys
					select new {
						Key = col.cid,
						Val = ix.name
					}).ToDictionary(x => x.Key, x => x.Val);


				var foreignKeysByColName = foreignKeys.ToDictionary(fk => fk.from);

				foreach (var column in columns) {
					var row = result.NewRow();
					row["TableName"] = table.name;
					row["ColumnName"] = column.name;
					row["Position"] = column.cid + 1;
					row["Type"] = column.type;
					row["Length"] = DetermineColumnLength(column.type);
					row["Precision"] = 10;
					row["Scale"] = 5;
					row["IsNullable"] = !column.notnull;
					row["UniqueName"] = uniqueContraintsByCID.ContainsKey(column.cid) ? (object)uniqueContraintsByCID[column.cid] : DBNull.Value;
					row["PrimaryKeyName"] = primaryKeyByCID.ContainsKey(column.cid) ? (object)primaryKeyByCID[column.cid] : DBNull.Value;
					row["Sequence"] = null; // no manual sequences in sqlite
#warning Detecting auto-increments via SQL create table statements is undesirable, no better known approach known (sequences created after first record inserted)
					row["IsAutoIncrement"] = primaryKeyByCID.ContainsKey(column.cid) && table.sql.RemoveNonAlphaNumeric().ToUpperInvariant().Contains("PRIMARYKEYAUTOINCREMENT"); /* sequences.ContainsKey(table.name); */
					if (foreignKeysByColName.ContainsKey(column.name)) {
						var fk = foreignKeysByColName[column.name];
						row["ForeignKeyName"] = "FK " + fk.id + " " + table.name;
						row["ReferenceTableName"] = fk.table;
						if (!string.IsNullOrEmpty(fk.to)) {
							row["ReferenceColumnName"] = fk.to;
						} else {
							// blank foreign key column denotes a single primary key column
							if (tablePKs.ContainsKey(fk.table) && tablePKs[fk.table].Count() == 1) {
								row["ReferenceColumnName"] = tablePKs[fk.table].Single().name;
							} else {
								throw new Exception(string.Format("Unable to determine reference column for foreign key from table {0}, column {1} to table {2}", table.name, column.name, fk.table));
							}
						}
						row["CascadeUpdate"] = fk.on_update != "NO ACTION";
						row["CascadeDelete"] = fk.on_delete != "NO ACTION";
					} else {
						row["ForeignKeyName"] = DBNull.Value;
						row["ReferenceTableName"] = DBNull.Value;
						row["ReferenceColumnName"] = DBNull.Value;
						row["CascadeUpdate"] = DBNull.Value;
						row["CascadeDelete"] = DBNull.Value;
					}
					result.Rows.Add(row);
				}
			}
		}
		return result;
	}

	private int DetermineColumnLength(string type) {
		if (String.IsNullOrWhiteSpace(type))
			return 0;
		var typeUpper = type.ToUpperInvariant().Trim();
		if (type.StartsWith("VAR") || type.StartsWith("TEXT"))
			return int.MaxValue;

		return 0;
	}

	protected override DataTable GetDenormalizedTriggerDescriptions() {
		return this.ExecuteQuery(@"select name from sqlite_master where type = 'trigger'");
	}

	public IEnumerable<sqlite_master_result> SelectSqliteMaster() {
		return this.Select(
				"sqlite_master"
			)
			.Rows
			.Cast<DataRow>()
			.Select(r =>
				new sqlite_master_result {
					type = r.Get<string>("type"),
					name = r.Get<string>("name"),
					tbl_name = r.Get<string>("tbl_name"),
					rootpage = r.Get<int>("rootpage"),
					sql = r.Get<string>("sql")
				}
			);
	}

	public IEnumerable<sqlite_sequence_result> SelectSqliteSequence() {
		var dt = this.Select(
			"sqlite_sequence"
		);

		var cols = dt.Columns;
		return dt
			.Rows
			.Cast<DataRow>()
			.Select(r =>
				new sqlite_sequence_result {
					name = r.Get<string>("name"),
					seq = r.Get<long?>("seq")
				}
			);
	}

	public IEnumerable<table_info_result> PragmaTableInfo(string tableName) {
		return
			this.ExecuteQuery("PRAGMA table_info([" + tableName + "]);")
				.Rows
				.Cast<DataRow>()
				.Select(r =>
					new table_info_result {
						cid = r.Get<int>("cid"),
						name = r.Get<string>("name"),
						type = r.Get<string>("type"),
						notnull = r.Get<bool>("notnull"),
						dflt_value = r.Get<string>("dflt_value"),
						pk = r.Get<bool>("pk")
					}
				);

	}

	public IEnumerable<index_list_result> PragmaIndexList(string tableName) {
		return
			this.ExecuteQuery("PRAGMA index_list([" + tableName + "]);")
				.Rows
				.Cast<DataRow>()
				.Select(r =>
					new index_list_result {
						seq = r.Get<int>("seq"),
						name = r.Get<string>("name"),
						unique = r.Get<bool>("unique")
					}
				);
	}

	public IEnumerable<index_info_result> PragmaIndexInfo(string indexName) {
		return
			this.ExecuteQuery("PRAGMA index_info([" + indexName + "]);")
				.Rows
				.Cast<DataRow>()
				.Select(r =>
					new index_info_result {
						seqno = r.Get<int>("seqno"),
						cid = r.Get<int>("cid"),
						name = r.Get<string>("name")
					}
				);
	}

	public IEnumerable<foreign_key_list> PragmaForeignKeyList(string tableName) {
		return
			this.ExecuteQuery("PRAGMA foreign_key_list([" + tableName + "]);")
				.Rows
				.Cast<DataRow>()
				.Select(r =>
					new foreign_key_list {
						id = r.Get<int>("id"),
						seq = r.Get<int>("seq"),
						table = r.Get<string>("table"),
						from = r.Get<string>("from"),
						to = r.Get<string>("to"),
						on_update = r.Get<string>("on_update"),
						on_delete = r.Get<string>("on_delete"),
						match = r.Get<string>("match"),
					}
				);
	}


	public class sqlite_master_result {
		public string type;
		public string name;
		public string tbl_name;
		public int rootpage;
		public string sql;

	}


	public class sqlite_sequence_result {
		public string name;
		public long? seq;
	}


	public class table_info_result {
		public int cid;
		public string name;
		public string type;
		public bool notnull;
		public string dflt_value;
		public bool pk;
	}


	public class index_list_result {
		public int seq;
		public string name;
		public bool unique;
	}


	public class index_info_result {
		public int seqno;
		public int cid;
		public string name;
	}


	public class foreign_key_list {
		public int id;
		public int seq;
		public string table;
		public string from;
		public string to;
		public string on_update;
		public string on_delete;
		public string match;

		// on_update & on_delete =
		// NO ACTION
		// RESTRICT 
		// CASCADE
		// SET NULL
		// SET DEFAULT
	}


}
