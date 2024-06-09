// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class RegexAlternation {
	readonly RegexPattern _precedingRegexPattern;
	internal RegexAlternation(RegexPattern precedingRegexPattern) {
		_precedingRegexPattern = precedingRegexPattern;
	}

	public RegexPattern Either(RegexPattern firstOption, RegexPattern secondOption) {
		return _precedingRegexPattern.RegEx($"({firstOption}|{secondOption})");
	}

	public RegexPattern EitherAny(params RegexPattern[] options) {
		if (options.Length == 0)
			throw new ArgumentOutOfRangeException(nameof(options), "Must contains at least 2 elements");
		_precedingRegexPattern.RegEx("(");
		_precedingRegexPattern.RegEx(options[0]);
		for (var i = 1; i < options.Length; i++) {
			_precedingRegexPattern.RegEx("|").RegEx(options[i]);
		}
		return _precedingRegexPattern.RegEx(")");
	}

	public RegexPattern If(RegexPattern matched, RegexPattern then, RegexPattern otherwise) {
		return _precedingRegexPattern.RegEx($"(?(?={matched}){then}|{otherwise})");
	}

	public RegexPattern If(string namedGroupToMatch, RegexPattern then, RegexPattern otherwise) {
		return _precedingRegexPattern.RegEx($"(?({namedGroupToMatch}){then}|{otherwise})");
	}

	public RegexPattern If(int unnamedCaptureToMatch, RegexPattern then, RegexPattern otherwise) {
		return _precedingRegexPattern.RegEx($"(?({unnamedCaptureToMatch}){then}|{otherwise})");
	}
}
