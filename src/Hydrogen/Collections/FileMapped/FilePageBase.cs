// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen;

public abstract class FilePageBase<TItem> : MemoryPageBase<TItem>, IFilePage<TItem> {

	protected FilePageBase(Stream stream, IItemSizer<TItem> sizer, int pageNumber, int pageSize, IExtendedList<TItem> memoryStore)
		: base(pageSize, sizer, memoryStore) {
		Stream = new BoundedStream(stream, (long)pageNumber * pageSize, (long)(pageNumber + 1) * pageSize - 1);
	}

	internal BoundedStream Stream { get; }

	public long StartPosition { get; set; }

	public long EndPosition { get; set; }

	public override void Dispose() {
		// Stream is managed by client		
	}

	protected override int AppendInternal(TItem[] items, out int newItemsSpace) {
		var appendCount = base.AppendInternal(items, out newItemsSpace);
		EndPosition += newItemsSpace;
		return appendCount;
	}

	protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSpace, out int newItemsSpace) {
		base.UpdateInternal(index, items, out oldItemsSpace, out newItemsSpace);
		var spaceDiff = newItemsSpace - oldItemsSpace;
		EndPosition += spaceDiff;
	}

	protected override void EraseFromEndInternal(int count, out int oldItemsSpace) {
		base.EraseFromEndInternal(count, out oldItemsSpace);
		EndPosition -= oldItemsSpace;
	}

	protected override Stream OpenReadStream() {
		Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
		return new NonClosingStream(Stream);
	}

	protected override Stream OpenWriteStream() {
		Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
		return new NonClosingStream(Stream);
	}
}
