// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// A page of data stored on a stream whose items are constant sized.
/// </summary>
/// <remarks>
/// Unlike <see cref="DynamicStreamPage{TItem}"/> no header is stored for the page.
/// </remarks>
internal class ConstantSizeStreamPage<TItem> : StreamPageBase<TItem> {
	private readonly long _item0Offset;
	private volatile int _version;

	public ConstantSizeStreamPage(StreamPagedList<TItem> parent) : base(parent) {
		Guard.Argument(parent.Serializer.IsConstantSize, nameof(parent), $"Parent list's serializer is not fixed size. {nameof(ConstantSizeStreamPage<TItem>)} only supports fixed sized items.");
		Guard.Ensure(parent.Serializer.ConstantSize > 0, $"{nameof(TItem)} serialization size size is not greater than 0");

		_version = 0;
		_item0Offset = Parent.IncludeListHeader ? StreamPagedList<TItem>.ListHeaderSize : 0;
		base.State = PageState.Loaded;
		base.StartIndex = 0L;
		base.EndIndex = (Stream.Length - _item0Offset) / ItemSize - 1;
		StartPosition = _item0Offset;
	}

	public long MaxItems => Parent.PageSize / ItemSize;

	public override long Count => (Stream.Length - _item0Offset) / ItemSize;

	public override long Size => Count * ItemSize;

	public override IEnumerator<TItem> GetEnumerator() {
		var currentVersion = _version;

		void CheckVersion() {
			if (currentVersion != _version)
				throw new InvalidOperationException("Page was changed during enumeration");
		}

		return ReadInternal(StartIndex, Count)
			.GetEnumerator()
			.OnMoveNext(CheckVersion);
	}

	protected override IEnumerable<TItem> ReadInternal(long index, long count) {
		var startIndex = index * ItemSize + _item0Offset;

		for (var i = 0L; i < count; i++) {
			Stream.Seek(startIndex + i * ItemSize, SeekOrigin.Begin);

			yield return Serializer.Deserialize(Reader);
		}
	}

	protected override long AppendInternal(TItem[] items, out long newItemsSize) {
		if (!items.Any()) {
			newItemsSize = items.Length;
			return items.Length;
		}

		if (items.Length + Count > MaxItems)
			throw new InvalidOperationException("Unable to append items, Max Items of page will be exceeded.");

		Stream.Seek(_item0Offset + Count * ItemSize, SeekOrigin.Begin);

		foreach (var item in items) {
			var bytesWritten = Serializer.SerializeReturnSize(item, Writer);
			Guard.Ensure(bytesWritten == Serializer.ConstantSize, $"Static serializer wrote {bytesWritten} bytes expected {Serializer.ConstantSize}");
		}

		newItemsSize = items.Length * ItemSize;
		Interlocked.Increment(ref _version);

		return items.Length;
	}

	protected override void UpdateInternal(long index, TItem[] items, out long oldItemsSize, out long newItemsSize) {
		CheckPageState(PageState.Loaded);
		Guard.Ensure(index + items.Length <= Count, "Update outside bounds of list");

		var itemsSize = items.Length * ItemSize;
		index = index * ItemSize + _item0Offset;

		Stream.Seek(index, SeekOrigin.Begin);

		foreach (var item in items) {
			var bytesWritten = Serializer.SerializeReturnSize(item, Writer);
			Guard.Ensure(bytesWritten == Serializer.ConstantSize, $"Static serializer wrote {bytesWritten} bytes expected {Serializer.ConstantSize}");
		}

		newItemsSize = itemsSize;
		oldItemsSize = itemsSize;
		Interlocked.Increment(ref _version);
	}

	protected override void EraseFromEndInternal(long count, out long oldItemsSize) {
		var erasedBytes = ItemSize * count;
		Stream.SetLength(Stream.Length - erasedBytes);
		oldItemsSize = erasedBytes;
		Interlocked.Increment(ref _version);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="itemIndex">Global index of the item (not the index within page)</param>
	/// <param name="byteOffset"></param>
	/// <param name="byteLength"></param>
	/// <param name="result"></param>
	/// <returns>Number of bytes actually read</returns>
	public override long ReadItemBytes(long itemIndex, long byteOffset, long? byteLength, out byte[] result) {
		Guard.ArgumentInRange(itemIndex, 0, Count - 1, nameof(itemIndex));
		byteLength ??= ItemSize;
		var offset = itemIndex * ItemSize + _item0Offset + byteOffset;

		Stream.Seek(offset, SeekOrigin.Begin);
		result = Reader.ReadBytes(byteLength.Value);
		return result.Length;
	}


	public override void WriteItemBytes(long itemIndex, long byteOffset, ReadOnlySpan<byte> bytes) {
		Guard.ArgumentInRange(itemIndex, 0, Count - 1, nameof(itemIndex));
		var offset = itemIndex * ItemSize + _item0Offset + byteOffset;
		//Stream.Seek(offset, SeekOrigin.Begin);
		using var _ = Stream.EnterRestorePositionSeek(offset, SeekOrigin.Begin);
		Writer.Write(bytes);
	}

}
