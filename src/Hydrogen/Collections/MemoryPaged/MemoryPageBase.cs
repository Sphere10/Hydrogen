// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hydrogen;

public abstract class MemoryPageBase<TItem> : PageBase<TItem>, IMemoryPage<TItem> {
	internal readonly IExtendedList<TItem> MemoryStore;

	protected MemoryPageBase(long maxSize, IItemSizer<TItem> sizer, IExtendedList<TItem> store) {
		MaxSize = maxSize;
		MemoryStore = store;
		Sizer = sizer;
	}

	public long MaxSize { get; set; }

	public IItemSizer<TItem> Sizer { get; }

	public void Save() {
		CheckPageState(PageState.Loaded);
		CheckDirty();
		using (var writeStream = OpenWriteStream()) {
			SaveInternal(MemoryStore, writeStream);
		}
		Dirty = false;
	}

	public void Load() {
		CheckPageState(PageState.Unloaded);
		State = PageState.Loading;
		if (Count > 0) {
			using (var readStream = OpenReadStream()) {
				// load store directly, otherwise gets registered as new items
				LoadInternal(readStream, MemoryStore);
			}
		}
		State = PageState.Loaded;
		Dirty = false;
	}

	public virtual void Unload() {
		CheckPageState(PageState.Loaded, PageState.Deleting);
		State = PageState.Unloading;
		MemoryStore.Clear();
		State = PageState.Unloaded;
		Dirty = false;
	}

	public abstract void Dispose();

	public override IEnumerator<TItem> GetEnumerator() {
		CheckPageState(PageState.Loaded);
		return MemoryStore.GetEnumerator();
	}

	protected override IEnumerable<TItem> ReadInternal(long index, long count) => MemoryStore.ReadRange(index - StartIndex, count);

	protected override long AppendInternal(TItem[] items, out long newItemsSpace) {
		TItem[] appendItems;
		if (Sizer.IsConstantSize) {
			// Optimized for constant sized objects (primitive types like bytes)
			var maxAppendCount = (MaxSize - Size) / Sizer.ConstantSize;
			appendItems = items.TakeL(maxAppendCount).ToArray();
			newItemsSpace = appendItems.Length * Sizer.ConstantSize;
		} else {
			// Used for variable length objects
			var newSpace = 0L;
			appendItems = items.TakeWhile(item => {
				var itemSize = Sizer.CalculateSize(item);
				if (Size + newSpace + itemSize > MaxSize)
					return false;
				newSpace += itemSize;
				return true;
			}).ToArray();
			newItemsSpace = newSpace;
		}
		MemoryStore.AddRange(appendItems);
		return appendItems.Length;
	}

	protected override void UpdateInternal(long index, TItem[] items, out long oldItemsSpace, out long newItemsSpace) {
		if (Sizer.IsConstantSize) {
			oldItemsSpace = Sizer.ConstantSize * items.Length;
			newItemsSpace = oldItemsSpace;
			MemoryStore.UpdateRange(index - StartIndex, items);
		} else {
			oldItemsSpace = MeasureConsumedSpace(index, items.Length, false, out _);
			newItemsSpace = items.Select(item => Sizer.CalculateSize(item)).Sum();
			MemoryStore.UpdateRange(index - StartIndex, items);
		}
	}

	protected override void EraseFromEndInternal(long count, out long oldItemsSpace) {
		var index = EndIndex - count + 1;
		oldItemsSpace = MeasureConsumedSpace(index, count, false, out _);
		MemoryStore.RemoveRange(index - StartIndex, count);
	}

	protected virtual long MeasureConsumedSpace(long index, long count, bool fetchIndividualSizes, out long[] sizes) {
		CheckRange(index, count);
		if (Sizer.IsConstantSize) {
			sizes = fetchIndividualSizes ? Tools.Array.Gen(count, Sizer.ConstantSize) : null;
			return Sizer.ConstantSize * count;
		}
		sizes = MemoryStore.ReadRange(index - StartIndex, count).Select(x => Sizer.CalculateSize(x)).ToArray();
		var totalSize = sizes.Sum();
		if (!fetchIndividualSizes)
			sizes = null;
		return totalSize;
	}

	protected abstract void SaveInternal(IExtendedList<TItem> memoryPage, Stream stream);

	protected abstract void LoadInternal(Stream stream, IExtendedList<TItem> memoryPage);

	protected abstract Stream OpenReadStream();

	protected abstract Stream OpenWriteStream();

}
