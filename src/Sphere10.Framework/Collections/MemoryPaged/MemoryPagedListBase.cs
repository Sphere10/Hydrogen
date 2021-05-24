using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public abstract class MemoryPagedListBase<TItem> : PagedListBase<TItem>, IMemoryPagedList<TItem>  {

		public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading;
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded;
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving;
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved;
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading;
		public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded;

		private readonly ICache<int, IMemoryPage<TItem>> _loadedPages;

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
			_loadedPages = new ActionCache<int, IMemoryPage<TItem>>(
				(page)=> {
					Guard.ArgumentInRange(page, 0, InternalPages.Count - 1, nameof(page), "Page not contained in list");
					var memPage = (IMemoryPage<TItem>)InternalPages[page];
					NotifyPageAccessing(memPage);
					NotifyPageLoading(memPage);
					memPage.Load();
					NotifyPageLoaded(memPage);
					NotifyPageAccessed(memPage);
					return memPage;
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
			
			_loadedPages.ItemRemoved += (page, key) => {
				var memPage = (IMemoryPage<TItem>)InternalPages[page];

				// Uncaching a deleted page, do nothing
				if (memPage.State == PageState.Deleted) 
					return;

				// Uncaching a dirty page, ensure saved
				if (memPage.Dirty && memPage.State != PageState.Deleting) {
					NotifyPageSaving(memPage);
					memPage.Save();
					NotifyPageSaved(memPage);
				}
				NotifyPageAccessing(memPage);
				NotifyPageUnloading(memPage);
				memPage.Unload();
				NotifyPageUnloaded(memPage);
				NotifyPageAccessed(memPage);
				if (memPage.State == PageState.Deleting)
					memPage.Dispose();
			};
		}

		public new IReadOnlyList<IMemoryPage<TItem>> Pages => new ReadOnlyListDecorator<IPage<TItem>, IMemoryPage<TItem>>(InternalPages);

		public int PageSize { get; }

		public int Size => InternalPages.Sum(p => p.Size);

		public int CurrentOpenPages => _loadedPages.ItemCount;

		public int MaxOpenPages {
			get => (int) _loadedPages.MaxCapacity;
			internal set => _loadedPages.MaxCapacity = value;
		}

		public virtual bool Dirty => InternalPages.Any(p => p.Dirty);

		public bool FlushOnDispose { get; set; }

		public sealed override IDisposable EnterOpenPageScope(IPage<TItem> page) {
			CheckNotDisposed();
			var _ = _loadedPages.Get(page.Number); // ensures page is fetched from storage if not cached
			return new Disposables(); // dont need to do anything, cache manages life-cycle of page
		}

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
			_loadedPages.Flush(); // clear cache (should be clear except obscure flows)
		}

		protected override void OnAccessing() {
			base.OnAccessing();
			CheckNotDisposed();
		}

		#region Events 

		protected override void OnPageDeleting(IPage<TItem> page) {
			base.OnPageDeleting(page);
			if (_loadedPages.ContainsCachedItem(page.Number))
				_loadedPages.Remove(page.Number);
		}

		protected virtual void OnPageLoading(IMemoryPage<TItem> page) {
		}

		protected virtual void OnPageLoaded(IMemoryPage<TItem> page) {
		}

		protected virtual void OnPageSaving(IMemoryPage<TItem> page) {
		}

		protected virtual void OnPageSaved(IMemoryPage<TItem> page) {
		}

		protected virtual void OnPageUnloading(IMemoryPage<TItem> page) {
		}

		protected virtual void OnPageUnloaded(IMemoryPage<TItem> page) {
		}
	
		private void NotifyPageLoading(IMemoryPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageLoading(page);
			PageLoading?.Invoke(this, page);
		}

		private void NotifyPageLoaded(IMemoryPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageLoaded(page);
			PageLoaded?.Invoke(this, page);
		}

		private void NotifyPageSaving(IMemoryPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageSaving(page);
			PageSaving?.Invoke(this, page);
		}

		private void NotifyPageSaved(IMemoryPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageSaved(page);
			PageSaved?.Invoke(this, page);
		}

		private void NotifyPageUnloading(IMemoryPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageUnloading(page);
			PageUnloading?.Invoke(this, page);
		}

		private void NotifyPageUnloaded(IMemoryPage<TItem> page) {
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
	
	}

}