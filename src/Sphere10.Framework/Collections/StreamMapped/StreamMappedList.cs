using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	/// <summary>
	/// A paged list whose pages are mapped onto a stream. 
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
	public class StreamMappedList<TItem> : PagedListBase<TItem> {
		public const uint MagicID = 31337;
		public const byte FormatVersion = 1;
		public const int ListHeaderSize = 256;
	
		public StreamMappedList(int pageSize, IObjectSerializer<TItem> serializer, Stream stream) {
			PageSize = pageSize;
			Serializer = serializer;
			Stream = stream;
			Reader = new EndianBinaryReader(EndianBitConverter.Little, Stream);
			Writer = new EndianBinaryWriter(EndianBitConverter.Little, Stream);
			RequiresLoad = Stream.Length > 0;
		}

		public int PageSize { get; }

		internal Stream Stream { get; }

		internal EndianBinaryReader Reader { get; }

		internal EndianBinaryWriter Writer { get; }

		internal IObjectSerializer<TItem> Serializer { get; }

		public override IDisposable EnterOpenPageScope(IPage<TItem> page) {
			var streamedPage = (StreamPage<TItem>) page;
			streamedPage.Open();
			return Tools.Scope.ExecuteOnDispose(streamedPage.Close);
		}

		protected override IPage<TItem>[] LoadPages() {
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

			// Load pages if any
			var pages = new List<StreamPage<TItem>>();
			while (Stream.Position < Stream.Length)
				pages.Add(InternalPages.Any() ? new StreamPage<TItem>(this) : new StreamPage<TItem>(pages.Last()));
			return pages.ToArray();
		}

		protected override void OnPageCreating(int pageNumber) {
			base.OnPageCreating(pageNumber);
			// Create list header if not created
			if (Stream.Length == 0) 
				CreateListHeader();
		}

		protected override void OnPageDeleted(IPage<TItem> page) {
			base.OnPageDeleted(page);
			var streamedPage = (StreamPage<TItem>)page;
			Stream.SetLength(page.Number > 0 ? streamedPage.StartPosition : 0L);
		}

		protected override IPage<TItem> NewPageInstance(int pageNumber) => pageNumber == 0 ? new StreamPage<TItem>(this) : new StreamPage<TItem>((StreamPage<TItem>)InternalPages.Last());

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
