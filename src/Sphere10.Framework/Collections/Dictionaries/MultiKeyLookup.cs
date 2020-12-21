//-----------------------------------------------------------------------
// <copyright file="MultiKeyLookup.cs" company="Sphere 10 Software">
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	public class MultiKeyLookup<K, V> : ILookup<IEnumerable<K>, V> {

		protected readonly MultiKeyDictionary2<K, List<V>> InternalLookup;

		internal MultiKeyLookup(IEqualityComparer<K> comparer) {
			InternalLookup = new MultiKeyDictionary2<K, List<V>>(comparer);
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

		public int Count { get { return InternalLookup.Count; } }

		public IEnumerable<V> this[IEnumerable<K> key] {
			get {
				List<V> list;
				return !InternalLookup.TryGetValue(key, out list) ? Enumerable.Empty<V>() : list.Select(x => x);
			}
		}

		#endregion

		#region Convenience Methods

		public IEnumerable<V> this[params K[] key] {
			get {
				return this[key.AsEnumerable()];
			}
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

}
