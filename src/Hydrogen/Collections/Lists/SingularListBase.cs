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
/// Base class for singular item-by-item based extended list implementations. This is not optimized for batch access.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingularListBase<T> : ExtendedListBase<T> {

	public override bool IsReadOnly => false;

	public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items.Select(Contains).ToArray();
	}

	public override IEnumerable<long> IndexOfRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items.Select(IndexOfL);
	}

	public override IEnumerable<T> ReadRange(long index, long count) {
		Guard.ArgumentGTE(count, 0, nameof(index));
		return Tools.Collection.RangeL(index, count).Select(Read);
	}

	public override void UpdateRange(long index, IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var x in items)
			Update(index++, x);
	}

	public override void InsertRange(long index, IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var x in items)
			Insert(index++, x);
	}

	public override void RemoveRange(long index, long count) {
		Guard.ArgumentGTE(count, 0, nameof(index));
		Tools.Collection.Repeat(() => RemoveAt(index), count);
	}

	public override void AddRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			Add(item);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			yield return Remove(item);
	}

	public override void CopyTo(T[] array, int arrayIndex) {
		foreach(var item in this)
			array[arrayIndex++] = item;
	}

	protected long EnsureSafe(long index, bool allowAtEnd = false) {
		CheckIndex(index, allowAtEnd);
		return index;
	}

	protected IEnumerable<T> EnsureSafe(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items;
	}
	
}
