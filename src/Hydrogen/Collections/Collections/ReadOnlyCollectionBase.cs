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

namespace Hydrogen;

public abstract class ReadOnlyCollectionBase<T> : ICollection<T> {
	
	public abstract int Count { get; }
	
	public bool IsReadOnly => true;

	public abstract IEnumerator<T> GetEnumerator();

	public abstract bool Contains(T item);

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public void CopyTo(T[] array, int arrayIndex) {
		foreach(var item in this)
			array[arrayIndex++] = item;
	}

	public void Add(T item) => throw new InvalidOperationException("This collection is read-only");

	public void Clear() => throw new InvalidOperationException("This collection is read-only");
	
	public bool Remove(T item) => throw new InvalidOperationException("This collection is read-only");

}
