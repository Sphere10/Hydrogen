// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using System.IO;
using Hydrogen.Maths.Compiler;
using NUnit.Framework.Legacy;

namespace Hydrogen.UnitTests;

[TestFixture]
public class MathCompilerParserTests {

	/// <summary>
	/// -x-y-z = gives tree:
	///                     -
	///                 -       z
	///             -x      y
	/// </summary>
	[Test]
	public void TestTreeStructure() {
		string exp = "-x-y-z";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		// (-x) - y - z
		ClassicAssert.IsInstanceOf(typeof(BinaryOperatorTree), tree);
		ClassicAssert.AreEqual(Operator.Subtraction, ((BinaryOperatorTree)tree).Operator);
		ClassicAssert.IsInstanceOf(typeof(BinaryOperatorTree),
			((BinaryOperatorTree)tree).LeftHandSide);
		ClassicAssert.AreEqual(Operator.Subtraction,
			((BinaryOperatorTree)((BinaryOperatorTree)tree).LeftHandSide).Operator);
		ClassicAssert.IsInstanceOf(typeof(UnaryOperatorTree),
			((BinaryOperatorTree)((BinaryOperatorTree)tree).LeftHandSide).LeftHandSide);
		ClassicAssert.IsInstanceOf(typeof(FactorTree),
			((BinaryOperatorTree)((BinaryOperatorTree)tree).LeftHandSide).RightHandSide);
		ClassicAssert.AreEqual(Operator.UnaryMinus,
			((UnaryOperatorTree)((BinaryOperatorTree)((BinaryOperatorTree)tree).LeftHandSide).LeftHandSide).Operator);
		;
		ClassicAssert.IsInstanceOf(typeof(FactorTree),
			((UnaryOperatorTree)((BinaryOperatorTree)((BinaryOperatorTree)tree).LeftHandSide).LeftHandSide).Operand);
		;
		ClassicAssert.IsInstanceOf(typeof(FactorTree),
			((BinaryOperatorTree)tree).RightHandSide);
	}

	[Test]
	public void TestUnaryAndPower() {
		string exp = "-x^2.5E-1";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("UnaryMinus(Power(Identifier(x),Scalar(2.5E-1)))", tree.ToString());
	}

	[Test]
	public void TestUnaryAndMultiplication() {
		string exp = "-x*-y";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("Multiplication(UnaryMinus(Identifier(x)),UnaryMinus(Identifier(y)))", tree.ToString());
	}

	[Test]
	public void TestUnaryAndNegativePower() {
		string exp = "-x^-y";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("UnaryMinus(Power(Identifier(x),UnaryMinus(Identifier(y))))", tree.ToString());
	}

	[Test]
	public void TestUnaryAndNegativePowerTower() {
		string exp = "-x^-y^-z";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("UnaryMinus(Power(Identifier(x),UnaryMinus(Power(Identifier(y),UnaryMinus(Identifier(z))))))", tree.ToString());
	}

	[Test]
	public void TestMultiplicationDivision() {
		string exp = "x*y/z";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("Multiplication(Identifier(x),Division(Identifier(y),Identifier(z)))", tree.ToString());
	}

	[Test]
	public void TestDivisionDivision() {
		string exp = "x/y/z";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("Division(Division(Identifier(x),Identifier(y)),Identifier(z))", tree.ToString());
	}

	[Test]
	public void TestDivisionMultiplication() {
		string exp = "x/y*z";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("Multiplication(Division(Identifier(x),Identifier(y)),Identifier(z))", tree.ToString());
	}

	[Test]
	public void TestAdditionSub() {
		string exp = "x+y-z";
		Parser parser = new Parser(new Scanner(new StringReader(exp)));
		SyntaxTree tree = parser.ParseExpression();
		ClassicAssert.AreEqual("Subtraction(Addition(Identifier(x),Identifier(y)),Identifier(z))", tree.ToString());
	}

}
