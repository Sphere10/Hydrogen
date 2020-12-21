//-----------------------------------------------------------------------
// <copyright file="GroupOfAdjacent.cs" company="Sphere 10 Software">
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {
	public class GroupOfAdjacent<TSource, TKey> : IEnumerable<TSource>, IGrouping<TKey, TSource> {
		public TKey Key { get; set; }
		private List<TSource> GroupList { get; set; }

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<TSource>) this).GetEnumerator();
		}

		IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() {
			return ((IEnumerable<TSource>) GroupList).GetEnumerator();
		}

		public GroupOfAdjacent(List<TSource> source, TKey key) {
			GroupList = source;
			Key = key;
		}
	}
}
