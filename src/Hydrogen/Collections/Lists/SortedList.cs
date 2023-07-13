// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class SortedList<T> : CollectionDecorator<T>, IReadOnlyList<T> {
	private readonly IComparer<T> _comparer;

	public SortedList(SortDirection sortDirection = SortDirection.Ascending, IComparer<T> comparer = null)
		: this(4, sortDirection, comparer) {
	}

	public SortedList(int initialCapacity, SortDirection sortDirection = SortDirection.Ascending, IComparer<T> comparer = null)
		: this(new List<T>(initialCapacity), sortDirection, comparer) {
	}

	public SortedList(IList<T> internalList, SortDirection sortDirection = SortDirection.Ascending, IComparer<T> comparer = null)
		: base(internalList) {
		Guard.ArgumentNotNull(internalList, nameof(internalList));
		comparer ??= Comparer<T>.Default;
		_comparer = sortDirection switch {
			SortDirection.Ascending => comparer,
			SortDirection.Descending => new ReverseComparer<T>(comparer),
			_ => throw new ArgumentException("Must be Ascending or Descending (or null)", nameof(sortDirection))
		};
	}

	protected IList<T> InternalList => (IList<T>)base.InternalCollection;

	public override void Add(T item) {
		var index = InternalList.BinarySearch(item, _comparer);
		var insertionPoint = index >= 0 ? index : ~index;
		InternalList.Insert(insertionPoint, item);
	}

	public override bool Contains(T item) => InternalList.BinarySearch(item) >= 0;

	public override bool Remove(T item) {
		var index = InternalList.BinarySearch(item, _comparer);
		if (index >= 0) {
			InternalList.RemoveAt(index);
			return true;
		}
		return false;
	}

	public virtual void RemoveAt(int index) => InternalList.RemoveAt(index);

	public virtual T this[int index] => InternalList[index];

}
