// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Maths.Compiler;

public class FunctionDeclarationTree : SyntaxTree {
	private List<Token> _parameters;
	private SyntaxTree _expression;


	public FunctionDeclarationTree() {
	}

	public FunctionDeclarationTree(Token token)
		: base(token) {
	}

	public SyntaxTree Expression {
		get { return _expression; }
		set { _expression = value; }
	}

	public List<Token> Parameters {
		get { return _parameters; }
		set { _parameters = value; }
	}

	public override string ToString() {
		StringBuilder paramsText = new StringBuilder();
		for (int i = 0; i < _parameters.Count; i++) {
			if (i > 0) paramsText.Append(",");
			paramsText.Append(_parameters[i].Value);
		}
		return string.Format("FunctionDeclaration({0},Params({1}),{2})", Token.Value, paramsText.ToString(), _expression.ToString());
	}
}
