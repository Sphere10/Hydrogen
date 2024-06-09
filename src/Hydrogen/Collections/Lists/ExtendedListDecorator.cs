// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Decorator pattern for an IExtendedList, but calls to non-range get routed to the range-based methods.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TConcrete"></typeparam>
/// <remarks>At first glance the implementation may counter-intuitive, but it is born out of extensive usage and optimization. The <see cref="TConcrete"/>
/// generic argument ensures sub-classes can retrieve the decorated list in it's type,without an expensive chain of casts/retrieves.</remarks>
public abstract class ExtendedListDecorator<TItem, TConcrete> : ExtendedCollectionDecorator<TItem, TConcrete>, IExtendedList<TItem>
	where TConcrete : IExtendedList<TItem> {

	protected ExtendedListDecorator(TConcrete internalExtendedList) : base(internalExtendedList) {
	}

	public virtual long IndexOfL(TItem item) => InternalCollection.IndexOfL(item);

	public virtual IEnumerable<long> IndexOfRange(IEnumerable<TItem> items) => InternalCollection.IndexOfRange(items);

	public virtual TItem Read(long index) => InternalCollection.Read(index);

	public virtual IEnumerable<TItem> ReadRange(long index, long count) => InternalCollection.ReadRange(index, count);

	public virtual void Update(long index, TItem item) => InternalCollection.Update(index, item);

	public virtual void UpdateRange(long index, IEnumerable<TItem> items) => InternalCollection.UpdateRange(index, items);

	public virtual void Insert(long index, TItem item) => InternalCollection.Insert(index, item);

	public virtual void InsertRange(long index, IEnumerable<TItem> items) => InternalCollection.InsertRange(index, items);

	public virtual void RemoveAt(long index) => InternalCollection.RemoveAt(index);

	public virtual void RemoveRange(long index, long count) => InternalCollection.RemoveRange(index, count);

	// The indexing operators are implemented to call the Read/Update methods to isolate this functionality
	// into a single place for decoration
	public TItem this[long index] {
		get => Read(index);
		set => Update(index, value);
	}

	#region Legacy int-indexing members

	// Int-addressing members are implemented by calling the long-based equivalents, to prevent decorating and discourage their use
	public int IndexOf(TItem item) => checked((int)IndexOfL(item));

	public void Insert(int index, TItem item) => Insert((long)index, item);

	public void RemoveAt(int index) => RemoveAt((long)index);

	public TItem this[int index] {
		get => this[(long)index];
		set => this[(long)index] = value;
	}

	#endregion

}


/// <summary>
/// Decorator pattern for an IExtendedList, but calls to non-range get routed to the range-based methods.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>At first glance the implementation may counter-intuitive, but it is born out of extensive usage and optimization. The <see cref="TConcrete"/>
/// generic argument ensures sub-classes can retrieve the decorated list in it's type,without an expensive chain of casts/retrieves.</remarks>
public abstract class ExtendedListDecorator<TItem> : ExtendedListDecorator<TItem, IExtendedList<TItem>> {
	protected ExtendedListDecorator(IExtendedList<TItem> internalExtendedList)
		: base(internalExtendedList) {
	}
}
