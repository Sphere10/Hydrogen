// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

public enum ScannerState {
	Start,
	Identifier,
	Number,
	AlmostDecimal,
	Decimal,
	NumberWithExponent,
	NumberWithSignedExponent,
	NumberWithUnsignedExponent,
	Power,
	Plus,
	Minus,
	Multiplication,
	Division,
	OpenBracket,
	CloseBracket,
	OpenParenthesis,
	CloseParenthesis,
	BeginBracket,
	EndBracket,
	Assign,
	Equality,
	AlmostOr,
	Or,
	AlmostAnd,
	And,
	Comma,
	Dot,
	SemiColon,
	Not,
	Inequality,
	Modulus,
	LessThan,
	LessThanEqualTo,
	GreaterThan,
	GreaterThanEqualTo,
	EndOfCode,
}
