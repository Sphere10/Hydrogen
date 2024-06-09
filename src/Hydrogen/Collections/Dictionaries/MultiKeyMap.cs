// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class MultiKeyMap<K, V, C> : EnumerableKeyDictionary<K, C> where C : ICollection<V>, new() {

	public MultiKeyMap() {
	}

	public MultiKeyMap(IEqualityComparer<K> comparer) : base(comparer) {
	}

	public void Add(IEnumerable<K> key, V value) {
		this[key].Add(value);
	}

	public void Remove(IEnumerable<K> key, V value) {
		C list;
		if (base.TryGetValue(key, out list)) {
			list.Remove(value);
		}
	}

	public override bool TryGetValue(IEnumerable<K> key, out C value) {
		if (!base.TryGetValue(key, out value)) {
			base.Add(key, new C());
			value = base[key];
		}
		return true;
	}

}


public class MultiKeyMap<K, V> : MultiKeyMap<K, V, List<V>> {
}
