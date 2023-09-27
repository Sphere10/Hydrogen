// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Maths.Compiler;

/// <summary>
/// Type checks variables and functions exist in math context. Enforces function declarations and 
/// function calls to be single variable. Does not accept boolean operators. Always assumes the
/// variable 'x' is defined as a parameter.
/// </summary>
public class BasicTypeChecker {
	private IMathContext _mathContext;
	private string _functionParameterName;

	public BasicTypeChecker(IMathContext mathContext) {
		MathContext = mathContext;
	}

	/// <summary>
	/// Stores the name of the function parameter. Caller must set this if variable is implicit in syntax
	/// tree. If parameter is explicitly declared then the typechecker sets this, and the variable can be
	/// retrieved by caller.
	/// </summary>
	public string FunctionParameterName {
		get { return _functionParameterName; }
		set { _functionParameterName = value; }
	}

	public IMathContext MathContext {
		get { return _mathContext; }
		set { _mathContext = value; }
	}

	public virtual List<CodeErrorException> TypeCheckFunctionDeclaration(FunctionDeclarationTree tree) {
		List<CodeErrorException> errors = new List<CodeErrorException>();
		if (tree.Parameters.Count != 1) {
			errors.Add(
				new CodeErrorException(
					tree.Token.Line,
					tree.Token.StartPosition,
					tree.Token.EndPosition,
					string.Format(
						"Function declaration '{0}' must be single variable.",
						tree.Token.Value
					)
				)
			);
		}
		FunctionParameterName = tree.Parameters[0].Value;
		errors.AddRange(TypeCheckExpression(tree.Expression));
		return errors;
	}

	public virtual List<CodeErrorException> TypeCheckExpression(SyntaxTree tree) {
		List<CodeErrorException> errors = new List<CodeErrorException>();
		if (tree is BinaryOperatorTree) {
			errors.AddRange(TypeCheckBinaryOperator(tree as BinaryOperatorTree));
		} else if (tree is UnaryOperatorTree) {
			errors.AddRange(TypeCheckUnaryOperator(tree as UnaryOperatorTree));
		} else if (tree is FactorTree) {
			errors.AddRange(TypeCheckFactor(tree as FactorTree));
		} else if (tree is FunctionCallTree) {
			errors.AddRange(TypeCheckFunctionCall(tree as FunctionCallTree));
		} else {
			errors.Add(
				new CodeErrorException(
					tree.Token.Line,
					tree.Token.StartPosition,
					tree.Token.EndPosition,
					string.Format(
						"Type-checker cannot understand token {0}",
						tree.Token.Value
					)
				)
			);
		}
		return errors;
	}


	protected virtual List<CodeErrorException> TypeCheckFunctionCall(FunctionCallTree functionCallTree) {
		List<CodeErrorException> errors = new List<CodeErrorException>();
		StandardFunctionResolver resolver = new StandardFunctionResolver();

		if (!resolver.IsStandardFunction(functionCallTree.Token.Value)) {
			// check function exists if its not standard function
			if (!MathContext.Functions.ContainsFunction(functionCallTree.Token.Value)) {
				errors.Add(
					new CodeErrorException(
						functionCallTree.Token.Line,
						functionCallTree.Token.StartPosition,
						functionCallTree.Token.EndPosition,
						string.Format(
							"Function '{0}' not defined in math context",
							functionCallTree.Token.Value
						)
					)
				);
			}
		}

		// check single parameter call
		if (functionCallTree.Arguments.Count != 1) {
			errors.Add(
				new CodeErrorException(
					functionCallTree.Token.Line,
					functionCallTree.Token.StartPosition,
					functionCallTree.Token.EndPosition,
					"Functions only take 1 parameter"
				)
			);
		}

		// check parameters
		foreach (SyntaxTree arg in functionCallTree.Arguments) {
			errors.AddRange(TypeCheckExpression(arg));
		}

		return errors;
	}

	protected virtual List<CodeErrorException> TypeCheckFactor(FactorTree factorTree) {
		List<CodeErrorException> errors = new List<CodeErrorException>();
		// if identifier is function parameter then it's okay, otherwise make sure it's defined in variable
		// context
		if (factorTree.Token.TokenType == TokenType.Identifier) {
			if (factorTree.Token.Value != FunctionParameterName) {
				if (!MathContext.Variables.ContainsVariable(factorTree.Token.Value)) {
					errors.Add(
						new CodeErrorException(
							factorTree.Token.Line,
							factorTree.Token.StartPosition,
							factorTree.Token.EndPosition,
							string.Format(
								"The variable '{0}' is not defined in math context",
								factorTree.Token.Value
							)
						)
					);
				}
			}
		}
		return errors;
	}


	protected virtual List<CodeErrorException> TypeCheckBinaryOperator(BinaryOperatorTree op) {
		List<CodeErrorException> errors = new List<CodeErrorException>();
		switch (op.Operator) {
			case Operator.Addition:
			case Operator.Subtraction:
			case Operator.Multiplication:
			case Operator.Division:
			case Operator.Power:
				break;
			default:
				errors.Add(
					new CodeErrorException(
						op.Token.Line,
						op.Token.StartPosition,
						op.Token.EndPosition,
						string.Format(
							"Type-checker does not support binary operator '{0}'",
							op.Token.Value
						)
					)
				);
				break;
		}

		// typecheck expressions
		errors.AddRange(TypeCheckExpression(op.LeftHandSide));
		errors.AddRange(TypeCheckExpression(op.RightHandSide));
		return errors;
	}


	protected virtual List<CodeErrorException> TypeCheckUnaryOperator(UnaryOperatorTree op) {
		List<CodeErrorException> errors = new List<CodeErrorException>();
		switch (op.Operator) {
			case Operator.UnaryMinus:
			case Operator.UnaryPlus:
				break;
			default:
				errors.Add(
					new CodeErrorException(
						op.Token.Line,
						op.Token.StartPosition,
						op.Token.EndPosition,
						string.Format(
							"Typechecker does not support unary operator '{0}'",
							op.Token.Value
						)
					)
				);
				break;
		}

		// typecheck expressions
		errors.AddRange(TypeCheckExpression(op.Operand));
		return errors;
	}
}
