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

namespace Hydrogen.Data;

public static class IDACExtensions {

	public static DACScope BeginScope(this IDAC dac, bool openConnection = true, ContextScopePolicy policy = ContextScopePolicy.None) {
		if (dac.UseScopeOsmosis)
			return new DACScope(dac, policy, openConnection);

		if (policy == ContextScopePolicy.MustBeNested)
			throw new ArgumentException("Policy cannot be MustBeNested for DAC that uses direct connections (i.e. UseScopeOsmosis == false).", "policy");

		return new DACScope(dac, ContextScopePolicy.None, openConnection, string.Format("{0}:", dac.InstanceID.ToStrictAlphaString()));
	}

	public static DACScope BeginDirtyReadScope(this IDAC dac, bool openConnection = true) {
		var scope = new DACScope(dac, ContextScopePolicy.None, openConnection);
		scope.BeginTransaction(IsolationLevel.ReadUncommitted);
		return scope;
	}

	public static IDbConnection CreateOpenConnection(this IDAC dac) {
		var connection = dac.CreateConnection();
		if (connection.State.IsIn(ConnectionState.Closed, ConnectionState.Broken))
			connection.Open();
		return connection;
	}

	public static long GetMaxID(this IDAC dac, string tableName, string idColName) {
		return dac.ExecuteScalar<long>(
			dac.QuickString(
				"SELECT MAX(T.ID) FROM (SELECT {0} ID FROM {1} WHERE ID IS NOT NULL UNION SELECT 0 ID ) T",
				SQLBuilderCommand.ColumnName(idColName),
				SQLBuilderCommand.TableName(tableName)
			)
		);
	}

	public static long Save(this IDAC dac, DataRow dataRow, bool ommitAutoIncrementPK = true) {
		using (var scope = dac.BeginScope(openConnection: true)) {
			return dataRow.RowState == DataRowState.Added ? dac.Insert(dataRow, ommitAutoIncrementPK) : dac.Update(dataRow);
		}
	}

	// Improve this
	public static DataTable Select(this IDAC dac, string tableName, IEnumerable<string> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                               string orderByClause = null) {
		var sqlBuilder = dac.CreateSQLBuilder();
		sqlBuilder.Select(tableName,
			columns: columns != null ? columns.Cast<object>() : null,
			distinct: distinct,
			limit: limit,
			offset: offset,
			columnMatches: columnMatches,
			whereClause: whereClause,
			orderByClause: orderByClause,
			endStatement: true);
		var table = dac.ExecuteQuery(sqlBuilder.ToString());
		table.TableName = tableName;
		return table;
	}

	public static DataTable Select(this IDAC dac, DataRow row) {
		return dac.Select(
			row.Table.TableName,
			columns: (row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)).ToArray(),
			columnMatches: (from key in row.Table.PrimaryKey select new ColumnValue(key.ColumnName, row[key.ColumnName])).ToArray()
		);
	}

	public static long Insert(this IDAC dac, DataRow row, bool ommitAutoIncrementPK = true) {
		var cols = row.Table.Columns.Cast<DataColumn>();
		var insertValues = cols.Select(c => new ColumnValue(c.ColumnName, row[c.ColumnName]));
		if (ommitAutoIncrementPK && row.Table.PrimaryKey.Length == 1 && row.Table.PrimaryKey[0].AutoIncrement)
			insertValues = insertValues.Except(new ColumnValue(row.Table.PrimaryKey[0].ColumnName, row[row.Table.PrimaryKey[0].ColumnName]));

		return dac.Insert(
			row.Table.TableName,
			insertValues
		);
	}

	public static long Update(this IDAC dac, DataRow row) {
		var allColumns =
			row
				.Table
				.Columns
				.Cast<DataColumn>()
				.Select(c => new ColumnValue(c.ColumnName, row[c.ColumnName]));

		var pkCols =
			row
				.Table
				.PrimaryKey
				.Select(c => new ColumnValue(c.ColumnName, row[c.ColumnName]));

		return dac.Update(
			row.Table.TableName,
			allColumns.Except(pkCols),
			pkCols
		);
	}

	public static IDictionary<string, long> CountRecords(this IDAC dac, IEnumerable<string> tableNames = null) {
		var result = new Dictionary<string, long>();

		if (tableNames == null) {
			tableNames = dac.GetSchemaCached().Tables.Select(table => table.Name);
		}

		if (!tableNames.Any())
			return result;

		var sqlBuilder = dac.CreateSQLBuilder();

		foreach (var table in tableNames.WithDescriptions()) {
			if (table.Index > 0)
				sqlBuilder.NewLine().Emit("UNION ALL").NewLine();
			sqlBuilder.Emit("SELECT '{0}', COUNT(1) FROM {1}", table.Item, SQLBuilderCommand.TableName(table.Item));
		}

		dac
			.ExecuteQuery(((Object)sqlBuilder).ToString())
			.Rows
			.Cast<DataRow>()
			.ForEach(row => result.Add(row.Get<string>(0), row.Get<long>(1)));
		return result;
	}

	public static long Count(this IDAC dac, string tableName, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null) {
		var table = dac.Select(tableName, columns: new[] { "COUNT(1)" }, columnMatches: columnMatches, whereClause: whereClause);
		if (table.Rows.Count == 0) {
			return 0;
		}
		return table.Rows[0].Get<long>(0);
	}

	public static bool Any(this IDAC dac, string tableName, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null) {
		var table = dac.Select(tableName, columns: new[] { "COUNT(1)" }, columnMatches: columnMatches, whereClause: whereClause);
		return table.Rows.Count > 0 && table.Rows[0].Get<long>(0) > 0;
	}

	public static string QuickString(this IDAC dac, string sql, params object[] args) {
		var newBuilder = dac.CreateSQLBuilder();
		return ((Object)newBuilder.Emit(sql, args)).ToString();
	}

	public static DataTable ExecuteQuery(this IDAC dac, string query) {
		using (var scope = dac.BeginScope())
		using (var reader = dac.ExecuteReader(query))
			return reader.ToDataTable();

	}

	public static DataTable ExecuteQuery(this IDAC dac, string query, params object[] args) {
		var builder = dac.CreateSQLBuilder();
		builder.Emit(query, args);
		return ExecuteQuery(dac, builder.ToString());
	}

	public static void CreateTable(this IDAC dac, TableSpecification tableSpecification) {
		var builder = dac.CreateSQLBuilder();
		builder.CreateTable(tableSpecification);
		dac.ExecuteBatch(builder);
	}

	public static T ExecuteScalar<T>(this IDAC dac, string query) {
		return Tools.Object.ChangeType<T>(dac.ExecuteScalar(query));
	}

	//private static void ApplyArtificialKeys(DBSchema schema) {
	//    if (!string.IsNullOrEmpty(ArtificialKeys)) {
	//        this.InvalidateCachedSchema();
	//        schema.ApplyArtificialKeys(ArtificialKeys);
	//    }
	//}
}
