// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System.Diagnostics;

namespace Hydrogen.Maths.Compiler;

public class UnaryOperatorTree : SyntaxTree {
	private SyntaxTree _operand;
	private Operator _operator;


	public UnaryOperatorTree(Token token)
		: base(token) {
	}

	public SyntaxTree Operand {
		get { return _operand; }
		set { _operand = value; }
	}

	public Operator Operator {
		get { return _operator; }
		set { _operator = value; }

	}

	public override string ToString() {
		Debug.Assert(_operand != null);
		return string.Format("{0}({1})", Operator, Operand.ToString());
	}
}
