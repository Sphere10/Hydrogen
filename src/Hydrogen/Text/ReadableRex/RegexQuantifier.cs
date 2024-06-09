// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class RegexQuantifier {
	readonly RegexPattern _quantifiedExpression;

	internal RegexQuantifier(RegexPattern quantifiedExpression) {
		_quantifiedExpression = quantifiedExpression;
	}

	public virtual RegexPattern Exactly(int timesToRepeat) {
		_quantifiedExpression.RegEx("{" + timesToRepeat + "}");
		return _quantifiedExpression;
	}

	public virtual RegexPattern ZeroOrMore {
		get {
			_quantifiedExpression.RegEx("*");
			return _quantifiedExpression;
		}
	}

	public virtual RegexPattern OneOrMore {
		get {
			_quantifiedExpression.RegEx("+");
			return _quantifiedExpression;
		}
	}

	public virtual RegexPattern Optional {
		get {
			_quantifiedExpression.RegEx("?");
			return _quantifiedExpression;
		}
	}

	public virtual RegexPattern AtLeast(int timesToRepeat) {
		_quantifiedExpression.RegEx("{" + timesToRepeat + ",}");
		return _quantifiedExpression;
	}

	public virtual RegexPattern AtMost(int timesToRepeat) {
		_quantifiedExpression.RegEx("{," + timesToRepeat + "}");
		return _quantifiedExpression;
	}

	public virtual RegexPattern InRange(int minimum, int maximum) {
		_quantifiedExpression.RegEx("{" + minimum + "," + maximum + "}");
		return _quantifiedExpression;
	}

	public RegexQuantifier Lazy => new RegexLazyQuantifier(_quantifiedExpression);
}
