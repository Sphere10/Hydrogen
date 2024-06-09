// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen;

public abstract class FilePageBase<TItem> : MemoryPageBase<TItem>, IFilePage<TItem> {

	protected FilePageBase(Stream stream, IItemSizer<TItem> sizer, long pageNumber, long pageSize, IExtendedList<TItem> memoryStore)
		: base(pageSize, sizer, memoryStore) {
		Stream = new BoundedStream(stream, pageNumber * pageSize, pageSize);
	}

	internal BoundedStream Stream { get; }

	public long StartPosition { get; set; }

	public long EndPosition { get; set; }

	public override void Dispose() {
		// Stream is managed by client		
	}

	protected override long AppendInternal(TItem[] items, out long newItemsSpace) {
		var appendCount = base.AppendInternal(items, out newItemsSpace);
		EndPosition += newItemsSpace;
		return appendCount;
	}

	protected override void UpdateInternal(long index, TItem[] items, out long oldItemsSpace, out long newItemsSpace) {
		base.UpdateInternal(index, items, out oldItemsSpace, out newItemsSpace);
		var spaceDiff = newItemsSpace - oldItemsSpace;
		EndPosition += spaceDiff;
	}

	protected override void EraseFromEndInternal(long count, out long oldItemsSpace) {
		base.EraseFromEndInternal(count, out oldItemsSpace);
		EndPosition -= oldItemsSpace;
	}

	protected override Stream OpenReadStream() {
		Stream.Seek(0L, SeekOrigin.Begin);
		return new NonClosingStream(Stream);
	}

	protected override Stream OpenWriteStream() {
		Stream.Seek(0L, SeekOrigin.Begin);
		return new NonClosingStream(Stream);
	}
}
