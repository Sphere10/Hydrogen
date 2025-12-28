// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Extends <see cref="IPagedList{TItem}"/> with behaviors for paging in and out of memory-constrained environments.
/// </summary>
/// <typeparam name="TItem">Item type stored in the list.</typeparam>
public interface IMemoryPagedList<TItem> : IPagedList<TItem>, IDisposable {

	/// <summary>
	/// Raised when a page begins loading into memory.
	/// </summary>
	event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading;
	/// <summary>
	/// Raised after a page is fully loaded into memory.
	/// </summary>
	event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded;
	/// <summary>
	/// Raised when a dirty page is about to be persisted.
	/// </summary>
	event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving;
	/// <summary>
	/// Raised after a page has been persisted.
	/// </summary>
	event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved;
	/// <summary>
	/// Raised when a page begins unloading from memory.
	/// </summary>
	event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading;
	/// <summary>
	/// Raised after a page has been unloaded from memory.
	/// </summary>
	event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded;

	/// <summary>
	/// Forces dirty pages to save and unloads any currently cached pages.
	/// </summary>
	void Flush();
}
