// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Data;

public class SqliteSQLBuilder : SQLBuilderBase {
	private bool _hasCreatedTempTable = false;
	private readonly IDictionary<string, Type> _variableDeclarations = new Dictionary<string, Type>();

	public override ISQLBuilder End() {
		if (_hasCreatedTempTable) {
			Emit("DROP TABLE _Variables").EndOfStatement();
			_hasCreatedTempTable = false;
		}
		return base.End();
	}

	public override ISQLBuilder ObjectName(string objectName) {
		var emitBrackets = !(objectName.StartsWith("[") && objectName.EndsWith("]"));
		if (emitBrackets)
			Emit("[");
		Emit(objectName);
		if (emitBrackets)
			Emit("]");
		return this;
	}


	public override ISQLBuilder BeginTransaction() {
		/*Emit("PRAGMA synchronous = OFF").EndOfStatement(SQLStatementType.TCL);
		Emit("PRAGMA journal_mode = MEMORY").EndOfStatement(SQLStatementType.TCL);*/

		Emit("BEGIN TRANSACTION").EndOfStatement(SQLStatementType.TCL);
		return this;
	}

	public override ISQLBuilder CommitTransaction() {
		return Emit("COMMIT TRANSACTION").EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder RollbackTransaction() {
		return Emit("ROLLBACK TRANSACTION").EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder DisableAutoIncrementID(string table) {
		// passing NULL into auto-increment ID column uses auto-increment, else it uses the value.
		// column should allow NULL values to work properly
		return this;
	}

	public override ISQLBuilder EnableAutoIncrementID(string table) {
		// passing NULL into auto-increment ID column uses auto-increment, else it uses the value.
		// column should allow NULL values to work properly
		return this;
	}

	public override ISQLBuilder NextSequenceValue(string sequenceName) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder GetLastIdentity(string hint = null) {
		return Emit("last_insert_rowid()");
	}

	public override ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null) {
		Emit("LIMIT ").Emit(limit.ToString());
		if (offset != null)
			Emit(" OFFSET ").Emit(offset.Value.ToString());
		return this;
	}


	public override ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML) {
		Emit(";").NewLine();
		return base.EndOfStatement(statementType);
	}


	public override ISQLBuilder InsertMany(string table, IEnumerable<string> columns, IEnumerable<IEnumerable<object>> values) {
		var colsArray = columns.ToArray();
		values.Partition(500).ForEach(valuePartition => base.InsertMany(table, colsArray, valuePartition).EndOfStatement());
		return this;
	}

	public override ISQLBuilder Select(string tableName, IEnumerable<object> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                   string orderByClause = null, bool endStatement = false) {
		Emit("SELECT ");

		if (distinct)
			EmitDistinct()
				.Emit(" ");

		if (columns != null && columns.Any())
			columns.WithDescriptions().ForEach(c => {
				if (!c.Description.HasFlag(EnumeratedItemDescription.First))
					Emit(", ");
				if (c.Item is string && !((string)c.Item).Contains('('))
					this.ColumnName(c.Item as string);
				else
					this.Emit("{0}", c.Item); // something else
			});
		else
			Emit("*");

		Emit(" FROM ").TableName(tableName);

		if (columnMatches != null && columnMatches.Any() || !string.IsNullOrEmpty(whereClause)) {
			Emit(" WHERE ");
		}

		if (columnMatches != null && columnMatches.Any()) {
			columnMatches.WithDescriptions().ForEach(w => {
				if (!w.Description.HasFlag(EnumeratedItemDescription.First))
					Emit(" AND");
				if ((w.Item.Value is IEnumerable && !(w.Item.Value is string)) || w.Item.Value is SQLExpressionCommand) {
					this.ColumnName(w.Item.ColumnName).Emit(" IN (");
					if (w.Item.Value is SQLExpressionCommand) {
						Emit("{0}", w.Item.Value);
					} else {
						int i = 0;
						foreach (var obj in ((IEnumerable)w.Item.Value)) {
							if (i++ > 0)
								Emit(", ");
							Literal(obj);
						}
					}
					Emit(")");
				} else {
					this.ColumnName(w.Item.ColumnName).Emit(" = ").Literal(w.Item.Value);
				}
			});
			if (!string.IsNullOrEmpty(whereClause))
				Emit(" AND ");
		}

		if (!string.IsNullOrEmpty(whereClause))
			Emit(whereClause);

		if (!string.IsNullOrEmpty(orderByClause))
			Emit(" ORDER BY {0}", orderByClause);

		if (limit.HasValue)
			Emit(" ")
				.EmitQueryResultLimit(limit.Value, offset);

		if (endStatement)
			EndOfStatement();

		return this;

	}

	public override ISQLBuilder DeclareVariable(string variableName, Type type) {
		VariableDeclarationCount++;
		if (!_hasCreatedTempTable) {
			Emit("PRAGMA temp_store = 2").EndOfStatement()
				.Emit("CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, RealValue REAL, IntegerValue INTEGER, BlobValue BLOB, TextValue TEXT)")
				.EndOfStatement();
			_hasCreatedTempTable = true;
		}

		// remember the type of this variable
		_variableDeclarations[variableName] = type;
		return Insert("_Variables", new[] { new ColumnValue("Name", variableName) });
	}

	public override ISQLBuilder AssignVariable(string variableName, object value) {
		if (!_variableDeclarations.ContainsKey(variableName))
			throw new Exception(string.Format("Variable '{0}' was not declared", variableName));

		var type = _variableDeclarations[variableName];

		return
			Update(
				"_Variables",
				new[] { new ColumnValue(TypeToVariableColumn(type), value) },
				new[] { new ColumnValue("Name", variableName) });
	}

	public override ISQLBuilder VariableName(string variableName) {
		return
			Emit("(SELECT coalesce(RealValue, IntegerValue, BlobValue, TextValue) FROM _Variables WHERE Name = '{0}' LIMIT 1)", variableName);
	}

	private string TypeToStorageClass(Type type) {
		if (type.IsNumeric()) {
			if (type.IsIn(typeof(float), typeof(double), typeof(decimal)))
				return "REAL";
			return "INTEGER";
		} else if (type.IsAssignableFrom(typeof(byte[]))) {
			return "BLOB";
		} else if (type.IsAssignableFrom(typeof(string)) || type.IsIn(typeof(char))) {
			return "TEXT";
		}
		throw new SoftwareException("Unable to determine SQLite streams class for type '{0}'", type);
	}

	private string TypeToVariableColumn(Type type) {
		var storageClass = TypeToStorageClass(type);
		switch (TypeToStorageClass(type)) {
			case "REAL":
				return "RealValue";
			case "INTEGER":
				return "IntegerValue";
			case "BLOB":
				return "BlobValue";
			case "TEXT":
				return "TextValue";
			default:
				throw new SoftwareException("Internal error [4EC2A231-A895-4F65-BD50-EC10BD7CC2DF]");
		}
	}

	public override ISQLBuilder Literal(object value) {
		if (value != null) {
			TypeSwitch.For(
				value,
				TypeSwitch.Case<byte[]>(x => Emit("X'").Emit(x.ToHexString(true)).Emit("'")),
				TypeSwitch.Case<Guid>(x => this.Literal(x.ToByteArray())),
				TypeSwitch.Case<Guid?>(x => {
					if (x.HasValue)
						this.Literal(x.Value.ToByteArray());
					else
						base.Literal(null);
				}),
				TypeSwitch.Default(() => base.Literal(value))
			);
		} else {
			base.Literal(null);
		}
		return this;
	}

	public ISQLBuilder Cast(SQLBuilderStringValueKind valueKind, object value, Type type) {
		Emit("CAST(");
		switch (valueKind) {
			case SQLBuilderStringValueKind.LiteralValue:
				Literal(value);
				break;
			case SQLBuilderStringValueKind.SQLCode:
				Emit(value.ToString());
				break;
			case SQLBuilderStringValueKind.VariableName:
				VariableName(value.ToString());
				break;
			default:
				throw new SoftwareException("Unsupported SQLBuilderCastValueType {0}", valueKind);
		}
		Emit(" AS ").TypeToSqlType(type).Emit(")");
		return this;
	}

	public override ISQLBuilder CreateBuilder() {
		return new SqliteSQLBuilder();
	}

	public override Type ConvertSQLTypeToType(string sqlType, int? length) {

		sqlType = sqlType.ToUpper();

		if (sqlType.IsIn("TIME", "DATETIME", "DATE", "TIME WITH TIME ZONE", "TIMETZ", "TIMESTAMP", "TIMESTAMP WITH TIME ZONE", "TIMESTAMPTZ"))
			return typeof(DateTime);

		switch (MapSQLTypeToSqliteType(sqlType)) {
			case "TEXT":
				return typeof(string);
			case "BLOB":
				return typeof(byte[]);
			case "INTEGER":
				return typeof(long);
			case "REAL":
				return typeof(double);
		}

		throw new SoftwareException("Unable to convert '{0}{1}' to .NET type", sqlType, length.HasValue ? "- " + length.Value : String.Empty);
	}

	public override string ConvertTypeToSQLType(Type type, bool preferUnicode = true, string maxLen = "4000") {
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

	public string MapSQLTypeToSqliteType(string dataType) {
		dataType = dataType.ToUpper();
		if (dataType.ContainsAnySubstrings(
			    "TEXT",
			    "NTEXT",
			    "CHARACTER",
			    "CHAR",
			    "CHARACTER VARYING",
			    "VARCHAR",
			    "NATIONAL CHARACTER",
			    "NCHAR",
			    "NATIONAL CHARACTER VARYING",
			    "NVARCHAR",
			    "CLOB"
		    )
		   ) return "TEXT";

		if (dataType.ContainsAnySubstrings(
			    "INT",
			    "INTEGER",
			    "TINYINT",
			    "SMALLINT",
			    "MEDIUM INT",
			    "BIGINT",
			    "UNSIGNED BIGINT",
			    "INT2",
			    "INT8",
			    "BOOL",
			    "BIT"
		    )
		   ) return "INTEGER";

		if (dataType.ContainsAnySubstrings(
			    "REAL",
			    "DOUBLE",
			    "DOUBLE PRECISION",
			    "FLOAT",
			    "NUMERIC",
			    "DECIMAL"
		    )
		   ) return "REAL";

		if (dataType.ContainsAnySubstrings(
			    "BLOB",
			    "UNIQUEIDENTIFIER",
			    "VARBINARY"
		    )
		   ) return "BLOB";

		throw new SoftwareException("Unable to convert data type '{0}' to an Sqlite streams class.", dataType);

	}
}
