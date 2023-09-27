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
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Hydrogen.Data;

public class MSSQLDAC : DACBase {

	public MSSQLDAC(string connectionString, ILogger logger = null)
		: base(connectionString, logger) {
	}

	public override DBMSType DBMSType {
		get { return DBMSType.SQLServer; }
	}

	public override IDbConnection CreateConnection() {
		return
			new SqlConnection {
				ConnectionString = ConnectionString
			};
	}

	public override ISQLBuilder CreateSQLBuilder() {
		return new MSSQLBuilder();
	}

	public override void EnlistInSystemTransaction(IDbConnection connection, Transaction transaction) {
		var mssqlConnection = connection as SqlConnection;
		if (mssqlConnection == null)
			throw new ArgumentException("Not an SqlConnection", "connection");
		mssqlConnection.EnlistTransaction(transaction);
	}

	public bool IsAzure() {
		return this.ExecuteScalar<bool>("SELECT CASE WHEN SERVERPROPERTY('EngineEdition') = 5 THEN 1 ELSE 0 END");
	}

	// This needs to be done properly!
	protected override IEnumerable<string> GetBatches(ISQLBuilder sqlBuilder) {
		yield return "SET XACT_ABORT ON;";
		yield return "BEGIN TRANSACTION;";
		// TODO: issue batches in smaller chunks
		var currentBatch = new FastStringBuilder();
		foreach (var statement in sqlBuilder.Statements.Where(x => x.Type != SQLStatementType.TCL)) {
			if (statement.Type == SQLStatementType.DDL) {
				if (currentBatch.Length > 0) {
					yield return currentBatch.ToString();
					currentBatch.Clear();
				}
				yield return statement.SQL;
			} else {
				currentBatch.Append(statement.SQL);
			}
		}
		if (currentBatch.Length > 0)
			yield return currentBatch.ToString();
		yield return "COMMIT TRANSACTION;";
	}

	public override void BulkInsert(DataTable table, BulkInsertOptions bulkInsertOptions, TimeSpan timeout) {
		using (var scope = this.BeginScope()) {
			using (var copy = new SqlBulkCopy(
				       (SqlConnection)((RestrictedConnection)scope.Connection).DangerousInternalConnection,
				       ConvertOptions(bulkInsertOptions),
				       (SqlTransaction)(scope.Transaction as RestrictedTransaction)?.DangerousInternalTransaction
			       )) {
				copy.BulkCopyTimeout = (int)Math.Round(timeout.TotalSeconds, 0);
				copy.DestinationTableName = table.TableName;
				copy.WriteToServer(table);
			}
		}
	}

	private static SqlBulkCopyOptions ConvertOptions(BulkInsertOptions bulkInsertOptions) {
		var options = (SqlBulkCopyOptions)0;
		if (bulkInsertOptions.HasFlag(BulkInsertOptions.Default))
			options |= SqlBulkCopyOptions.Default;

		if (bulkInsertOptions.HasFlag(BulkInsertOptions.KeepIdentity))
			options |= SqlBulkCopyOptions.KeepIdentity;

		if (bulkInsertOptions.HasFlag(BulkInsertOptions.MaintainForeignKeys))
			options |= SqlBulkCopyOptions.CheckConstraints;

		return options;
	}


	protected override DataTable GetDenormalizedTableDescriptions() {
		return this.ExecuteQuery(
			@"SELECT 
	DISTINCT
	T.name AS TableName,
	C.name AS ColumnName,
	C.column_id AS Position,
	UPPER(TY.name) AS Type,
	C.max_length AS Length,
	TY.precision as Precision,
	TY.scale as Scale,
	C.is_nullable AS IsNullable,
	CASE WHEN A.is_primary_key = 0 THEN A.name ELSE NULL END AS UniqueName,
	CASE WHEN A.is_primary_key = 1 THEN A.name ELSE NULL END AS PrimaryKeyName,
	C.is_identity AS IsAutoIncrement,
	NULL AS Sequence,
	B.ForeignKeyName AS ForeignKeyName,
	B.ReferenceTableName AS ReferenceTableName,
	B.ReferenceColumnName AS ReferenceColumnName,
	CASE WHEN B.update_referential_action > 0 THEN 1 ELSE 0 END AS CascadeUpdate,
	CASE WHEN B.delete_referential_action > 0 THEN 1 ELSE 0 END AS CascadeDelete
FROM 
	sys.tables T INNER JOIN
	sys.columns C ON T.object_id = C.object_id INNER JOIN
	sys.types TY ON C.user_type_id = TY.user_type_id LEFT JOIN (
		SELECT 
			IC.object_id,
			IC.column_id,
			I.name,
			I.is_primary_key
		FROM  sys.index_columns IC INNER JOIN
			  sys.indexes I on IC.object_id = I.object_id AND IC.index_id = I.index_id
		WHERE
			I.is_unique = 1
	) A ON T.object_id = A.object_id AND C.column_id = A.column_id LEFT JOIN (
		SELECT 
			F.parent_object_id as object_id,
			FC.parent_column_id as column_id,
			F.name AS ForeignKeyName,
			F.delete_referential_action,
			F.update_referential_action,
			OBJECT_NAME (F.referenced_object_id) AS ReferenceTableName,
			COL_NAME(FC.referenced_object_id,FC.referenced_column_id) AS ReferenceColumnName
		FROM
			sys.foreign_keys F INNER JOIN
			sys.foreign_key_columns FC  on FC.constraint_object_id = F.object_id
	) B ON T.object_id = B.object_id AND C.column_id = B.column_id"
		);
	}

	protected override DataTable GetDenormalizedTriggerDescriptions() {
		return this.ExecuteQuery(@"SELECT Name FROM sys.triggers");

	}
}
