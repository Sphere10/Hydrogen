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

public abstract class SQLBuilderCommand {

	public abstract void Execute(ISQLBuilder builder);

	public static SQLBuilderCommand Generic(Action<ISQLBuilder> sqlBuilder) {
		return new GenericActionCommand(sqlBuilder);
	}

	public static SQLBuilderCommand Expression(Type expressionType, string expression, params object[] formatArgs) {
		return new SQLExpressionCommand(expressionType, expression, formatArgs);
	}

	public static SQLBuilderCommand Variable(string variableName) {
		return new VariableNameCommand(variableName);
	}

	public static SQLBuilderCommand Cast(SQLBuilderStringValueKind valueKind, object value, Type type) {
		return new CastCommand(valueKind, value, type);
	}

	public static SQLBuilderCommand TableName(string name, TableType tableType = TableType.Persistent) {
		return new TableNameCommand(name, tableType);
	}

	public static SQLBuilderCommand ColumnName(string name) {
		return new ColumnNameCommand(name);
	}

	public static SQLBuilderCommand TriggerName(string name) {
		return new TriggerNameCommand(name);
	}

	public static SQLBuilderCommand Literal(object value) {
		return new LiteralCommand(value);
	}

	public static SQLBuilderCommand NextSequenceValue(string sequenceName) {
		return new NextSequenceValueCommand(sequenceName);
	}


	public static SQLBuilderCommand LastIdentity(string tableHint = null) {
		return new LastIdentityCommand(tableHint);
	}

	public static SQLBuilderCommand EndOfStatement(SQLStatementType sqlStatementType = SQLStatementType.DML) {
		return new EndOfStatementCommand(sqlStatementType);
	}

	public static SQLBuilderCommand CommaSeparatedValues<T>(IEnumerable<T> values) {
		return Generic(sqlBuilder =>
			values.WithDescriptions().ForEach(
				value => {
					if (value.Index > 0)
						sqlBuilder.Emit(", ");
					sqlBuilder.Literal(value.Item);
				}
			)
		);
	}


}


public sealed class GenericActionCommand : SQLBuilderCommand {
	readonly Action<ISQLBuilder> _action;
	public GenericActionCommand(Action<ISQLBuilder> action) {
		_action = action;
	}
	public override void Execute(ISQLBuilder builder) {
		_action(builder);
	}
}


public sealed class NextSequenceValueCommand : SQLBuilderCommand {
	public NextSequenceValueCommand(string sequenceName) {
		SequenceName = sequenceName;
	}
	public string SequenceName { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.NextSequenceValue(SequenceName);
	}
}


public sealed class TableNameCommand : SQLBuilderCommand {
	public TableNameCommand(string tableName, TableType tableType) {
		TableName = tableName;
		TableType = tableType;
	}
	public string TableName { get; set; }
	public TableType TableType { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.TableName(TableName, TableType);
	}
}


public sealed class ColumnNameCommand : SQLBuilderCommand {
	public ColumnNameCommand(string columnName) {
		ColumnName = columnName;
	}
	public string ColumnName { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.ColumnName(ColumnName);
	}
}


public sealed class TriggerNameCommand : SQLBuilderCommand {
	public TriggerNameCommand(string triggerName) {
		TriggerName = triggerName;
	}
	public string TriggerName { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.TriggerName(TriggerName);
	}
}


public sealed class LastIdentityCommand : SQLBuilderCommand {
	public LastIdentityCommand(string tableHint) {
		TableHint = tableHint;
	}
	public string TableHint { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.GetLastIdentity(TableHint);
	}
}


public sealed class SQLExpressionCommand : SQLBuilderCommand {
	public SQLExpressionCommand(Type expressionResultType, string expression, params object[] formatArgs) {
		Expression = expression;
		ExpessionArgs = formatArgs;
		ResultType = expressionResultType;
	}
	public string Expression { get; set; }
	public object[] ExpessionArgs { get; set; }
	public Type ResultType { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.Emit("(").Emit(Expression, ExpessionArgs).Emit(")");
	}
}


public sealed class VariableNameCommand : SQLBuilderCommand {
	public VariableNameCommand(string variableName) {
		VariableName = variableName;
	}
	public string VariableName { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.VariableName(VariableName);
	}
}


public sealed class LiteralCommand : SQLBuilderCommand {
	public LiteralCommand(object literalValue) {
		LiteralValue = literalValue;
	}
	public object LiteralValue { get; set; }
	public override void Execute(ISQLBuilder builder) {
		builder.Literal(LiteralValue);
	}
}


public sealed class CastCommand : SQLBuilderCommand {
	public CastCommand(SQLBuilderStringValueKind valueKind, object value, Type type) {
		ValueKind = valueKind;
		Value = value;
		Type = type;
	}

	public SQLBuilderStringValueKind ValueKind { get; set; }
	public object Value { get; set; }
	public Type Type { get; set; }

	public override void Execute(ISQLBuilder builder) {
		builder.Cast(ValueKind, Value, Type);
	}
}


public sealed class EndOfStatementCommand : SQLBuilderCommand {

	public EndOfStatementCommand(SQLStatementType sqlStatementType) {
		StatementType = sqlStatementType;
	}

	public SQLStatementType StatementType { get; set; }

	public override void Execute(ISQLBuilder builder) {
		builder.EndOfStatement(StatementType);
	}
}
