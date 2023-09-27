// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Diagnostics;

namespace Hydrogen.Maths.Compiler;

/// <summary>
/// Compiles a function declaration and returns an object which can be evaluated as such
/// </summary>
public class BasicFunctionGenerator {
	private IMathContext _mathContext;
	private string _functionParameterName = "x";


	public BasicFunctionGenerator(IMathContext mathContext) {
		MathContext = mathContext;
	}

	public string FunctionParameterName {
		get { return _functionParameterName; }
		set { _functionParameterName = value; }
	}

	public IMathContext MathContext {
		get { return _mathContext; }
		set { _mathContext = value; }
	}

	public IFunction GenerateFunctionFromExpression(string functionExpression) {
		Parser parser = new Parser(new Scanner(new StringReader(functionExpression)));
		SyntaxTree tree = parser.ParseExpression();
		BasicTypeChecker typeChecker = new BasicTypeChecker(MathContext);
		typeChecker.FunctionParameterName = FunctionParameterName;
		List<CodeErrorException> errors = typeChecker.TypeCheckExpression(tree);
		if (errors.Count > 0) {
			throw new CodeErrorsException(errors);
		}
		return GenerateFunction(
			tree
		);
	}

	public virtual IFunction GenerateFunction(SyntaxTree typeCheckedTree) {
		IDynamicallyCompiledFunction function = null;

		// create a unique name for this function
		string strongName = GenerateUniqueFunctionName();

		// emit the function in c-sharp code
		string srcCode = EmitFunctionInCSharp(strongName, typeCheckedTree);

		// compile c-sharp code
		CSharpCodeProvider cp = new CSharpCodeProvider();
		ICodeCompiler ic = cp.CreateCompiler();
		CompilerParameters cpar = new CompilerParameters();
		cpar.GenerateInMemory = true;
		cpar.GenerateExecutable = false;
		cpar.ReferencedAssemblies.Add("system.dll");
		cpar.ReferencedAssemblies.Add("SchoenfeldSoftware.Mathematics.dll");
		CompilerResults cr = ic.CompileAssemblyFromSource(cpar, srcCode);

		Debug.Assert(cr.Errors.Count == 0);
		Debug.Assert(cr.CompiledAssembly != null);

		// create instance of compiled function
		Type functionType = cr.CompiledAssembly.GetType(
			"SchoenfeldSoftware.Mathematics." + strongName
		);
		function = (IDynamicallyCompiledFunction)Activator.CreateInstance(functionType);

		// set function math context
		function.MathContext = MathContext;

		return function;
	}

	protected virtual string AssembleErrorMessages(List<string> errors) {
		return errors.ToParagraphCase();
	}

	protected virtual string EmitFunctionInCSharp(string strongName, SyntaxTree expression) {
		string expSrc = EmitExpression(expression);
		string src =
			@"  using System;
                    using Hydrogen.Maths.Compiler;
                    namespace SchoenfeldSoftware.Mathematics {

                        public class " + strongName + @" : IDynamicallyCompiledFunction {
                            public IMathContext _mathContext;
        
                            public IMathContext MathContext {
                                get { return _mathContext; }
                                set { _mathContext = value; }
                            }

                            public double Eval(double ___x) {
                                return " + expSrc + @" ;
                            }
                        }
                    }";
		return src;

	}

	protected virtual string EmitExpression(SyntaxTree exp) {
		string code = string.Empty;
		if (exp is BinaryOperatorTree) {
			code = EmitBinaryOperator(exp as BinaryOperatorTree);
		} else if (exp is UnaryOperatorTree) {
			code = EmitUnaryOperator(exp as UnaryOperatorTree);
		} else if (exp is FactorTree) {
			code = EmitFactor(exp as FactorTree);
		} else if (exp is FunctionCallTree) {
			code = EmitFunctionCallTree(exp as FunctionCallTree);
		} else {
			throw new InternalCompilerException(
				string.Format(
					"Could not emit code for {0}",
					exp.GetType()
				)
			);
		}
		return code;
	}


	protected virtual string EmitBinaryOperator(BinaryOperatorTree op) {
		string code = string.Empty;
		switch (op.Operator) {

			case Operator.MemberSelection:
				code = string.Format(
					"{0}.{1}",
					EmitExpression(op.LeftHandSide),
					op.RightHandSide.Token.Value
				);
				break;

			case Operator.Power:
				code = string.Format(
					"Math.Pow({0},{1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);
				break;
			case Operator.Multiplication:
				code = string.Format(
					"({0})*({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Division:
				code = string.Format(
					"({0})/({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Modulus:
				code = string.Format(
					"({0})%({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Addition:
				code = string.Format(
					"({0}) + ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Subtraction:
				code = string.Format(
					"({0}) - ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.LessThan:
				code = string.Format(
					"({0}) < ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.GreaterThan:
				code = string.Format(
					"({0}) > ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.LessThanEqualTo:
				code = string.Format(
					"({0}) <= ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.GreaterThanEqualTo:
				code = string.Format(
					"({0}) >= ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Equality:
				code = string.Format(
					"({0}) == ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Inequality:
				code = string.Format(
					"({0}) != ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.And:
				code = string.Format(
					"({0}) && ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);

				break;
			case Operator.Or:
				code = string.Format(
					"({0}) || ({1})",
					EmitExpression(op.LeftHandSide),
					EmitExpression(op.RightHandSide)
				);
				break;
			default:
				throw new InternalCompilerException(
					string.Format("Could not generate code for binary operator: {0}", op.Operator)
				);
		}
		return code;
	}


	protected virtual string EmitUnaryOperator(UnaryOperatorTree op) {
		string code = string.Empty;
		switch (op.Operator) {
			case Operator.UnaryNot:
				code = string.Format(
					"!{0}",
					EmitExpression(op.Operand)
				);
				break;

			case Operator.UnaryMinus:
				code = string.Format(
					"-({0})",
					EmitExpression(op.Operand)
				);
				break;

			case Operator.UnaryPlus:
				code = string.Format(
					"+({0})",
					EmitExpression(op.Operand)
				);
				break;

			default:
				throw new InternalCompilerException(
					string.Format("Could not generate code for unary operator: {0}", op.Operator)
				);
		}
		return code;
	}

	protected virtual string EmitFactor(FactorTree factor) {
		StandardFunctionResolver resolver = new StandardFunctionResolver();
		string src = string.Empty;
		if (factor.Token.TokenType == TokenType.Identifier) {

			if (resolver.IsStandardVariable(factor.Token.Value)) {
				src = resolver.ResolveToStandardVariableName(factor.Token.Value);
			} else if (factor.Token.Value == FunctionParameterName) {
				// refers to actual parameter of function, which will always be emitted as ___x
				src = "___x";
			} else {
				src = string.Format(
					"MathContext.Variables[\"{0}\"]",
					factor.Token.Value
				);
			}
		} else {
			src = factor.Token.Value;
		}
		return src;
	}

	protected virtual string EmitFunctionCallTree(FunctionCallTree tree) {
		Debug.Assert(tree.Arguments.Count == 1);
		string src = string.Empty;
		StandardFunctionResolver resolver = new StandardFunctionResolver();

		// if it's a standard math function, then call it here
		if (resolver.IsStandardFunction(tree.Token.Value)) {
			src = string.Format(
				"{0}({1})",
				resolver.ResolveToStandardFunctionName(tree.Token.Value),
				EmitExpression(tree.Arguments[0])
			);
		} else {
			// else call it from the math context
			src = string.Format(
				"MathContext.Functions[\"{0}\"].Eval({1})",
				tree.Token.Value,
				EmitExpression(tree.Arguments[0])
			);
		}
		return src;
	}


	protected virtual string GenerateUniqueFunctionName() {
		string classid = Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "");
		return string.Format("Function{0}", classid);
	}
}
