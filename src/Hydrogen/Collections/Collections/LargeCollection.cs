// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class LargeCollection<TItem> : CollectionDecorator<TItem>, IDisposable {
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading {
		add => InternalPagedList.PageLoading += value;
		remove => InternalPagedList.PageLoading -= value;
	}

	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded {
		add => InternalPagedList.PageLoaded += value;
		remove => InternalPagedList.PageLoaded -= value;
	}

	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading {
		add => InternalPagedList.PageUnloading += value;
		remove => InternalPagedList.PageUnloading -= value;
	}

	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded {
		add => InternalPagedList.PageUnloaded += value;
		remove => InternalPagedList.PageUnloaded -= value;
	}

	private readonly ReadOnlyListDecorator<IPage<TItem>, IMemoryPage<TItem>> _pagesDecorator;

	public LargeCollection(int pageSize, long maxMemory)
		: this(pageSize, maxMemory, null) {
		_pagesDecorator = new ReadOnlyListDecorator<IPage<TItem>, IMemoryPage<TItem>>(InternalPagedList.Pages);
	}

	public LargeCollection(int pageSize, long maxMemory, Func<TItem, long> itemSizer)
		: base(new MemoryPagedList<TItem>(pageSize, maxMemory, itemSizer)) {
	}

	protected MemoryPagedListBase<TItem> InternalPagedList => (MemoryPagedListBase<TItem>)base.InternalCollection;

	public IReadOnlyList<IMemoryPage<TItem>> Pages => _pagesDecorator;

	public void Dispose() {
		InternalPagedList.Dispose();
	}

}
