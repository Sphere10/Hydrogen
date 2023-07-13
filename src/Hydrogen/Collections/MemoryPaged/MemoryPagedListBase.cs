// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public abstract class MemoryPagedListBase<TItem> : PagedListBase<TItem>, IMemoryPagedList<TItem> {

	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading;
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded;
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving;
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved;
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading;
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded;

	private readonly ICache<int, IMemoryPage<TItem>> _loadedPages;
	private readonly ReadOnlyListDecorator<IPage<TItem>, IMemoryPage<TItem>> _pagesDecorator;

	protected bool Disposing;

	protected MemoryPagedListBase(int pageSize, long maxMemory) {
		Guard.ArgumentInRange(pageSize, 1, int.MaxValue, nameof(pageSize));
		Guard.ArgumentInRange(maxMemory, pageSize, long.MaxValue, nameof(maxMemory));
		PageSize = pageSize;
		FlushOnDispose = false;
		Disposing = false;
		_pagesDecorator = new ReadOnlyListDecorator<IPage<TItem>, IMemoryPage<TItem>>(new ReadOnlyListAdapter<IPage<TItem>>(InternalPages));
		_loadedPages = new ActionCache<int, IMemoryPage<TItem>>(
			(page) => {
				Guard.ArgumentInRange(page, 0, InternalPages.Count - 1, nameof(page), "Page not contained in list");
				var memPage = (IMemoryPage<TItem>)InternalPages[page];
				NotifyPageAccessing(memPage);
				NotifyPageLoading(memPage);
				memPage.Load();
				NotifyPageLoaded(memPage);
				NotifyPageAccessed(memPage);
				return memPage;
			},
			sizeEstimator: p => (uint)pageSize,
			reapStrategy: CacheReapPolicy.LeastUsed,
			expirationStrategy: ExpirationPolicy.SinceLastAccessedTime,
			maxCapacity: (uint)maxMemory
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

	public new IReadOnlyList<IMemoryPage<TItem>> Pages => _pagesDecorator;

	public int PageSize { get; }

	public int CurrentOpenPages => _loadedPages.ItemCount;

	public bool Disposed { get; protected set; }

	public long MaxMemory {
		get => (int)_loadedPages.MaxCapacity;
		//internal set => _loadedPages.MaxCapacity = value;
	}

	public virtual bool Dirty => InternalPages.Any(p => p.Dirty);

	public bool FlushOnDispose { get; set; }

	public int CalculateTotalSize() => InternalPages.Sum(p => p.Size);

	public sealed override IDisposable EnterOpenPageScope(IPage<TItem> page) {
		// dont need to do much since cache manages life-cycle of page
		CheckNotDisposed();
		var cachedItem = _loadedPages.Get(page.Number); // ensures page is fetched from storage if not cached
		cachedItem.CanPurge = false;
		return new ActionScope(() => cachedItem.CanPurge = true);
	}

	public virtual void Flush() {
		// Causes all dirty pages to be saved
		// and all loaded pages to be unloaded
		_loadedPages.Flush();
	}

	public virtual void Dispose() {
		if (FlushOnDispose)
			Flush();
		Disposing = true;
		if (!RequiresLoad)
			Clear();
		Disposed = true;
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
		OnPageLoading(page);
		PageLoading?.Invoke(this, page);
	}

	private void NotifyPageLoaded(IMemoryPage<TItem> page) {
		OnPageLoaded(page);
		PageLoaded?.Invoke(this, page);
	}

	private void NotifyPageSaving(IMemoryPage<TItem> page) {
		OnPageSaving(page);
		PageSaving?.Invoke(this, page);
	}

	private void NotifyPageSaved(IMemoryPage<TItem> page) {
		OnPageSaved(page);
		PageSaved?.Invoke(this, page);
	}

	private void NotifyPageUnloading(IMemoryPage<TItem> page) {
		OnPageUnloading(page);
		PageUnloading?.Invoke(this, page);
	}

	private void NotifyPageUnloaded(IMemoryPage<TItem> page) {
		OnPageUnloaded(page);
		PageUnloaded?.Invoke(this, page);
	}

	#endregion

	#region Private methods

	private void CheckNotDisposed() {
		if (Disposed)
			throw new InvalidOperationException("Disposing or disposed");
	}

	#endregion

}
