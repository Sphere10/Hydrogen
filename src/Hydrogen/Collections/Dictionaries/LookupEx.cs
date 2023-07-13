// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class LookupEx<TKey, TValue> : ILookup<TKey, TValue> {
	private readonly Dictionary<TKey, List<TValue>> _map;

	public LookupEx()
		: this(EqualityComparer<TKey>.Default) {
	}

	public LookupEx(IEqualityComparer<TKey> comparer) {
		_map = new Dictionary<TKey, List<TValue>>(comparer);
	}

	public LookupEx(ILookup<TKey, TValue> values) : this(values, EqualityComparer<TKey>.Default) {
	}

	public LookupEx(ILookup<TKey, TValue> values, IEqualityComparer<TKey> comparer) {
		_map = new Dictionary<TKey, List<TValue>>(values.ToDictionary(v => v.Key, v => v.ToList()), comparer);
	}

	public IEnumerable<TKey> Keys => _map.Keys;

	public void Add(TKey key) {
		FetchMap(key);
	}

	public void Add(TKey key, TValue value) {
		FetchMap(key).Add(value);
	}

	public void Remove(TKey key, TValue value) {
		FetchMap(key).Remove(value);
	}

	public void AddRange(TKey key, IEnumerable<TValue> collection) {
		FetchMap(key).AddRange(collection);
	}

	public int Count => _map.Count;

	public IEnumerable<TValue> this[TKey key] {
		get {
			if (!_map.TryGetValue(key, out var list)) {
				return Enumerable.Empty<TValue>();
			}
			return list.AsReadOnly();
		}
		set { _map[key] = value.ToList(); }
	}

	public int CountForKey(TKey key) {
		if (_map.ContainsKey(key)) {
			return _map[key].Count;
		}
		return 0;
	}

	public bool Contains(TKey key) {
		return _map.ContainsKey(key);
	}

	public IDictionary<TKey, TValue[]> ToDictionary()
		=> _map.ToDictionary(x => x.Key, x => x.Value.ToArray());


	public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator() {
		return _map.Keys
			.Select(key => new Grouping<TKey, TValue>(key, _map[key]))
			.Cast<IGrouping<TKey, TValue>>()
			.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public void Clear() {
		_map.Clear();
	}

	private List<TValue> FetchMap(TKey key) {
		if (!_map.TryGetValue(key, out var values)) {
			values = new List<TValue>();
			_map[key] = values;
		}
		return values;
	}
}
