//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

#if !__MOBILE__
using System;
using System.Collections.Generic;

namespace Hydrogen.Maths.Compiler {


	/// <summary>
	/// A precedence-climbing recursive-descent parser based on the following grammar
	/// 
	/// FunctionDeclaration = LET Identifier(ParameterList) = Expression ;
	/// ParameterList = Identifier {,Identifer}
	/// Expression = Exp(0) 
	/// Exp(p) = ConditionalExp|MathExp(p)
	/// ConditionalExp = IF ( Expression ) { Expression } [ELSE { Expression } ]
	/// MathExp = Term {Op Exp(q)}
	/// Term = UnaryOp Exp(q)|(Expression)|Factor
	/// Op = +|-|*|/|^||||&&|==|!=|<||<=|>|>=|.
	/// UnaryOp = -|+|!
	/// Factor = Identifier|Number|FunctionCall
	/// FunctionCall = Identifier ( ArgumentList )
	/// ArgumentList = Expression {,Expression}
	/// See http://www.engr.mun.ca/~theo/Misc/exp_parsing.htm
	/// </summary>
	public class Parser {

		private Scanner _scanner;
		private Token _currentToken;

		public Token CurrentToken {
			get { return _currentToken; }
			set { _currentToken = value; }
		}

		public Parser(Scanner scanner) {
			_scanner = scanner;
			// get the first token
			CurrentToken = _scanner.GetNextToken();
		}

		private Associativity GetOperatorAssociativity(Operator op) {
			Attribute[] attrs = Attribute.GetCustomAttributes(op.GetType().GetField(op.ToString()));
			if (attrs.Length != 1 || !(attrs[0] is OpAttr)) {
				throw new InternalCompilerException(
					string.Format(
						"Operator '{0}' did not specify attributes",
						op
					)
				);
			}
			OpAttr attribute = attrs[0] as OpAttr;
			return attribute.Associativity;
		}

		private Aryness GetOperatorAryness(Operator op) {
			Attribute[] attrs = Attribute.GetCustomAttributes(op.GetType().GetField(op.ToString()));
			if (attrs.Length != 1 || !(attrs[0] is OpAttr)) {
				throw new InternalCompilerException(
					string.Format(
						"Operator '{0}' did not specify attributes",
						op
					)
				);
			}
			OpAttr attribute = attrs[0] as OpAttr;
			return attribute.Aryness;
		}

		private int GetPrecedenceLevel(Operator op) {
			Attribute[] attrs = Attribute.GetCustomAttributes(op.GetType().GetField(op.ToString()));
			if (attrs.Length != 1 || !(attrs[0] is OpAttr)) {
				throw new InternalCompilerException(
					string.Format(
						"Operator '{0}' did not specify attributes",
						op
					)
				);
			}
			OpAttr attribute = attrs[0] as OpAttr;
			return attribute.Precedence;
		}

		private bool TryResolveOperator(TokenType tokenType, bool expectingUnary, out Operator op) {
			bool resolved = true;
			op = Operator.Addition;
			switch (tokenType) {
				case TokenType.Dot:
					op = Operator.MemberSelection;
					break;
				case TokenType.Not:
					if (expectingUnary) {
						op = Operator.UnaryNot;
					} else {
						throw new InternalCompilerException("Could not resolve non-unary not operator");
					}
					break;
				case TokenType.Power:
					op = Operator.Power;
					break;
				case TokenType.Minus:
					if (expectingUnary) {
						op = Operator.UnaryMinus;
					} else {
						op = Operator.Subtraction;
					}
					break;
				case TokenType.Plus:
					if (expectingUnary) {
						op = Operator.UnaryPlus;
					} else {
						op = Operator.Addition;
					}
					break;
				case TokenType.Multiply:
					op = Operator.Multiplication;
					break;
				case TokenType.Divide:
					op = Operator.Division;
					break;
				case TokenType.Modulus:
					op = Operator.Modulus;
					break;
				case TokenType.LessThan:
					op = Operator.LessThan;
					break;
				case TokenType.GreaterThan:
					op = Operator.GreaterThan;
					break;
				case TokenType.LessThanEqualTo:
					op = Operator.LessThanEqualTo;
					break;
				case TokenType.GreaterThanEqualTo:
					op = Operator.GreaterThanEqualTo;
					break;
				case TokenType.Equality:
					op = Operator.Equality;
					break;
				case TokenType.Inequality:
					op = Operator.Inequality;
					break;
				case TokenType.And:
					op = Operator.And;
					break;
				case TokenType.Or:
					op = Operator.Or;
					break;
				default:
					resolved = false;
					break;
			}
			return resolved;
		}

		private Operator ResolveOperator(TokenType tokenType, bool isUnary) {
			Operator op;
			if (!TryResolveOperator(tokenType, isUnary, out op)) {
				throw new InternalCompilerException(
					string.Format(
						"Could not resolve operator. Token = {0}, IsUnary={1}",
						tokenType,
						isUnary
					)
				);
			}
			return op;
		}

		public FunctionDeclarationTree ParseFunctionDeclaration() {
			FunctionDeclarationTree function = new FunctionDeclarationTree();
			Match(TokenType.Let);
			function.Token = Match(TokenType.Identifier);
			Match(TokenType.OpenParenthesis);
			function.Parameters = ParseParameterList();
			Match(TokenType.CloseParenthesis);
			Match(TokenType.Assignment);
			function.Expression = ParseExpression();
			Match(TokenType.SemiColon);
			return function;
		}

		private List<Token> ParseParameterList() {
			List<Token> parameters = new List<Token>();
			{
				while (true) {
					if (parameters.Count > 0) {
						Match(TokenType.Comma);
					}
					Token param = Match(TokenType.Identifier);
					parameters.Add(param);
					if (CurrentToken.TokenType == TokenType.CloseParenthesis) {
						//moving this to top of while loop may parse empty parameter list
						break;
					}
				}
			}
			return parameters;
		}

		public SyntaxTree ParseExpression() {
			return ParseExpression(0);
		}


		public SyntaxTree ParseExpression(int precedence) {
			if (CurrentToken.TokenType == TokenType.If) {
				return ParseConditionalExpression();
			} else {
				return ParseMathExpression(precedence);
			}
		}


		private SyntaxTree ParseConditionalExpression() {
			IfThenTree tree = null;
			Match(TokenType.If);
			Match(TokenType.OpenParenthesis);
			SyntaxTree condition = ParseExpression();
			Match(TokenType.CloseParenthesis);
			Match(TokenType.BeginBracket);
			SyntaxTree expression = ParseExpression();
			Match(TokenType.EndBracket);
			if (CurrentToken.TokenType == TokenType.Else) {
				tree = new IfThenElseTree();
				Match(TokenType.Else);
				Match(TokenType.BeginBracket);
				SyntaxTree elseExpression = ParseExpression();
				((IfThenElseTree)tree).ElseExpression = elseExpression;
				Match(TokenType.EndBracket);
			} else {
				tree = new IfThenTree();
			}
			tree.Condition = condition;
			tree.Expression = expression;
			return tree;
		}

		private SyntaxTree ParseMathExpression(int level) {
			SyntaxTree tree = ParseTerm(0);
			Operator op;

			while (true) {
				// if next token is not an operator bail out
				if (!TryResolveOperator(CurrentToken.TokenType, false, out op)) {
					break;
				}

				// if next token is not a binary operator bail out
				if (GetOperatorAryness(op) != Aryness.Binary) {
					break;
				}

				// if next operator precedence is less than existing level, bail out
				if (!(GetPrecedenceLevel(op) >= level)) {
					break;
				}

				BinaryOperatorTree opTree = new BinaryOperatorTree(CurrentToken);
				opTree.Operator = op;
				Match(CurrentToken.TokenType);

				// parse the right hand side of the expression
				int operatorPrecedence = GetPrecedenceLevel(op);
				int newPrecedenceLevel = operatorPrecedence;
				if (GetOperatorAssociativity(op) == Associativity.Left) {
					newPrecedenceLevel = newPrecedenceLevel + 1;
				}
				SyntaxTree rightHandTree = ParseExpression(newPrecedenceLevel);

				// set the left hand-side to the existing tree, right-hand side to new parsed tree
				opTree.LeftHandSide = tree;
				opTree.RightHandSide = rightHandTree;

				// set main xpression tree to the new operator tree
				tree = opTree;
			}
			return tree;
		}

		private SyntaxTree ParseTerm(int precedenceLevel) {
			Token token = CurrentToken;
			SyntaxTree tree = null;
			Operator op;

			if (TryResolveOperator(token.TokenType, true, out op) && GetOperatorAryness(op) == Aryness.Unary) {
				Match(CurrentToken.TokenType);
				UnaryOperatorTree unaryOperator = new UnaryOperatorTree(token);
				unaryOperator.Operator = op;
				unaryOperator.Operand = ParseExpression(GetPrecedenceLevel(op));
				tree = unaryOperator;
			} else if (token.TokenType == TokenType.OpenParenthesis) {
				Match(TokenType.OpenParenthesis);
				tree = ParseMathExpression(0);
				Match(TokenType.CloseParenthesis);
			} else {
				tree = ParseFactorOrFunctionCall();
			}
			return tree;
		}

		private SyntaxTree ParseFactorOrFunctionCall() {
			SyntaxTree factor = null;
			Token token = CurrentToken;
			if (token.TokenType == TokenType.Identifier) {
				Match(TokenType.Identifier);
				if (CurrentToken.TokenType == TokenType.OpenParenthesis) {
					Match(TokenType.OpenParenthesis);
					FunctionCallTree functionCall = new FunctionCallTree(token);
					functionCall.Arguments = ParseArgumentList();
					Match(TokenType.CloseParenthesis);
					factor = functionCall;
				} else {
					factor = new FactorTree(token);
				}
			} else if (CurrentToken.TokenType == TokenType.Scalar) {
				Match(TokenType.Scalar);
				factor = new FactorTree(token);
			} else {
				throw new ParserException(token, TokenType.Identifier, TokenType.Scalar);
			}
			return factor;
		}

		private List<SyntaxTree> ParseArgumentList() {
			List<SyntaxTree> arguments = new List<SyntaxTree>();
			do {
				if (CurrentToken.TokenType == TokenType.Comma) {
					Match(TokenType.Comma);
				}
				SyntaxTree expression = ParseExpression();
				arguments.Add(expression);
			} while (CurrentToken.TokenType == TokenType.Comma);
			return arguments;
		}


		/// <summary>
		/// Advances to the next token so long as the current token matches the supplied token type. Returns
		/// the current token.
		/// </summary>
		/// <param name="tokenType">The type the current token must match before advancing to next token.</param>
		/// <returns></returns>
		public Token Match(TokenType tokenType) {
			if (CurrentToken.TokenType != tokenType) {
				throw new ParserException(CurrentToken, tokenType);
			}
			Token token = CurrentToken;
			CurrentToken = _scanner.GetNextToken();
			return token;
		}

	}

}
#endif
