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

internal interface IPagedListDelegate<TItem> {

	Action<long> IncCount { get; }

	Action<long> DecCount { get; }

	Action UpdateVersion { get; }

	Action CheckRequiresLoad { get; }

	Action<long, long, bool> CheckRange { get; }

	Func<IReadOnlyList<IPage<TItem>>> InternalPages { get; }

	Func<IPage<TItem>, IDisposable> EnterOpenPageScope { get; }

	Func<long, long, List<Tuple<IPage<TItem>, long, long>>> GetPageSegments { get; }

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
