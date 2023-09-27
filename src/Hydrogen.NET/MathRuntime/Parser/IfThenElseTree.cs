// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

public class IfThenElseTree : IfThenTree {
	private SyntaxTree _elseExpression;

	public IfThenElseTree()
		: base() {
	}

	public SyntaxTree ElseExpression {
		get { return _elseExpression; }
		set { _elseExpression = value; }
	}

	public override string ToString() {
		return string.Format(
			"IFTHENELSE({0},{1},{2})",
			Condition.ToString(),
			Expression.ToString(),
			ElseExpression.ToString()
		);
	}
}
