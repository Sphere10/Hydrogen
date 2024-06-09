// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

internal class PagedListDelegate<TItem> : IPagedListDelegate<TItem> {

	public PagedListDelegate(
		Action<long> incCount,
		Action<long> decCount,
		Action updateVersion,
		Action checkRequiresLoad,
		Action<long, long, bool> checkRange,
		Func<IPage<TItem>, IDisposable> enterOpenPageScope,
		Func<long, long, List<Tuple<IPage<TItem>, long, long>>> getPageSegments,
		Func<IReadOnlyList<IPage<TItem>>> internalPages,
		Func<IPage<TItem>> createNextPage,
		Action notifyAccessing,
		Action notifyAccessed,
		Action<IPage<TItem>> notifyPageAccessing,
		Action<IPage<TItem>> notifyPageAccessed,
		Action<IPage<TItem>> notifyPageReading,
		Action<IPage<TItem>> notifyPageRead,
		Action<IPage<TItem>> notifyPageWriting,
		Action<IPage<TItem>> notifyPageWrite) {
		IncCount = incCount;
		DecCount = decCount;
		UpdateVersion = updateVersion;
		CheckRequiresLoad = checkRequiresLoad;
		CheckRange = checkRange;
		EnterOpenPageScope = enterOpenPageScope;
		InternalPages = internalPages;
		CreateNextPage = createNextPage;
		NotifyAccessing = notifyAccessing;
		NotifyPageAccessing = notifyPageAccessing;
		NotifyPageReading = notifyPageReading;
		NotifyPageRead = notifyPageRead;
		NotifyPageWriting = notifyPageWriting;
		NotifyPageWrite = notifyPageWrite;
		NotifyPageAccessed = notifyPageAccessed;
		NotifyAccessed = notifyAccessed;
		GetPageSegments = getPageSegments;
	}

	public Action<long> IncCount { get; }

	public Action<long> DecCount { get; }

	public Action UpdateVersion { get; }

	public Action CheckRequiresLoad { get; }

	public Action<long, long, bool> CheckRange { get; }

	public Func<IReadOnlyList<IPage<TItem>>> InternalPages { get; }

	public Func<IPage<TItem>, IDisposable> EnterOpenPageScope { get; }

	public Func<long, long, List<Tuple<IPage<TItem>, long, long>>> GetPageSegments { get; }

	public Func<IPage<TItem>> CreateNextPage { get; }

	public Action NotifyAccessing { get; }

	public Action NotifyAccessed { get; }

	public Action<IPage<TItem>> NotifyPageAccessing { get; }

	public Action<IPage<TItem>> NotifyPageAccessed { get; }

	public Action<IPage<TItem>> NotifyPageReading { get; }

	public Action<IPage<TItem>> NotifyPageRead { get; }

	public Action<IPage<TItem>> NotifyPageWriting { get; }

	public Action<IPage<TItem>> NotifyPageWrite { get; }
}
