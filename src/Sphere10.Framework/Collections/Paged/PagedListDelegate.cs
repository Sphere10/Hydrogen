﻿using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	internal class PagedListDelegate<TItem> : IPagedListDelegate<TItem> {

		public PagedListDelegate(
			Action<int> incCount,
			Action<int> decCount,
			Action updateVersion,
			Action checkRequiresLoad,
			Action<int, int, bool> checkRange,
			Func<IPage<TItem>, IDisposable> enterOpenPageScope,
			Func<int, int, List<Tuple<IPage<TItem>, int, int>>> getPageSegments,
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

		public Action<int> IncCount { get; }

		public Action<int> DecCount { get; }

		public Action UpdateVersion { get; }

		public Action CheckRequiresLoad { get; }

		public Action<int, int, bool> CheckRange { get; }

		public Func<IReadOnlyList<IPage<TItem>>> InternalPages { get; }

		public Func<IPage<TItem>, IDisposable> EnterOpenPageScope { get; }

		public Func<int, int, List<Tuple<IPage<TItem>, int, int>>> GetPageSegments { get; }

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

}
