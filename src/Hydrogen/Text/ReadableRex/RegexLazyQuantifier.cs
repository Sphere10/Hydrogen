// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class RegexLazyQuantifier : RegexQuantifier {
	public RegexLazyQuantifier(RegexPattern quantifiedExpression) : base(quantifiedExpression) {
	}

	public override RegexPattern ZeroOrMore => base.ZeroOrMore.RegEx("?");

	public override RegexPattern OneOrMore => base.OneOrMore.RegEx("?");

	public override RegexPattern Exactly(int timesToRepeat) {
		return base.Exactly(timesToRepeat).RegEx("?");
	}

	public override RegexPattern AtLeast(int timesToRepeat) {
		return base.AtLeast(timesToRepeat).RegEx("?");
	}

	public override RegexPattern Optional => base.Optional.RegEx("?");

	public override RegexPattern InRange(int minimum, int maximum) {
		return base.InRange(minimum, maximum).RegEx("?");
	}

	public override RegexPattern AtMost(int timesToRepeat) {
		throw new InvalidOperationException("You cannot perform lazy evaluation of the AtMost {,n} quantifier.");
	}
}
