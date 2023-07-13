// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

public class SQL {

	public static SQLExpression SchemaName(string name) {
		return new SQLExpression(SQLExpressionValueType.SchemaName, name);
	}

	public static SQLExpression TableName(string name) {
		return new SQLExpression(SQLExpressionValueType.TableName, name);
	}

	public static SQLExpression ColumnName(string name) {
		return new SQLExpression(SQLExpressionValueType.ColumnName, name);
	}


	public static SQLExpression Literal(object value) {
		return new SQLExpression(SQLExpressionValueType.Literal, value);
	}

	public static SQLExpression Variable(string variableName) {
		return new SQLExpression(SQLExpressionValueType.VariableName, variableName);
	}

	public static SQLExpression RawSQL(string sql) {
		return new SQLExpression(SQLExpressionValueType.SQL, sql);
	}


	public static SQLExpression IsNull(SQLExpression expression) {
		return new SQLExpression(SQLOperator.IsNull, new[] { expression });
	}

	public static SQLExpression IsNull(string sql) {
		return IsNull(RawSQL(sql));
	}


	public static SQLExpression IsNotNull(SQLExpression expression) {
		return new SQLExpression(SQLOperator.IsNotNull, new[] { expression });
	}


	public static SQLExpression IsNotNull(string sql) {
		return IsNotNull(RawSQL(sql));
	}


	public static SQLExpression Negative(SQLExpression expression) {
		return new SQLExpression(SQLOperator.Minus, new[] { expression });
	}

	public static SQLExpression Negative(string sql) {
		return Negative(RawSQL(sql));
	}

	public static SQLExpression Positive(SQLExpression expression) {
		return new SQLExpression(SQLOperator.Plus, new[] { expression });
	}

	public static SQLExpression Positive(string sql) {
		return Positive(RawSQL(sql));
	}

	public static SQLExpression BitwiseComplement(SQLExpression expression) {
		return new SQLExpression(SQLOperator.BitwiseComplement, new[] { expression });
	}


	public static SQLExpression Equals(SQLExpression left, SQLExpression right) {
		return new SQLExpression(SQLOperator.Equal, new[] { left, right });
	}

	public static SQLExpression In(SQLExpression left, SQLExpression right) {
		return new SQLExpression(SQLOperator.In, new[] { left, right });
	}

	public static SQLExpression In(string aliasLeft, SQLExpression right) {
		return In(ColumnName(aliasLeft), right);
	}


	public static SQLExpression Like(SQLExpression left, SQLExpression right) {
		return new SQLExpression(SQLOperator.Like, new[] { left, right });
	}

	public static SQLExpression Plus(params SQLExpression[] expressions) {
		return new SQLExpression(SQLOperator.Plus, expressions);
	}

	public static SQLExpression Mul(params SQLExpression[] expressions) {
		return new SQLExpression(SQLOperator.Mul, expressions);
	}

	public static SQLExpression Div(params SQLExpression[] expressions) {
		return new SQLExpression(SQLOperator.Div, expressions);
	}


	public static SQLExpression InnerJoin(params SQLExpression[] expressions) {
		throw new NotImplementedException();
	}


	public static SQLExpression LeftJoin(params SQLExpression[] expressions) {
		throw new NotImplementedException();
	}


}

/*public class AndExpression : BaseExpression {

	public AndExpression(IEnumerable<BaseExpression> expressions) {
	    Expressions = expressions;
	}

	public IEnumerable<BaseExpression> Expressions;
}


public class OrExpression : BaseExpression {

    public OrExpression(IEnumerable<BaseExpression> expressions) {
        Expressions = expressions;
    }

    public IEnumerable<BaseExpression> Expressions;
}


public class NotExpression {

    public BaseExpression Expression;
}

public class OrExpression {
    public OrExpression(BaseExpression left, BaseExpression right) {
	    LeftHandSide = left;
	    RightHandSide = right;
	}

    public BaseExpression LeftHandSide { get; set; }

    public BaseExpression RightHandSide { get; set; }
}*/
