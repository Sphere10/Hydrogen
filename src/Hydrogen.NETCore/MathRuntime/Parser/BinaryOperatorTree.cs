// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;

namespace Hydrogen.Maths.Compiler;

public class BinaryOperatorTree : SyntaxTree {
	private SyntaxTree _leftHandSide;
	private SyntaxTree _rightHandSide;
	private Operator _operator;


	public BinaryOperatorTree(Token token)
		: base(token) {
	}

	public Operator Operator {
		get { return _operator; }
		set { _operator = value; }
	}

	public SyntaxTree LeftHandSide {
		get { return _leftHandSide; }
		set { _leftHandSide = value; }
	}

	public SyntaxTree RightHandSide {
		get { return _rightHandSide; }
		set { _rightHandSide = value; }
	}

	public override string ToString() {
		Debug.Assert(_leftHandSide != null);
		Debug.Assert(_rightHandSide != null);
		return string.Format("{0}({1},{2})", Operator, LeftHandSide.ToString(), RightHandSide.ToString());
	}
}
