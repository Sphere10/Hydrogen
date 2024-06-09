// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class ReadOnlyDictionaryDecorator<TKey, TValue, TReadOnlyDictionary> : IReadOnlyDictionary<TKey, TValue>
	where TReadOnlyDictionary : IReadOnlyDictionary<TKey, TValue> {

	protected readonly TReadOnlyDictionary Internal;
	public ReadOnlyDictionaryDecorator(TReadOnlyDictionary internalDictionary) {
		Internal = internalDictionary;
	}

	public virtual TValue this[TKey key] => Internal[key];

	public virtual IEnumerable<TKey> Keys => Internal.Keys;

	public virtual IEnumerable<TValue> Values => Internal.Values;

	public virtual int Count => Internal.Count;

	public virtual bool ContainsKey(TKey key) => Internal.ContainsKey(key);

	public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Internal.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	public virtual bool TryGetValue(TKey key, out TValue value) => Internal.TryGetValue(key, out value);

}
