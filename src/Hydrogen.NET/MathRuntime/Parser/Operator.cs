// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Maths.Compiler;

public enum Operator {

	[OpAttr(10, Aryness.Binary, Associativity.Left)]
	MemberSelection,

	[OpAttr(9, Aryness.Unary, Associativity.Right)]
	UnaryNot,

	[OpAttr(8, Aryness.Binary, Associativity.Right)]
	Power,

	[OpAttr(7, Aryness.Unary, Associativity.Right)]
	UnaryMinus,

	[OpAttr(7, Aryness.Unary, Associativity.Right)]
	UnaryPlus,

	[OpAttr(6, Aryness.Binary, Associativity.Left)]
	Division,

	[OpAttr(5, Aryness.Binary, Associativity.Left)]
	Multiplication,

	[OpAttr(5, Aryness.Binary, Associativity.Left)]
	Modulus,

	[OpAttr(4, Aryness.Binary, Associativity.Left)]
	Addition,

	[OpAttr(4, Aryness.Binary, Associativity.Left)]
	Subtraction,

	[OpAttr(3, Aryness.Binary, Associativity.Left)]
	LessThan,

	[OpAttr(3, Aryness.Binary, Associativity.Left)]
	GreaterThan,

	[OpAttr(3, Aryness.Binary, Associativity.Left)]
	LessThanEqualTo,

	[OpAttr(3, Aryness.Binary, Associativity.Left)]
	GreaterThanEqualTo,

	[OpAttr(2, Aryness.Binary, Associativity.Left)]
	Equality,

	[OpAttr(2, Aryness.Binary, Associativity.Left)]
	Inequality,

	[OpAttr(1, Aryness.Binary, Associativity.Left)]
	And,

	[OpAttr(0, Aryness.Binary, Associativity.Left)]
	Or,

}


public enum Aryness {
	Unary,
	Binary
}


public class OpAttr : Attribute {
	int _precedence;
	Associativity _associativity;
	Aryness _aryness;


	public OpAttr(int precedence, Aryness aryness, Associativity associativity) {
		Precedence = precedence;
		Associativity = associativity;
		Aryness = aryness;
	}

	public int Precedence {
		get { return _precedence; }
		set { _precedence = value; }
	}

	public Associativity Associativity {
		get { return _associativity; }
		set { _associativity = value; }
	}

	public Aryness Aryness {
		get { return _aryness; }
		set { _aryness = value; }
	}
}
