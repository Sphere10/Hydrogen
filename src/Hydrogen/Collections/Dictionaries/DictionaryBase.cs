// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Base class for dictionary implementation. Implements index operator and misc.
/// </summary>
public abstract class DictionaryBase<TKey, TValue> : ExtendedCollectionBase<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> {
	private readonly EnumerableCollectionAdapter<TKey> _keysCollectionProperty;
	private readonly EnumerableCollectionAdapter<TValue> _valuesCollectionProperty;

	protected DictionaryBase() {
		_keysCollectionProperty = new EnumerableCollectionAdapter<TKey>(GetKeysEnumerator, () => ((ICollection<KeyValuePair<TKey, TValue>>)this).Count, ContainsKey );
		_valuesCollectionProperty = new EnumerableCollectionAdapter<TValue>(GetValuesEnumerator, () => ((ICollection<KeyValuePair<TKey, TValue>>)this).Count);
	}

	public virtual ICollection<TKey> Keys => _keysCollectionProperty;
	
	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	public virtual ICollection<TValue> Values => _valuesCollectionProperty;

	public TValue Get(TKey key) {
		if (TryGetValue(key, out var value))
			return value;
		throw new KeyNotFoundException($"The key '{key}' was not found");
	}

	public abstract bool TryGetValue(TKey key, out TValue value);

	public override void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

	public abstract void Add(TKey key, TValue value);

	public abstract void Update(TKey key, TValue value);

	public abstract bool ContainsKey(TKey key);

	public virtual TValue this[TKey key] {
		get => Get(key);
		set => AddOrUpdate(key, value);
	}

	public abstract bool Remove(TKey item);
	
	protected virtual void AddOrUpdate(TKey key, TValue value) {
		if (ContainsKey(key))
			Update(key, value);
		else
			Add(key, value);
	}

	protected virtual IEnumerator<TKey> GetKeysEnumerator() => this.Select(x => x.Key).GetEnumerator();

	protected virtual IEnumerator<TValue> GetValuesEnumerator() => this.Select(x => x.Value).GetEnumerator();

	public override void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			Add(item);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			yield return Remove(item);
	}

	public override IEnumerable<bool> ContainsRange(IEnumerable<KeyValuePair<TKey, TValue>> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items.Select(Contains).ToArray();
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
