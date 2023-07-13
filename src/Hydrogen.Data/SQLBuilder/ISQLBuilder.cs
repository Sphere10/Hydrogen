// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Data;

public interface ISQLBuilder {

	ISQLBuilder LeafBuilder { get; set; }

	ICollection<SQLStatement> CreateContainerForStatements();

	ISQLBuilder NewLine();

	ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML);

	ISQLBuilder End();

	ISQLBuilder BeginTransaction();

	ISQLBuilder CommitTransaction();

	ISQLBuilder RollbackTransaction();

	ISQLBuilder DisableAutoIncrementID(string table);

	ISQLBuilder EnableAutoIncrementID(string table);

	ISQLBuilder CreateTable(TableSpecification tableSpecification);

	ISQLBuilder DropTable(string tableName, TableType tableType);

	ISQLBuilder SelectValues(IEnumerable<object> values, string whereClause, params object[] whereClauseFormatArgs);

	ISQLBuilder Select(string tableName, IEnumerable<object> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null, string orderByClause = null,
	                   bool endStatement = false);

	ISQLBuilder Insert(string table, IEnumerable<ColumnValue> values);

	ISQLBuilder InsertMany(string table, IEnumerable<string> columns, IEnumerable<IEnumerable<object>> values);

	ISQLBuilder Update(string table, IEnumerable<ColumnValue> setColumns, IEnumerable<ColumnValue> matchColumns = null, string whereClause = null);

	ISQLBuilder Delete(string table, IEnumerable<ColumnValue> matchColumns = null);

	ISQLBuilder TableName(string tableName, TableType tableType);

	ISQLBuilder ObjectName(string objectName);

	ISQLBuilder Literal(object value);

	ISQLBuilder NextSequenceValue(string sequenceName);

	ISQLBuilder GetLastIdentity(string hint = null);

	ISQLBuilder DefaultValues();

	ISQLBuilder VariableName(string variableName);

	ISQLBuilder DeclareVariable(string variableName, Type type);

	ISQLBuilder AssignVariable(string variableName, object value);

	ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null);

	ISQLBuilder EmitDistinct();

	ISQLBuilder Emit(string statement, params object[] formatArgs);

	ISQLBuilder TypeToSqlType(Type type, bool preferUnicode = true);

	ISQLBuilder Cast(SQLBuilderStringValueKind valueKind, object value, Type type);

	string ToString();


	IEnumerable<SQLStatement> Statements { get; }

	ISequentialGuidGenerator CreateSequentialGuidGenerator(long capacity, bool regenerateOnEmpty = false);

	int StatementCount { get; set; }

	int VariableDeclarationCount { get; set; }

	string ConvertTypeToSQLType(Type type, bool preferUnicode = true, string maxLen = "4000");

	Type ConvertSQLTypeToType(string sqlType, int? length);

	string FormatCreateTable(TableType tableType);

	string FormatCreateTableEnd(TableType type);

	ISQLBuilder CreateBuilder();

	ISQLBuilder Clear();

}


public static class ISQLBuilderExtensions {

	public static ISQLBuilder TableName(this ISQLBuilder sqlBuilder, string name, TableType tableType = TableType.Persistent) {
		return sqlBuilder.TableName(name, tableType);
	}

	public static ISQLBuilder ColumnName(this ISQLBuilder sqlBuilder, string name) {
		return sqlBuilder.ObjectName(name);
	}

	public static ISQLBuilder IndexName(this ISQLBuilder sqlBuilder, string name) {
		return sqlBuilder.ObjectName(name);
	}

	public static ISQLBuilder TriggerName(this ISQLBuilder sqlBuilder, string name) {
		return sqlBuilder.ObjectName(name);
	}

	public static ISQLBuilder InsertMany<T>(this ISQLBuilder sqlBuilder, string table, IEnumerable<string> columns, IEnumerable<T> values) where T : IEnumerable<object> {
		return sqlBuilder.InsertMany(table, columns, values.Select(v => v.Cast<object>()));
	}

	public static ISQLBuilder Select(this ISQLBuilder sqlBuilder, string tableName, IEnumerable<string> columns = null, bool distinct = false, int? limit = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                 string orderByClause = null) {
		return sqlBuilder.Select(tableName, columns: columns != null ? columns.Cast<object>() : null, distinct: distinct, limit: limit, columnMatches: columnMatches, whereClause: whereClause, orderByClause: orderByClause);
	}

	public static ISQLBuilder InsertSelect(
		this ISQLBuilder sqlBuilder,
		string insertTable,
		IEnumerable<string> insertColumns,
		string table,
		IEnumerable<object> columns = null,
		bool distinct = false,
		int? limit = null,
		IEnumerable<ColumnValue> columnMatches = null,
		string whereClause = null,
		string orderByClause = null
	) {
		if (columns != null && columns.Count() != insertColumns.Count()) {
			throw new ArgumentException("Insert column count doesnt match select", "insertColumns");
		}
		sqlBuilder.Emit("INSERT INTO ").TableName(insertTable);

		if (insertColumns.Any()) {
			sqlBuilder.Emit(" (");
			insertColumns.WithDescriptions().ForEach(
				c => {
					if (!c.Description.HasFlag(EnumeratedItemDescription.First))
						sqlBuilder.Emit(", ");

					sqlBuilder.ColumnName(c.Item);
				}
			);
		}
		sqlBuilder.Emit(")").NewLine();
		return sqlBuilder.Select(table, columns: columns, distinct: distinct, limit: limit, columnMatches: columnMatches, whereClause: whereClause, orderByClause: orderByClause);
	}


	public static string QuickString(this ISQLBuilder sqlBuilder, string sql, params object[] args) {
		var newBuilder = sqlBuilder.CreateBuilder();
		return newBuilder.Emit(sql, args).ToString();
	}

	public static ISQLBuilder DuplicateRow(this ISQLBuilder sqlBuilder, DBTableSchema table, IEnumerable<object> sourcePrimaryKey, IEnumerable<object> destPrimaryKey, out string identityVariable, IEnumerable<ColumnValue> overrideColumns = null) {
		// Query structure:
		// INSERT INTO Table([PkCol1], ..., [PkColN], [NonPkCol1], ..., [NonPkColN]) VALUES SELECT {destPrimaryKey1}, ..., {destPrimaryKeyN}, [NonPkCol1], ..., [NonPkColN] FROM Table WHERE [PkCol1] = {sourcePrimaryKey1}, ..., [PkColN] = {sourcePrimaryKeyN}

		bool isAutoIncrement;
		bool usesGenerator;
		bool specifiesPrimaryKey;

		#region Validation

		if (sourcePrimaryKey == null)
			throw new SoftwareException("Source primary key not specified");

		if (destPrimaryKey == null)
			destPrimaryKey = Enumerable.Empty<object>();

		if (overrideColumns == null)
			overrideColumns = Enumerable.Empty<ColumnValue>();

		if (table.PrimaryKeyColumns.Length == 0)
			throw new SoftwareException("Table '{0}' does not have a primary key", table.Name);

		if (!sourcePrimaryKey.Any())
			throw new SoftwareException("Inconsistent primary key parameter. Table {0} primary key has {1} columns, argument specified {2} values", table.Name, table.PrimaryKeyColumns.Length, sourcePrimaryKey.Count());

		isAutoIncrement = table.PrimaryKeyColumns.Length == 1 && table.PrimaryKeyColumns[0].IsAutoIncrement;
		usesGenerator = false;
		specifiesPrimaryKey = destPrimaryKey.Any();

		if (!(isAutoIncrement || usesGenerator) && !specifiesPrimaryKey)
			throw new SoftwareException("Destination primary key not specified");

		#endregion

		identityVariable = null;

		var sourcePrimaryKeyArray = sourcePrimaryKey.ToArray();

		// Declare variable to store generated identity (if applicable)
		if ((isAutoIncrement || usesGenerator) && !specifiesPrimaryKey) {
			identityVariable = string.Format("uniquedup{0}", sqlBuilder.VariableDeclarationCount + 1);
			sqlBuilder.DeclareVariable(identityVariable, typeof(long));
		}

		// get the non-primary key columns
		var nonPkColumns = table.Columns.Where(c => !c.IsPrimaryKey);

		// disable autoincrement if user specified key
		if (isAutoIncrement && specifiesPrimaryKey)
			sqlBuilder.DisableAutoIncrementID(table.Name);

		sqlBuilder
			.Emit("INSERT INTO ").TableName(table.Name).Emit("(");

		// TODO: Changed Union to Concat -- will this introduce errors?
		((specifiesPrimaryKey || usesGenerator) ? table.PrimaryKeyColumns : Enumerable.Empty<DBColumnSchema>()
			).Concat(nonPkColumns)
			.WithDescriptions()
			.ForEach(
				colDescription => {
					if (colDescription.Index > 0)
						sqlBuilder.Emit(", ");
					sqlBuilder.ColumnName(colDescription.Item.Name);
				}
			);


		sqlBuilder.Emit(") SELECT ");

		var overrideColumnsLookup = overrideColumns.ToDictionary(c => c.ColumnName, c => c.Value);
		if (specifiesPrimaryKey) {
			// insert explicit primary key values
			destPrimaryKey
				.WithDescriptions()
				.ForEach(
					destPrimaryKeyValue => {
						if (destPrimaryKeyValue.Index > 0)
							sqlBuilder.Emit(", ");
						sqlBuilder.Literal(destPrimaryKeyValue.Item);
					}
				);
		} else if (usesGenerator) {
			// insert call to generator
			throw new NotImplementedException();
		}

		nonPkColumns
			.WithDescriptions()
			.ForEach(
				colDescription => {
					if (specifiesPrimaryKey || usesGenerator || colDescription.Index > 0)
						sqlBuilder.Emit(", ");

					if (overrideColumnsLookup.ContainsKey(colDescription.Item.Name))
						sqlBuilder.Literal(overrideColumnsLookup[colDescription.Item.Name]);
					else
						sqlBuilder.ColumnName(colDescription.Item.Name);
				}
			);
		sqlBuilder.Emit(" FROM ").TableName(table.Name).Emit(" WHERE ");
		table
			.PrimaryKeyColumns
			.WithDescriptions()
			.ForEach(
				pkCol => {
					if (pkCol.Index > 0)
						sqlBuilder.Emit(" AND ");

					sqlBuilder
						.ColumnName(pkCol.Item.Name)
						.Emit(" = ")
						.Literal(sourcePrimaryKeyArray[pkCol.Index]);
				}
			);

		sqlBuilder.EndOfStatement();

		if (isAutoIncrement)
			sqlBuilder.EnableAutoIncrementID(table.Name);

		if ((isAutoIncrement || usesGenerator) && !specifiesPrimaryKey) {
			sqlBuilder.AssignVariable(identityVariable, SQLBuilderCommand.LastIdentity(table.Name));
		}

		return sqlBuilder;
	}

	public static ISQLBuilder EmitOrderByExpression(this ISQLBuilder sqlBuilder, IEnumerable<SortOption> sortOptions) {
		foreach (var so in sortOptions.WithDescriptions()) {
			if (!so.Description.HasFlag(EnumeratedItemDescription.First)) {
				sqlBuilder.Emit(", ");
			}
			sqlBuilder.TableName(so.Item.Name);
			switch (so.Item.Direction) {
				case SortDirection.Ascending:
					sqlBuilder.Emit(" ASC");
					break;
				case SortDirection.Descending:
					sqlBuilder.Emit(" DESC");
					break;
				case SortDirection.None:
				default:
					break;
			}
		}
		return sqlBuilder;
	}

}
