// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen.Maths.Compiler;

public interface IMathContext {
	IMathContext ParentContext { get; set; }

	IVariableContext Variables { get; }

	IFunctionContext Functions { get; }

	IFunction GenerateFunction(string expression);

	IFunction GenerateFunction(string expression, string parameterName);

}
