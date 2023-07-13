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

public class FirebirdSQLBuilder : SQLBuilderBase {
	private const string DefaultStatementTerminator = "^";
	private volatile int _transactionBlockCount;
	private readonly IDictionary<string, Type> _variableDeclarations;


	public FirebirdSQLBuilder()
		: this(DefaultStatementTerminator) {
	}

	public FirebirdSQLBuilder(string statementTerminator) {
		_transactionBlockCount = 0;
		_variableDeclarations = new Dictionary<string, Type>();
		StatementTerminator = statementTerminator;
	}

	public string StatementTerminator { get; protected set; }

	public override IEnumerable<SQLStatement> Statements {
		get {
			return base.Statements.Select(s => {
				s.SQL = s.SQL.Trim().TrimEnd(DefaultStatementTerminator.ToCharArray());
				return s;
			});
		}
	}

	public override ISQLBuilder Clear() {
		if (_variableDeclarations != null)
			_variableDeclarations.Clear();
		_transactionBlockCount = 0;
		return base.Clear();
	}

	public override ISQLBuilder BeginTransaction() {
		return Emit("SAVEPOINT TRAN" + ++_transactionBlockCount).EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder CommitTransaction() {
		return Emit("COMMIT").EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder RollbackTransaction() {
		return Emit("ROLLBACK TO TRAN" + _transactionBlockCount).EndOfStatement(SQLStatementType.TCL);
	}

	public override ISQLBuilder ObjectName(string objectName) {
		if (objectName.StartsWith("\""))
			return Emit(objectName);

		return Emit("\"").Emit(objectName).Emit("\"");
	}

	public override ISQLBuilder DisableAutoIncrementID(string table) {
		// do nothing as autoincrement triggers do nothing when explicitly passed ID's
		return this;
	}

	public override ISQLBuilder EnableAutoIncrementID(string table) {
		// do nothing as autoincrement triggers do nothing when explicitly passed ID's
		return this;
	}

	public override ISQLBuilder SelectValues(IEnumerable<object> values, string whereClause, params object[] whereClauseFormatArgs) {
		if (!values.Any())
			throw new ArgumentException("values is empty", "values");
		Emit("SELECT ");
		foreach (var value in values.WithDescriptions()) {
			if (value.Index > 0)
				Emit(", ");

			if (value.Item == null) {
				Emit("NULL");
			} else if (value.Item is ColumnValue) {
				var columnValue = value.Item as ColumnValue;
				Emit("{0} AS ", columnValue.Value, SQLBuilderCommand.ColumnName(columnValue.ColumnName));
			} else {
				Literal(value.Item);
			}
		}
		Emit(" FROM RDB$DATABASE");
		if (!string.IsNullOrEmpty(whereClause)) {
			Emit(" WHERE ").Emit(whereClause, whereClauseFormatArgs);
		}
		return this;
	}

	public override ISQLBuilder InsertMany(string table, IEnumerable<string> columns, IEnumerable<IEnumerable<object>> values) {
		//Emit("EXECUTE BLOCK AS BEGIN").NewLine();   the entire script is now a block
		values.ForEach(valueSet => Insert(table, columns.Zip(valueSet, (c, v) => new ColumnValue(c, v))));
		//Emit("END").NewLine();
		return this;
	}

	public override ISQLBuilder NextSequenceValue(string sequenceName) {
		return Emit("NEXT VALUE FOR ").ObjectName(sequenceName);
	}

	public override ISQLBuilder GetLastIdentity(string hint = null) {
		if (string.IsNullOrEmpty(hint)) {
			throw new ArgumentNullException("Firebird SQL Builder requires hint be the sequence name", hint);
		}
		return Emit("gen_id(").ObjectName(hint).Emit(", 0)");
	}

	public override ISQLBuilder DefaultValues() {
		return Emit("DEFAULT VALUES");
	}

	public override ISQLBuilder VariableName(string variableName) {
		return Emit(":").Emit(variableName);
	}

	public override ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null) {
		Emit("FIRST ").Emit(limit.ToString());
		if (offset != null)
			Emit("SKIP ").Emit(offset.Value.ToString());

		return this;
	}

	public override ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML) {
		this.Emit(StatementTerminator).NewLine();
		base.EndOfStatement(statementType);
		return this;
	}

	public override ISQLBuilder Literal(object value) {
		if (value == null || value == DBNull.Value)
			return Emit("NULL");

		TypeSwitch.For(
			value,
			TypeSwitch.Case<Guid>(g => Emit("CHAR_TO_UUID('{0}')", g.ToString().ToUpper())),
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
		_variableDeclarations.Add(variableName, type);
		return this;
	}

	public override ISQLBuilder AssignVariable(string variableName, object value) {
		return
			Emit(variableName) // no need for colon prefix in assignment
				.Emit(" = ")
				.Literal(value)
				.EndOfStatement();
	}

	public override ISQLBuilder Cast(SQLBuilderStringValueKind valueKind, object value, Type type) {
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
			case SQLBuilderStringValueKind.Auto:
				Emit("{0}", value);
				break;
			default:
				throw new SoftwareException("Unsupported SQLBuilderCastValueType {0}", valueKind);
		}
		Emit(" AS ").TypeToSqlType(type).Emit(")");
		return this;
	}

	public override ISQLBuilder CreateBuilder() {
		return new FirebirdSQLBuilder();
	}

	public override ISequentialGuidGenerator CreateSequentialGuidGenerator(long capacity, bool regenerateOnEmpty = false) {
		return new PreGenSequentialGuidGenerator(capacity, regenerateOnEmpty);
	}

	private string GetSequenceName(string table) {
		throw new NotImplementedException();
		throw new SoftwareException("Table '{0}' does not have a sequence associated with it. Make sure you associate sequences to tables before using the firebird SQL Builder");
	}

	public override Type ConvertSQLTypeToType(string sqlType, int? length) {

//BLOB SUB_TYPE BINARY
//BLOB SUB_TYPE TEXT
//CHAR(1) CHARACTER SET ISO8859_1
//CHAR(16) CHARACTER SET OCTETS
//CHAR(20) CHARACTER SET ISO8859_1
//CHAR(60) CHARACTER SET UNICODE_FSS
//DATE
//DOUBLE
//FLOAT
//INT64
//INTEGER
//SMALLINT
//TIME
//TIMESTAMP
//VARCHAR(20) CHARACTER SET ISO8859_1
//VARCHAR(255) CHARACTER SET ISO8859_1
//VARCHAR(60) CHARACTER SET UNICODE_FSS

		sqlType = sqlType.ToUpper();

		if (sqlType == "CHAR(16) CHARACTER SET OCTETS")
			return typeof(Guid);

		if (sqlType.ContainsAnySubstrings("VARCHAR", "CHAR"))
			if (length.HasValue && length.Value == 1)
				return typeof(Char);
			else
				return typeof(String);


		if (sqlType.Contains("BLOB")) {
			return sqlType.Contains("SUB_TYPE TEXT") ? typeof(string) : typeof(Byte[]);
		}

		if (sqlType.IsIn("DATE", "TIMESTAMP", "TIME"))
			return typeof(DateTime);

		if (sqlType.IsIn("DOUBLE"))
			return typeof(Double);

		if (sqlType.IsIn("FLOAT"))
			return typeof(Single);


		if (sqlType.IsIn("SMALLINT"))
			return typeof(Int16);

		if (sqlType.IsIn("INTEGER"))
			return typeof(Int32);

		if (sqlType.IsIn("INT64"))
			return typeof(Int64);

		if (sqlType.ContainsAnySubstrings("NUMERIC", "DECIMAL")) {
			return typeof(Decimal);
		}

		throw new SoftwareException("Unable to convert '{0}{1}' to .NET type", sqlType, length.HasValue ? "- " + length.Value : String.Empty);
	}

	public override string ConvertTypeToSQLType(Type type, bool preferUnicode = true, string maxLen = "4000") {
		string retval = null;
		TypeSwitch.ForType(type,
			TypeSwitch.Case<char>(() => retval = preferUnicode ? "CHAR(1) CHARACTER SET ISO8859_1" : "CHAR(1) CHARACTER SET ASCII"),
			TypeSwitch.Case<string>(() => retval = (preferUnicode ? "VARCHAR({0}) CHARACTER SET ISO8859_1" : "VARCHAR({0}) CHARACTER SET ASCII").FormatWith(maxLen)),
			TypeSwitch.Case<byte>(() => retval = "SMALLINT"),
			TypeSwitch.Case<bool>(() => retval = "CHAR(1)"),
			TypeSwitch.Case<Guid>(() => retval = "CHAR(16) CHARACTER SET OCTETS"),
			TypeSwitch.Case<short>(() => retval = "SMALLINT"),
			TypeSwitch.Case<int>(() => retval = "INTEGER"),
			TypeSwitch.Case<long>(() => retval = "BIGINT"),
			TypeSwitch.Case<decimal>(() => retval = "DECIMAL(18,4)"),
			TypeSwitch.Case<float>(() => retval = "FLOAT"),
			TypeSwitch.Case<double>(() => retval = "DOUBLE"),
			TypeSwitch.Case<DateTime>(() => retval = "TIMESTAMP"),
			TypeSwitch.Case<Byte[]>(() => retval = "BLOB")
		);


		if (retval == null) {
			throw new SoftwareException("Unable to convert type '{0}' to sql type.", type);
		}

		return retval;
	}

	public override string ToString() {
		var query = base.ToString();
		if (StatementCount <= 1) {
			query = query.Trim(); // only trim for small queries as memory may be wasted otherwise
			if (query.EndsWith(StatementTerminator))
				query = query.Substring(0, query.Length - StatementTerminator.Length);
		} else {
			var stringBuilder = new FastStringBuilder();
			stringBuilder.AppendFormat("SET TERM {0};{1}", StatementTerminator, Environment.NewLine);
			if (_variableDeclarations.Count > 0) {
				stringBuilder.AppendLine("EXECUTE BLOCK AS");
				stringBuilder.AppendLine("BEGIN");
				foreach (var variable in _variableDeclarations.Keys) {
					stringBuilder.AppendFormat("DECLARE VARIABLE {0} {1}{2}{3}", variable, ConvertTypeToSQLType(_variableDeclarations[variable]), StatementTerminator, Environment.NewLine);
				}
			}

			stringBuilder.Append(query);
			if (_variableDeclarations.Count > 0) {
				stringBuilder.Append("END");
			}
			query = stringBuilder.ToString();
		}

		return query;
	}

	public override string FormatCreateTable(TableType tableType) {
		switch (tableType) {
			case TableType.InMemory:
				throw new SoftwareException("In memory tables are not supported in Firebird");
			case TableType.Temporary:
				return "CREATE GLOBAL TEMPORARY TABLE";
		}
		return "CREATE TABLE";
	}

	public override string FormatCreateTableEnd(TableType type) {
		if (type == TableType.Temporary)
			return " ON COMMIT PRESERVE ROWS";
		return string.Empty;
	}

}
