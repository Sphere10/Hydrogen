using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sphere10.Framework {

	public abstract class MemoryPagedListBase<TItem, TPage> : PagedListBase<TItem, TPage>, IDisposable
		where TPage : MemoryPagedListBase<TItem, TPage>.MemoryPageBase {

		public event EventHandlerEx<object, TPage> PageLoading;
		public event EventHandlerEx<object, TPage> PageLoaded;
		public event EventHandlerEx<object, TPage> PageSaving;
		public event EventHandlerEx<object, TPage> PageSaved;
		public event EventHandlerEx<object, TPage> PageUnloading;
		public event EventHandlerEx<object, TPage> PageUnloaded;

		private readonly ICache<TPage, TPage> _loadedPages;

		protected bool Disposing;

		protected MemoryPagedListBase(int pageSize, int maxCacheCapacity, CacheCapacityPolicy cachePolicy) {
			Guard.ArgumentInRange(pageSize, 1, int.MaxValue, nameof(pageSize));
			PageSize = pageSize;
			switch (cachePolicy) {
				case CacheCapacityPolicy.CapacityIsMaxOpenPages:
				case CacheCapacityPolicy.CapacityIsTotalMemoryConsumed:
					Guard.ArgumentInRange(maxCacheCapacity, 1, uint.MaxValue, nameof(maxCacheCapacity));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(cachePolicy), cachePolicy, null);
			}
			FlushOnDispose = false;
			Disposing = false;
			_loadedPages = new ActionCache<TPage, TPage>(
				(page)=> {
					Guard.Argument(_pages.Contains(page), nameof(page), "Page not contained in list");
					NotifyPageAccessing(page);
					NotifyPageLoading(page);
					page.Load();
					NotifyPageLoaded(page);
					NotifyPageAccessed(page);
					return page;
				},
				sizeEstimator: p => {
					switch(cachePolicy) {
						case CacheCapacityPolicy.CapacityIsMaxOpenPages:
							return 1;
						case CacheCapacityPolicy.CapacityIsTotalMemoryConsumed:
							throw new NotImplementedException();
							//if (Sizer is ConstantObjectSizer<TItem> constantSizer) {
							//	return (uint)constantSizer.FixedSize * p.ItemCount;
							//}
							//return (uint)p.Select(Sizer.CalculateSize).Sum();
						default:
							throw new ArgumentOutOfRangeException(nameof(cachePolicy), cachePolicy, null);
					}
				}, 
				reapStrategy: CacheReapPolicy.LeastUsed,
				expirationStrategy: ExpirationPolicy.SinceLastAccessedTime,
				maxCapacity: (uint)maxCacheCapacity
			);
			
			_loadedPages.ItemRemoved += (page, item) => {
				if (page.Dirty && page.State != PageState.Deleting) {
					NotifyPageSaving(page);
					page.Save();
					NotifyPageSaved(page);
				}
				NotifyPageAccessing(page);
				NotifyPageUnloading(page);
				page.Unload();
				NotifyPageUnloaded(page);
				NotifyPageAccessed(page);
				if (page.State == PageState.Deleting)
					page.Dispose();
			};
		}

		public int PageSize { get; }

		public int Size => _pages.Sum(p => p.Size);

		public int CurrentOpenPages => _loadedPages.ItemCount;

		public int MaxOpenPages {
			get => (int) _loadedPages.MaxCapacity;
			internal set => _loadedPages.MaxCapacity = value;
		}

		public virtual bool Dirty => _pages.Any(p => p.Dirty);

		public bool FlushOnDispose { get; set; }

		public virtual void Flush() {
			// Causes all dirty pages to be saved
			// and all loaded pages to be unloaded
			_loadedPages.Flush();
		}

		public virtual void Dispose() {
			if (FlushOnDispose)
				Flush();
			SuppressNotifications = true;
			Disposing = true;
			Clear();
		}

		protected sealed override IDisposable EnterOpenPageScope(TPage page) {
			var _ = _loadedPages.Get(page); // ensures page is fetched from storage if not cached
			return new Disposables(); // dont need to do anything, cache manages life-cycle of page
		}

		protected override void OnAccessing() {
			base.OnAccessing();
			CheckNotDisposed();
		}

		protected override void OnPageDeleting(TPage page) {
			base.OnPageDeleting(page);
			if (_loadedPages.ContainsCachedItem(page))
				_loadedPages.Remove(page);
		}

		#region Events 

		protected virtual void OnPageLoading(TPage page) {
		}

		protected virtual void OnPageLoaded(TPage page) {
		}

		protected virtual void OnPageSaving(TPage page) {
		}

		protected virtual void OnPageSaved(TPage page) {
		}

		protected virtual void OnPageUnloading(TPage page) {
		}

		protected virtual void OnPageUnloaded(TPage page) {
		}
	
		private void NotifyPageLoading(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageLoading(page);
			PageLoading?.Invoke(this, page);
		}

		private void NotifyPageLoaded(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageLoaded(page);
			PageLoaded?.Invoke(this, page);
		}

		private void NotifyPageSaving(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageSaving(page);
			PageSaving?.Invoke(this, page);
		}

		private void NotifyPageSaved(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageSaved(page);
			PageSaved?.Invoke(this, page);
		}

		private void NotifyPageUnloading(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageUnloading(page);
			PageUnloading?.Invoke(this, page);
		}

		private void NotifyPageUnloaded(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageUnloaded(page);
			PageUnloaded?.Invoke(this, page);
		}

		#endregion

		#region Private methods

		private void CheckNotDisposed() {
			if (Disposing)
				throw new InvalidOperationException("Disposing or disposed");
		}

		#endregion

		#region Inner Types

		public enum CacheCapacityPolicy {
			CapacityIsMaxOpenPages,
			CapacityIsTotalMemoryConsumed,
		}

		#endregion

		public abstract class MemoryPageBase : PageBase {
			private readonly IExtendedList<TItem> _store;
			private readonly IObjectSizer<TItem> _sizer;

			protected MemoryPageBase(int maxSize, IObjectSizer<TItem> sizer, IExtendedList<TItem> store) {
				MaxSize = maxSize;
				_store = store;
				_sizer = sizer;
			}

			public int MaxSize { get; set; }

			public void Save() {
				CheckPageState(PageState.Loaded);
				CheckDirty();
				using (var writeStream = OpenWriteStream()) {
					SaveInternal(this, writeStream);
				}
				Dirty = false;
			}

			public void Load() {
				CheckPageState(PageState.Unloaded);
				State = PageState.Loading;
				if (Count > 0) {
					using (var readStream = OpenReadStream()) {
						// load store directly, otherwise gets registered as new items
						_store.AddRange(LoadInternal(readStream)); 
					}
				}
				State = PageState.Loaded;
				Dirty = false;
			}

			public virtual void Unload() {
				CheckPageState(PageState.Loaded, PageState.Deleting);
				State = PageState.Unloading;
				_store.Clear();
				State = PageState.Unloaded;
				Dirty = false;
			}

			public abstract void Dispose();

			public override IEnumerator<TItem> GetEnumerator() => _store.GetEnumerator();

			protected override IEnumerable<TItem> ReadInternal(int index, int count) => _store.ReadRange(index - StartIndex, count);

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
				_store.AddRange(appendItems);
				return appendItems.Length;
			}

			protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSpace, out int newItemsSpace) {
				if (_sizer.IsFixedSize) {
					oldItemsSpace = _sizer.FixedSize * items.Length;
					newItemsSpace = oldItemsSpace;
					_store.UpdateRange(index - StartIndex, items);
				} else {
					oldItemsSpace = MeasureConsumedSpace(index, items.Length, false, out _);
					newItemsSpace = items.Select(_sizer.CalculateSize).Sum();
					_store.UpdateRange(index - StartIndex, items);
				}
			}

			protected override void EraseFromEndInternal(int count, out int oldItemsSpace) {
				var index = EndIndex - count + 1;
				oldItemsSpace = MeasureConsumedSpace(index, count, false, out _);
				_store.RemoveRange(index - StartIndex, count);
			}

			protected virtual int MeasureConsumedSpace(int index, int count, bool fetchIndividualSizes, out int[] sizes) {
				CheckRange(index, count);
				if (_sizer.IsFixedSize) {
					sizes = fetchIndividualSizes ? Tools.Array.Gen(count, _sizer.FixedSize) : null;
					return _sizer.FixedSize * count;
				} else {
					sizes = _store.ReadRange(index - StartIndex, count).Select(_sizer.CalculateSize).ToArray();
					var totalSize = sizes.Sum();
					if (!fetchIndividualSizes)
						sizes = null;
					return totalSize;
				}
			}

			protected abstract void SaveInternal(IEnumerable<TItem> items, Stream stream);

			protected abstract IEnumerable<TItem> LoadInternal(Stream stream);

			protected abstract Stream OpenReadStream();

			protected abstract Stream OpenWriteStream();

		}

		public abstract class FileSwappedMemoryPage : MemoryPageBase {
			private readonly string _fileStore;

			protected FileSwappedMemoryPage(int pageSize, IObjectSizer<TItem> sizer, IExtendedList<TItem> memoryStore) 
				: this(pageSize, Tools.FileSystem.GetTempFileName(false), sizer, memoryStore) {
			}

			protected FileSwappedMemoryPage(int pageSize, string fileStore, IObjectSizer<TItem> sizer, IExtendedList<TItem> memoryStore) 
				: base(pageSize, sizer, memoryStore) {
				_fileStore = fileStore;
			}

			public override void Dispose() {
				if (File.Exists(_fileStore))
					File.Delete(_fileStore);
			}

			protected override Stream OpenReadStream() {
				if (!File.Exists(_fileStore))
					return Stream.Null;
				return File.OpenRead(_fileStore);
			}

			protected override Stream OpenWriteStream() {
				return File.Open(_fileStore, FileMode.OpenOrCreate, FileAccess.Write);
			}
		}
	}
}