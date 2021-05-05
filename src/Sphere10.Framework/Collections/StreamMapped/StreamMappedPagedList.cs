using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	/// <summary>
	/// A list implementation which is mapped onto a stream via contiguous pages. Items can only be added/removed from the end of the stream.
	/// </summary>
	/// <remarks>
	/// List Serialization Format
	/// ==========================
	/// [List Header]
	/// [Page 0] 
	/// ...
	/// [Page N]
	///
	/// List Header Format (256 bytes)
	/// ===================
	/// MagicID (DWORD): special signature number to signify a StreamedList exists here
	/// Version (BYTE): version of the list data format
	/// Traits (DWORD): traits associated with the storage format of this list
	/// PADDING: 246 bytes
	/// 
	/// Page Format
	/// ===========
	/// [PageHeader]
	/// [Item 0 Raw Bytes]
	/// ....
	/// [Item N Raw Bytes]
	///
	/// Page Header Format
	/// ==================
	///	Count (UINT32): number of items contained in this page
	/// MaxItems (UINT32): max number of items page can contain
	/// PreviousPageOffset (UINT64): absolute position in the byte array where previous page begins
	/// NextPageOffset (UINT64): absolute position in the byte array where next page begins
	///	Item 0 Size (UINT32): size of first item in this page
	///	...
	///	Item N Size (UINT32): size of last item in this page
	///
	///
	/// Version 2 Design Nodes:
	/// =======================
	///  - traits to include "ListHeaderMaintainsPageCount", "FixedSizeItems", "FixedSizePages", "TrackPreviousPageOffset", "TrackNextPageOffset"
	///  - page headers to be slimmed down depending on traits
	///  - indexing
	/// </remarks>
	public class StreamMappedPagedList<TItem> : PagedListBase<TItem> {
		public const uint MagicID = 31337;
		public const byte FormatVersion = 1;
		public const int ListHeaderSize = 256;
		public const int DefaultPageSize = 100;

		public StreamMappedPagedList(IItemSerializer<TItem> serializer, Stream stream) : this(
			serializer.IsFixedSize ? StreamMappedPagedListType.FixedSize : StreamMappedPagedListType.Dynamic,
			serializer,
			stream,
			serializer.IsFixedSize ? int.MaxValue : DefaultPageSize) {
		}

		public StreamMappedPagedList(IItemSerializer<TItem> serializer, Stream stream, int pageSize)
			: this(serializer.IsFixedSize ? StreamMappedPagedListType.FixedSize : StreamMappedPagedListType.Dynamic, serializer, stream, pageSize) {
		}

		public StreamMappedPagedList(StreamMappedPagedListType type, IItemSerializer<TItem> serializer, Stream stream, int pageSize) {
			PageSize = pageSize;
			Serializer = serializer;
			Stream = stream;
			Reader = new EndianBinaryReader(EndianBitConverter.Little, Stream);
			Writer = new EndianBinaryWriter(EndianBitConverter.Little, Stream);
			RequiresLoad = Stream.Length > 0;
			Type = type;
		}

		public int PageSize { get; }

		public bool IncludeListHeader { get; set; } = true;

		public StreamMappedPagedListType Type { get; }

		internal Stream Stream { get; }

		internal EndianBinaryReader Reader { get; }

		internal EndianBinaryWriter Writer { get; }

		internal IItemSerializer<TItem> Serializer { get; }

		public override IDisposable EnterOpenPageScope(IPage<TItem> page) {
			switch (Type) {
				case StreamMappedPagedListType.Dynamic: {
					var streamedPage = (DynamicStreamPage<TItem>)page;
					streamedPage.Open();

					return Tools.Scope.ExecuteOnDispose(streamedPage.Close);
				}
				case StreamMappedPagedListType.FixedSize:
					return Tools.Scope.ExecuteOnDispose(() => { }); //no-op
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public int ReadItemRaw(int itemIndex, int byteOffset, int byteLength, out byte[] result) {
			CheckRequiresLoad();
			NotifyAccessing();
			CheckRange(itemIndex, 1);

			var pageSegment = GetPageSegments(itemIndex, 1).Single();
			var page = (StreamPageBase<TItem>)pageSegment.Item1;
			NotifyPageAccessing(page);
			using (EnterOpenPageScope(page)) {
				NotifyPageReading(page);
				page.ReadItemRaw(itemIndex, byteOffset, byteLength, out result);
				NotifyPageRead(page);
			}
			NotifyPageAccessed(page);
			NotifyAccessed();

			return result.Length;
		}

		public virtual IEnumerable<IEnumerable<TItem>> ReadRangeByPage(int index, int count) {
			CheckRequiresLoad();
			NotifyAccessing();
			CheckRange(index, count);
			foreach (var pageSegment in GetPageSegments(index, count)) {
				var page = pageSegment.Item1;
				var pageStartIndex = pageSegment.Item2;
				var pageItemCount = pageSegment.Item3;
				NotifyPageAccessing(page);
				using (EnterOpenPageScope(page)) {
					NotifyPageReading(page);
					yield return page.Read(pageStartIndex, pageItemCount);
					NotifyPageRead(page);
				}
				NotifyPageAccessed(page);
			}
			NotifyAccessed();
		}

		protected override IPage<TItem>[] LoadPages() {
			if (IncludeListHeader) {
				Stream.Seek(0L, SeekOrigin.Begin);
				var magic = Reader.ReadUInt32();
				if (magic != MagicID)
					throw new InvalidDataFormatException($"Incorrect or missing MagicID field");
				var version = Reader.ReadUInt16();
				if (version != 1)
					throw new NotSupportedException($"Version {version} data format not supported");
				var traits = Reader.ReadUInt32();
				if (traits != 0)
					throw new NotSupportedException($"Unrecognized traits {traits}");
			}

			// Load pages if any
			var pages = new List<IPage<TItem>>();
			if (Type is StreamMappedPagedListType.Dynamic) {
				while (Stream.Position < Stream.Length)
					pages.Add(InternalPages.Any() ? new DynamicStreamPage<TItem>(this) : new DynamicStreamPage<TItem>((DynamicStreamPage<TItem>)pages.Last()));
			} else pages.Add(new FixedSizeStreamPage<TItem>(this));

			return pages.ToArray();
		}

		protected override void OnPageCreating(int pageNumber) {
			base.OnPageCreating(pageNumber);
			// Create list header if not created
			if (IncludeListHeader && Stream.Length == 0) {
				CreateListHeader();
			}
		}

		protected override void OnPageDeleted(IPage<TItem> page) {
			base.OnPageDeleted(page);

			if (page is StreamPageBase<TItem> streamPage)
				Stream.SetLength(page.Number > 0 ? streamPage.StartPosition : 0L);

		}

		protected override IPage<TItem> NewPageInstance(int pageNumber) =>
			Type switch {
				StreamMappedPagedListType.Dynamic => pageNumber == 0 ? new DynamicStreamPage<TItem>(this) : new DynamicStreamPage<TItem>((DynamicStreamPage<TItem>)InternalPages.Last()),
				StreamMappedPagedListType.FixedSize => pageNumber == 0
					? new FixedSizeStreamPage<TItem>(this)
					: throw new InvalidOperationException($"{nameof(StreamMappedPagedListType.FixedSize)} only supports a single page."),
				_ => throw new ArgumentOutOfRangeException()
			};

		private void CreateListHeader() {
			Stream.Seek(0, SeekOrigin.Begin);
			Writer.Write((uint)MagicID);
			Writer.Write((byte)FormatVersion);
			Writer.Write((uint)0U); // traits (used in v2)
			Writer.Write(Tools.Array.Gen(ListHeaderSize - (int)Stream.Position, (byte)0)); // padding
			Debug.Assert(Stream.Position == ListHeaderSize);
		}
	}

}
