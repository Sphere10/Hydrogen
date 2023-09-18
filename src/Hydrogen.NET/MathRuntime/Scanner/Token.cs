// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

public class Token {
	private TokenType _type;
	private string _value;
	private int _line;
	private int _startPosition;
	private int _endPosition;

	public Token(TokenType type, string value, int line, int startPos, int endPos) {
		TokenType = type;
		Value = value;
		Line = line;
		StartPosition = startPos;
		EndPosition = endPos;
	}

	public string Value {
		get { return _value; }
		set { _value = value; }
	}

	public TokenType TokenType {
		get { return _type; }
		set { _type = value; }
	}

	public int Line {
		get { return _line; }
		set { _line = value; }
	}

	public int StartPosition {
		get { return _startPosition; }
		set { _startPosition = value; }
	}

	public int EndPosition {
		get { return _endPosition; }
		set { _endPosition = value; }
	}


}
