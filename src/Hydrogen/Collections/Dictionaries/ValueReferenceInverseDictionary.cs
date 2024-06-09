// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Collections;

public class ValueReferenceInverseDictionary<TKey, TValue> : ILookup<TValue, TKey> where TValue : class {
	private readonly ILookup<Reference<TValue>, TKey> _valueReferenceToKeyLookup;

	public ValueReferenceInverseDictionary(IDictionary<TKey, TValue> dictionary) {
		_valueReferenceToKeyLookup = dictionary.ToLookup(kv => Reference.For(kv.Value), kv => kv.Key);
	}

	#region ILookup<TValue, TKey>

	public IEnumerator<IGrouping<TValue, TKey>> GetEnumerator() {
		return _valueReferenceToKeyLookup.Select(g => (IGrouping<TValue, TKey>)new Grouping<TValue, TKey>(g.Key.Object, g)).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public bool Contains(TValue key) {
		return _valueReferenceToKeyLookup.Contains(Reference.For(key));
	}

	public int Count => _valueReferenceToKeyLookup.Count;

	public IEnumerable<TKey> this[TValue key] => _valueReferenceToKeyLookup[Reference.For(key)];

	#endregion

}
