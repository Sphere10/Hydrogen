// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Base class for dictionary implementation. Implements index operator and misc.
/// </summary>
public abstract class DictionaryBase<TKey, TValue> : ExtendedCollectionBase<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue> {
	private readonly KeysCollection _keysCollectionProperty;
	private readonly ValuesCollection _valuesCollectionProperty;

	protected DictionaryBase() {
		_keysCollectionProperty = new KeysCollection(this);
		_valuesCollectionProperty = new ValuesCollection(this);
	}

	public virtual ICollection<TKey> Keys => _keysCollectionProperty;

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

	protected virtual IEnumerator<TKey> GetKeysEnumerator() => Keys.GetEnumerator();

	protected virtual IEnumerator<TValue> GetValuesEnumerator() => Values.GetEnumerator();

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
	
	private class KeysCollection : ICollection<TKey> {

		private readonly DictionaryBase<TKey, TValue> _parent;

		public KeysCollection(DictionaryBase<TKey, TValue> parent) {
			_parent = parent;
		}

		public IEnumerator<TKey> GetEnumerator() => _parent.GetKeysEnumerator();

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Add(TKey item) => throw new NotSupportedException();

		public void Clear() => _parent.Clear();

		public bool Contains(TKey item) => _parent.ContainsKey(item);

		public void CopyTo(TKey[] array, int arrayIndex) {
			foreach (var key in this)
				array[arrayIndex++] = key;
		}

		public bool Remove(TKey item) => _parent.Remove(item);

		public int Count => checked((int)_parent.Count);

		public bool IsReadOnly => _parent.IsReadOnly;
	}
	
	private class ValuesCollection : ICollection<TValue> {

		private readonly DictionaryBase<TKey, TValue> _parent;

		public ValuesCollection(DictionaryBase<TKey, TValue> parent) {
			_parent = parent;
		}

		public IEnumerator<TValue> GetEnumerator() => _parent.GetValuesEnumerator();

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Add(TValue item) => throw new NotSupportedException();

		public void Clear() => _parent.Clear();

		public bool Contains(TValue item) => throw new NotSupportedException();

		public void CopyTo(TValue[] array, int arrayIndex) {
			foreach (var value in this)
				array[arrayIndex++] = value;
		}

		public bool Remove(TValue item) => throw new NotSupportedException();

		public int Count => checked((int) _parent.Count);

		public bool IsReadOnly => _parent.IsReadOnly;
	}

}
