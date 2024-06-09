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
using System.Linq;

namespace Hydrogen;

public sealed class ExtendedLookup<TKey, TValue> : ILookup<TKey, TValue> {
	private readonly IDictionary<TKey, IExtendedCollection<TValue>> _map;
	private readonly Func<TKey, IExtendedCollection<TValue>> _listFactory;

	public ExtendedLookup() 
		: this(EqualityComparer<TKey>.Default) {
	}

	public ExtendedLookup(IEqualityComparer<TKey> keyComparer) 
		: this( _ => new ExtendedList<TValue>(), keyComparer) {
	}

	public ExtendedLookup(Func<TKey, IExtendedCollection<TValue>> listFactory, IEqualityComparer<TKey> keyComparer = null)
		: this(new Dictionary<TKey, IExtendedCollection<TValue>>(keyComparer), listFactory) {
	}

	public ExtendedLookup(IDictionary<TKey, IExtendedCollection<TValue>> map, Func<TKey, IExtendedCollection<TValue>> listFactory) 
		: this(null, map, listFactory) {
	}

	public ExtendedLookup(ILookup<TKey, TValue> values, IDictionary<TKey, IExtendedCollection<TValue>> map, Func<TKey, IExtendedCollection<TValue>> listFactory) {
		_map = map;
		_listFactory = listFactory;

		if (values != null)
			foreach(var group in values)
				AddRange(group.Key, group);
	}

	public int Count => _map.Count;

	public IEnumerable<TKey> Keys => _map.Keys;

	public long GetValuesCount(TKey key) {
		if (!_map.TryGetValue(key, out var valuesCollection)) {
			return 0;
		}
		return valuesCollection.Count;
	}

	public bool Contains(TKey key) {
		return _map.ContainsKey(key);
	}

	public void Add(TKey key) {
		FetchValuesCollection(key);
	}

	public void Add(TKey key, TValue value) {
		FetchValuesCollection(key).Add(value);
	}

	public bool Remove(TKey key, TValue value) 
		=> FetchValuesCollection(key).Remove(value);
	

	public void AddRange(TKey key, IEnumerable<TValue> values) {
		FetchValuesCollection(key).AddRange(values);
	}

	public void Clear() {
		_map.Clear();
	}

	public Dictionary<TKey, TValue[]> ToDictionary() 
		=> _map.ToDictionary(x => x.Key, x => x.Value.ToArray());

	public IEnumerable<TValue> this[TKey key] {
		get {
			if (!_map.TryGetValue(key, out var valuesCollection)) {
				return Enumerable.Empty<TValue>();
			}
			return valuesCollection;
		}
	}

	public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator() {
		return _map.Keys
			.Select(key => new Grouping<TKey, TValue>(key, _map[key]))
			.Cast<IGrouping<TKey, TValue>>()
			.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	private IExtendedCollection<TValue> FetchValuesCollection(TKey key) {
		if (!_map.TryGetValue(key, out var values)) {
			values = _listFactory(key);
			_map[key] = values;
		}
		return values;
	}
}
