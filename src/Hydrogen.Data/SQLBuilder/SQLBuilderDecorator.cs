// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Data;

public abstract class SQLBuilderDecorator : ISQLBuilder {
	private readonly ISQLBuilder _internalBuilder;

	protected SQLBuilderDecorator(ISQLBuilder internalBuilder) {
		_internalBuilder = internalBuilder;
		LeafBuilder = this;
	}

	protected ISQLBuilder DecoratedBuilder {
		get { return _internalBuilder; }
	}

	public ISQLBuilder LeafBuilder {
		get { return _internalBuilder.LeafBuilder; }
		set { _internalBuilder.LeafBuilder = value; }
	}

	public virtual ICollection<SQLStatement> CreateContainerForStatements() {
		return _internalBuilder.CreateContainerForStatements();
	}

	public virtual ISQLBuilder Clear() {
		_internalBuilder.Clear();
		return this;
	}

	public virtual ISQLBuilder NewLine() {
		_internalBuilder.NewLine();
		return this;
	}

	public virtual ISQLBuilder End() {
		_internalBuilder.End();
		return this;
	}

	public virtual ISQLBuilder BeginTransaction() {
		_internalBuilder.BeginTransaction();
		return this;
	}

	public virtual ISQLBuilder CommitTransaction() {
		_internalBuilder.CommitTransaction();
		return this;
	}

	public virtual ISQLBuilder RollbackTransaction() {
		_internalBuilder.RollbackTransaction();
		return this;
	}

	public virtual ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML) {
		_internalBuilder.EndOfStatement(statementType);
		return this;
	}

	public virtual ISQLBuilder DisableAutoIncrementID(string table) {
		_internalBuilder.DisableAutoIncrementID(table);
		return this;
	}

	public virtual ISQLBuilder EnableAutoIncrementID(string table) {
		_internalBuilder.EnableAutoIncrementID(table);
		return this;
	}

	public ISQLBuilder CreateTable(TableSpecification tableSpecification) {
		_internalBuilder.CreateTable(tableSpecification);
		return this;
	}

	public ISQLBuilder DropTable(string tableName, TableType tableType) {
		_internalBuilder.DropTable(tableName, tableType);
		return this;
	}

	public ISQLBuilder SelectValues(IEnumerable<object> values, string whereClause, params object[] whereClauseFormatArgs) {
		_internalBuilder.SelectValues(values, whereClause, whereClauseFormatArgs);
		return this;
	}

	public virtual ISQLBuilder Select(string tableName, IEnumerable<object> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                  string orderByClause = null, bool endStatement = false) {
		_internalBuilder.Select(tableName, columns: columns, distinct: distinct, limit: limit, offset: offset, columnMatches: columnMatches, whereClause: whereClause, orderByClause: orderByClause, endStatement: endStatement);
		return this;
	}

	public virtual ISQLBuilder Insert(string table, IEnumerable<ColumnValue> values) {
		_internalBuilder.Insert(table, values);
		return this;
	}

	public ISQLBuilder InsertMany(string table, IEnumerable<string> columns, IEnumerable<IEnumerable<object>> values) {
		_internalBuilder.InsertMany(table, columns, values);
		return this;
	}

	public virtual ISQLBuilder Update(string table, IEnumerable<ColumnValue> setColumns, IEnumerable<ColumnValue> matchColumns = null, string whereClause = null) {
		_internalBuilder.Update(table, setColumns, matchColumns: matchColumns, whereClause: whereClause);
		return this;
	}

	public virtual ISQLBuilder Delete(string table, IEnumerable<ColumnValue> matchColumns) {
		_internalBuilder.Delete(table, matchColumns);
		return this;
	}

	public ISQLBuilder TableName(string tableName, TableType tableType) {
		_internalBuilder.TableName(tableName, tableType);
		return this;
	}

	public virtual ISQLBuilder ObjectName(string objectName) {
		_internalBuilder.TableName(objectName);
		return this;
	}

	public virtual ISQLBuilder Literal(object value) {
		_internalBuilder.Literal(value);
		return this;
	}

	public ISQLBuilder NextSequenceValue(string sequenceName) {
		_internalBuilder.NextSequenceValue(sequenceName);
		return this;
	}

	public virtual ISQLBuilder GetLastIdentity(string hint) {
		_internalBuilder.GetLastIdentity(hint);
		return this;
	}

	public virtual ISQLBuilder DefaultValues() {
		_internalBuilder.DefaultValues();
		return this;
	}

	public virtual ISQLBuilder VariableName(string variableName) {
		_internalBuilder.VariableName(variableName);
		return this;
	}

	public virtual ISQLBuilder DeclareVariable(string variableName, Type type) {
		_internalBuilder.DeclareVariable(variableName, type);
		return this;
	}

	public virtual ISQLBuilder AssignVariable(string variableName, object value) {
		_internalBuilder.AssignVariable(variableName, value);
		return this;
	}

	public virtual ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null) {
		_internalBuilder.EmitQueryResultLimit(limit, offset: offset);
		return this;
	}

	public virtual ISQLBuilder EmitDistinct() {
		_internalBuilder.EmitDistinct();
		return this;
	}

	public virtual ISQLBuilder Emit(string statement, params object[] formatArgs) {
		_internalBuilder.Emit(statement, formatArgs);
		return this;
	}

	public virtual ISQLBuilder TypeToSqlType(Type type, bool preferUnicode = true) {
		_internalBuilder.TypeToSqlType(type, preferUnicode);
		return this;
	}

	public virtual ISQLBuilder Cast(SQLBuilderStringValueKind valueKind, object value, Type type) {
		_internalBuilder.Cast(valueKind, value, type);
		return this;
	}

	public override string ToString() {
		return _internalBuilder.ToString();
	}

	public IEnumerable<SQLStatement> Statements {
		get { return _internalBuilder.Statements; }
	}

	public virtual ISQLBuilder CreateBuilder() {
		return _internalBuilder.CreateBuilder();
	}

	public virtual ISequentialGuidGenerator CreateSequentialGuidGenerator(long capacity, bool regenerateOnEmpty = false) {
		return _internalBuilder.CreateSequentialGuidGenerator(capacity, regenerateOnEmpty);
	}

	public virtual int StatementCount {
		get { return _internalBuilder.StatementCount; }
		set { _internalBuilder.StatementCount = value; }
	}

	public virtual int VariableDeclarationCount {
		get { return _internalBuilder.VariableDeclarationCount; }
		set { _internalBuilder.VariableDeclarationCount = value; }
	}

	public string ConvertTypeToSQLType(Type type, bool preferUnicode = true, string maxLen = "4000") {
		return _internalBuilder.ConvertTypeToSQLType(type, preferUnicode, maxLen);
	}

	public Type ConvertSQLTypeToType(string sqlType, int? length) {
		return _internalBuilder.ConvertSQLTypeToType(sqlType, length);
	}

	public string FormatCreateTable(TableType tableType) {
		return _internalBuilder.FormatCreateTable(tableType);
	}

	public string FormatCreateTableEnd(TableType type) {
		return _internalBuilder.FormatCreateTableEnd(type);
	}
}
