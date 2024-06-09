// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public abstract class CollectionDecorator<TItem, TConcrete> : ICollection<TItem> where TConcrete : ICollection<TItem> {
	internal TConcrete InternalCollection;

	public virtual int Count => InternalCollection.Count;

	public virtual bool IsReadOnly => InternalCollection.IsReadOnly;

	protected CollectionDecorator(TConcrete innerCollection) {
		InternalCollection = innerCollection;
	}

	public virtual IEnumerator<TItem> GetEnumerator() {
		return InternalCollection.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public virtual void Add(TItem item) {
		InternalCollection.Add(item);
	}

	public virtual void Clear() {
		InternalCollection.Clear();
	}

	public virtual bool Contains(TItem item) {
		return InternalCollection.Contains(item);
	}

	public virtual void CopyTo(TItem[] array, int arrayIndex) {
		InternalCollection.CopyTo(array, arrayIndex);
	}

	public virtual bool Remove(TItem item) {
		return InternalCollection.Remove(item);
	}

}


public abstract class CollectionDecorator<TItem> : CollectionDecorator<TItem, ICollection<TItem>> {
	protected CollectionDecorator(ICollection<TItem> innerCollection)
		: base(innerCollection) {
	}
}
