// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Represents a contiguous, indexed slice of items backing a paged list.
/// </summary>
/// <typeparam name="TItem">Item type stored in the page.</typeparam>
public interface IPage<TItem> : IEnumerable<TItem> {
	/// <summary>
	/// Logical page number within the list.
	/// </summary>
	long Number { get; set; }

	/// <summary>
	/// Inclusive starting index of the first item held in the page.
	/// </summary>
	long StartIndex { get; set; }

	/// <summary>
	/// Inclusive ending index of the last item held in the page.
	/// </summary>
	long EndIndex { get; set; }

	/// <summary>
	/// Number of items currently present in the page.
	/// </summary>
	long Count { get; set; }

	/// <summary>
	/// Size of the page in bytes (or another unit chosen by the implementation).
	/// </summary>
	long Size { get; set; }

	/// <summary>
	/// Indicates whether the page contains changes that have not been persisted.
	/// </summary>
	bool Dirty { get; set; }

	/// <summary>
	/// Tracks the lifecycle state of the page.
	/// </summary>
	PageState State { get; set; }

	/// <summary>
	/// Reads a range of items from the page.
	/// </summary>
	/// <param name="index">The zero-based index within the page.</param>
	/// <param name="count">Number of items to read.</param>
	/// <returns>The requested items.</returns>
	IEnumerable<TItem> Read(long index, long count);

	/// <summary>
	/// Writes items to the page, returning any overflow when the page reaches capacity.
	/// </summary>
	/// <param name="index">Zero-based index within the page.</param>
	/// <param name="items">Items to write.</param>
	/// <param name="overflow">Items that could not fit within the page.</param>
	/// <returns><c>true</c> if everything fit; otherwise <c>false</c> and overflow is populated.</returns>
	bool Write(long index, IEnumerable<TItem> items, out IEnumerable<TItem> overflow);

	/// <summary>
	/// Removes items from the logical end of the page without reallocating.
	/// </summary>
	/// <param name="count">Number of items to remove.</param>
	void EraseFromEnd(long count);
}
