// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Data;

public class SQLExpression {

	internal SQLExpression(SQLOperator op, IEnumerable<SQLExpression> expressions) {
		Type = SQLExpressionType.Node;
		Operator = op;
		Expressions = expressions;
		ValueKind = SQLExpressionValueType.None;
		Value = null;
	}

	internal SQLExpression(SQLExpressionValueType valueKind, object value) {
		Type = SQLExpressionType.Leaf;
		ValueKind = valueKind;
		Value = value;
	}

	public readonly SQLExpressionType Type;
	public readonly SQLOperator Operator;
	public readonly IEnumerable<SQLExpression> Expressions;
	public readonly SQLExpressionValueType ValueKind;
	public readonly object Value;


}
