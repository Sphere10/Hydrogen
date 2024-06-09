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

public class CollectionGroup<TKey, TElement, TCollection> : IGrouping<TKey, TElement> where TCollection : ICollection<TElement>, new() {

	public CollectionGroup(TKey key, IEnumerable<TElement> elements) {
		Key = key;
		Elements = new TCollection();
		foreach (var element in elements)
			Elements.Add(element);
	}

	public TKey Key { get; }

	public TCollection Elements { get; }

	public IEnumerator<TElement> GetEnumerator() {
		return Elements.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}


public class CollectionGroup<TKey, TElement> : CollectionGroup<TKey, TElement, List<TElement>> {
	public CollectionGroup(TKey key, IEnumerable<TElement> elements)
		: base(key, elements) {
	}
}
