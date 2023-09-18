// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using System.IO;

namespace Hydrogen.Maths.Compiler;

public sealed class Scanner {
	private TextReader _reader;
	private int _line;
	private int _prevLineEndPos;
	private bool _haveRestoredChar;
	private char _restoredChar;
	int _pos;
	int _startPosition;

	public Scanner(TextReader reader) {
		_reader = reader;
		_line = 0;
		_pos = 0;
		_startPosition = 0;
		_prevLineEndPos = -1;
		_haveRestoredChar = false;
		_restoredChar = char.MinValue;
	}

	public bool IsSymbol(char c) {
		bool retval = false;
		switch (c) {
			case '+':
			case '-':
			case '*':
			case '/':
			case '^':
			case '(':
			case ')':
			case '[':
			case ']':
			case '{':
			case '}':
			case '&':
			case '|':
			case '=':
			case ',':
			case '.':
			case ';':
			case '!':
			case '<':
			case '>':
			case '%':
				retval = true;
				break;
		}
		return retval;
	}

	public Token GetNextToken() {
		StringBuilder tokenValue = new StringBuilder();
		ScannerState scannerState = ScannerState.Start;
		ScannerErrorState scannerError = ScannerErrorState.None;
		bool finishedScan = false;
		_startPosition = _pos;
		while (!finishedScan) {
			bool ignoreCharacter = false;
			char nextChar = GetNextRelevantChar();

			#region DFA Implementation

			switch (scannerState) {
				case ScannerState.Start:
					if (char.IsWhiteSpace(nextChar)) {
						scannerState = ScannerState.Start;
						ignoreCharacter = true;
					} else if (char.IsLetter(nextChar)) {
						scannerState = ScannerState.Identifier;
					} else if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.Number;
					} else if (IsSymbol(nextChar)) {

						#region Scan symbol

						switch (nextChar) {
							case '^':
								scannerState = ScannerState.Power;
								finishedScan = true;
								break;
							case '+':
								scannerState = ScannerState.Plus;
								finishedScan = true;
								break;
							case '-':
								scannerState = ScannerState.Minus;
								finishedScan = true;
								break;
							case '*':
								scannerState = ScannerState.Multiplication;
								finishedScan = true;
								break;
							case '/':
								scannerState = ScannerState.Division;
								finishedScan = true;
								break;
							case '[':
								scannerState = ScannerState.OpenBracket;
								finishedScan = true;
								break;
							case ']':
								scannerState = ScannerState.CloseBracket;
								finishedScan = true;
								break;
							case '(':
								scannerState = ScannerState.OpenParenthesis;
								finishedScan = true;
								break;
							case ')':
								scannerState = ScannerState.CloseParenthesis;
								finishedScan = true;
								break;
							case '{':
								scannerState = ScannerState.BeginBracket;
								finishedScan = true;
								break;
							case '}':
								scannerState = ScannerState.EndBracket;
								finishedScan = true;
								break;
							case '=':
								scannerState = ScannerState.Assign;
								break;
							case '&':
								scannerState = ScannerState.AlmostAnd;
								break;
							case '|':
								scannerState = ScannerState.AlmostOr;
								break;
							case ',':
								scannerState = ScannerState.Comma;
								finishedScan = true;
								break;
							case '.':
								scannerState = ScannerState.Dot;
								finishedScan = true;
								break;
							case ';':
								scannerState = ScannerState.SemiColon;
								finishedScan = true;
								break;
							case '!':
								scannerState = ScannerState.Not;
								break;
							case '<':
								scannerState = ScannerState.LessThan;
								break;
							case '>':
								scannerState = ScannerState.GreaterThan;
								break;
							case '%':
								scannerState = ScannerState.Modulus;
								finishedScan = true;
								break;
							default:
								finishedScan = true;
								scannerError = ScannerErrorState.UnexpectedSymbol;
								RestoreChar(nextChar);
								break;
						}

						#endregion

					} else if (nextChar == char.MaxValue) {
						scannerState = ScannerState.EndOfCode;
						finishedScan = true;
					} else {
						finishedScan = true;
						scannerError = ScannerErrorState.UnexpectedCharacter;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.Identifier:
					if (char.IsLetterOrDigit(nextChar)) {
						scannerState = ScannerState.Identifier;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.Number:
					if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.Number;
					} else if (nextChar == '.') {
						scannerState = ScannerState.AlmostDecimal;
					} else if (nextChar == 'E') {
						scannerState = ScannerState.NumberWithExponent;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.AlmostDecimal:
					if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.Decimal;
					} else {
						finishedScan = true;
						scannerError = ScannerErrorState.CannotHaveBlankMantissa;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.Decimal:
					if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.Decimal;
					} else if (nextChar == 'E') {
						scannerState = ScannerState.NumberWithExponent;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.NumberWithExponent:
					if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.NumberWithUnsignedExponent;
					} else if (nextChar == '+') {
						scannerState = ScannerState.NumberWithSignedExponent;
					} else if (nextChar == '-') {
						scannerState = ScannerState.NumberWithSignedExponent;
					} else {
						finishedScan = true;
						scannerError = ScannerErrorState.CannotHaveBlankExponent;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.NumberWithSignedExponent:
					if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.NumberWithSignedExponent;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.NumberWithUnsignedExponent:
					if (char.IsDigit(nextChar)) {
						scannerState = ScannerState.NumberWithUnsignedExponent;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.Assign:
					if (nextChar == '=') {
						scannerState = ScannerState.Equality;
						finishedScan = true;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.Not:
					if (nextChar == '=') {
						scannerState = ScannerState.Inequality;
						finishedScan = true;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.LessThan:
					if (nextChar == '=') {
						scannerState = ScannerState.LessThanEqualTo;
						finishedScan = true;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.GreaterThan:
					if (nextChar == '=') {
						scannerState = ScannerState.GreaterThanEqualTo;
						finishedScan = true;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.AlmostOr:
					if (nextChar == '|') {
						scannerState = ScannerState.Or;
						finishedScan = true;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;
				case ScannerState.AlmostAnd:
					if (nextChar == '&') {
						scannerState = ScannerState.And;
						finishedScan = true;
					} else {
						finishedScan = true;
						RestoreChar(nextChar);
						ignoreCharacter = true;
					}
					break;

				default:
					scannerError = ScannerErrorState.InternalErrorCouldNotTransitionState;
					break;
			}

			#endregion

			if (scannerError != ScannerErrorState.None) {
				RestoreChar(nextChar);
				ignoreCharacter = true;
				throw new ScannerException(
					scannerError,
					_line,
					_startPosition,
					_pos
				);
			} else {
				if (!ignoreCharacter) {
					tokenValue.Append(nextChar);
				}
			}
		}

		string tokenString = tokenValue.ToString();
		Token token = new Token(
			ResolveTokenType(scannerState, tokenString),
			tokenString,
			_line,
			_startPosition,
			!_haveRestoredChar ? _pos : _pos - 1
		);
		return token;
	}

	private void RestoreChar(char c) {
		_haveRestoredChar = true;
		_restoredChar = c;
	}

	private char GetNextRelevantChar() {
		char nextChar = char.MinValue;
		if (_haveRestoredChar) {
			nextChar = _restoredChar;
			_haveRestoredChar = false;
		} else {
			nextChar = GetNextChar();
			//nextChar = GetNextChar();
			//// if its a whitespace, keep consuming characters until non-whitespace
			//while (char.IsWhiteSpace(nextChar))
			//{
			//    nextChar = GetNextChar();
			//}
		}
		return nextChar;
	}

	private char GetNextChar() {
		char nextChar = char.MinValue;
		// read next char
		int nextValue = _reader.Read();
		nextChar = (char)nextValue;
		// bail out if its end of stream
		if (nextValue == -1) {
			nextChar = char.MaxValue;
		} else {
			// advance line and position pointers
			if (nextChar == '\n') {
				_line++;
				_prevLineEndPos = _pos;
				_pos = 0;
			} else {
				_pos++;
			}
		}
		return nextChar;
	}

	private TokenType ResolveTokenType(ScannerState scannerState, string tokenString) {
		TokenType tokenType = TokenType.And;
		switch (scannerState) {
			case ScannerState.Identifier:
				// if the value is a reserved word then match it so
				switch (tokenString.ToUpper()) {
					case "LET":
						tokenType = TokenType.Let;
						break;
					case "IF":
						tokenType = TokenType.If;
						break;
					case "THEN":
						tokenType = TokenType.Then;
						break;
					case "ELSE":
						tokenType = TokenType.Else;
						break;
					default:
						tokenType = TokenType.Identifier;
						break;
				}
				break;
			case ScannerState.Number:
				tokenType = TokenType.Scalar;
				break;
			case ScannerState.Decimal:
				tokenType = TokenType.Scalar;
				break;
			case ScannerState.NumberWithSignedExponent:
				tokenType = TokenType.Scalar;
				break;
			case ScannerState.NumberWithUnsignedExponent:
				tokenType = TokenType.Scalar;
				break;
			case ScannerState.Power:
				tokenType = TokenType.Power;
				break;
			case ScannerState.Multiplication:
				tokenType = TokenType.Multiply;
				break;
			case ScannerState.Division:
				tokenType = TokenType.Divide;
				break;
			case ScannerState.Plus:
				tokenType = TokenType.Plus;
				break;
			case ScannerState.Minus:
				tokenType = TokenType.Minus;
				break;
			case ScannerState.OpenBracket:
				tokenType = TokenType.OpenBracket;
				break;
			case ScannerState.CloseBracket:
				tokenType = TokenType.CloseBracket;
				break;
			case ScannerState.OpenParenthesis:
				tokenType = TokenType.OpenParenthesis;
				break;
			case ScannerState.CloseParenthesis:
				tokenType = TokenType.CloseParenthesis;
				break;
			case ScannerState.BeginBracket:
				tokenType = TokenType.BeginBracket;
				break;
			case ScannerState.EndBracket:
				tokenType = TokenType.EndBracket;
				break;
			case ScannerState.Assign:
				tokenType = TokenType.Assignment;
				break;
			case ScannerState.Equality:
				tokenType = TokenType.Equality;
				break;
			case ScannerState.Or:
				tokenType = TokenType.Or;
				break;
			case ScannerState.And:
				tokenType = TokenType.And;
				break;
			case ScannerState.EndOfCode:
				tokenType = TokenType.EndOfCode;
				break;
			case ScannerState.Comma:
				tokenType = TokenType.Comma;
				break;
			case ScannerState.Dot:
				tokenType = TokenType.Dot;
				break;
			case ScannerState.Not:
				tokenType = TokenType.Not;
				break;
			case ScannerState.Inequality:
				tokenType = TokenType.Inequality;
				break;
			case ScannerState.Modulus:
				tokenType = TokenType.Modulus;
				break;
			case ScannerState.LessThan:
				tokenType = TokenType.LessThan;
				break;
			case ScannerState.LessThanEqualTo:
				tokenType = TokenType.LessThanEqualTo;
				break;
			case ScannerState.GreaterThan:
				tokenType = TokenType.GreaterThan;
				break;
			case ScannerState.GreaterThanEqualTo:
				tokenType = TokenType.GreaterThanEqualTo;
				break;
			case ScannerState.SemiColon:
				tokenType = TokenType.SemiColon;
				break;
			default:
				throw new ScannerException(
					ScannerErrorState.InternalErrorCouldNotResolveTokenType,
					_line,
					_startPosition,
					_pos
				);
		}
		return tokenType;
	}
}
