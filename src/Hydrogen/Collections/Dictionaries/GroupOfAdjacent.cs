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

public class GroupOfAdjacent<TSource, TKey> : IEnumerable<TSource>, IGrouping<TKey, TSource> {
	public TKey Key { get; set; }
	private List<TSource> GroupList { get; set; }

	IEnumerator IEnumerable.GetEnumerator() {
		return ((IEnumerable<TSource>)this).GetEnumerator();
	}

	IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() {
		return ((IEnumerable<TSource>)GroupList).GetEnumerator();
	}

	public GroupOfAdjacent(List<TSource> source, TKey key) {
		GroupList = source;
		Key = key;
	}
}
