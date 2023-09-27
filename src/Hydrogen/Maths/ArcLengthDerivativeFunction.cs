// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Maths;

namespace Hydrogen;

public class ArcLengthDerivativeFunction : IFunction {
	private readonly IFunction _derivativeFunction = null;

	public ArcLengthDerivativeFunction(IFunction function) {
		_derivativeFunction = new FunctionDerivative(function);
	}

	public double Eval(double x) {
		return Math.Sqrt(1 + Math.Pow(_derivativeFunction.Eval(x), 2));

	}
}
