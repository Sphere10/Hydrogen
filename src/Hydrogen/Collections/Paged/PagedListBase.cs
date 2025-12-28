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
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Base implementation for paged lists that handles page lifecycle bookkeeping, event dispatching, and range operations.
/// Subclasses provide the storage mechanics for creating, loading, and persisting individual pages.
/// </summary>
public abstract class PagedListBase<TItem> : RangedListBase<TItem>, IPagedList<TItem> {

	/// <inheritdoc />
	public event EventHandlerEx<object> Accessing;
	/// <inheritdoc />
	public event EventHandlerEx<object> Accessed;
	/// <inheritdoc />
	public event EventHandlerEx<object> Loading;
	/// <inheritdoc />
	public event EventHandlerEx<object> Loaded;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageAccessing;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageAccessed;
	/// <inheritdoc />
	public event EventHandlerEx<object, long> PageCreating;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageCreated;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageReading;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageRead;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageWriting;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageWrite;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageDeleting;
	/// <inheritdoc />
	public event EventHandlerEx<object, IPage<TItem>> PageDeleted;

	private long _count;
	private long _lastFoundPage;
	private readonly ReadOnlyListAdapter<IPage<TItem>> _pagesAdapter;
	protected IExtendedList<IPage<TItem>> InternalPages;

	protected PagedListBase(bool autoLoad) {
		RequiresLoad = false;
		IsLoading = false;
		InternalPages = new ExtendedList<IPage<TItem>>();
		_count = 0;
		_pagesAdapter = new ReadOnlyListAdapter<IPage<TItem>>(InternalPages);
		_lastFoundPage = -1;

		if (autoLoad)
			Load();
	}

	/// <summary>
	/// Gets the current item count across all pages.
	/// </summary>
	public override long Count => _count;

	/// <summary>
	/// Exposes the pages composing the list in read-only form.
	/// </summary>
	public virtual IReadOnlyList<IPage<TItem>> Pages => _pagesAdapter;

	/// <summary>
	/// Indicates whether the list must be loaded before accessing data.
	/// </summary>
	public bool RequiresLoad { get; protected set; }

	protected bool IsLoading { get; private set; }

	/// <summary>
	/// Loads all persisted pages into the list, rebuilding the in-memory index.
	/// </summary>
	public void Load() {
		if (!RequiresLoad)
			return;

		NotifyLoading();
		RequiresLoad = false;
		IsLoading = true;
		try {
			Clear();
			foreach (var page in LoadPages()) {
				InternalPages.Add(page);
				_count += page.Count;
			}
		} finally {
			IsLoading = false;
		}
		NotifyLoaded();
	}

	/// <summary>
	/// Asynchronously invokes <see cref="Load"/>.
	/// </summary>
	public Task LoadAsync() => Task.Run(Load);

	public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => throw new NotSupportedException();

	public override IEnumerable<long> IndexOfRange(IEnumerable<TItem> items) => throw new NotSupportedException();

	/// <summary>
	/// Reads items across one or more pages, yielding each page segment in order.
	/// </summary>
	/// <param name="index">Zero-based starting index.</param>
	/// <param name="count">Number of items to read.</param>
	/// <returns>Sequential batches aligned to page boundaries.</returns>
	public override IEnumerable<TItem> ReadRange(long index, long count) {
		return ReadRangeByPage(index, count).SelectMany(x => x);
	}

	/// <summary>
	/// Reads items grouped by the pages they fall into, issuing page lifecycle notifications as needed.
	/// </summary>
	public virtual IEnumerable<IEnumerable<TItem>> ReadRangeByPage(long index, long count) {
		CheckRange(index, count);
		CheckLoaded();
		NotifyAccessing();
		CheckRange(index, count);
		foreach (var pageSegment in GetPageSegments(index, count)) {
			var page = pageSegment.Item1;
			var pageStartIndex = pageSegment.Item2;
			var pageItemCount = pageSegment.Item3;
			NotifyPageAccessing(page);
			using (EnterOpenPageScope(page)) {
				NotifyPageReading(page);
				yield return page.Read(pageStartIndex, pageItemCount);
				NotifyPageRead(page);
			}
			NotifyPageAccessed(page);
		}
		NotifyAccessed();
	}

	public override void AddRange(IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		CheckLoaded();
		NotifyAccessing();

		var page = InternalPages.Any() ? InternalPages.Last() : CreateNextPage();
		_count -= page.Count;

		bool AppendToPage(out IEnumerable<TItem> remaining) {
			NotifyPageAccessing(page);
			bool fittedCompletely;
			using (EnterOpenPageScope(page)) {
				NotifyPageWriting(page);
				UpdateVersion();
				fittedCompletely = page.Write(page.EndIndex + 1, items, out remaining);
				NotifyPageWrite(page);
			}
			NotifyPageAccessed(page);
			return fittedCompletely;
		}

		while (!AppendToPage(out var remaining)) {
			_count += page.Count;
			page = CreateNextPage();
			items = remaining;
		}
		_count += page.Count;
		NotifyAccessed();
	}

	public override void UpdateRange(long index, IEnumerable<TItem> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		CheckLoaded();
		NotifyAccessing();
		var newItems = items.ToExtendedList();
		var newItemsCount = newItems.Count;
		CheckRange(index, newItemsCount);

		if (newItemsCount == 0) return;
		var endIndex = index + newItemsCount - 1;
		foreach (var pageSegment in GetPageSegments(index, newItemsCount)) {
			var page = pageSegment.Item1;
			var pageStartIX = pageSegment.Item2;
			var pageCount = pageSegment.Item3;
			_count -= page.Count;
			var pageItems = newItems.ReadRange(pageStartIX - index, pageCount).ToArray();
			using (EnterOpenPageScope(page)) {
				NotifyPageAccessing(page);
				NotifyPageWriting(page);
				UpdateVersion();
				if (!page.Write(pageStartIX, pageItems, out var overflow)) {
					throw new NotSupportedException("Overflow when updating is not supported");
				}
				_count += page.Count;
				NotifyPageWrite(page);
			}
			NotifyPageAccessed(page);
		}
		NotifyAccessed();
	}

	public override void InsertRange(long index, IEnumerable<TItem> items) {
		if (index == Count)
			AddRange(items);
		else throw new NotSupportedException("This collection can only be mutated from the end");
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) => throw new NotSupportedException();

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count, rightAligned: true);
		CheckLoaded();
		NotifyAccessing();
		CheckRange(index, count);
		if (count == 0)
			return;
		var endIndex = index + count - 1;
		if (endIndex != InternalPages.Last().EndIndex)
			throw new NotSupportedException("Removing an inner region of items is not supported. This collection only supports removing from the end.");
		var pages = GetPagesInRange(index, endIndex);
		pages.Reverse();
		foreach (var page in pages) {
			_count -= page.Count;
			var toRemoveCount = count > page.Count ? page.Count : count;
			if (toRemoveCount < page.Count) {
				NotifyPageAccessing(page);
				NotifyPageWriting(page);
				UpdateVersion();
				using (EnterOpenPageScope(page)) {
					page.EraseFromEnd(toRemoveCount);
					_count += page.Count;
					NotifyPageWrite(page);
				}
				NotifyPageAccessed(page);
			} else {
				DeletePage(page);
			}
			count -= toRemoveCount;
		}
		NotifyAccessed();
	}

	public override void Clear() {
		CheckLoaded();
		NotifyAccessing();
		while (InternalPages.Count > 0)
			DeletePage(InternalPages[InternalPages.Count - 1]);
		InternalPages.Clear();
		_lastFoundPage = -1;
		_count = 0;
		NotifyAccessed();
	}

	public override IEnumerator<TItem> GetEnumerator() {
		var currentVersion = Version;
		IDisposable lastScope = default;
		return
			InternalPages
				.SelectMany(p => {
					lastScope?.Dispose();
					lastScope = EnterOpenPageScope(p);
					return p;
				})
				.GetEnumerator()
				.OnMoveNext(
					preMoveNextAction: () => CheckVersion(currentVersion),
					postMoveNextAction: (result) => {
						if (!result)
							lastScope?.Dispose(); // last scope
					}
				);
	}

	protected abstract IPage<TItem> NewPageInstance(long pageNumber);

	protected abstract IPage<TItem>[] LoadPages();

	protected IPage<TItem> CreateNextPage() {
		IPage<TItem> newPage;
		if (!InternalPages.Any()) {
			// First page
			NotifyPageCreating(0);
			newPage = NewPageInstance(0);
			newPage.Number = 0;
			newPage.StartIndex = 0;
			newPage.EndIndex = -1;
			newPage.Count = 0;
			newPage.Size = 0;
		} else {
			// Next page
			var lastPage = InternalPages.Last();
			var nextPageNumber = lastPage.Number + 1;
			NotifyPageCreating(nextPageNumber);
			newPage = NewPageInstance(nextPageNumber);
			newPage.Number = nextPageNumber;
			newPage.StartIndex = lastPage.EndIndex + 1;
			newPage.EndIndex = lastPage.EndIndex;
			newPage.Count = 0;
			newPage.Size = 0;
		}
		;
		InternalPages.Add(newPage);
		NotifyPageCreated(newPage);
		return newPage;
	}

	protected void DeletePage(IPage<TItem> page) {
		if (page.Number != Pages.Last().Number)
			throw new NotSupportedException("Deleting inner pages is not currently supported in this collection. Only deleting pages from the end is supported.");
		NotifyPageAccessing(page);
		page.State = PageState.Deleting;
		NotifyPageDeleting(page);
		InternalPages.RemoveAt(InternalPages.Count - 1);
		page.State = PageState.Deleted;
		NotifyPageDeleted(page);
		NotifyPageAccessed(page);
	}

	protected List<Tuple<IPage<TItem>, long, long>> GetPageSegments(long startIndex, long count) {
		var pageSegments = new List<Tuple<IPage<TItem>, long, long>>();
		if (count == 0)
			return pageSegments;

		foreach (var page in GetPagesInRange(startIndex, startIndex + count - 1)) {
			Debug.Assert(startIndex >= page.StartIndex);
			var pageItemsToRead = Math.Min(page.Count - (startIndex - page.StartIndex), count); // read only what's needed
			pageSegments.Add(Tuple.Create(page, startIndex, pageItemsToRead));
			startIndex += pageItemsToRead;
			count -= pageItemsToRead;
			if (count <= 0)
				break;
		}
		return pageSegments;
	}

	protected List<IPage<TItem>> GetPagesInRange(long startIndex, long endIndex) {
		var index = FindPageContainingIndex(startIndex);
		var pages = new List<IPage<TItem>>();
		IPage<TItem> page;
		do {
			page = InternalPages[index++];
			pages.Add(page);
		} while (endIndex > page.EndIndex && index < InternalPages.Count);
		return pages;
	}

	protected long FindPageContainingIndex(long index) {
		var internalPagesCount = InternalPages.Count;
		if (internalPagesCount == 0)
			return -1;

		long lower, upper;
		if (_lastFoundPage != -1) {
			// Optimization 1: check the last binary searched page again (index seeks tend to be clustered together)
			var currentPage = InternalPages[_lastFoundPage];
			var cpStartIndex = currentPage.StartIndex;
			var cpEndIndex = currentPage.EndIndex;
			if (cpStartIndex <= index && index <= cpEndIndex)
				return _lastFoundPage;

			// Optimization 2: if index is just beyond current page boundary, it has to be adjacent (assuming it exists)
			if (index == cpEndIndex + 1 && internalPagesCount > _lastFoundPage + 1) {
				return ++_lastFoundPage;
			}

			if (index == cpStartIndex - 1 && _lastFoundPage > 0) {
				return --_lastFoundPage;
			}

			// Optimization 3: Restrict binary search range since we know if after/before current page
			if (index < cpStartIndex) {
				lower = 0;
				upper = _lastFoundPage - 1;
			} else {
				lower = _lastFoundPage + 1;
				upper = internalPagesCount - 1;
			}
		} else {
			lower = 0;
			upper = internalPagesCount - 1;
		}

		// Binary search pages to find the one containing the index
		_lastFoundPage = Tools.Collection.BinarySearch(
			InternalPages,
			index,
			lower,
			upper,
			(_, p) => {
				if (index < p.StartIndex)
					return -1;
				if (index > p.EndIndex)
					return +1;
				return 0;
			});

		return _lastFoundPage;
	}

	public abstract IDisposable EnterOpenPageScope(IPage<TItem> page);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void CheckLoaded() {
		if (RequiresLoad)
			throw new InvalidOperationException("Paged collection has not been loaded");
	}

	protected override void CheckIndex(long index, bool allowAtEnd = false) {
		if (allowAtEnd && index == 0)
			return;

		if (InternalPages.Count == 0)
			throw new InvalidOperationException("No pages");

		var startIX = InternalPages[0].StartIndex;
		var lastIX = InternalPages[InternalPages.Count - 1].EndIndex;
		var collectionCount = lastIX - startIX + 1;
		Guard.CheckIndex(index, startIX, collectionCount, allowAtEnd);
	}

	protected override void CheckRange(long index, long count, bool rightAligned = false) {
		if (InternalPages.Count == 0)
			throw new InvalidOperationException("No pages");

		var startIX = InternalPages[0].StartIndex;
		var lastIX = InternalPages[InternalPages.Count - 1].EndIndex;
		var collectionCount = lastIX - startIX + 1;
		Guard.CheckRange(index, count, rightAligned, startIX, collectionCount);
	}

	#region Events

	protected virtual void OnAccessing() {
	}

	protected virtual void OnAccessed() {
	}

	protected virtual void OnLoading() {
	}

	protected virtual void OnLoaded() {
	}

	protected virtual void OnPageAccessing(IPage<TItem> page) {
	}

	protected virtual void OnPageAccessed(IPage<TItem> page) {
	}

	protected virtual void OnPageCreating(long pageNumber) {
	}

	protected virtual void OnPageCreated(IPage<TItem> page) {
		// set current page to end, since most likely place to search 
		_lastFoundPage = page.Number;
	}

	protected virtual void OnPageReading(IPage<TItem> page) {
	}

	protected virtual void OnPageRead(IPage<TItem> page) {
	}

	protected virtual void OnPageWriting(IPage<TItem> page) {
	}

	protected virtual void OnPageWrite(IPage<TItem> page) {
	}

	protected virtual void OnPageDeleting(IPage<TItem> page) {
	}

	protected virtual void OnPageDeleted(IPage<TItem> page) {
		if (_lastFoundPage == page.Number) {
			// reset current page to end, since most likely place to search 
			var pCount = Pages.Count;
			_lastFoundPage = pCount > 0 ? pCount - 1 : -1;
		}
	}

	protected void NotifyAccessing() {
		OnAccessing();
		Accessing?.Invoke(this);
	}

	protected void NotifyAccessed() {
		OnAccessed();
		Accessed?.Invoke(this);
	}

	protected void NotifyLoading() {
		OnLoading();
		Loading?.Invoke(this);
	}

	protected void NotifyLoaded() {
		OnLoaded();
		Loaded?.Invoke(this);
	}

	protected void NotifyPageAccessing(IPage<TItem> page) {
		OnPageAccessing(page);
		PageAccessing?.Invoke(this, page);
	}

	protected void NotifyPageAccessed(IPage<TItem> page) {
		OnPageAccessed(page);
		PageAccessed?.Invoke(this, page);
	}

	protected void NotifyPageCreating(long pageNumber) {
		OnPageCreating(pageNumber);
		PageCreating?.Invoke(this, pageNumber);
	}

	protected void NotifyPageCreated(IPage<TItem> page) {
		OnPageCreated(page);
		PageCreated?.Invoke(this, page);
	}

	protected void NotifyPageWriting(IPage<TItem> page) {
		OnPageWriting(page);
		PageWriting?.Invoke(this, page);
	}

	protected void NotifyPageWrite(IPage<TItem> page) {
		OnPageWrite(page);
		PageWrite?.Invoke(this, page);
	}

	protected void NotifyPageReading(IPage<TItem> page) {
		OnPageReading(page);
		PageReading?.Invoke(this, page);
	}

	protected void NotifyPageRead(IPage<TItem> page) {
		OnPageRead(page);
		PageRead?.Invoke(this, page);
	}

	protected void NotifyPageDeleting(IPage<TItem> page) {
		OnPageDeleting(page);
		PageDeleting?.Invoke(this, page);
	}

	protected void NotifyPageDeleted(IPage<TItem> page) {
		OnPageDeleted(page);
		PageDeleted?.Invoke(this, page);
	}

	// Needed since C# lacks "friend" modifier
	internal IPagedListDelegate<TItem> CreateFriendDelegate() => new PagedListDelegate<TItem>(
		x => _count += x,
		x => _count -= x,
		UpdateVersion,
		CheckLoaded,
		CheckRange,
		EnterOpenPageScope,
		GetPageSegments,
		() => InternalPages,
		CreateNextPage,
		NotifyAccessing,
		NotifyAccessed,
		NotifyPageAccessing,
		NotifyPageAccessed,
		NotifyPageReading,
		NotifyPageRead,
		NotifyPageWriting,
		NotifyPageWrite
	);

	#endregion

}
