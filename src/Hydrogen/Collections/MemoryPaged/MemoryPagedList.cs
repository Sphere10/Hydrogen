// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class MemoryPagedList<TItem> : MemoryPagedListBase<TItem> {
	private readonly IItemSizer<TItem> _sizer;

	public MemoryPagedList(long pageSize, long maxMemory, int fixedItemSize)
		: this(pageSize, maxMemory, new ConstantLengthItemSizer<TItem>(fixedItemSize, false)) {
	}

	public MemoryPagedList(long pageSize, long maxMemory, Func<TItem, long> itemSizer)
		: this(pageSize, maxMemory, new ActionItemSizer<TItem>(itemSizer)) {
	}

	private MemoryPagedList(long pageSize, long maxMemory, IItemSizer<TItem> sizer)
		: base(pageSize, maxMemory) {
		_sizer = sizer;
	}

	protected override IPage<TItem> NewPageInstance(long pageNumber) {
		return new BinaryFormattedPage<TItem>(this.PageSize, _sizer);
	}

	protected override IPage<TItem>[] LoadPages() {
		throw new NotSupportedException("Pages are not loadable across runtime sessions in this implementation. See FileMappedList class.");
	}
}
