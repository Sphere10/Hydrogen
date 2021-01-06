using System;
using System.Linq;

namespace Sphere10.Framework {

    public abstract class MemoryPagedList<TItem, TPage> : PagedListBase<TItem, TPage>, IDisposable where TPage : IMemoryPage<TItem> {

		public event EventHandlerEx<object, TPage> PageLoading;
		public event EventHandlerEx<object, TPage> PageLoaded;
		public event EventHandlerEx<object, TPage> PageSaving;
		public event EventHandlerEx<object, TPage> PageSaved;
		public event EventHandlerEx<object, TPage> PageUnloading;
		public event EventHandlerEx<object, TPage> PageUnloaded;

		private readonly ICache<TPage, TPage> _loadedPages;

		protected bool Disposing;

		protected MemoryPagedList(int pageSize, int maxCacheCapacity, CacheCapacityPolicy cachePolicy) {
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
	
	}


    public class MemoryPagedList<TItem> : MemoryPagedList<TItem, BinaryFormattedPage<TItem>> {
	    private readonly IObjectSizer<TItem> _sizer;

	    public MemoryPagedList(int pageSize, int maxOpenPages, int fixedItemSize)
		    : this(pageSize, maxOpenPages, new ConstantObjectSizer<TItem>(fixedItemSize)) {
	    }

	    public MemoryPagedList(int pageSize, int maxOpenPages, Func<TItem, int> itemSizer)
		    : this(pageSize, maxOpenPages, new ActionObjectSizer<TItem>(itemSizer)) {
	    }

	    private MemoryPagedList(int pageSize, int maxOpenPages, IObjectSizer<TItem> sizer)
		    : base(pageSize, maxOpenPages, CacheCapacityPolicy.CapacityIsMaxOpenPages) {
		    _sizer = sizer;
	    }

	    protected override BinaryFormattedPage<TItem> NewPageInstance(int pageNumber) {
		    return new BinaryFormattedPage<TItem>(this.PageSize, _sizer);
	    }

	 
    }

}