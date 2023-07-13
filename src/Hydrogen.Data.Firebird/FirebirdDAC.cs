// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using FirebirdSql.Data.FirebirdClient;
using Hydrogen.Data.Firebird;
using IsolationLevel = System.Data.IsolationLevel;

namespace Hydrogen.Data;

public class FirebirdDAC : DACBase {

	public FirebirdDAC(string connectionString, ILogger logger = null)
		: base(connectionString, logger) {
	}

	public override DBMSType DBMSType {
		get {
			if (ConnectionString != null && ConnectionString.RemoveNonAlphaNumeric().ToUpper().Contains("SERVERTYPE1"))
				return DBMSType.FirebirdFile;
			return DBMSType.Firebird;
		}
	}

	public override IDbConnection CreateConnection() {
		return
			new FbConnection {
				ConnectionString = ConnectionString
			};
	}

	public override void EnlistInSystemTransaction(IDbConnection connection, Transaction transaction) {
		var fbConn = connection as FbConnection;
		if (fbConn == null)
			throw new ArgumentException("Not an FbConnection", "connection");
		fbConn.EnlistTransaction(transaction);
	}

	public override object ExecuteScalar(string query) {
		var result = base.ExecuteScalar(query);
		if (result is Guid) {
			result = FirebirdCorrectingReader.CorrectGuid((Guid)result);
		}
		return result;
	}


	public override IDataReader ExecuteReader(string query) {
		return new FirebirdCorrectingReader(base.ExecuteReader(query));
	}

	public override void BulkInsert(DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout) {
		throw new NotImplementedException();
	}

	public override DataTable[] ExecuteBatch(ISQLBuilder sqlBuilder) {
		// Firebird doesn't like mixing DDL with DML, so we break up
		// DDL's that contains CREATE into an initial transaction
		// Then rest of statements in another transaction

		var statements = sqlBuilder.Statements.Where(s => s.Type.IsIn(SQLStatementType.DDL, SQLStatementType.DML)).ToList();
		statements.RemoveAll(s => string.IsNullOrEmpty(s.SQL));

		var ddls = statements.Where(statement => statement.Type == SQLStatementType.DDL && statement.SQL.TrimStart().StartsWith("CREATE")).ToList();
		statements.RemoveAll(ddls.Contains);

		var results = new List<DataTable>();

		if (ddls.Count > 0)
			results.AddRange(ExecuteStatementBatch(ddls.Select(s => s.SQL)));
		if (statements.Count > 0)
			results.AddRange(ExecuteStatementBatch(statements.Select(s => s.SQL)));
		return results.ToArray();
	}

	protected DataTable[] ExecuteStatementBatch(IEnumerable<string> statements) {
		try {
			var results = new List<DataTable>();
			using (var scope = this.BeginScope()) {
				scope.BeginTransaction(IsolationLevel.ReadCommitted);
				using (var command = scope.Connection.CreateCommand()) {
					command.CommandTimeout = 0;
					command.CommandType = CommandType.Text;
					if (scope.Transaction != null)
						command.Transaction = ((RestrictedTransaction)scope.Transaction).DangerousInternalTransaction;
					foreach (var statement in statements) {
						command.CommandText = statement;
						Log.Debug(Environment.NewLine + statement);
						try {
							using (var reader = new FirebirdCorrectingReader(command.ExecuteReader(CommandBehavior.Default))) {
								results.AddRange(reader.ToDataTables());
							}
						} catch (Exception error) {
							throw new SoftwareException(error, "An error happened excuting statement: '{0}", statement);
						}
					}
				}
				scope.Commit();
			}
			return results.ToArray();
		} catch (Exception error) {
			Log.Error(error.ToDiagnosticString());
			throw;
		}
	}


	protected override IEnumerable<string> GetBatches(ISQLBuilder sqlBuilder) {
		return
			sqlBuilder
				.Statements
				.Where(s => s.Type != SQLStatementType.TCL && !string.IsNullOrEmpty(s.SQL))
				.Select(statement => statement.SQL);
	}

	public override ISQLBuilder CreateSQLBuilder() {
		return new FirebirdSQLBuilder();
	}

	protected override DataTable GetDenormalizedTableDescriptions() {
		// Useful link: http://www.alberton.info/firebird_sql_meta_info.html#.Uari3UDI2MA
		return this.ExecuteQuery(
			@"SELECT
	A.RDB$RELATION_NAME ""TableName"",
	B.RDB$FIELD_NAME ""ColumnName"",
	B.RDB$FIELD_POSITION ""Position"",
    CASE D.RDB$TYPE_NAME
    	WHEN 'TEXT' THEN 'CHAR(' || C.RDB$FIELD_LENGTH  || ') CHARACTER SET ' || E2.RDB$CHARACTER_SET_NAME
        WHEN 'VARYING' THEN 'VARCHAR(' || C.RDB$FIELD_LENGTH  || ') CHARACTER SET ' || E2.RDB$CHARACTER_SET_NAME
        WHEN 'BLOB' THEN 'BLOB SUB_TYPE '|| E.RDB$TYPE_NAME
        WHEN 'SHORT' THEN 'SMALLINT'
        WHEN 'LONG' THEN 'INTEGER'
        WHEN 'INT64' THEN
        	CASE C.RDB$FIELD_PRECISION
            	WHEN 0 THEN 'INT64'
                ELSE 'DECIMAL(' || C.RDB$FIELD_PRECISION || ', ' || (CAST(C.RDB$FIELD_SCALE AS INTEGER) * -1)   || ')'
            END
        ELSE D.RDB$TYPE_NAME 
    END ""Type"",
	C.RDB$FIELD_LENGTH ""Length"",
	C.RDB$FIELD_PRECISION  ""Precision"",
	(CAST(C.RDB$FIELD_SCALE AS INTEGER) * -1) ""Scale"",
	CASE B.RDB$NULL_FLAG WHEN 1 THEN 0 ELSE 1 END ""IsNullable"",
	F.RDB$INDEX_NAME ""UniqueName"",
	G.RDB$INDEX_NAME ""PrimaryKeyName"",
	NULL ""IsAutoIncrement"",
	H.ForeignKeyName ""ForeignKeyName"",
	H.ReferenceTable ""ReferenceTableName"",
	H.ReferenceColumn  ""ReferenceColumnName"",
	CASE H.update_rule 
        WHEN 'RESTRICT' THEN 0
        WHEN 'NO ACTION' THEN 1
        WHEN 'CASCADE' THEN 1
        WHEN 'SET NULL' THEN 1
        WHEN 'SET DEFAULT' THEN 1
        ELSE 0 
    END ""CascadeUpdate"",
	CASE H.delete_rule 
        WHEN 'RESTRICT' THEN 0
        WHEN 'NO ACTION' THEN 1
        WHEN 'CASCADE' THEN 1
        WHEN 'SET NULL' THEN 1
        WHEN 'SET DEFAULT' THEN 1
        ELSE 0 
    END ""CascadeDelete""
FROM 
	RDB$RELATIONS A LEFT JOIN 
	RDB$RELATION_FIELDS B ON A.RDB$RELATION_NAME = B.RDB$RELATION_NAME LEFT JOIN 
    RDB$FIELDS C ON B.RDB$FIELD_SOURCE = C.RDB$FIELD_NAME LEFT JOIN 
	RDB$TYPES D ON C.RDB$FIELD_TYPE = D.RDB$TYPE AND D.RDB$FIELD_NAME = 'RDB$FIELD_TYPE' LEFT JOIN
	RDB$TYPES E ON C.RDB$FIELD_SUB_TYPE = E.RDB$TYPE AND E.RDB$FIELD_NAME = 'RDB$FIELD_SUB_TYPE' LEFT JOIN
    RDB$CHARACTER_SETS E2 ON C.RDB$CHARACTER_SET_ID = E2.RDB$CHARACTER_SET_ID LEFT JOIN (
      SELECT
          B.RDB$RELATION_NAME,
          A.RDB$FIELD_NAME,
          A.RDB$INDEX_NAME
      FROM
          RDB$INDEX_SEGMENTS A LEFT JOIN
          RDB$INDICES B ON A.RDB$INDEX_NAME = B.RDB$INDEX_NAME LEFT JOIN
          RDB$RELATION_CONSTRAINTS C ON (B.RDB$INDEX_NAME = C.RDB$INDEX_NAME)
      WHERE
          B.RDB$UNIQUE_FLAG = 1 AND
          (C.RDB$CONSTRAINT_TYPE = 'UNIQUE' OR C.RDB$CONSTRAINT_TYPE IS NULL)
   ) F ON A.RDB$RELATION_NAME = F.RDB$RELATION_NAME AND B.RDB$FIELD_NAME = F.RDB$FIELD_NAME LEFT JOIN (
      SELECT 
          B.RDB$RELATION_NAME,
          A.RDB$FIELD_NAME,
          A.RDB$INDEX_NAME
      FROM 
          RDB$INDEX_SEGMENTS A LEFT JOIN 
          RDB$RELATION_CONSTRAINTS B ON (A.RDB$INDEX_NAME = B.RDB$INDEX_NAME)
      WHERE 
          B.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
    ) G ON A.RDB$RELATION_NAME = G.RDB$RELATION_NAME AND B.RDB$FIELD_NAME = G.RDB$FIELD_NAME LEFT JOIN (
      SELECT
          rc.rdb$constraint_name AS ForeignKeyName, 
          rcc.rdb$relation_name AS ForeignKeyTable, 
          isc.rdb$field_name AS ForeignKeyColumn, 
          rcp.rdb$relation_name AS ReferenceTable, 
          isp.rdb$field_name AS ReferenceColumn, 
          rc.rdb$update_rule AS update_rule, 
          rc.rdb$delete_rule AS delete_rule
      FROM
          rdb$ref_constraints AS rc INNER JOIN 
          rdb$relation_constraints AS rcc on rc.rdb$constraint_name = rcc.rdb$constraint_name INNER JOIN
          rdb$index_segments AS isc on rcc.rdb$index_name = isc.rdb$index_name INNER JOIN
          rdb$relation_constraints AS rcp on rc.rdb$const_name_uq  = rcp.rdb$constraint_name INNER JOIN
          rdb$index_segments AS isp on rcp.rdb$index_name = isp.rdb$index_name
    ) H ON A.RDB$RELATION_NAME = H.ForeignKeyTable AND B.RDB$FIELD_NAME = H.ForeignKeyColumn
    
WHERE 
	A.RDB$SYSTEM_FLAG = 0 AND
  	B.RDB$VIEW_CONTEXT IS NULL"
		);
	}

	protected override DataTable GetDenormalizedTriggerDescriptions() {
		return this.ExecuteQuery("SELECT RDB$TRIGGER_NAME \"Name\" FROM RDB$TRIGGERS WHERE RDB$SYSTEM_FLAG=0");

	}

}
