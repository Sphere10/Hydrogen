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

/// <summary>
/// Represents a list partitioned into discrete pages that can be independently loaded, mutated, and unloaded.
/// </summary>
/// <typeparam name="TItem">Type of item stored in the list.</typeparam>
public interface IPagedList<TItem> : IExtendedList<TItem>, ILoadable {

	/// <summary>
	/// Raised before any page operation is executed (read, write, load or unload).
	/// </summary>
	event EventHandlerEx<object> Accessing;

	/// <summary>
	/// Raised after a page operation completes.
	/// </summary>
	event EventHandlerEx<object> Accessed;

	/// <summary>
	/// Raised immediately before a page is accessed.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageAccessing;

	/// <summary>
	/// Raised immediately after a page has been accessed.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageAccessed;

	/// <summary>
	/// Raised just before a new page is allocated.
	/// </summary>
	event EventHandlerEx<object, long> PageCreating;

	/// <summary>
	/// Raised after a new page instance has been created.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageCreated;

	/// <summary>
	/// Raised before items are read from a page.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageReading;

	/// <summary>
	/// Raised after items have been read from a page.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageRead;

	/// <summary>
	/// Raised before items are written to a page.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageWriting;

	/// <summary>
	/// Raised after items have been written to a page.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageWrite;

	/// <summary>
	/// Raised before a page is removed from the list.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageDeleting;

	/// <summary>
	/// Raised after a page has been removed from the list.
	/// </summary>
	event EventHandlerEx<object, IPage<TItem>> PageDeleted;

	/// <summary>
	/// Enters a scope in which the supplied page is considered open, preventing eviction while work is performed.
	/// </summary>
	/// <param name="page">The target page.</param>
	/// <returns>An <see cref="IDisposable"/> that marks the end of the scope when disposed.</returns>
	IDisposable EnterOpenPageScope(IPage<TItem> page);

	/// <summary>
	/// Exposes the logical pages that compose the list.
	/// </summary>
	IReadOnlyList<IPage<TItem>> Pages { get; }

}
