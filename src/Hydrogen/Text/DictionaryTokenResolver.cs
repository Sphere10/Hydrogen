// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class DictionaryTokenResolver : ITokenResolver {
	private readonly IDictionary<string, object> _dictionary;

	public DictionaryTokenResolver(IDictionary<string, object> dictionary) {
		_dictionary = dictionary;
	}

	public bool TryResolve(string token, out object value) {
		return _dictionary.TryGetValue(token, out value);
	}
}
