// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

/// <summary>
/// Tokens grammar
/// Number = Digit Digit*
/// Real = 
/// Letter = [a..z]|[A..Z]
/// Digit = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
/// </summary>
public enum TokenType {
	Scalar, // Digit(Digit)*[.][E][+|-]Digit(Digit)*
	Identifier, // Letter(Letter|Digit)*
	Plus, // +
	Minus, // -
	Multiply, // *
	Divide, // /
	Power, // ^
	Assignment, // =
	Equality, // ==
	And, // &&
	Or, // ||
	OpenBracket, // [
	CloseBracket, // ]
	OpenParenthesis, // (
	CloseParenthesis, // )
	BeginBracket, // {
	EndBracket, // }
	Comma, // ,
	Dot, // .
	Let, // LET (case insensitive)
	If, // IF
	Then, // THEN
	Else, // ELSE
	SemiColon, // ;
	Not, // !
	Inequality, // !=
	Modulus, // %
	LessThan, // <
	LessThanEqualTo, // <= 
	GreaterThan, // >
	GreaterThanEqualTo, // >=
	EndOfCode

}
