// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Maths.Compiler;

public class DefaultFunctionContext : IFunctionContext {
	private IMathContext _mathContext;
	private IDictionary<string, IFunction> _functionDictionary;

	public DefaultFunctionContext(IMathContext context) {
		_mathContext = context;
		_functionDictionary = new Dictionary<string, IFunction>();
	}

	public IMathContext MathContext {
		get { return _mathContext; }
	}

	public bool ContainsFunction(string name) {
		bool containsFunction = false;
		containsFunction = _functionDictionary.ContainsKey(name);
		if (!containsFunction && MathContext.ParentContext != null) {
			containsFunction = MathContext.ParentContext.Functions.ContainsFunction(name);
		}
		return containsFunction;
	}

	public void RemoveFunction(string name) {
		if (_functionDictionary.ContainsKey(name)) {
			_functionDictionary.Remove(name);
		} else {
			if (MathContext.ParentContext != null) {
				MathContext.ParentContext.Functions.RemoveFunction(name);
			}
		}
	}

	public IFunction this[string var] {
		get {
			IFunction function = null;
			if (_functionDictionary.ContainsKey(var)) {
				function = _functionDictionary[var];
			} else if (MathContext.ParentContext != null) {
				return MathContext.ParentContext.Functions[var];
			} else {
				throw new ApplicationException("The function '{0}' was called but it is not defined in the math context");
			}
			return function;
		}
		set {
			// if function defined in parent context, set it in parent context
			if (MathContext.ParentContext != null &&
			    MathContext.ParentContext.Functions.ContainsFunction(var)) {
				MathContext.ParentContext.Functions[var] = value;
			} else {
				// else set function in current context
				_functionDictionary[var] = value;
			}

		}
	}
}
