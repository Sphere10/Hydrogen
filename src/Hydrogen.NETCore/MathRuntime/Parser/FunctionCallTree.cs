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

public class FunctionCallTree : SyntaxTree {
	List<SyntaxTree> _arguments;

	public FunctionCallTree(Token token)
		: base(token) {

		_arguments = new List<SyntaxTree>();
	}

	public List<SyntaxTree> Arguments {
		get { return _arguments; }
		set { _arguments = value; }
	}

	public override string ToString() {
		StringBuilder paramsText = new StringBuilder();
		for (int i = 0; i < _arguments.Count; i++) {
			if (i > 0) paramsText.Append(",");
			paramsText.Append(_arguments[i].ToString());
		}
		return string.Format("FunctionCall({0},Args({1})", Token.Value, paramsText.ToString());
	}

}
