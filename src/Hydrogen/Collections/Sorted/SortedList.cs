// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class SortedList<T> : ExtendedCollectionDecorator<T>, ISortedList<T> {
	private readonly IComparer<T> _comparer;

	public SortedList(SortDirection sortDirection = SortDirection.Ascending, IComparer<T> comparer = null)
		: this(4, sortDirection, comparer) {
	}

	public SortedList(int initialCapacity, SortDirection sortDirection = SortDirection.Ascending, IComparer<T> comparer = null)
		: this(new ExtendedList<T>(initialCapacity, comparer.AsEqualityComparer()), sortDirection, comparer) {
	}

	public SortedList(IExtendedList<T> internalList, SortDirection sortDirection = SortDirection.Ascending, IComparer<T> comparer = null)
		: base(internalList) {
		Guard.ArgumentNotNull(internalList, nameof(internalList));
		comparer ??= Comparer<T>.Default;
		_comparer = sortDirection switch {
			SortDirection.Ascending => comparer,
			SortDirection.Descending => comparer.AsInverted(),
			_ => throw new ArgumentException("Must be Ascending or Descending (or null)", nameof(sortDirection))
		};
		if (internalList.Count > 0) {
			QuickSorter.Sort(InternalList, _comparer);
		}
	}

	protected IExtendedList<T> InternalList => (IExtendedList<T>)InternalCollection;

	public override void Add(T item) {
		var index = IndexOfL(item);
		var insertionPoint = index >= 0 ? index : ~index;
		InternalList.Insert(insertionPoint, item);
	}

	public override bool Contains(T item) => IndexOfL(item) >= 0;

	public override bool Remove(T item) {
		var index = IndexOfL(item);
		if (index < 0)
			return false;
		InternalList.RemoveAt(index);
		return true;
	}

	public virtual void RemoveAt(long index) => InternalList.RemoveAt(index);

	public long IndexOfL(T item) => InternalList.BinarySearch(item, _comparer);

	public IEnumerable<long> IndexOfRange(IEnumerable<T> items) => InternalList.IndexOfRange(items);

	public T Read(long index) => InternalList.Read(index);

	public IEnumerable<T> ReadRange(long index, long count) => InternalList.ReadRange(index, count);

	public virtual T this[long index] => Read(index);

	public T this[int index] => this[(long)index];
}
