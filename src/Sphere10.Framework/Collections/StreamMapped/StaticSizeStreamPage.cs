using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// A page of data stored on a stream whose items are constant sized.
	/// </summary>
	/// <remarks>
	/// Unlike <see cref="DynamicStreamPage{TItem}"/> no header is stored for the page.
	/// </remarks>

	internal class StaticStreamPage<TItem> : StreamPageBase<TItem> {
		private readonly int _item0Offset;
		private volatile int _version;

		public StaticStreamPage(StreamPagedList<TItem> parent) : base(parent) {
			Guard.Argument(parent.Serializer.IsStaticSize, nameof(parent), $"Parent list's serializer is not fixed size. {nameof(StaticStreamPage<TItem>)} only supports fixed sized items.");
			Guard.Ensure(parent.Serializer.StaticSize > 0, $"{nameof(TItem)} serialization size size is not greater than 0");
			
			_version = 0;
			_item0Offset = Parent.IncludeListHeader ? StreamPagedList<TItem>.ListHeaderSize : 0;
			base.State = PageState.Loaded;
			base.StartIndex = 0;
			base.EndIndex = (int)(Stream.Length - _item0Offset) / ItemSize - 1;
			StartPosition = _item0Offset;
		}

		public int MaxItems => Parent.PageSize / ItemSize;

		public override int Count => (int)(Stream.Length - _item0Offset) / ItemSize;

		public override int Size => Count * ItemSize;

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

		protected override IEnumerable<TItem> ReadInternal(int index, int count) {
			var startIndex = index * ItemSize + _item0Offset;

			for (var i = 0; i < count; i++) {
				Stream.Seek(startIndex + i * ItemSize, SeekOrigin.Begin);

				yield return Serializer.Deserialize(ItemSize, Reader);
			}
		}

		protected override int AppendInternal(TItem[] items, out int newItemsSize) {
			if (!items.Any()) {
				newItemsSize = items.Length;
				return items.Length;
			}

			if (items.Length + Count > MaxItems)
				throw new InvalidOperationException("Unable to append items, Max Items of page will be exceeded.");

			Stream.Seek(_item0Offset + Count * ItemSize, SeekOrigin.Begin);

			foreach (var item in items) {
				var bytesWritten = Serializer.Serialize(item, Writer);
				Guard.Ensure(bytesWritten == Serializer.StaticSize, $"Static serializer wrote {bytesWritten} bytes expected {Serializer.StaticSize}");
			}

			newItemsSize = items.Length * ItemSize;
			Interlocked.Increment(ref _version);

			return items.Length;
		}

		protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSize, out int newItemsSize) {
			CheckPageState(PageState.Loaded);
			Guard.Ensure(index + items.Length <= Count, "Update outside bounds of list");

			var itemsSize = items.Length * ItemSize;
			index = index * ItemSize + _item0Offset;

			Stream.Seek(index, SeekOrigin.Begin);

			foreach (var item in items) {
				var bytesWritten = Serializer.Serialize(item, Writer);
				Guard.Ensure(bytesWritten == Serializer.StaticSize, $"Static serializer wrote {bytesWritten} bytes expected {Serializer.StaticSize}");
			}

			newItemsSize = itemsSize;
			oldItemsSize = itemsSize;
			Interlocked.Increment(ref _version);
		}

		protected override void EraseFromEndInternal(int count, out int oldItemsSize) {
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
		public override int ReadItemBytes(int itemIndex, int byteOffset, int byteLength, out byte[] result) {
			Guard.ArgumentInRange(itemIndex, 0, Count - 1, nameof(itemIndex));

			int offset = itemIndex * ItemSize + _item0Offset + byteOffset;

			Stream.Seek(offset, SeekOrigin.Begin);
			result = Reader.ReadBytes(byteLength);
			return result.Length;
		}
	}

}
