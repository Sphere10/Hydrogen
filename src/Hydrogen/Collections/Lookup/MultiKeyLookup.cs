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

namespace Hydrogen;

public class MultiKeyLookup<K, V> : ILookup<IEnumerable<K>, V> {

	protected readonly EnumerableKeyDictionary<K, List<V>> InternalLookup;

	internal MultiKeyLookup(IEqualityComparer<K> comparer) {
		InternalLookup = new EnumerableKeyDictionary<K, List<V>>(comparer);
	}

	#region ILookup Implementation

	public IEnumerator<IGrouping<IEnumerable<K>, V>> GetEnumerator() {
		return
			InternalLookup.Keys.Select(key => new Grouping<IEnumerable<K>, V>(key, InternalLookup[key]))
				.Cast<IGrouping<IEnumerable<K>, V>>()
				.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public bool Contains(IEnumerable<K> key) {
		return InternalLookup.ContainsKey(key);
	}

	public int Count {
		get { return InternalLookup.Count; }
	}

	public IEnumerable<V> this[IEnumerable<K> key] {
		get {
			List<V> list;
			return !InternalLookup.TryGetValue(key, out list) ? Enumerable.Empty<V>() : list.Select(x => x);
		}
	}

	#endregion

	#region Convenience Methods

	public IEnumerable<V> this[params K[] key] {
		get { return this[key.AsEnumerable()]; }
	}

	public void Add(V value, params K[] key) {
		Add(key, value);
	}

	public bool Contains(params K[] key) {
		return Contains(key.AsEnumerable());
	}

	#endregion

	#region Auxilliary Methods

	internal void Add(IEnumerable<K> key, V element) {
		List<V> list;
		if (!InternalLookup.TryGetValue(key, out list)) {
			list = new List<V>();
			InternalLookup[key] = list;
		}
		list.Add(element);
	}

	#endregion

}
