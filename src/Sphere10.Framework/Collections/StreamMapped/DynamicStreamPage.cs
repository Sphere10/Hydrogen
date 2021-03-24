using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

	/// Page Header Format
	/// ===================
	///	Count (UINT16)
	/// PreviousPageOffset (QWORD)
	/// NextPageOffset (QWORD)
	///	Object 0 Size (UINT32)
	///	Object 1 Size
	///	...
	///	Object N Size
	internal class DynamicStreamPage<TItem> : StreamPageBase<TItem> {
		private const int Page0Offset = 256;
		private const int CountFieldOffset = 0;
		private const int CountFieldSize = sizeof(uint);
		private const int MaxItemsFieldOffset = CountFieldOffset + CountFieldSize;
		private const int MaxItemsFieldSize = sizeof(uint);
		private const int PreviousPageOffsetFieldOffset = MaxItemsFieldOffset + MaxItemsFieldSize;
		private const int PreviousPageOffsetFieldSize = sizeof(ulong);
		private const int NextPageOffsetFieldOffset = PreviousPageOffsetFieldOffset + PreviousPageOffsetFieldSize;
		private const int NextPageOffsetFieldSize = sizeof(ulong);
		private const int Object0SizeFieldOffset = NextPageOffsetFieldOffset + NextPageOffsetFieldSize;
		private const int ObjectSizeFieldSize = sizeof(uint);

		private volatile int _version;
		private readonly StreamMappedList<TItem> _parent;
		private int[] _itemSizes;
		private long _previousPagePosition;
		private long _nextPagePosition;
		private long[] _offsets;

		public DynamicStreamPage(StreamMappedList<TItem> parent)
			: this(Page0Offset, Page0Offset, parent) {
		}

		public DynamicStreamPage(DynamicStreamPage<TItem> previousPage)
			: this(previousPage.StartPosition, previousPage.NextPagePosition, previousPage._parent) {
		}

		private DynamicStreamPage(long previousPagePosition, long startPosition, StreamMappedList<TItem> parent) 
			: base(parent) {
			Guard.ArgumentNotNull(parent, nameof(parent));
			Guard.ArgumentNotNull(parent.Stream, nameof(parent.Stream));
			Guard.ArgumentInRange(startPosition, Page0Offset, parent.Stream.Length, nameof(startPosition));
			Guard.ArgumentInRange(previousPagePosition, Page0Offset, startPosition, nameof(previousPagePosition));
			_version = 0;
			_parent = parent;
			_itemSizes = new int[0];
			_offsets = null;

			if (startPosition < parent.Stream.Length) {
				// CASE: Page already exists in storage, load it
				Stream.Seek(StartPosition, SeekOrigin.Begin);
				// NOTE: we set the field not the property to avoid rewriting same value just read
				var itemCount = Reader.ReadUInt32();
				base.Count = (int)itemCount;
				var maxItems = (int)Reader.ReadUInt32();
				_previousPagePosition = (long)Reader.ReadUInt64();
				_nextPagePosition = (long)Reader.ReadUInt64();
				_itemSizes = Tools.Collection.Generate(Reader.ReadUInt32).Cast<int>().Take(maxItems).ToArray();
			} else if (startPosition == parent.Stream.Length) {
				// CASE: Page begins end of storage as it is newly appended, so write out default header
				StartPosition = startPosition;
				Count = 0;
				MaxItems = _parent.PageSize;
				PreviousPagePosition = previousPagePosition;
				NextPagePosition = GetItem0DataOffset(); // Empty, so next page begins straight after this header
				SetItemSizes(0, Enumerable.Repeat(0, MaxItems).ToArray(), out _);
			} else {
				// ILLEGAL: Page starts beyond known storage boundary, invalid
				throw new ArgumentOutOfRangeException(nameof(startPosition), startPosition, "Page start position beyond storage boundary");
			}

		}

		public override int Count {
			get => base.Count;
			set {
				Guard.ArgumentInRange(value, 0, int.MaxValue, "Value");
				base.Count = value;
				Stream.Seek(StartPosition + CountFieldOffset, SeekOrigin.Begin);
				Writer.Write((uint)value);
			}
		}

		public int MaxItems {
			get => _itemSizes.Length;
			set {
				Guard.ArgumentInRange(value, 1, int.MaxValue, "Value");
				Array.Resize(ref _itemSizes, value);
				Stream.Seek(StartPosition + MaxItemsFieldOffset, SeekOrigin.Begin);
				Writer.Write((uint)value);
				if (State == PageState.Loaded)
					CalculateOffsets(1);
			}
		}

		public long PreviousPagePosition {
			get => _previousPagePosition;
			set {
				_previousPagePosition = value;
				Guard.ArgumentInRange(value, 0, uint.MaxValue, "Value");
				Stream.Seek(StartPosition + PreviousPageOffsetFieldOffset, SeekOrigin.Begin);
				Writer.Write((ulong)value);
			}
		}

		public long NextPagePosition {
			get => _nextPagePosition;
			set {
				_nextPagePosition = value;
				Guard.ArgumentInRange(value, 0, uint.MaxValue, "Value");
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
			State = PageState.Loaded;
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

		protected override IEnumerable<TItem> ReadInternal(int index, int count) {
			CheckPageState(PageState.Loaded);
			// Transform list index into page index
			index -= StartIndex;
			Stream.Seek(_offsets[index], SeekOrigin.Begin);
			yield return Serializer.Deserialize(_itemSizes[index], Reader);
		}

		protected override int AppendInternal(TItem[] items, out int newItemsSize) {
			CheckPageState(PageState.Loaded);
			var itemsToAdd = Math.Min(items.Length, MaxItems - Count);

			// Page full case
			if (itemsToAdd == 0) {
				newItemsSize = 0;
				return 0;
			}
			UpdateInternal(StartIndex + Count, items.Take(itemsToAdd).ToArray(), out _, out newItemsSize);
			return itemsToAdd;
		}

		protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSize, out int newItemsSize) {
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
			Stream.Seek(_offsets[index], SeekOrigin.Begin);  // offset should always be set (append/update case)
			newItemsSize = 0;
			var itemSizes = items.Select(x => Serializer.Serialize(x, Writer)).ToArray();
			newItemsSize = itemSizes.Sum();
			SetItemSizes(index, itemSizes, out oldItemsSize);
			NextPagePosition += newItemsSize - oldItemsSize;
			Interlocked.Increment(ref _version);
		}

		protected override void EraseFromEndInternal(int count, out int oldItemsSize) {
			CheckPageState(PageState.Loaded);
			var removeFrom = this.Count - count;
			SetItemSizes(removeFrom, Enumerable.Repeat(0, count).ToArray(), out oldItemsSize);
			Stream.SetLength(Stream.Length - oldItemsSize);
			Interlocked.Increment(ref _version);
		}

		private long GetItem0DataOffset() {
			return StartPosition + Object0SizeFieldOffset + MaxItems * ObjectSizeFieldSize;
		}

		private void SetItemSizes(int index, int[] sizes, out int oldItemsSize) {
			Guard.ArgumentInRange(index, 0, MaxItems - 1, nameof(index));
			Guard.ArgumentNotNull(sizes, nameof(sizes));
			Guard.Ensure(index + sizes.Length <= MaxItems, "Sizes array is incorrectly sized");

			oldItemsSize = _itemSizes.Skip(index).Take(sizes.Length).Sum();
			Array.Copy(sizes, 0, _itemSizes, index, sizes.Length);
			Stream.Seek(StartPosition + Object0SizeFieldOffset + index * ObjectSizeFieldSize, SeekOrigin.Begin);
			sizes.Cast<uint>().ForEach(Writer.Write);

			// Calculate the updated offsets if opened
			if (State == PageState.Loaded)
				CalculateOffsets(index + 1);
		}

		private void CalculateOffsets(int from) {
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
}
