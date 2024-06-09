// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// A wrapper for <see cref="IExtendedList{T}"/> that implements insertion, deletion and append as update operations over a pre-allocated collection of items.
/// This is useful for converting an <see cref="IExtendedList{T}"/> that can only be mutated via "UPDATE" operations into one that supports INSERT/UPDATE/DELETE
/// via sequential UPDATE operations. This is achieved by maintaining a local <see cref="Count"/> property and by overwriting pre-allocated items on
/// append/insert, and "forgetting" them on delete. When the pre-allocated items are exhausted, a <see cref="PreAllocationPolicy"/> is used to grow
/// the underlying list. 
/// </summary>
/// <remarks>
/// When shuffling items around via copy/paste operations, they are done "one at a time" rather than in "in ranges" so as not to exhaust memory on
/// unbounded lists. Thus this class is suitable for wrapping unbounded lists of data without memory/computational complexity blowout.
/// Additionally, <see cref="Contains"/> and <see cref="ContainsRange"/> are overriden and implemented based on <see cref="IndexOf"/> and <see cref="IndexOfRange"/>
/// so as to ensure only the logical objects are searched (avoids false positives). Same is true for <see cref="Remove"/> and <see cref="RemoveRange(int,int)"/>.
/// </remarks>
public class UpdateOnlyList<TItem, TInner> : ExtendedListDecorator<TItem, TInner> where TInner : IExtendedList<TItem> {
	private long _count;
	private readonly PreAllocationPolicy _preAllocationPolicy;
	private readonly long _blockSize;
	private readonly Func<TItem> _activator;

	public UpdateOnlyList(TInner internalStore, Func<TItem> itemActivator)
		: this(internalStore, PreAllocationPolicy.MinimumRequired, 0, itemActivator) {
	}

	public UpdateOnlyList(TInner internalStore, long preAllocatedItemCount, Func<TItem> itemActivator)
		: this(internalStore, PreAllocationPolicy.Fixed, preAllocatedItemCount, itemActivator) {
	}

	public UpdateOnlyList(TInner internalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<TItem> itemActivator)
		: this(internalStore, 0, preAllocationPolicy, blockSize, itemActivator) {
	}

	public UpdateOnlyList(TInner internalStore, long existingItemsInInternalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<TItem> itemActivator)
		: base(internalStore) {
		_count = existingItemsInInternalStore;
		if (preAllocationPolicy.IsIn(PreAllocationPolicy.ByBlock, PreAllocationPolicy.Fixed)) {
			Guard.Argument(blockSize > 0, nameof(blockSize), $"Must be greater than 0 for policy {preAllocationPolicy}");
		}

		_preAllocationPolicy = preAllocationPolicy;
		_blockSize = blockSize;
		_activator = itemActivator;

		// Ensure enough capacity when in Fixed mode (since never allocates again)
		if (preAllocationPolicy == PreAllocationPolicy.Fixed) {
			internalStore.AddRange(Tools.Collection.RepeatValue(_activator(), Math.Max(0L, _blockSize - internalStore.Count)));
		}
	}

	public override long Count => _count;

	public virtual long Capacity => base.Count;

	public override long IndexOfL(TItem item) => ToLogicalIndex(base.IndexOf(item));

	public override IEnumerable<long> IndexOfRange(IEnumerable<TItem> items) => base.IndexOfRange(items).Select(ToLogicalIndex);

	public override bool Contains(TItem item) => IndexOf(item) > 0;

	public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => IndexOfRange(items).Select(ix => ix > 0);

	public override TItem Read(long index) {
		CheckIndex(index);
		return base.Read(index);
	}

	public override IEnumerable<TItem> ReadRange(long index, long count) {
		CheckRange(index, count);
		return base.ReadRange(index, count);
	}

	public override void Add(TItem item) => AddRange(new[] { item });

	public override void AddRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as TItem[] ?? items.ToArray();
		EnsureSpace(itemsArr.Length);
		base.UpdateRange(_count, itemsArr);
		_count += itemsArr.Length;
	}

	public override void Update(long index, TItem item) {
		CheckIndex(index, true);
		UpdateRange(index, new[] { item });
	}

	public override void UpdateRange(long index, IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as TItem[] ?? items.ToArray();
		CheckRange(index, itemsArr.Length);
		base.UpdateRange(index, itemsArr);
	}


	/*
	 public override void UpdateRange(long index, IEnumerable<T> items) {
	// TODO: needs updating to support multiple _internalArray's and 2^64-1 addressable items
	Guard.ArgumentNotNull(items, nameof(items));
	var itemsArr = items as T[] ?? items.ToArray();
	CheckRange(index, itemsArr.Length);

	if (itemsArr.Length == 0)
		return;

	UpdateVersion();
	Array.Copy(itemsArr, 0, _internalArray, index, itemsArr.Length);
}
 
 
 */



	public override void Insert(long index, TItem item) => this.InsertRange(index, new[] { item });

	public override void InsertRange(long index, IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as TItem[] ?? items.ToArray();
		CheckIndex(index, true);
		EnsureSpace(itemsArr.Length);

		// shuffle item after insertion point forward
		var movedRegionFromStartIX = index;
		var movedRegionFromEndIX = _count - 1;
		var movedRegionToStartIX = movedRegionFromStartIX + itemsArr.Length;
		//var movedRegionToEndIX = movedRegionFromEndIX + itemsArr.Length;

		ShuffleRight(InternalCollection, movedRegionFromStartIX, movedRegionToStartIX, _count - index, false);

		// finally, save the new items
		base.UpdateRange(index, itemsArr);

		_count += itemsArr.Length;
	}

	public override bool Remove(TItem item) => this.RemoveRange(new[] { item }).First();

	public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var indexes = IndexOfRange(items).WithIndex();
		// need to delete items in descending order
		throw new NotImplementedException();
	}

	public override void RemoveAt(long index) => this.RemoveRange(index, 1);

	public override void RemoveRange(long index, long count) {
		// TODO: this could be optimized by copying bounded ranges instead of 1-by-1. Will 
		// improve stream record performance in StreamContainer (but has to be in partitioned to avoid memory exhaustion on huge ranges)
		CheckRange(index, count);

		var movedRegionFromStartIX = index + count;
		//var movedRegionFromEndIX = _count - 1;
		var movedRegionToStartIX = index;
		//var movedRegionToEndIX = index + (movedRegionFromEndIX - movedRegionFromStartIX);

		ShuffleLeft(InternalCollection, movedRegionFromStartIX, movedRegionToStartIX, _count - movedRegionFromStartIX, false);

		_count -= count;
		if (_preAllocationPolicy == PreAllocationPolicy.MinimumRequired)
			ReduceExcessCapacity();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleLeft<T>(IExtendedList<T> list, long fromIndex, long toIndex, long count, bool integrityChecks = false) {
		if (integrityChecks) {
			Guard.ArgumentNotNull(list, nameof(list));
			Guard.ArgumentInRange(fromIndex, 0, list.Count, nameof(fromIndex));
			Guard.ArgumentInRange(toIndex, 0, list.Count, nameof(toIndex));
			Guard.ArgumentInRange(count, 0, list.Count - fromIndex, nameof(count));
			Guard.ArgumentLTE(toIndex, fromIndex, nameof(fromIndex));
		}
		for (var i = 0; i < count; i++) {
			var read = list.Read(i + fromIndex);
			list.Update(toIndex + i, read);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ShuffleRight<T>(IExtendedList<T> list, long fromIndex, long toIndex, long count, bool integrityChecks = false) {
		if (integrityChecks) {
			Guard.ArgumentNotNull(list, nameof(list));
			Guard.ArgumentInRange(fromIndex, 0, list.Count, nameof(fromIndex));
			Guard.ArgumentInRange(toIndex, 0, list.Count, nameof(toIndex));
			Guard.ArgumentInRange(count, 0, list.Count - fromIndex, nameof(count));
			Guard.ArgumentLTE(fromIndex, toIndex, nameof(fromIndex));
		}
		for (var i = count - 1; i >= 0; i--) {
			var read = list.Read(i + fromIndex);
			list.Update(toIndex + i, read);
		}
	}
	
	public override void Clear() {
		_count = 0;
		if (_preAllocationPolicy == PreAllocationPolicy.MinimumRequired)
			ReduceExcessCapacity();
	}

	public override void CopyTo(TItem[] array, int arrayIndex) {
		foreach (var item in this)
			array[arrayIndex++] = item;
	}

	public override IEnumerator<TItem> GetEnumerator() => base.GetEnumerator().WithBoundary(_count);

	private void EnsureSpace(long quantity) {
		var spareCapacity = (Capacity - Count) - quantity;
		if (spareCapacity < 0) {
			var required = -spareCapacity;
			var newPreAllocatedItems = _preAllocationPolicy switch {
				PreAllocationPolicy.Fixed => Enumerable.Empty<TItem>().ToArray(),
				PreAllocationPolicy.MinimumRequired => Tools.Collection.RepeatValue(_activator(), required).ToArray(),
				PreAllocationPolicy.ByBlock => Tools.Collection.RepeatValue(_activator(), _blockSize * (long)Math.Ceiling(required / (float)_blockSize)).ToArray(),
				_ => throw new ArgumentOutOfRangeException(nameof(_preAllocationPolicy), _preAllocationPolicy, null)
			};
			InternalCollection.AddRange(newPreAllocatedItems);
			spareCapacity = (Capacity - Count) - quantity;
			Guard.Ensure(spareCapacity >= 0, "Insufficient space");
		}
	}

	private void ReduceExcessCapacity() {
		Debug.Assert(_preAllocationPolicy == PreAllocationPolicy.MinimumRequired);
		var spareCapacity = (Capacity - Count);
		var spareCapacityI = Tools.Collection.CheckNotImplemented64bitAddressingLength(spareCapacity);
		if (spareCapacityI > 0) {
			if (typeof(TItem).HasSubType(typeof(IDisposable)))
				foreach (var item in InternalCollection.ReadRange(^spareCapacityI..).Cast<IDisposable>())
					item.Dispose();

			InternalCollection.RemoveRange(^spareCapacityI..);
		}
	}

	private long ToLogicalIndex(long index) {
		if (0 <= index && index <= _count - 1)
			return index;
		return -1;
	}

	protected bool ValidIndex(long index, bool allowAtEnd = false) => 0 <= index && (allowAtEnd ? index <= Count : index < Count);

	protected void CheckIndex(long index, bool allowAtEnd = false) => Guard.CheckIndex(index, 0, Count, allowAtEnd);

	protected void CheckRange(long index, long count, bool rightAligned = false) => Guard.CheckRange(index, count, rightAligned, 0, Count);

}

public class UpdateOnlyList<TItem> : UpdateOnlyList<TItem, IExtendedList<TItem>> {

	public UpdateOnlyList(Func<TItem> itemActivator)
		: this(PreAllocationPolicy.MinimumRequired, 0, itemActivator) {
	}

	public UpdateOnlyList(long preallocatedItemCount, Func<TItem> itemActivator)
		: this(PreAllocationPolicy.Fixed, preallocatedItemCount, itemActivator) {
	}

	public UpdateOnlyList(PreAllocationPolicy preAllocationPolicy, long blockSize, Func<TItem> itemActivator)
		: this(new ExtendedList<TItem>(), 0, preAllocationPolicy, blockSize, itemActivator) {
	}

	public UpdateOnlyList(IExtendedList<TItem> internalStore, long existingItemsInInternalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<TItem> itemActivator)
		: base(internalStore, existingItemsInInternalStore, preAllocationPolicy, blockSize, itemActivator) {
	}
}