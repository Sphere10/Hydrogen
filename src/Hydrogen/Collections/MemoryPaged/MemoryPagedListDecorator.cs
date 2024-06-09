// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class MemoryPagedListDecorator<TItem, TMemoryPagedList> : PagedListDecorator<TItem, TMemoryPagedList>, IMemoryPagedList<TItem> where TMemoryPagedList : IMemoryPagedList<TItem> {

	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading { add => InternalCollection.PageLoading += value; remove => InternalCollection.PageLoading -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded { add => InternalCollection.PageLoaded += value; remove => InternalCollection.PageLoaded -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving { add => InternalCollection.PageSaving += value; remove => InternalCollection.PageSaving -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved { add => InternalCollection.PageSaved += value; remove => InternalCollection.PageSaved -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading { add => InternalCollection.PageUnloading += value; remove => InternalCollection.PageUnloading -= value; }
	public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded { add => InternalCollection.PageUnloaded += value; remove => InternalCollection.PageUnloaded -= value; }

	public MemoryPagedListDecorator(TMemoryPagedList internalPagedList)
		: base(internalPagedList) {
		internalPagedList.PageLoading += (o, p) => OnPageLoading(p);
		internalPagedList.PageLoaded += (o, p) => OnPageLoaded(p);
		internalPagedList.PageSaving += (o, p) => OnPageSaving(p);
		internalPagedList.PageSaved += (o, p) => OnPageSaved(p);
		internalPagedList.PageUnloading += (o, p) => OnPageUnloading(p);
		internalPagedList.PageUnloaded += (o, p) => OnPageUnloaded(p);
	}

	public virtual void Flush() => InternalCollection.Flush();

	public virtual void Dispose() => InternalCollection.Dispose();

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
}


public abstract class MemoryPagedListDecorator<TItem> : MemoryPagedListDecorator<TItem, IMemoryPagedList<TItem>> {

	protected MemoryPagedListDecorator(IMemoryPagedList<TItem> internalPagedList)
		: base(internalPagedList) {
	}
}
