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
using System.Text;

namespace Hydrogen.Data;

public class MSSQLBuilder : SQLBuilderBase {

	public override ISQLBuilder BeginTransaction() {
		return
			Emit("SET XACT_ABORT ON").EndOfStatement(SQLStatementType.TCL)
				.Emit("BEGIN TRANSACTION").EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder CommitTransaction() {
		return Emit("COMMIT TRANSACTION").EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder RollbackTransaction() {
		return Emit("ROLLBACK TRANSACTION").EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder TableName(string tableName, TableType tableType) {
		if (tableName.StartsWith("[")) {
			tableName = tableName.Trim().Replace("[", string.Empty).Replace("]", string.Empty);
		}

		switch (tableType) {
			case TableType.InMemory:
				return base.TableName("@" + tableName, tableType);
			case TableType.Temporary:
				return base.TableName("#" + tableName, tableType);
		}

		return base.TableName(tableName, tableType);
	}

	public override ISQLBuilder ObjectName(string objectName) {
		if (!objectName.ToUpper().StartsWith("SYS."))
			return Emit("[").Emit(objectName).Emit("]");
		return Emit(objectName);
	}

	public override ISQLBuilder DisableAutoIncrementID(string table) {
		return Emit("IF (SELECT count(is_identity) FROM sys.columns WHERE [object_id] = object_id('EW_Asset')) = 0 SET IDENTITY_INSERT ").TableName(table).Emit(" ON").EndOfStatement();
	}

	public override ISQLBuilder EnableAutoIncrementID(string table) {
		return Emit("IF (SELECT count(is_identity) FROM sys.columns WHERE [object_id] = object_id('EW_Asset')) = 0 SET IDENTITY_INSERT ").TableName(table).Emit(" OFF").EndOfStatement();
	}

	public override ISQLBuilder Select(string tableName, IEnumerable<object> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                   string orderByClause = null, bool endStatement = false) {
		if (offset.HasValue) {
			if (!limit.HasValue)
				throw new NotSupportedException("When selecting by an offset the limit parameter must also be supplied for this SQL Builder");

			const string pagedQuery =
				@"WITH __UNPAGED_RESULT AS (	
    {0}
), __PAGED_RESULT AS (
    SELECT *, ROW_NUMBER() OVER (ORDER BY {3}) AS __ROWNUM FROM __UNPAGED_RESULT
)
SELECT * FROM __PAGED_RESULT WHERE __ROWNUM > {2} AND __ROWNUM <= ({2} + {1})";

			Emit(
				pagedQuery,
				SQLBuilderCommand.Generic(b => b.Select(tableName, columns: columns, distinct: distinct, limit: null, offset: null, columnMatches: columnMatches, whereClause: whereClause, orderByClause: null, endStatement: false)),
				limit.Value,
				offset.Value,
				string.IsNullOrEmpty(orderByClause) ? "@@IDENTITY" : orderByClause
			);

			if (endStatement)
				base.EndOfStatement();

			return this;
		}

		return base.Select(tableName, columns: columns, distinct: distinct, limit: limit, offset: null, columnMatches: columnMatches, whereClause: whereClause, orderByClause: orderByClause, endStatement: endStatement);
	}

	public override ISQLBuilder InsertMany(string table, IEnumerable<string> columns, IEnumerable<IEnumerable<object>> values) {
		values.Partition(1000).ForEach(
			valuesPartition => {
				Emit("INSERT INTO ").TableName(table).Emit("(");

				foreach (var columnName in columns.WithDescriptions()) {
					if (columnName.Index > 0)
						Emit(", ");
					this.ColumnName(columnName.Item);
				}
				this.Emit(") VALUES").NewLine();
				foreach (var valueSet in valuesPartition.WithDescriptions()) {
					if (valueSet.Index > 0)
						Emit(", ").NewLine();
					Emit("(");
					foreach (var value in valueSet.Item.WithDescriptions()) {
						if (value.Index > 0)
							Emit(", ");
						Literal(value.Item);
					}
					Emit(")");
				}
				EndOfStatement();
			}
		);
		return this;
	}

	public override ISQLBuilder NextSequenceValue(string sequenceName) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder GetLastIdentity(string hint = null) {
		return Emit("SCOPE_IDENTITY()");
	}

	public override ISQLBuilder VariableName(string variableName) {
		return Emit("@{0} ", variableName);
	}

	public override ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null) {
		if (offset.HasValue)
			throw new NotSupportedException("Offset not supported in this SQL Builder. Try calling Select directly with limit/offset parameters");

		return Emit("TOP ").Emit(limit.ToString());
	}

	public override ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML) {
		Emit(";").NewLine();
		return base.EndOfStatement(statementType);
	}

	public override ISQLBuilder Literal(object value) {
		if (value == null || value == DBNull.Value)
			return Emit("NULL");

		TypeSwitch.For(
			value,
			TypeSwitch.Case<byte[]>(x => {
				Emit("CONVERT(varbinary(max), ");
				base.Literal(value);
				Emit(" , 1)");
			}),
			TypeSwitch.Default(() => base.Literal(value))
		);
		return this;
	}

	public override ISQLBuilder DeclareVariable(string variableName, Type type) {
		VariableDeclarationCount++;
		return
			Emit("DECLARE ")
				.VariableName(variableName)
				.TypeToSqlType(type)
				.EndOfStatement();
	}

	public override ISQLBuilder AssignVariable(string variableName, object value) {
		return
			Emit("SET ")
				.VariableName(variableName)
				.Emit(" = ")
				.Literal(value)
				.EndOfStatement();
	}

	public override ISQLBuilder CreateBuilder() {
		return new MSSQLBuilder();
	}

	public override ISequentialGuidGenerator CreateSequentialGuidGenerator(long capacity, bool regenerateOnEmpty = false) {
		return new SequentialSqlGuidGenerator(capacity, regenerateOnEmpty);
	}

	public override Type ConvertSQLTypeToType(string sqlType, int? length) {

		sqlType = sqlType.ToLower();

		if (sqlType.IsIn("varchar", "char", "nvarchar", "nchar", "text", "ntext", "sysname"))
			if (length.HasValue && length.Value == 1)
				return typeof(Char);
			else
				return typeof(String);

		if (sqlType.IsIn("xml"))
			return typeof(string);

		if (sqlType.IsIn("varbinary", "binary", "rowversion"))
			if (length.HasValue && length.Value == 1)
				return typeof(Byte);
			else
				return typeof(Byte[]);

		if (sqlType.IsIn("image", "timestamp"))
			return typeof(Byte[]);

		if (sqlType.IsIn("uniqueidentifier"))
			return typeof(Guid);

		if (sqlType.IsIn("bit"))
			return typeof(Boolean);

		if (sqlType.IsIn("tinyint"))
			return typeof(Byte);

		if (sqlType.IsIn("smallint"))
			return typeof(Int16);

		if (sqlType.IsIn("int"))
			return typeof(Int32);

		if (sqlType.IsIn("bigint"))
			return typeof(Int64);

		if (sqlType.IsIn("smallmoney"))
			return typeof(Decimal);

		if (sqlType.IsIn("money"))
			return typeof(Decimal);

		if (sqlType.IsIn("numeric"))
			return typeof(Decimal);

		if (sqlType.IsIn("decimal"))
			return typeof(Decimal);

		if (sqlType.IsIn("real", "float")) {
			return typeof(Single);
		}

		if (sqlType.IsIn("double")) {
			return typeof(Double);
		}

		if (sqlType.IsIn("datetimeoffset"))
			return typeof(DateTimeOffset);

		if (sqlType.IsIn("smalldatetime", "datetime", "datetime2"))
			return typeof(DateTime);

		if (sqlType.IsIn("time"))
			return typeof(TimeSpan);

		if (sqlType.IsIn("sql_variant"))
			return typeof(object);

		throw new SoftwareException("Unable to convert '{0}{1}' to .NET type", sqlType, length.HasValue ? " - " + length.Value : String.Empty);
	}

	public override string ConvertTypeToSQLType(Type type, bool preferUnicode = true, string maxLen = "MAX") {

		string retval = null;

		TypeSwitch.ForType(type,
			TypeSwitch.Case<char>(() => retval = preferUnicode ? "NCHAR" : "CHAR"),
			TypeSwitch.Case<string>(() => retval = (preferUnicode ? "NVARCHAR({0})" : "VARCHAR({0})").FormatWith(maxLen)),
			TypeSwitch.Case<byte>(() => retval = "TINYINT"),
			TypeSwitch.Case<bool>(() => retval = "BIT"),
			TypeSwitch.Case<Guid>(() => retval = "UNIQUEIDENTIFIER"),
			TypeSwitch.Case<short>(() => retval = "SMALLINT"),
			TypeSwitch.Case<int>(() => retval = "INT"),
			TypeSwitch.Case<long>(() => retval = "BIGINT"),
			TypeSwitch.Case<decimal>(() => retval = "MONEY"),
			TypeSwitch.Case<float>(() => retval = "REAL"),
			TypeSwitch.Case<double>(() => retval = "FLOAT"),
			TypeSwitch.Case<DateTime>(() => retval = "DATETIME"),
			TypeSwitch.Case<DateTimeOffset>(() => retval = "DATETIMEOFFSET"),
			TypeSwitch.Case<Byte[]>(() => retval = "VARBINARY(" + (maxLen ?? "MAX") + ")")
		);


		if (retval == null) {
			throw new SoftwareException("Unable to convert type '{0}' to sql type.", type);
		}

		return retval;


	}

	public override string ToString() {
		var totalLength = PreviousStatements.Sum(s => s.SQL.Length) + CurrentStatement.Item2.Length;
		var totalDMLStatements = PreviousStatements.Count(s => s.Type == SQLStatementType.DDL);
		var stringBuilder = new StringBuilder(totalLength + 10 * totalDMLStatements);
		foreach (var statement in PreviousStatements) {
			if (statement.Type == SQLStatementType.DDL) {
				stringBuilder.AppendFormat("{0}GO{0}", Environment.NewLine);
			}
			stringBuilder.Append(statement.SQL);
		}
		if (CurrentStatement.Item2.Length > 0) {
			if (CurrentStatement.Item1 == SQLStatementType.DDL) {
				stringBuilder.AppendFormat("{0}GO{0}", Environment.NewLine);
			}
			stringBuilder.Append(CurrentStatement.Item2);

		}
		return stringBuilder.ToString();
	}

}
