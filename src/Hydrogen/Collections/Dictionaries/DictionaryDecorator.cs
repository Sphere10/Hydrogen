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

public abstract class DictionaryDecorator<TKey, TValue, TDictionary> : IDictionary<TKey, TValue> where TDictionary : IDictionary<TKey, TValue> {
	protected readonly TDictionary InternalDictionary;

	protected DictionaryDecorator(TDictionary internalDictionary) {
		Guard.ArgumentNotNull(internalDictionary, nameof(internalDictionary));
		InternalDictionary = internalDictionary;
	}

	#region IDictionary Implementation

	public virtual ICollection<TKey> Keys => InternalDictionary.Keys;

	public virtual ICollection<TValue> Values => InternalDictionary.Values;

	public virtual void Add(TKey key, TValue value) => InternalDictionary.Add(key, value);

	public virtual bool ContainsKey(TKey key) => InternalDictionary.ContainsKey(key);

	public virtual bool TryGetValue(TKey key, out TValue value) => InternalDictionary.TryGetValue(key, out value);

	public virtual TValue this[TKey key] {
		get => InternalDictionary[key];
		set => InternalDictionary[key] = value;
	}

	public virtual void Add(KeyValuePair<TKey, TValue> item) => InternalDictionary.Add(item);

	public virtual bool Contains(KeyValuePair<TKey, TValue> item) => InternalDictionary.Contains(item);

	public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => InternalDictionary.CopyTo(array, arrayIndex);

	public virtual bool Remove(KeyValuePair<TKey, TValue> item) => InternalDictionary.Remove(item);

	public virtual bool Remove(TKey item) => InternalDictionary.Remove(item);

	public virtual void Clear() => InternalDictionary.Clear();

	public virtual int Count => InternalDictionary.Count;

	public virtual bool IsReadOnly => InternalDictionary.IsReadOnly;

	public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => InternalDictionary.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => (InternalDictionary as IEnumerable).GetEnumerator();

	#endregion

}


public abstract class DictionaryDecorator<TKey, TValue> : DictionaryDecorator<TKey, TValue, IDictionary<TKey, TValue>> {

	protected DictionaryDecorator(IDictionary<TKey, TValue> internalDictionary)
		: base(internalDictionary) {
	}

}
