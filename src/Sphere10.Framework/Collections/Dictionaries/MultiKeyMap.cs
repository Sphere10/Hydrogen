//-----------------------------------------------------------------------
// <copyright file="MultiKeyMap.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sphere10.Framework {


	public class MultiKeyMap<K, V, C> : MultiKeyDictionary2<K, C> where C : ICollection<V>, new() {

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

}
