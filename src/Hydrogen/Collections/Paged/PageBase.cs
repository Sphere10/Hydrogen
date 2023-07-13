// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hydrogen;

public abstract class PageBase<TItem> : IEnumerable<TItem>, IPage<TItem> {

	protected PageBase() {
		State = PageState.Unloaded;
	}

	public virtual int Number { get; set; }
	public virtual int StartIndex { get; set; }
	public virtual int EndIndex { get; set; }
	public virtual int Count { get; set; }
	public virtual int Size { get; set; }
	public virtual bool Dirty { get; set; }
	public virtual PageState State { get; set; }

	public IEnumerable<TItem> Read(int index, int count) {
		CheckPageState(PageState.Loaded);
		CheckRange(index, count);
		return ReadInternal(index, count);
	}

	public bool Write(int index, IEnumerable<TItem> items, out IEnumerable<TItem> overflow) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as TItem[] ?? items.ToArray();
		CheckPageState(PageState.Loaded);
		Guard.ArgumentInRange(index, StartIndex, Math.Max(StartIndex, EndIndex) + 1, nameof(index));

		// Nothing to write case
		if (itemsArr.Length == 0) {
			overflow = Enumerable.Empty<TItem>();
			return true;
		}

		// Update segment
		var updateCount = Math.Min(StartIndex + Count - index, itemsArr.Length);
		if (updateCount > 0) {
			var updateItems = itemsArr.Take(updateCount).ToArray();
			UpdateInternal(index, updateItems, out var oldItemsSpace, out var newItemsSpace);

			// TODO: support this scenario if ever needed, lots of complexity in ensuring updated page doesn't overflow max size from superclasses.
			// Can lead to cascading page updates. 
			// For constant sized objects (like byte arrays) this will never fail since the updated regions will always remain the same size.
			Guard.Against(oldItemsSpace != newItemsSpace, "Updated a page with different sized objects is not supported in this collection.");

			Size = Size - oldItemsSpace + newItemsSpace;
		}

		// Append segment
		var appendItems = updateCount > 0 ? itemsArr.Skip(updateCount).ToArray() : itemsArr;
		var appendCount = 0;
		if (appendItems.Length > 0) {
			appendCount = AppendInternal(appendItems, out var appendedItemsSpace);
			Count += appendCount;
			EndIndex += appendCount;
			Size += appendedItemsSpace;
		}

		var totalWriteCount = updateCount + appendCount;
		// Was unable to write the first element in an empty page, item too large
		Guard.Against(Count == 0 && totalWriteCount == 0, $"Item cannot be fitted onto a page of this collection");

		if (totalWriteCount > 0)
			Dirty = true;
		overflow = totalWriteCount < itemsArr.Length ? itemsArr.Skip(totalWriteCount).ToArray() : Enumerable.Empty<TItem>();
		Debug.Assert(totalWriteCount <= itemsArr.Length);
		return totalWriteCount == itemsArr.Length;
	}

	public void EraseFromEnd(int count) {
		Guard.ArgumentInRange(count, 0, Count, nameof(count));
		if (count <= 0)
			return;

		EraseFromEndInternal(count, out var oldItemsSpace);
		Size -= oldItemsSpace;
		Count -= count;
		EndIndex -= count;
		Dirty = true;
	}

	public abstract IEnumerator<TItem> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	protected abstract IEnumerable<TItem> ReadInternal(int index, int count);

	protected abstract int AppendInternal(TItem[] items, out int newItemsSize);

	protected abstract void UpdateInternal(int index, TItem[] items, out int oldItemsSize, out int newItemsSize);

	protected abstract void EraseFromEndInternal(int count, out int oldItemsSize);

	internal void CheckRange(int index, int count) {
		var startIX = StartIndex;
		var lastIX = startIX + (Count - 1).ClipTo(startIX, int.MaxValue);
		Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
		if (count > 0)
			Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
	}

	internal void CheckPageState(PageState status)
		=> Guard.Ensure(State == status, $"Page not {status}");

	protected void CheckPageState(params PageState[] statuses)
		=> Guard.Ensure(State.IsIn(statuses), $"Page not in states {statuses.ToDelimittedString(",")}");

	protected void CheckDirty()
		=> Guard.Ensure(Dirty, $"Page was not dirty");

	protected void CheckNotDirty()
		=> Guard.Ensure(!Dirty, $"Page was dirty");

}
