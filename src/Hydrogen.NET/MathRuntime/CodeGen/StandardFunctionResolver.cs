// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

internal class StandardFunctionResolver {

	public bool IsStandardVariable(string name) {
		switch (name.ToLower()) {
			case "pi":
			case "e":
			case "phi":
				return true;
			default:
				return false;
		}
	}

	public string ResolveToStandardVariableName(string name) {

		string src = string.Empty;
		switch (name.ToLower()) {
			case "pi":
				src = "Math.PI";
				break;
			case "e":
				src += "Math.E";
				break;
			case "phi":
#warning Emit proper reference here
				src += "Math.Phi WILL RETURN FAILURE";
				break;
			default:
				throw new InternalCompilerException(
					string.Format("Variable '{0}' is not a standard math variable", name)
				);
		}
		return src;
	}


	public bool IsStandardFunction(string name) {
		switch (name.ToLower()) {
			case "sin":
			case "cos":
			case "tan":
			case "sinh":
			case "cosh":
			case "tanh":
			case "arcsin":
			case "arccos":
			case "arctan":
			case "arcsinh":
			case "arccosh":
			case "arctanh":
			case "exp":
			case "ln":
			case "floor":
			case "ceil":
			case "sqrt":
				return true;
			default:
				return false;
		}
	}

	public string ResolveToStandardFunctionName(string name) {

		string src = string.Empty;
		switch (name.ToLower()) {
			case "sin":
				src += "Math.Sin";
				break;
			case "cos":
				src += "Math.Cos";
				break;
			case "tan":
				src += "Math.Tan";
				break;
			case "sinh":
				src += "Math.Sinh";
				break;
			case "cosh":
				src += "Math.Cosh";
				break;
			case "tanh":
				src += "Math.Tanh";
				break;
			case "arcsin":
				src += "Math.Asin";
				break;
			case "arccos":
				src += "Math.Acos";
				break;
			case "arctan":
				src += "Math.Atan";
				break;
			case "arcsinh":
				src += "MathUtil.Asinh";
				break;
			case "arccosh":
				src += "MathUtil.Acosh";
				break;
			case "arctanh":
				src += "MathUtil.Atanh";
				break;
			case "exp":
				src += "Math.Exp";
				break;
			case "ln":
				src += "Math.Log";
				break;
			case "floor":
				src += "Math.Floor";
				break;
			case "ceil":
				src += "Math.Ceiling";
				break;
			case "sqrt":
				src += "Math.Sqrt";
				break;
			default:
				throw new InternalCompilerException(
					string.Format("Function '{0}' is not a standard math function", name)
				);
		}
		return src;
	}
}
