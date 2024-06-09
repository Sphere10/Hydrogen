// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// An <see cref="ExtendedListDecorator{TItem, TConcrete}"/> that wraps the range-based members as calls to the equivalent singular-based members.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TConcrete"></typeparam>
public abstract class SingularListDecorator<TItem, TConcrete> : ExtendedListDecorator<TItem, TConcrete>
	where TConcrete : IExtendedList<TItem> {

	protected SingularListDecorator(TConcrete extendedList) : base(extendedList) {
	}

	public sealed override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items.Select(Contains).ToArray();
	}

	public sealed override IEnumerable<long> IndexOfRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items.Select(IndexOfL);
	}

	public sealed override IEnumerable<TItem> ReadRange(long index, long count) {
		Guard.ArgumentGTE(count, 0, nameof(index));
		return Tools.Collection.RangeL(index, count).Select(Read);
	}

	public sealed override void UpdateRange(long index, IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var x in items)
			Update(index++, x);
	}

	public sealed override void InsertRange(long index, IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var x in items)
			Insert(index++, x);
	}

	public sealed override void RemoveRange(long index, long count) {
		Guard.ArgumentGTE(count, 0, nameof(index));
		Tools.Collection.Repeat(() => RemoveAt(index), count);
	}

	public sealed override void AddRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			Add(item);
	}

	public sealed override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			yield return Remove(item);
	}

}


public abstract class SingularListDecorator<TItem> : SingularListDecorator<TItem, IExtendedList<TItem>> {
	protected SingularListDecorator(IExtendedList<TItem> internalExtendedList)
		: base(internalExtendedList) {
	}
}
