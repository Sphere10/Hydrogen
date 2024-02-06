// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public abstract class RecyclableListDecorator<TItem, TConcrete> : ExtendedListDecorator<TItem, TConcrete>, IRecyclableList<TItem>
	where TConcrete : IRecyclableList<TItem> {

	protected RecyclableListDecorator(TConcrete internalExtendedList) : base(internalExtendedList) {
	}

	public virtual long ListCount => InternalCollection.ListCount;

	public virtual long RecycledCount => InternalCollection.RecycledCount;

	public void Add(TItem item, out long index) => InternalCollection.Add(item, out index);

	public virtual void Recycle(long index) => InternalCollection.Recycle(index);

	public virtual bool IsRecycled(long index) => InternalCollection.IsRecycled(index);

}

public abstract class RecyclableListDecorator<TItem> : RecyclableListDecorator<TItem, IRecyclableList<TItem>> {
	protected RecyclableListDecorator(IRecyclableList<TItem> internalExtendedList)
		: base(internalExtendedList) {
	}
}
