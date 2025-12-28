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
/// Implements page caching and lifecycle management for paged lists that keep only a subset of pages resident in memory.
/// Eviction, loading, and persistence hooks are surfaced via events so consumers can observe paging behavior.
/// </summary>
public abstract class MemoryPagedListBase<TItem> : PagedListBase<TItem>, IMemoryPagedList<TItem> {

	/// <inheritdoc />
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading;
	/// <inheritdoc />
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded;
	/// <inheritdoc />
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving;
	/// <inheritdoc />
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved;
	/// <inheritdoc />
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading;
	/// <inheritdoc />
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded;

	private readonly ICache<long, IMemoryPage<TItem>> _loadedPages;

	protected bool Disposing;

	protected MemoryPagedListBase(long pageSize, long maxMemory, bool autoLoad = false) 
		: base(false) {
		Guard.ArgumentInRange(pageSize, 1, int.MaxValue, nameof(pageSize));
		Guard.ArgumentInRange(maxMemory, pageSize, long.MaxValue, nameof(maxMemory));
		PageSize = pageSize;
		FlushOnDispose = false;
		Disposing = false;
		Pages = InternalPages.AsReadOnly().WithProjection(p => (IMemoryPage<TItem>)p);
		_loadedPages = new ActionCache<long, IMemoryPage<TItem>>(
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

		if (autoLoad && RequiresLoad)
			Load();
	}

	/// <summary>
	/// Exposes the pages managed by this list, including those that may be currently unloaded.
	/// </summary>
	public new IReadOnlyList<IMemoryPage<TItem>> Pages { get; }

	/// <summary>
	/// Maximum logical size of each page in items (not bytes).
	/// </summary>
	public long PageSize { get; }

	/// <summary>
	/// Number of pages currently cached in memory.
	/// </summary>
	public long CurrentOpenPages => _loadedPages.ItemCount;

	/// <summary>
	/// Indicates whether the list has been disposed.
	/// </summary>
	public bool Disposed { get; protected set; }

	/// <summary>
	/// Maximum number of bytes that can be occupied by loaded pages.
	/// </summary>
	public long MaxMemory => _loadedPages.MaxCapacity;

	/// <summary>
	/// Returns <c>true</c> when any page in the list is dirty.
	/// </summary>
	public virtual bool Dirty => InternalPages.Any(p => p.Dirty);

	/// <summary>
	/// When set, calls to <see cref="Dispose"/> automatically flush dirty pages and unload cached ones.
	/// </summary>
	public bool FlushOnDispose { get; set; }

	/// <summary>
	/// Calculates the aggregate size of every page, respecting implementations that measure size differently.
	/// </summary>
	public long CalculateTotalSize() => InternalPages.Sum(p => p.Size);

	/// <inheritdoc />
	public sealed override IDisposable EnterOpenPageScope(IPage<TItem> page) {
		// dont need to do much since cache manages life-cycle of page
		CheckNotDisposed();
		var cachedItem = _loadedPages.Get(page.Number); // ensures page is fetched from streams if not cached
		cachedItem.CanPurge = false;
		return new ActionScope(() => cachedItem.CanPurge = true);
	}

	/// <summary>
	/// Saves all dirty pages and unloads everything currently cached.
	/// </summary>
	public virtual void Flush() {
		// Causes all dirty pages to be saved
		// and all loaded pages to be unloaded
		_loadedPages.Purge();
	}

	/// <summary>
	/// Disposes the list, optionally flushing dirty pages first and clearing page metadata.
	/// </summary>
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
