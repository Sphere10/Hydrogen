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
/// An in-memory paged list that serializes items into binary pages sized by a supplied <see cref="IItemSizer{T}"/>.
/// </summary>
public class MemoryPagedList<TItem> : MemoryPagedListBase<TItem> {
	private readonly IItemSizer<TItem> _sizer;

	/// <summary>
	/// Creates a paged list with a fixed-size item serializer.
	/// </summary>
	/// <param name="pageSize">Logical items per page.</param>
	/// <param name="maxMemory">Maximum bytes available for loaded pages.</param>
	/// <param name="fixedItemSize">Fixed serialized size of an item.</param>
	public MemoryPagedList(long pageSize, long maxMemory, int fixedItemSize)
		: this(pageSize, maxMemory, new ConstantLengthItemSizer<TItem>(fixedItemSize, false)) {
	}

	/// <summary>
	/// Creates a paged list using a delegate to compute serialized item sizes.
	/// </summary>
	/// <param name="pageSize">Logical items per page.</param>
	/// <param name="maxMemory">Maximum bytes available for loaded pages.</param>
	/// <param name="itemSizer">Function that estimates the serialized size for each item.</param>
	public MemoryPagedList(long pageSize, long maxMemory, Func<TItem, long> itemSizer)
		: this(pageSize, maxMemory, new ActionItemSizer<TItem>(itemSizer)) {
	}

	private MemoryPagedList(long pageSize, long maxMemory, IItemSizer<TItem> sizer)
		: base(pageSize, maxMemory) {
		_sizer = sizer;
	}

	/// <inheritdoc />
	protected override IPage<TItem> NewPageInstance(long pageNumber) {
		return new BinarySerializedPage<TItem>(this.PageSize, _sizer);
	}

	/// <summary>
	/// Memory-backed lists cannot reload state across process boundaries; consumers should use file-mapped implementations instead.
	/// </summary>
	/// <exception cref="NotSupportedException">Always thrown to signal the lack of persistence.</exception>
	protected override IPage<TItem>[] LoadPages() {
		throw new NotSupportedException("Pages are not loadable across runtime sessions in this implementation. See FileMappedList class.");
	}
}
