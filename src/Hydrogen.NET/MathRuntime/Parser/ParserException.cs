//-----------------------------------------------------------------------
// <copyright file="ParserException.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen.Maths.Compiler;

public class ParserException : CodeErrorException {


	public ParserException(Token unexpectedToken, params TokenType[] expectedTokens)
		: base(
			unexpectedToken.Line,
			unexpectedToken.StartPosition,
			unexpectedToken.EndPosition,
			ConstructErrorMessage(unexpectedToken, expectedTokens)
		) {
	}


	public static string ConstructErrorMessage(Token unexpectedToken, TokenType[] expectedTokens) {
		string msg = string.Empty;
		for (int i = 0; i < expectedTokens.Length; i++) {
			if (i > 0) {
				msg += " or ";
			}
			msg += expectedTokens[i].ToString();
		}
		return string.Format("Expected {0} not {1}", msg, unexpectedToken.TokenType);
	}

}
