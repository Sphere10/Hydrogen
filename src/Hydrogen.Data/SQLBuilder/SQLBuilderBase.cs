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
using System.Data;
using System.Linq;
using System.Text;

namespace Hydrogen.Data;

// Need to change implementation
// - InternalStatements gets appended on statement finish/tostring/iteration 
// - PreviousStatements is a page-cached list
public abstract class SQLBuilderBase : ISQLBuilder {
	private const int ScriptPageSize = 1 * 1000000; // 1 MB

	// Need to use a paged list here of SQLStatementType
	protected ICollection<SQLStatement> PreviousStatements;
	protected Tuple<SQLStatementType, FastStringBuilder> CurrentStatement;
	private int _tabLevel = 0;

	protected SQLBuilderBase() {
		LeafBuilder = this;
		Clear();
	}

	public int StatementCount { get; set; }

	public int VariableDeclarationCount { get; set; }

	public ISQLBuilder LeafBuilder { get; set; }

	public virtual ISQLBuilder Clear() {
		if (PreviousStatements != null) {
			PreviousStatements.Clear();
		}
		PreviousStatements = LeafBuilder.CreateContainerForStatements();
		CurrentStatement = Tuple.Create(SQLStatementType.DML, new FastStringBuilder());
		StatementCount = -1;
		VariableDeclarationCount = 0;
		AdvanceNextStatement();
		return this;
	}

	public virtual ICollection<SQLStatement> CreateContainerForStatements() {
		return new List<SQLStatement>();
	}

	public virtual ISQLBuilder NewLine() {
		Emit(Environment.NewLine);
		return EmitTabs();
	}

	public virtual ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML) {
		SetCurrentStatementType(statementType);
		AdvanceNextStatement();
		return this;
	}

	public virtual ISQLBuilder End() {
		return this;
	}

	public abstract ISQLBuilder BeginTransaction();

	public abstract ISQLBuilder CommitTransaction();

	public abstract ISQLBuilder RollbackTransaction();

	public abstract ISQLBuilder DisableAutoIncrementID(string table);

	public abstract ISQLBuilder EnableAutoIncrementID(string table);

	public virtual ISQLBuilder CreateTable(TableSpecification tableSpecification) {
		bool singularyPrimaryKey = tableSpecification.PrimaryKey.Columns.Length == 1;
		bool hasDeclarationsAfterColumns = !singularyPrimaryKey;
		Emit("{0} ", FormatCreateTable(tableSpecification.Type))
			.TableName(tableSpecification.Name, tableSpecification.Type)
			.Emit("(")
			.NewLine();
		this.TabRight();
		foreach (var column in tableSpecification.Columns.WithDescriptions()) {
			Emit(
					"{0} {1}{2}{3}{4}",
					SQLBuilderCommand.ColumnName(column.Item.Name),
					!string.IsNullOrEmpty(column.Item.DataType) ? column.Item.DataType : ConvertTypeToSQLType(column.Item.Type),
					singularyPrimaryKey && column.Item.Name == tableSpecification.PrimaryKey.Columns[0] ? " PRIMARY KEY" : string.Empty,
					!column.Item.Nullable ? " NOT NULL" : string.Empty,
					(column.Description.HasFlag(EnumeratedItemDescription.Last) && !hasDeclarationsAfterColumns) ? string.Empty : ", "
				)
				.NewLine();
		}
		if (!singularyPrimaryKey) {
			Emit("PRIMARY KEY(");
			foreach (var primaryKeyColumn in tableSpecification.PrimaryKey.Columns.WithDescriptions()) {
				if (primaryKeyColumn.Description != EnumeratedItemDescription.Last)
					Emit(", ");
				this.ColumnName(primaryKeyColumn.Item);
			}
		}
		this.Untab();
		Emit(")");
		Emit(FormatCreateTableEnd(tableSpecification.Type));
		return EndOfStatement(SQLStatementType.DDL);
	}

	public virtual ISQLBuilder DropTable(string tableName, TableType tableType) {
		return
			Emit("DROP TABLE {0}", FormatTableName(tableName, tableType))
				.EndOfStatement();
	}

	public virtual ISQLBuilder SelectValues(IEnumerable<object> values, string whereClause, params object[] whereClauseFormatArgs) {
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
		if (!string.IsNullOrEmpty(whereClause)) {
			Emit(" WHERE ").Emit(whereClause, whereClauseFormatArgs);
		}
		return this;
	}

	public virtual ISQLBuilder Select(string tableName, IEnumerable<object> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                  string orderByClause = null, bool endStatement = false) {


		Emit("SELECT ");
		if (limit.HasValue)
			EmitQueryResultLimit(limit.Value, offset)
				.Emit(" ");

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
				if ((w.Item.Value is IEnumerable && !(w.Item.Value is string) && !(w.Item.Value is byte[])) || w.Item.Value is SQLExpressionCommand) {
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

		if (endStatement)
			EndOfStatement();

		return this;
	}

	public virtual ISQLBuilder Insert(string table, IEnumerable<ColumnValue> values) {
		Emit("INSERT INTO ").TableName(table);

		if (values.Any()) {
			Emit(" (");
			values.WithDescriptions().ForEach(
				v => {
					if (!v.Description.HasFlag(EnumeratedItemDescription.First))
						Emit(", ");

					this.ColumnName(v.Item.ColumnName);
				}
			);
			Emit(") VALUES (");
			values.WithDescriptions().ForEach(
				v => {
					if (!v.Description.HasFlag(EnumeratedItemDescription.First))
						Emit(", ");
					Literal(v.Item.Value);
				}
			);
			Emit(")");
		} else {
			Emit(" ").DefaultValues();
		}

		return EndOfStatement();
	}

	public virtual ISQLBuilder InsertMany(string table, IEnumerable<string> columns, IEnumerable<IEnumerable<object>> values) {
		Emit("INSERT INTO ").TableName(table).Emit("(");


		foreach (var columnName in columns.WithDescriptions()) {
			if (columnName.Index > 0)
				Emit(", ");
			this.ColumnName(columnName.Item);
		}
		this.Emit(")").NewLine();

		foreach (var valueSet in values.WithDescriptions()) {
			if (valueSet.Index > 0) {
				this.NewLine().Emit("UNION ALL").NewLine();
			}
			Emit("SELECT ");
			foreach (var value in valueSet.Item.WithDescriptions()) {
				if (value.Index > 0)
					Emit(", ");
				Literal(value.Item);
			}
		}


		return EndOfStatement();
	}

	public virtual ISQLBuilder Update(string table, IEnumerable<ColumnValue> setColumns, IEnumerable<ColumnValue> whereColumns = null, string whereClause = null) {
		if (!setColumns.Any())
			return this;

		Emit("UPDATE ").TableName(table).Emit(" SET ");
		setColumns.WithDescriptions().ForEach(
			v => {
				if (!v.Description.HasFlag(EnumeratedItemDescription.First))
					Emit(", ");

				this.ColumnName(v.Item.ColumnName).Emit(" = ").Literal(v.Item.Value);
			}
		);

		bool emittedWhere = false;
		if (whereColumns != null && whereColumns.Any()) {
			Emit(" WHERE ");
			emittedWhere = true;
			whereColumns.WithDescriptions().ForEach(
				v => {
					if (!v.Description.HasFlag(EnumeratedItemDescription.First))
						Emit(" AND ");
					if (v.Item.Value is IEnumerable && !(v.Item.Value is string)) {
						this.ColumnName(v.Item.ColumnName).Emit(" IN (");
						int i = 0;
						foreach (var obj in ((IEnumerable)v.Item.Value)) {
							if (i++ > 0)
								Emit(", ");
							Literal(obj);
						}
						Emit(")");
					} else {
						this.ColumnName(v.Item.ColumnName).Emit(" = ").Literal(v.Item.Value);
					}
				}
			);
		}

		if (!string.IsNullOrEmpty(whereClause)) {
			if (!emittedWhere)
				Emit(" WHERE ");
			else
				Emit(" AND (");

			Emit(whereClause);

			if (emittedWhere)
				Emit(")");

		}


		return EndOfStatement();
	}

	public virtual ISQLBuilder Delete(string table, IEnumerable<ColumnValue> matchColumns = null) {

		Emit("DELETE FROM ").TableName(table);

		if (matchColumns != null && matchColumns.Any()) {
			Emit(" WHERE ");
			matchColumns.WithDescriptions().ForEach(
				v => {
					if (!v.Description.HasFlag(EnumeratedItemDescription.First))
						Emit(" AND ");
					if (v.Item.Value is IEnumerable && !(v.Item.Value is string)) {
						this.ColumnName(v.Item.ColumnName).Emit(" IN (");
						int i = 0;
						foreach (var obj in ((IEnumerable)v.Item.Value)) {
							if (i++ > 0)
								Emit(", ");
							Literal(obj);
						}
						Emit(")");
					} else {

						this.ColumnName(v.Item.ColumnName).Emit(" = ").Literal(v.Item.Value);
					}
				}
			);
		}
		return EndOfStatement();
	}

	public virtual ISQLBuilder TableName(string tableName, TableType tableType) {
		return ObjectName(tableName);
	}

	public virtual ISQLBuilder ObjectName(string objectName) {
		return Emit("\"").Emit(objectName).Emit("\"");
	}

	public virtual ISQLBuilder Literal(object value) {
		if (value == null || value == DBNull.Value)
			return Emit("NULL");

		TypeSwitch.For(value,
			TypeSwitch.Case<SQLBuilderCommand>(c => { c.Execute(this.LeafBuilder); }),
			TypeSwitch.Default(() => Emit(Tools.Object.ToSQLString(value)))
		);
		return this;
	}

	public abstract ISQLBuilder NextSequenceValue(string sequenceName);

	public abstract ISQLBuilder GetLastIdentity(string hint = null);

	public virtual ISQLBuilder DefaultValues() {
		return Emit("DEFAULT VALUES");
	}

	public abstract ISQLBuilder VariableName(string variableName);

	public abstract ISQLBuilder DeclareVariable(string variableName, Type type);

	public abstract ISQLBuilder AssignVariable(string variableName, object value);

	public abstract ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null);

	public virtual ISQLBuilder EmitDistinct() {
		return Emit("DISTINCT");
	}

	public virtual ISQLBuilder Emit(string statement, params object[] formatArgs) {
		if (formatArgs.Length > 0) {
			var formatArgs2 = formatArgs.Select(o => {
				if (o is ISQLBuilder)
					return ((ISQLBuilder)o).ToString();

				if (o is SQLBuilderCommand) {
					var command = o as SQLBuilderCommand;
					var builder = LeafBuilder.CreateBuilder();
					command.Execute(builder);
					return builder.ToString();
					/*var command = o as SQLBuilderCommand;
				var stringBuilder = _currentStatement.Item2;
				var currentLength = stringBuilder.Length;
				command.TargetBuilder = command.TargetBuilder ?? this;		// decorator's set the target to themselves if they need to
				command.Execute();
				return (object)stringBuilder.ChopFromEnd(stringBuilder.Length - currentLength);*/
				}
				return o;
			});
			CurrentStatement.Item2.AppendFormat(statement, formatArgs2.ToArray());
		} else {
			CurrentStatement.Item2.Append(statement);
		}
		return this;
	}

	public virtual ISQLBuilder TypeToSqlType(Type type, bool preferUnicode = true) {
		return Emit(ConvertTypeToSQLType(type, preferUnicode));
	}

	public virtual ISQLBuilder Cast(SQLBuilderStringValueKind valueKind, object value, Type type) {
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

	public override string ToString() {
		var totalLength = PreviousStatements.Sum(s => s.SQL.Length) + CurrentStatement.Item2.Length;
		var stringBuilder = new StringBuilder(totalLength + Environment.NewLine.Length * StatementCount + 1024);
		foreach (var statement in PreviousStatements) {
			stringBuilder.Append(statement.SQL);
		}
		if (CurrentStatement.Item2.Length > 0)
			stringBuilder.Append(CurrentStatement.Item2);

		return stringBuilder.ToString();
	}

	public virtual IEnumerable<SQLStatement> Statements {
		get {
			// This needs to be changed to iterate the paged list
			IEnumerable<SQLStatement> statements = PreviousStatements;
			if (CurrentStatement.Item2.Length > 0)
				statements = statements.Concat(new SQLStatement { Type = CurrentStatement.Item1, SQL = CurrentStatement.Item2.ToString() });
			return statements;
		}
	}

	public abstract ISQLBuilder CreateBuilder();

	public virtual ISequentialGuidGenerator CreateSequentialGuidGenerator(long capacity, bool regenerateOnEmpty = false) {
		return new PreGenSequentialGuidGenerator(capacity, regenerateOnEmpty);
	}

	public virtual string ConvertTypeToSQLType(Type type, bool preferUnicode = true, string maxLen = "4000") {
		string retval = null;
		TypeSwitch.ForType(type,
			TypeSwitch.Case<Char>(() => retval = preferUnicode ? "NCHAR" : "CHAR"),
			TypeSwitch.Case<String>(() => retval = preferUnicode ? "NVARCHAR(MAX)" : "VARCHAR(MAX)"),
			TypeSwitch.Case<Byte>(() => retval = "TINYINT"),
			TypeSwitch.Case<Boolean>(() => retval = "BIT"),
			TypeSwitch.Case<Guid>(() => retval = "UNIQUEIDENTIFIER"),
			TypeSwitch.Case<Int16>(() => retval = "SMALLINT"),
			TypeSwitch.Case<Int32>(() => retval = "INT"),
			TypeSwitch.Case<Int64>(() => retval = "BIGINT"),
			TypeSwitch.Case<Decimal>(() => retval = "MONEY"),
			TypeSwitch.Case<Single>(() => retval = "REAL"),
			TypeSwitch.Case<Double>(() => retval = "FLOAT"),
			TypeSwitch.Case<DateTime>(() => retval = "DATETIME")
		);


		if (retval == null) {
			throw new SoftwareException("Unable to convert type '{0}' to sql type.", type);
		}

		return retval;
	}

	public virtual Type ConvertSQLTypeToType(string sqlType, int? length) {
		sqlType = sqlType.ToUpper();
		if (sqlType.IsIn("CHARACTER", "CHAR", "CHARACTER VARYING", "VARCHAR", "NATIONAL CHARACTER", "NCHAR", "NATIONAL CHARACTER VARYING", "NVARCHAR")) {
			if (length.HasValue && length.Value == 1)
				return typeof(Char);
			return typeof(String);
		}

		if (sqlType.IsIn("BIT", "BIT VARYING")) {
			if (length.HasValue && length.Value == 1)
				return typeof(Boolean);
			return typeof(BitArray);
		}

		if (sqlType.IsIn("SMALLINT"))
			return typeof(Int16);

		if (sqlType.IsIn("INTEGER"))
			return typeof(Int32);

		if (sqlType.IsIn("FLOAT", "REAL"))
			return typeof(Single);

		if (sqlType.IsIn("DOUBLE PRECISION"))
			return typeof(Double);

		if (sqlType.IsIn("NUMERIC"))
			return typeof(Decimal);

		if (sqlType.IsIn("DATE", "TIME", "TIME WITH TIME ZONE", "TIMETZ", "TIMESTAMP", "TIMESTAMP WITH TIME ZONE", "TIMESTAMPTZ"))
			return typeof(DateTime);

		throw new SoftwareException("Unable to convert '{0}{1}' to .NET type", sqlType, length.HasValue ? "- " + length.Value : String.Empty);
	}

	public virtual string FormatCreateTable(TableType tableType) {
		return "CREATE TABLE";
	}

	public virtual string FormatTableName(string tableName, TableType tableType) {
		return this.QuickString("{0}", SQLBuilderCommand.TableName(tableName));
	}

	public virtual string FormatCreateTableEnd(TableType type) {
		return string.Empty;
	}

	#region Auxillary methods

	protected ISQLBuilder SetCurrentStatementType(SQLStatementType statementType) {
		CurrentStatement = Tuple.Create(statementType, CurrentStatement.Item2);
		return this;
	}


	protected ISQLBuilder TabRight() {
		_tabLevel++.ClipTo(0, int.MaxValue);
		return this;
	}

	protected ISQLBuilder Untab() {
		_tabLevel = (_tabLevel - 1).ClipTo(0, int.MaxValue);
		return this;
	}

	protected ISQLBuilder EmitTabs() {
		Tools.Collection.Repeat(() => Emit("\t"), _tabLevel);
		return this;
	}

	protected void AdvanceNextStatement() {
		if (CurrentStatement.Item2.Length > 0) {
			PreviousStatements.Add(new SQLStatement { Type = CurrentStatement.Item1, SQL = CurrentStatement.Item2.ToString() });
		}
		CurrentStatement = Tuple.Create(SQLStatementType.DML, new FastStringBuilder());
		StatementCount++;
	}

	protected IEnumerable<ColumnValue> GetColumnValuePairs(DataRow row, bool excludePrimaryKeyColumns = true) {
		var columns = row.Table.Columns.Cast<DataColumn>();
		if (excludePrimaryKeyColumns)
			columns = columns.Except(row.Table.PrimaryKey);

		return GetColumnValuePairs(columns, row);
	}

	protected IEnumerable<ColumnValue> GetColumnValuePairs(IEnumerable<DataColumn> dataColumns, DataRow row) {
		return
			from DataColumn column in dataColumns
			select new ColumnValue(
				column.ColumnName,
				row[column.ColumnName]
			);
	}

	#endregion

}
