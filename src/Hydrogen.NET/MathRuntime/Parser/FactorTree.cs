// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths.Compiler;

public class FactorTree : SyntaxTree {

	public FactorTree() {
	}

	public FactorTree(Token token)
		: base(token) {
	}

	public override string ToString() {
		return string.Format("{0}({1})", Token.TokenType, Token.Value);
	}
}
