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
/// A page of data stored on a stream whose items are dynamically sized. The page header thus stores the item sizes.
/// </summary>
/// <remarks>
/// Page Header Format
/// ===================
///	Count (UINT16)
/// PreviousPageOffset (QWORD)
/// NextPageOffset (QWORD)
///	Object 0 Size (UINT32)
///	Object 1 Size
///	...
///	Object N Size
/// </remarks>
internal class DynamicStreamPage<TItem> : StreamPageBase<TItem> {
	private const int Page0Offset = 256;

	private const int CountFieldSize = sizeof(ulong);
	private const int MaxItemsFieldSize = sizeof(ulong);
	private const int PreviousPageOffsetFieldSize = sizeof(ulong);
	private const int NextPageOffsetFieldSize = sizeof(ulong);
	private const int ObjectSizeFieldSize = sizeof(ulong);

	private const int CountFieldOffset = 0;
	private const int MaxItemsFieldOffset = CountFieldOffset + CountFieldSize;
	private const int PreviousPageOffsetFieldOffset = MaxItemsFieldOffset + MaxItemsFieldSize;
	private const int NextPageOffsetFieldOffset = PreviousPageOffsetFieldOffset + PreviousPageOffsetFieldSize;
	private const int Object0SizeFieldOffset = NextPageOffsetFieldOffset + NextPageOffsetFieldSize;


	private volatile int _version;
	private readonly StreamPagedList<TItem> _parent;
	private long[] _itemSizes;
	private long _previousPagePosition;
	private long _nextPagePosition;
	private long[] _offsets;

	public DynamicStreamPage(StreamPagedList<TItem> parent)
		: this(Page0Offset, Page0Offset, parent) {
	}

	public DynamicStreamPage(DynamicStreamPage<TItem> previousPage)
		: this(previousPage.StartPosition, previousPage.NextPagePosition, previousPage._parent) {
	}

	private DynamicStreamPage(long previousPagePosition, long startPosition, StreamPagedList<TItem> parent)
		: base(parent) {
		Guard.ArgumentNotNull(parent, nameof(parent));
		Guard.ArgumentNotNull(parent.Stream, nameof(parent.Stream));
		Guard.ArgumentInRange(startPosition, Page0Offset, parent.Stream.Length, nameof(startPosition));
		Guard.ArgumentInRange(previousPagePosition, Page0Offset, startPosition, nameof(previousPagePosition));
		_version = 0;
		_parent = parent;
		_itemSizes = Array.Empty<long>();
		_offsets = null;

		if (startPosition < parent.Stream.Length) {
			// CASE: Page already exists in streams, load it
			Stream.Seek(StartPosition, SeekOrigin.Begin);
			// NOTE: we set the field not the property to avoid rewriting same value just read
			var itemCount = Reader.ReadUInt64();
			base.Count = (long)itemCount;
			var maxItems = (long)Reader.ReadUInt64();
			_previousPagePosition = (long)Reader.ReadUInt64();
			_nextPagePosition = (long)Reader.ReadUInt64();
			_itemSizes = Tools.Collection.Generate(Reader.ReadUInt64).Cast<long>().TakeL(maxItems).ToArray();
		} else if (startPosition == parent.Stream.Length) {
			// CASE: Page begins end of streams as it is newly appended, so write out default header
			StartPosition = startPosition;
			Count = 0;
			MaxItems = _parent.PageSize;
			PreviousPagePosition = previousPagePosition;
			NextPagePosition = GetItem0DataOffset(); // Empty, so next page begins straight after this header
			SetItemSizes(0, new long[MaxItems], out _);
		} else {
			// ILLEGAL: Page starts beyond known streams boundary, invalid
			throw new ArgumentOutOfRangeException(nameof(startPosition), startPosition, "Page start position beyond streams boundary");
		}

	}

	public override long Count {
		get => base.Count;
		set {
			Guard.ArgumentInRange(value, 0, long.MaxValue, "Value");
			base.Count = value;
			Stream.Seek(StartPosition + CountFieldOffset, SeekOrigin.Begin);
			Writer.Write((ulong)value);
		}
	}

	public long MaxItems {
		get => _itemSizes.Length;
		set {
			Guard.ArgumentInRange(value, 1, long.MaxValue, "Value");
			var valueI = Tools.Collection.CheckNotImplemented64bitAddressingLength(value);
			Array.Resize(ref _itemSizes, valueI);
			Stream.Seek(StartPosition + MaxItemsFieldOffset, SeekOrigin.Begin);
			Writer.Write((ulong)value);
			if (State == PageState.Loaded)
				CalculateOffsets(1);
		}
	}

	public long PreviousPagePosition {
		get => _previousPagePosition;
		set {
			_previousPagePosition = value;
			Guard.ArgumentInRange(value, 0, long.MaxValue, "Value");
			Stream.Seek(StartPosition + PreviousPageOffsetFieldOffset, SeekOrigin.Begin);
			Writer.Write((ulong)value);
		}
	}

	public long NextPagePosition {
		get => _nextPagePosition;
		set {
			_nextPagePosition = value;
			Guard.ArgumentInRange(value, 0, long.MaxValue, "Value");
			Stream.Seek(StartPosition + NextPageOffsetFieldOffset, SeekOrigin.Begin);
			Writer.Write((ulong)value);
		}
	}

	public void Open() {
		// Calculate the object offsets (this array is nullified on close to save memory 
		// in cases when many page headers exist).
		State = PageState.Loading;
		_offsets = new long[MaxItems];
		CalculateOffsets(0);
		State = PageState.Loaded;
	}

	public void Close() {
		State = PageState.Unloading;
		// Unload cached offsets array (makes a mem difference when large numbers of pages loaded)
		_offsets = null;
		State = PageState.Unloaded;
	}

	public override IEnumerator<TItem> GetEnumerator() {
		CheckPageState(PageState.Loaded);
		var currentVersion = _version;

		void CheckVersion() {
			if (currentVersion != _version)
				throw new InvalidOperationException("Page was changed during enumeration");
		}

		return ReadInternal(StartIndex, Count).GetEnumerator().OnMoveNext(CheckVersion);
	}

	public override long ReadItemBytes(long itemIndex, long byteOffset, long? byteLength, out byte[] result) {
		byteLength = byteLength ?? _itemSizes[itemIndex]; 
		Stream.Seek(_offsets[itemIndex] + byteOffset, SeekOrigin.Begin);
		result = Reader.ReadBytes(byteLength.Value);

		return result.Length;
	}

	public override void WriteItemBytes(long itemIndex, long byteOffset, ReadOnlySpan<byte> bytes) {
		Stream.Seek(_offsets[itemIndex] + byteOffset, SeekOrigin.Begin);
		Writer.Write(bytes);
		// TODO: check doesn't overwrite other item
	}

	protected override IEnumerable<TItem> ReadInternal(long index, long count) {
		CheckPageState(PageState.Loaded);
		// Transform list index into page index
		index -= StartIndex;
		Stream.Seek(_offsets[index], SeekOrigin.Begin);
		yield return Serializer.Deserialize(Reader);
	}

	protected override long AppendInternal(TItem[] items, out long newItemsSize) {
		CheckPageState(PageState.Loaded);
		var itemsToAdd = Math.Min(items.Length, MaxItems - Count);

		// Page full case
		if (itemsToAdd == 0) {
			newItemsSize = 0;
			return 0;
		}
		UpdateInternal(StartIndex + Count, items.TakeL(itemsToAdd).ToArray(), out _, out newItemsSize);
		return itemsToAdd;
	}

	protected override void UpdateInternal(long index, TItem[] items, out long oldItemsSize, out long newItemsSize) {
		CheckPageState(PageState.Loaded);
		Guard.Ensure(index + items.Length >= Count, "Illegal page region"); // must cover reach or exceed tip boundary, no internal region

		// Transform list index into page index
		index -= StartIndex;

		if (items.Length == 0) {
			oldItemsSize = 0;
			newItemsSize = 0;
			return;
		}

		// Replace items
		Stream.Seek(_offsets[index], SeekOrigin.Begin); // offset should always be set (append/update case)
		newItemsSize = 0;
		var itemSizes = items.Select(x => Serializer.SerializeReturnSize(x, Writer)).ToArray();
		newItemsSize = itemSizes.Sum();
		SetItemSizes(index, itemSizes, out oldItemsSize);
		NextPagePosition += newItemsSize - oldItemsSize;
		Interlocked.Increment(ref _version);
	}

	protected override void EraseFromEndInternal(long count, out long oldItemsSize) {
		CheckPageState(PageState.Loaded);
		var removeFrom = this.Count - count;
		SetItemSizes(removeFrom, new long[count], out oldItemsSize);
		Stream.SetLength(Stream.Length - oldItemsSize);
		Interlocked.Increment(ref _version);
	}

	private long GetItem0DataOffset() {
		return StartPosition + Object0SizeFieldOffset + MaxItems * ObjectSizeFieldSize;
	}

	private void SetItemSizes(long index, long[] sizes, out long oldItemsSize) {
		Guard.ArgumentInRange(index, 0, MaxItems - 1, nameof(index));
		Guard.ArgumentNotNull(sizes, nameof(sizes));
		Guard.Ensure(index + sizes.Length <= MaxItems, "Sizes array is incorrectly sized");

		oldItemsSize = _itemSizes.SkipL(index).TakeL(sizes.LongLength).Sum();
		Array.Copy(sizes, 0, _itemSizes, index, sizes.Length);
		Stream.Seek(StartPosition + Object0SizeFieldOffset + index * ObjectSizeFieldSize, SeekOrigin.Begin);
		foreach (var size in sizes.Cast<ulong>())
			Writer.Write(size);

		// Calculate the updated offsets if opened
		if (State == PageState.Loaded)
			CalculateOffsets(index + 1);
	}

	private void CalculateOffsets(long from) {
		CheckPageState(PageState.Loading, PageState.Loaded);
		if (from == 0) {
			_offsets[0] = GetItem0DataOffset();
			from++;
		}
		var count = Count;
		for (var i = from; i < MaxItems; i++)
			_offsets[i] = i <= count ? _offsets[i - 1] + _itemSizes[i - 1] : -1;

	}


}
