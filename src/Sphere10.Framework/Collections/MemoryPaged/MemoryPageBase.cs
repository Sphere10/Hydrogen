using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
    public abstract class MemoryPageBase<TItem> : PageBase<TItem>, IMemoryPage<TItem> {
		private readonly IObjectSizer<TItem> _sizer;
		protected readonly IExtendedList<TItem> MemoryStore;

		protected MemoryPageBase(int maxSize, IObjectSizer<TItem> sizer, IExtendedList<TItem> store) {
			MaxSize = maxSize;
			MemoryStore = store;
			_sizer = sizer;
		}

		public int MaxSize { get; set; }

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

		protected override IEnumerable<TItem> ReadInternal(int index, int count) => MemoryStore.ReadRange(index - StartIndex, count);

		protected override int AppendInternal(TItem[] items, out int newItemsSpace) {
			TItem[] appendItems;
			if (_sizer.IsFixedSize) {
				// Optimized for constant sized objects (primitive types like bytes)
				var maxAppendCount = (MaxSize - Size) / _sizer.FixedSize;
				appendItems = items.Take(maxAppendCount).ToArray();
				newItemsSpace = appendItems.Length * _sizer.FixedSize;
			} else {
				// Used for variable length objects
				var newSpace = 0;
				appendItems = items.TakeWhile(item => {
					var itemSize = _sizer.CalculateSize(item);
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

		protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSpace, out int newItemsSpace) {
			if (_sizer.IsFixedSize) {
				oldItemsSpace = _sizer.FixedSize * items.Length;
				newItemsSpace = oldItemsSpace;
				MemoryStore.UpdateRange(index - StartIndex, items);
			} else {
				oldItemsSpace = MeasureConsumedSpace(index, items.Length, false, out _);
				newItemsSpace = items.Select(_sizer.CalculateSize).Sum();
				MemoryStore.UpdateRange(index - StartIndex, items);
			}
		}

		protected override void EraseFromEndInternal(int count, out int oldItemsSpace) {
			var index = EndIndex - count + 1;
			oldItemsSpace = MeasureConsumedSpace(index, count, false, out _);
			MemoryStore.RemoveRange(index - StartIndex, count);
		}

		protected virtual int MeasureConsumedSpace(int index, int count, bool fetchIndividualSizes, out int[] sizes) {
			CheckRange(index, count);
			if (_sizer.IsFixedSize) {
				sizes = fetchIndividualSizes ? Tools.Array.Gen(count, _sizer.FixedSize) : null;
				return _sizer.FixedSize * count;
			} else {
				sizes = MemoryStore.ReadRange(index - StartIndex, count).Select(_sizer.CalculateSize).ToArray();
				var totalSize = sizes.Sum();
				if (!fetchIndividualSizes)
					sizes = null;
				return totalSize;
			}
		}

		protected abstract void SaveInternal(IExtendedList<TItem> memoryPage, Stream stream);

		protected abstract void LoadInternal(Stream stream, IExtendedList<TItem> memoryPage);

		protected abstract Stream OpenReadStream();

		protected abstract Stream OpenWriteStream();

	}
}