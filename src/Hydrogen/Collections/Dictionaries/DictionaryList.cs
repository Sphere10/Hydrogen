// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hydrogen;

public class DictionaryList<TKey, TVal> : DictionaryBase<TKey, TVal>, IReadOnlyDictionaryList<TKey, TVal> {
	private readonly IEqualityComparer<TVal> _valueComparer;
	private readonly IList<TVal> _list;
	private readonly IDictionary<TKey, int> _dictionary;

	public DictionaryList() 
		: this(EqualityComparer<TKey>.Default, EqualityComparer<TVal>.Default) {
	}

	public DictionaryList(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TVal> valueComparer) {
		keyComparer ??= EqualityComparer<TKey>.Default;
		valueComparer ??= EqualityComparer<TVal>.Default;
		_list = new List<TVal>();
		Values = new ReadOnlyCollection<TVal>(_list);
		_dictionary = new Dictionary<TKey, int>(keyComparer);
		_valueComparer = valueComparer;
	}

	public override long Count => _dictionary.Count;

	int IReadOnlyCollection<TVal>.Count => _dictionary.Count;

	public override ICollection<TVal> Values { get; }

	public override bool IsReadOnly => _dictionary.IsReadOnly;

	public override bool Remove(TKey item) {
		if (_dictionary.TryGetValue(item, out var valueIndex)) {
			RemoveInternal(item, valueIndex);
			return true;
		}
		return false;
	}

	public int IndexOf(TKey key) => _dictionary.TryGetValue(key, out var index) ? index : -1;

	public override bool TryGetValue(TKey key, out TVal value) {
		if (!_dictionary.TryGetValue(key, out var valueIndex)) {
			value = default;
			return false;
		}
		value = _list[valueIndex];
		return true;
	}

	public override void Add(TKey key, TVal value) {
		var valueIndex = _list.Count;
		_list.Add(value);
		_dictionary.Add(key, valueIndex);
	}

	public override bool ContainsKey(TKey key) 
		=> _dictionary.ContainsKey(key);

	public override void Update(TKey key, TVal value) {
		if (!_dictionary.TryGetValue(key, out var valueIndex))
			throw new InvalidOperationException($"Key '{key}' not found");
		_list[valueIndex] = value;
	}

	IEnumerator<TVal> IEnumerable<TVal>.GetEnumerator() 
		=> _list.GetEnumerator();

	public override IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator() 
		=> _dictionary.Select(x => x.AsProjection(key => key, valueIndex => _list[valueIndex])).GetEnumerator();

	protected override IEnumerator<TKey> GetKeysEnumerator() 
		=> _dictionary.Keys.GetEnumerator();

	protected override IEnumerator<TVal> GetValuesEnumerator() 
		=> _list.GetEnumerator();

	public override bool Remove(KeyValuePair<TKey, TVal> item) {
		if (_dictionary.TryGetValue(item.Key, out var valueIndex) && _valueComparer.Equals(_list[valueIndex], item.Value)) {
			RemoveInternal(item.Key, valueIndex);
			return true;
		}
		return false;
	}

	public override bool Contains(KeyValuePair<TKey, TVal> item) 
		=> _dictionary.TryGetValue(item.Key, out var valueIndex) && _valueComparer.Equals(_list[valueIndex], item.Value);

	public override void Clear() {
		_dictionary.Clear();
		_list.Clear();
	}

	public override void CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex) {
		foreach(var item in this)
			array[arrayIndex++] = item;
	}

	public TVal this[int index] => _list[index];

	private void RemoveInternal(TKey key, int index) {
		_dictionary.Remove(key);
		_list.RemoveAt(index);
			
		// update dict
		foreach(var pair in _dictionary)
			if (pair.Value > index)
				_dictionary[pair.Key] = pair.Value - 1;
	}
	
}
