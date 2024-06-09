// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// A base implementation of <see cref="IExtendedList{T}"/> optimized for range-based (batch) operations. Single-item operations are implemented in terms of range-based operations.
/// </summary>
public abstract class RangedListBase<T> : ExtendedListBase<T> {

	protected RangedListBase() {
		Version = 0;
	}

	public override bool IsReadOnly => false;

	public sealed override long IndexOfL(T item) {
		return IndexOfRange(new[] { item }).Single();
	}

	public sealed override bool Contains(T item) {
		return ContainsRange(new[] { item }).First();
	}

	public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) => IndexOfRange(items).Select(ix => ix >= 0);

	public sealed override T Read(long index) {
		return ReadRange(index, 1).Single();
	}

	public sealed override void Add(T item) {
		AddRange(new[] { item });
	}

	public sealed override void Update(long index, T item) {
		UpdateRange(index, new[] { item });
	}

	public sealed override void Insert(long index, T item) {
		InsertRange(index, new[] { item });
	}

	public sealed override void RemoveAt(long index) {
		RemoveRange(index, 1);
	}

	public sealed override bool Remove(T item) {
		return RemoveRange(new[] { item }).First();
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) {
		var itemsArr = items as T[] ?? items.ToArray();

		// Remove nothing
		if (itemsArr.Length == 0)
			return Enumerable.Empty<bool>();

		// optimize for single read
		if (itemsArr.Length == 1) {
			var ix = IndexOfRange(itemsArr).First();
			if (ix >= 0) {
				RemoveRange(ix, 1);
				return new[] { true };
			}
			return new[] { false };
		}
		throw new NotImplementedException("For more than 1 item, todo");
		//var itemIndexes =  itemsArr.ZipWith(IndexOfRange(itemsArr), Tuple.Create);

		//// remove in reverse
		//itemIndexes.OrderByDescending(x => x.Item2).RemoveRange(
	}

	public override void Clear() {
		RemoveRange(0, Count);
	}

	public override void CopyTo(T[] array, int arrayIndex) {
		// This needs to be batch optimized
		foreach (var item in this)
			array[arrayIndex++] = item;
	}

	public override IEnumerator<T> GetEnumerator() {
		/// Try removing this method and CopyTo, seems to be being called in TransactionalList when specialized overloads are not (decorator issue)
		var version = this.Version;
		for (var i = 0; i < Count; i++) {
			CheckVersion(version);
			yield return Read(i);
		}
	}

	protected void CheckNotReadonly() {
		if (IsReadOnly)
			throw new InvalidOperationException("Collection is read-only");
	}

}
