﻿using System;
using System.Collections.Generic;

namespace Hydrogen {

	internal interface IPagedListDelegate<TItem> {

		Action<int> IncCount { get; }

		Action<int> DecCount { get; }

		Action UpdateVersion { get; }

		Action CheckRequiresLoad { get; }

		Action<int, int, bool> CheckRange { get; }

		Func<IReadOnlyList<IPage<TItem>>> InternalPages { get; }

		Func<IPage<TItem>, IDisposable> EnterOpenPageScope { get; }

		Func<int, int, List<Tuple<IPage<TItem>, int, int>>> GetPageSegments { get; }

		Func<IPage<TItem>> CreateNextPage { get; }

		Action NotifyAccessing { get; }

		Action NotifyAccessed { get; }

		Action<IPage<TItem>> NotifyPageAccessing { get; }

		Action<IPage<TItem>> NotifyPageAccessed { get; }

		Action<IPage<TItem>> NotifyPageReading { get; }

		Action<IPage<TItem>> NotifyPageRead { get; }

		Action<IPage<TItem>> NotifyPageWriting { get; }

		Action<IPage<TItem>> NotifyPageWrite { get; }
	}
}