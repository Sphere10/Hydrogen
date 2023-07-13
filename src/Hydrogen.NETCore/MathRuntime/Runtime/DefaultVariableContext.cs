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

public class DefaultVariableContext : IVariableContext {
	private IMathContext _mathContext;
	private IDictionary<string, double> _variableDictionary;

	public DefaultVariableContext(IMathContext context) {
		_mathContext = context;
		_variableDictionary = new Dictionary<string, double>();
	}

	public IMathContext MathContext {
		get { return _mathContext; }
	}

	public bool ContainsVariable(string name) {
		bool containsVariable = false;
		containsVariable = _variableDictionary.ContainsKey(name);
		if (!containsVariable && MathContext.ParentContext != null) {
			containsVariable = MathContext.ParentContext.Variables.ContainsVariable(name);
		}
		return containsVariable;
	}

	public void RemoveVariable(string name) {
		if (_variableDictionary.ContainsKey(name)) {
			_variableDictionary.Remove(name);
		} else {
			if (MathContext.ParentContext != null) {
				MathContext.ParentContext.Variables.RemoveVariable(name);
			}
		}
	}

	public double this[string var] {
		get {
			double value = 0;
			if (_variableDictionary.ContainsKey(var)) {
				value = _variableDictionary[var];
			} else if (MathContext.ParentContext != null) {
				return MathContext.ParentContext.Variables[var];
			} else {
				throw new ApplicationException(
					string.Format(
						"The variable '{0}' is not defined in the math context.",
						var
					)
				);
			}
			return value;
		}
		set {
			// if variable defined in parent context, set it in parent context
			if (MathContext.ParentContext != null &&
			    MathContext.ParentContext.Variables.ContainsVariable(var)) {
				MathContext.ParentContext.Variables[var] = value;
			} else {
				// else set variable in current context
				_variableDictionary[var] = value;
			}
		}
	}
}
