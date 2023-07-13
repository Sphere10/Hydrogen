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

public sealed class BatchSQLBuilder : SQLBuilderDecorator {
	private const string BatchDelimitter = "Æº¹ºÆ";
	private readonly uint _batchSize;

	public BatchSQLBuilder(ISQLBuilder decoratedBuilder, uint batchSize)
		: base(decoratedBuilder) {
		_batchSize = Tools.Values.ClipValue(batchSize, 1, uint.MaxValue - 1);
	}

	public override ISQLBuilder Clear() {
		base.Clear();
		return this;
	}


	public override ISQLBuilder BeginTransaction() {
		throw new SoftwareException("Transaction should be scoped/declared in code when using BatchSQLBuilder");
	}

	public override ISQLBuilder CommitTransaction() {
		throw new SoftwareException("Transaction should be scoped/declared in code when using BatchSQLBuilder");
	}

	public override ISQLBuilder RollbackTransaction() {
		throw new SoftwareException("Transaction should be scoped/declared in code when using BatchSQLBuilder");
	}

	public override ISQLBuilder EndOfStatement(SQLStatementType statementType = SQLStatementType.DML) {
		try {
			return base.EndOfStatement(statementType);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder DisableAutoIncrementID(string table) {
		try {
			return base.DisableAutoIncrementID(table);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder EnableAutoIncrementID(string table) {
		try {
			return base.EnableAutoIncrementID(table);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Select(string tableName, IEnumerable<object> columns = null, bool distinct = false, int? limit = null, int? offset = null, IEnumerable<ColumnValue> columnMatches = null, string whereClause = null,
	                                   string orderByClause = null, bool endStatement = false) {
		try {
			return base.Select(tableName, columns: columns, distinct: distinct, limit: limit, columnMatches: columnMatches, whereClause: whereClause, orderByClause: orderByClause);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Insert(string table, IEnumerable<ColumnValue> values) {
		try {
			return base.Insert(table, values);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Update(string table, IEnumerable<ColumnValue> setColumns, IEnumerable<ColumnValue> matchColumns = null, string whereClause = null) {
		try {
			return base.Update(table, setColumns, matchColumns: matchColumns, whereClause: whereClause);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Delete(string table, IEnumerable<ColumnValue> matchColumns) {
		try {
			return base.Delete(table, matchColumns);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder ObjectName(string objectName) {
		try {
			return base.ObjectName(objectName);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Literal(object value) {
		try {
			return base.Literal(value);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder GetLastIdentity(string hint) {
		try {
			return base.GetLastIdentity(hint);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder DefaultValues() {
		try {
			return base.DefaultValues();
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder VariableName(string variableName) {
		try {
			return base.VariableName(variableName);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder DeclareVariable(string variableName, Type type) {
		try {
			return base.DeclareVariable(variableName, type);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder AssignVariable(string variableName, object value) {
		try {
			return base.AssignVariable(variableName, value);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null) {
		try {
			return base.EmitQueryResultLimit(limit, offset: offset);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder EmitDistinct() {
		try {
			return base.EmitDistinct();
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Emit(string statement, params object[] formatArgs) {
		try {
			return base.Emit(statement, formatArgs);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder TypeToSqlType(Type type, bool preferUnicode = true) {
		try {
			return base.TypeToSqlType(type, preferUnicode);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override ISQLBuilder Cast(SQLBuilderStringValueKind valueKind, object value, Type type) {
		try {
			return base.Cast(valueKind, value, type);
		} finally {
			AddDelimiterIfApplicable();
		}
	}

	public override string ToString() {
		throw new SoftwareException("The ToString method is disabled in BatchSQLBuilder, use ToStringBatches instead.");
	}

	public IEnumerable<string> ToStringBatches() {
		return base.ToString().Split(new[] { BatchDelimitter }, StringSplitOptions.RemoveEmptyEntries);
	}

	public override ISQLBuilder CreateBuilder() {
		return new BatchSQLBuilder(DecoratedBuilder.CreateBuilder(), _batchSize);
	}

	private void AddDelimiterIfApplicable() {
		if (StatementCount >= _batchSize) {
			base.Emit(BatchDelimitter);
			StatementCount = 0;
		}
	}

}
