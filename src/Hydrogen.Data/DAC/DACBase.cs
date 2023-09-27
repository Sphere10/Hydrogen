// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;

namespace Hydrogen.Data;

public abstract class DACBase : IDAC {
	private ArtificialKeys _artificialKeys;

	public event EventHandlerEx<IDAC, string> Executing;
	public event EventHandlerEx<IDAC, string> Executed;

	protected DACBase(string connectionString, ILogger logger = null) {
		ConnectionString = connectionString;
		Log = logger ?? new NoOpLogger();
		_artificialKeys = null;
		DefaultIsolationLevel = IsolationLevel.ReadCommitted;
		UseScopeOsmosis = true;
		InstanceID = Guid.NewGuid();
	}

	#region Properties

	public Guid InstanceID { get; set; }

	public bool UseScopeOsmosis { get; set; }

	public IsolationLevel DefaultIsolationLevel { get; set; }

	public abstract DBMSType DBMSType { get; }

	public string ConnectionString { get; private set; }

	public ArtificialKeys ArtificialKeys {
		get { return _artificialKeys; }
		set {
			_artificialKeys = value;
			SchemaCache.InvalidateSchema(ConnectionString);
		}
	}

	public ILogger Log { get; set; }

	#endregion

	#region Methods

	public abstract IDbConnection CreateConnection();

	public abstract ISQLBuilder CreateSQLBuilder();

	public abstract void EnlistInSystemTransaction(IDbConnection connection, System.Transactions.Transaction transaction);

	public virtual int ExecuteNonQuery(string query) {
		Log.Debug(Environment.NewLine + query);
		try {
			using (var scope = this.BeginScope(true))
			using (var command = scope.Connection.CreateCommand()) {
				command.CommandTimeout = 0;
				command.CommandText = query;
				command.CommandType = CommandType.Text;
				if (scope.Transaction != null)
					command.Transaction = ((RestrictedTransaction)scope.Transaction).DangerousInternalTransaction;
				var updates = command.ExecuteNonQuery();
				return updates;
			}
		} catch (Exception error) {
			Log.Error(error.ToDiagnosticString());
			throw;
		}
	}

	public virtual object ExecuteScalar(string query) {
		Log.Debug(Environment.NewLine + query);

		using (var scope = this.BeginScope(true))
		using (var command = scope.Connection.CreateCommand()) {
			command.CommandText = query;
			command.CommandType = CommandType.Text;
			if (scope.Transaction != null)
				command.Transaction = ((RestrictedTransaction)scope.Transaction).DangerousInternalTransaction;
			return command.ExecuteScalar();
		}
	}

	public virtual DataTable[] ExecuteBatch(ISQLBuilder sqlBuilder) {
		try {
			var results = new List<DataTable>();

			using (var scope = this.BeginScope(true)) {
				using (var command = scope.Connection.CreateCommand()) {
					command.CommandTimeout = 0;
					command.CommandType = CommandType.Text;
					//command.Transaction =;
					if (scope.Transaction != null)
						command.Transaction = ((RestrictedTransaction)scope.Transaction).DangerousInternalTransaction;
					// var fileID = Path.Combine("c:\\temp\\", this.ConnectionString.ToPathSafe() + ".txt");
					foreach (var batch in GetBatches(sqlBuilder)) {
						// File.AppendAllText(fileID, batch);
						command.CommandText = batch;
						Log.Debug(Environment.NewLine + batch);
						//command.ExecuteNonQuery();
#warning this code needs to be studied for optimization

						using (var reader = command.ExecuteReader(CommandBehavior.Default)) {
							results.AddRange(reader.ToDataTables());
						}
					}
				}
			}
			return results.ToArray();
		} catch (Exception error) {
			Log.Error(error.ToDiagnosticString());
			throw;
		}
	}

	public virtual IDataReader ExecuteReader(string query) {
		Log.Debug(Environment.NewLine + query);

		using (var scope = this.BeginScope(true)) {
			try {
				var command = scope.Connection.CreateCommand();
				if (scope.Transaction != null)
					command.Transaction = ((RestrictedTransaction)scope.Transaction).DangerousInternalTransaction;
				command.CommandTimeout = 0;
				command.CommandText = query;
				command.CommandType = CommandType.Text;
				var reader = command.ExecuteReader(CommandBehavior.Default);
				return new AutoClosingDataReader(reader, command);
			} catch (Exception error) {
				Log.Error(error.ToDiagnosticString());
				throw;
			}
		}

	}

	public virtual long Insert(string tableName, IEnumerable<ColumnValue> values) {
		var sqlBuilder = CreateSQLBuilder();
		sqlBuilder.Insert(tableName, values);
		return ExecuteNonQuery(sqlBuilder.ToString());
	}

	public virtual long Update(string tableName, IEnumerable<ColumnValue> setValues, IEnumerable<ColumnValue> whereValues) {
		var sqlBuilder = CreateSQLBuilder();
		sqlBuilder.Update(tableName, setValues, matchColumns: whereValues);
		return ExecuteNonQuery(sqlBuilder.ToString());
	}

	public virtual long Delete(string tableName, IEnumerable<ColumnValue> matchColumns) {
		var sqlBuilder = CreateSQLBuilder();
		sqlBuilder.Delete(tableName, matchColumns);
		return ExecuteNonQuery(sqlBuilder.ToString());
	}

	public abstract void BulkInsert(DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout);

	public DBSchema GetSchema() {
		using (var scope = this.BeginScope()) {
			var schema =
				new DBSchema {
					DatabaseID = this.ConnectionString,
					Tables = NormalizeTables(GetDenormalizedTableDescriptions()),
					Triggers = NormalizeTriggers(GetDenormalizedTriggerDescriptions()),
					SQL = string.Empty
				};
			if (ArtificialKeys != null) {
				ArtificialKeys.ApplyToSchema(schema);
			}
			schema.FinishedBuilding();
			return schema;
		}
	}

	protected virtual void OnExecuting(string sql) {
	}

	protected virtual void OnExecuted(string sql) {
	}

	#endregion

	#region Auxilliary

	protected void NotifyExecuting(string sql) {
		OnExecuting(sql);
		if (Executing != null)
			Executing(this, sql);
	}

	protected void NotifyExecuted(string sql) {
		OnExecuted(sql);
		if (Executed != null)
			Executed(this, sql);
	}


	protected abstract DataTable GetDenormalizedTableDescriptions();

	protected abstract DataTable GetDenormalizedTriggerDescriptions();

	protected virtual IEnumerable<string> GetBatches(ISQLBuilder sqlBuilder) {
		return new[] { sqlBuilder.ToString() };
	}

	private DBTableSchema[] NormalizeTables(DataTable denormalizedTableDescriptions) {

		var sqlBuilder = CreateSQLBuilder();
		return
			(from row in denormalizedTableDescriptions.Rows.Cast<DataRow>()
			 group row by row.Get<string>("TableName").Trim()
			 into tableResults
			 select new DBTableSchema {
				 Name = tableResults.Key,
				 Columns = (
					 from groupedRow in tableResults
					 orderby groupedRow.Get<int>("Position") ascending
					 let dataType = groupedRow.Get<string>("Type").Trim()
					 let dataTypeLen = groupedRow.Get<int?>("Length") ?? 0
					 select new DBColumnSchema() {
						 Name = groupedRow.Get<string>("ColumnName").Trim(),
						 Position = groupedRow.Get<int>("Position"),
						 DataType = dataType,
						 DataTypeLength = dataTypeLen,
						 Precision = groupedRow.Get<int?>("Precision") ?? 0,
						 Scale = groupedRow.Get<int?>("Scale") ?? 0,
						 IsAutoIncrement = groupedRow.Get<bool?>("IsAutoIncrement") ?? false,
						 Sequence = null,
						 IsForeignKey = groupedRow["ForeignKeyName"] != DBNull.Value,
						 IsNullable = groupedRow.Get<string>("IsNullable").Trim() == "1",
						 IsPrimaryKey = groupedRow["PrimaryKeyName"] != DBNull.Value,
						 IsUnique = groupedRow["UniqueName"] != DBNull.Value,
						 CascadesOnUpdate = groupedRow.Get<string>("CascadeUpdate").Trim() == "1",
						 CascadesOnDelete = groupedRow.Get<string>("CascadeDelete").Trim() == "1",
						 Type = sqlBuilder.ConvertSQLTypeToType(dataType, dataTypeLen),

					 }
				 ).ToArray(),

				 PrimaryKey =
					 tableResults.Any(groupedRow => groupedRow["PrimaryKeyName"] != DBNull.Value)
						 ? new DBPrimaryKeySchema {
							 Name = tableResults.First(row => row["PrimaryKeyName"] != DBNull.Value).Get<string>("PrimaryKeyName").Trim(),
							 ColumnNames = (
								 from groupedRow in tableResults
								 where groupedRow["PrimaryKeyName"] != DBNull.Value
								 orderby groupedRow.Get<int>("Position") ascending
								 select groupedRow.Get<string>("ColumnName").Trim()
							 ).ToArray(),
							 Sequence = null
						 }
						 : null,

				 ForeignKeys = (
					 from columnMetaData in tableResults
					 where columnMetaData["ForeignKeyName"] != DBNull.Value
					 group columnMetaData by columnMetaData.Get<string>("ForeignKeyName").Trim()
					 into foreignKeyGroups
					 select new DBForeignKeySchema {
						 Name = foreignKeyGroups.Key,
						 ForeignKeyTable = foreignKeyGroups.First().Get<string>("TableName").Trim(),
						 ForeignKeyColumns = (
							 from fkCol in foreignKeyGroups
							 orderby fkCol.Get<int>("Position") ascending
							 select fkCol.Get<string>("ColumnName").Trim()
						 ).ToArray(),
						 ReferenceTable = foreignKeyGroups.First().Get<string>("ReferenceTableName").Trim(),
						 ReferenceColumns = (
							 from fkCol in foreignKeyGroups
							 orderby fkCol.Get<int>("Position") ascending
							 select fkCol.Get<string>("ReferenceColumnName").Trim()
						 ).ToArray(),
						 CascadesOnUpdate = foreignKeyGroups.First().Get<string>("CascadeUpdate").Trim() == "1",
						 CascadesOnDelete = foreignKeyGroups.First().Get<string>("CascadeDelete").Trim() == "1"
					 }
				 ).ToArray(),

				 UniqueConstraints = (
					 from columnMetaData in tableResults
					 where columnMetaData["UniqueName"] != DBNull.Value
					 group columnMetaData by columnMetaData.Get<string>("UniqueName").Trim()
					 into uniqueConstraints
					 select new DBUniqueConstraintSchema {
						 Name = uniqueConstraints.Key,
						 Columns = (
							 from uniqueCol in uniqueConstraints
							 orderby uniqueCol.Get<int>("Position") ascending
							 select uniqueCol.Get<string>("ColumnName").Trim()
						 ).ToArray(),
					 }
				 ).ToArray(),
			 }
			).ToArray();
	}

	private DBTriggerSchema[] NormalizeTriggers(DataTable denormalizedTriggerDescriptions) {
		return (
			from row in denormalizedTriggerDescriptions.Rows.Cast<DataRow>()
			select new DBTriggerSchema {
				Name = row.Get<string>("Name").Trim()
			}
		).ToArray();
	}

	#endregion

}
