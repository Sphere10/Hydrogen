// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public abstract class ExtendedCollectionDecorator<TItem, TConcrete> : CollectionDecorator<TItem, TConcrete>, IExtendedCollection<TItem>
	where TConcrete : IExtendedCollection<TItem> {
	protected ExtendedCollectionDecorator(TConcrete innerCollection)
		: base(innerCollection) {
	}

	int ICollection<TItem>.Count => checked((int)Count);

	public new virtual long Count => InternalCollection.Count;

	public virtual IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => InternalCollection.ContainsRange(items);

	public virtual void AddRange(IEnumerable<TItem> items) => InternalCollection.AddRange(items);

	public virtual IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) => InternalCollection.RemoveRange(items);
}


public abstract class ExtendedCollectionDecorator<TItem> : ExtendedCollectionDecorator<TItem, IExtendedCollection<TItem>> {
	protected ExtendedCollectionDecorator(IExtendedCollection<TItem> innerCollection)
		: base(innerCollection) {
	}
}
