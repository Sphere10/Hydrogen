//-----------------------------------------------------------------------
// <copyright file="ScannerTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using NUnit.Framework;
using System.IO;
using Hydrogen.Maths.Compiler;

namespace Hydrogen.UnitTests {

    [TestFixture]
    public class MathCompilerScannerTests {

        [Test]
        public void TestComplexTokenSequence() {
            string text = "ID1E-1E-3++";
            Scanner scanner = new Scanner(new StringReader(text));
            Assert.AreEqual(TokenType.Identifier, scanner.GetNextToken().TokenType);
            Assert.AreEqual(TokenType.Minus, scanner.GetNextToken().TokenType);
            Assert.AreEqual(TokenType.Scalar, scanner.GetNextToken().TokenType);
            Assert.AreEqual(TokenType.Plus, scanner.GetNextToken().TokenType);
            Assert.AreEqual(TokenType.Plus, scanner.GetNextToken().TokenType);
        }
        

        [Test]
        public void TestNumber() {
            string text = "5";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Scalar,token.TokenType);
            Assert.AreEqual(5.ToString(),token.Value);
        }

        [Test]
        public void TestDecimal() {
            string text = "123.456";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Scalar, token.TokenType);
            Assert.AreEqual((123.456).ToString(), token.Value);
        }

        [Test]
        public void TestReal() {
            string text = "123.456E12";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Scalar, token.TokenType);
            Assert.AreEqual(double.Parse(text), double.Parse(token.Value));
        }

        [Test]
        public void TestRealWithSign() {
            string text = "   123.456E-12   e";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Scalar, token.TokenType);
            Assert.AreEqual(double.Parse("123.456E-12"), double.Parse(token.Value));
        }

        [Test]
        public void TestIdentifier() {
            string text = "abc123   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Identifier, token.TokenType);
            Assert.AreEqual("abc123", token.Value);

        }

        [Test]
        public void TestPlus() {
            string text = " +   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Plus, token.TokenType);
        }

        [Test]
        public void TestMinus() {
            string text = " -   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Minus, token.TokenType);
        }

        [Test]
        public void TestMultiply() {
            string text = " *   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Multiply, token.TokenType);

        }

        [Test]
        public void TestDivide() {
            string text = " /   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Divide, token.TokenType);

        }

        [Test]
        public void TestPower() {
            string text = " ^   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Power, token.TokenType);

        }

        [Test]
        public void TestAssignment() {
            string text = " =   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Assignment, token.TokenType);
        }

        [Test]
        public void TestEquality() {
            string text = " ==   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Equality, token.TokenType);
        }

        [Test]
        public void TestAnd() {
            string text = " &&   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.And, token.TokenType);
        }

        [Test]
        public void TestOr() {
            string text = " ||   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Or, token.TokenType);
        }

        [Test]
        public void TestOpenBracket() {
            string text = " [   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.OpenBracket, token.TokenType);
        }

        [Test]
        public void TestCloseBracket() {
            string text = " ]   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.CloseBracket, token.TokenType);
        }


        [Test]
        public void TestOpenParenthesis() {
            string text = " (   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.OpenParenthesis, token.TokenType);
        }

        [Test]
        public void TestCloseParenthesis() {
            string text = " )   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.CloseParenthesis, token.TokenType);
        }

        [Test]
        public void TestBeginBracket() {
            string text = " {   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.BeginBracket, token.TokenType);
        }

        [Test]
        public void TestEndBracket() {
            string text = " }   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.EndBracket, token.TokenType);
        }

        [Test]
        public void TestComma() {
            string text = " ,   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Comma, token.TokenType);
        }
        
        [Test]
        public void TestDot() {
            string text = " .   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Dot, token.TokenType);
        }

        [Test]
        public void TestLet() {
            string text = " leT   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Let, token.TokenType);
        }

        [Test]
        public void TestIf() {
            string text = " If   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.If, token.TokenType);
        }

        [Test]
        public void TestThen() {
            string text = " tHeN   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Then, token.TokenType);
        }


        [Test]
        public void TestElse() {
            string text = " else   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Else, token.TokenType);
        }


        [Test]
        public void TestSemiColon() {
            string text = " ;   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.SemiColon, token.TokenType);
        }


        [Test]
        public void TestNot() {
            string text = " !   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Not, token.TokenType);
        }


        [Test]
        public void TestInequality() {
            string text = " !=   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Inequality, token.TokenType);
        }


        [Test]
        public void TestModulus() {
            string text = " %   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Modulus, token.TokenType);
        }

        [Test]
        public void TestLessThan() {
            string text = " <   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.LessThan, token.TokenType);
        }

        [Test]
        public void TesLessThanEqualTo() {
            string text = " <=   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.LessThanEqualTo, token.TokenType);
        }

        [Test]
        public void TestGreaterThan() {
            string text = " >   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.GreaterThan, token.TokenType);
        }


        [Test]
        public void TestGreaterThanEqualTo() {
            string text = " >=   ";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.GreaterThanEqualTo, token.TokenType);
        }


        [Test]
        public void TestEndOfCode() {
            string text = "";
            Scanner scanner = new Scanner(new StringReader(text));
            Token token = scanner.GetNextToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.EndOfCode, token.TokenType);
        }
    }
}
