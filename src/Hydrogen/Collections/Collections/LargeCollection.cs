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

public class LargeCollection<TItem> : CollectionDecorator<TItem>, IDisposable {
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading { add => InternalCollection.PageLoading += value; remove => InternalCollection.PageLoading -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded { add => InternalCollection.PageLoaded += value; remove => InternalCollection.PageLoaded -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading { add => InternalCollection.PageUnloading += value; remove => InternalCollection.PageUnloading -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded { add => InternalCollection.PageUnloaded += value; remove => InternalCollection.PageUnloaded -= value; }

	protected new MemoryPagedList<TItem> InternalCollection; 

	public LargeCollection(int pageSize, long maxMemory)
		: this(pageSize, maxMemory, null) {
	}

	public LargeCollection(int pageSize, long maxMemory, Func<TItem, long> itemSizer)
		: base(new MemoryPagedList<TItem>(pageSize, maxMemory, itemSizer)) {
		InternalCollection = (MemoryPagedList<TItem>)base.InternalCollection;
		Pages = InternalCollection.Pages;
	}

	public IReadOnlyList<IMemoryPage<TItem>> Pages { get; }

	public void Dispose() {
		InternalCollection.Dispose();
	}

}
