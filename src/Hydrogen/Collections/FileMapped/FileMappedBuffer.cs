using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen {

	/// <summary>
	/// A collection of bytes that is mapped over a file and accessed via intelligent memory-mapped paging.
	/// <remarks>
	/// By varying the pageSize and in inMemoryPages arguments the performance of this list can by tuned to meet the specific use case.
	/// In cases where appending is more demanded, the page sizes should be larger so they are swapped less often.
	/// In cases where random accessing the list, the page sizes should be smaller so they can be loaded faster.
	/// In general, the more pages allowed in memory the less frequently they will be swapped to storage.
	/// </remarks>
	/// </summary>
	public sealed class FileMappedBuffer : FilePagedListBase<byte>, IMemoryPagedBuffer {
		private readonly IPagedListDelegate<byte> _friend;
		private readonly ReadOnlyListDecorator<IPage<byte>, IBufferPage> _pagesDecorator;

		public FileMappedBuffer(string filename, int pageSize, long maxMemory, bool readOnly = false)
			: base(filename, pageSize, maxMemory, readOnly) {
			_friend = CreateFriendDelegate();
			_pagesDecorator = new ReadOnlyListDecorator<IPage<byte>, IBufferPage>(new ReadOnlyListAdapter<IPage<byte>>(base.InternalPages));
		}

		public new IReadOnlyList<IBufferPage> Pages => _pagesDecorator;

		public ReadOnlySpan<byte> ReadSpan(int index, int count) => PagedBufferImplementationHelper.ReadRange(_friend, index, count);

		public void AddRange(ReadOnlySpan<byte> span) => PagedBufferImplementationHelper.AddRange(_friend, span);

		public void UpdateRange(int index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.UpdateRange(_friend, index, items);

		public void InsertRange(int index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.InsertRange(_friend, Count, index, items);

		public Span<byte> AsSpan(int index, int count) => PagedBufferImplementationHelper.AsSpan(_friend, index, count);

		public void ExpandTo(int totalBytes) => PagedBufferImplementationHelper.ExpandTo(_friend, totalBytes);

		public void ExpandBy(int newBytes) => PagedBufferImplementationHelper.ExpandBy(_friend, newBytes);

		protected override IPage<byte> NewPageInstance(int pageNumber) {
			return new PageImpl(Stream, pageNumber, PageSize);
		}

		protected override IPage<byte>[] LoadPages() {
			var fileLength = (int)Stream.Length;
			if (fileLength == 0)
				return new PageImpl[0];

			var numPages = (int)Math.Ceiling(fileLength / (double)PageSize);
			var lastPageSize = (int)(fileLength - (numPages - 1) * PageSize);
			return
				Enumerable
				.Range(0, numPages - 1)
				.Select((x, i) =>
				   new PageImpl(Stream, i, PageSize) {
					   Number = i,
					   StartPosition = i * sizeof(byte) * PageSize,
					   StartIndex = i * PageSize,
					   EndIndex = (i + 1) * PageSize - 1,
					   EndPosition = (i + 1) * sizeof(byte) * PageSize - 1,
					   Count = PageSize,
					   Size = PageSize,
					   State = PageState.Unloaded,
				   }
				).Concat(
					// Last page
					new PageImpl(Stream, numPages - 1, PageSize) {
						Number = numPages - 1,
						StartPosition = (numPages - 1) * sizeof(byte) * PageSize,
						StartIndex = (numPages - 1) * PageSize,
						EndIndex = fileLength - 1,
						EndPosition = fileLength - 1,
						Count = lastPageSize,
						Size = lastPageSize,
						State = PageState.Unloaded,
					}
				)
				.Cast<IPage<byte>>()
				.ToArray();
		}

		protected override void OnPageSaving(IMemoryPage<byte> page) {
			// Always ensure file-stream is long enough to save this page
			base.OnPageSaving(page);
			var requiredLen = ((PageImpl)page).EndIndex + 1;
			if (Stream.Length < requiredLen)
				Stream.SetLength(requiredLen);
		}

		protected override void OnPageSaved(IMemoryPage<byte> page) {
			// Always ensure file-stream is never larger than last page
			base.OnPageSaved(page);
			if (page.Number == InternalPages.Count - 1) {
				TruncateFile();
			}
		}

		protected override void OnPageDeleted(IPage<byte> page) {
			base.OnPageDeleted(page);
			if (Disposing)
				return;
			if (!IsReadOnly && (page.Number == 0 || page.Number == InternalPages.Count)) {
				TruncateFile();
			}
		}

		/// <summary>
		/// The page is mapped to a section of a single file
		/// </summary>
		public class PageImpl : FilePageBase<byte>, IBufferPage {

			public PageImpl(Stream stream, int pageNumber, int pageSize)
				: base(stream, new StaticSizeItemSizer<byte>(sizeof(byte)), pageNumber, pageSize, new MemoryBuffer(0, pageSize, pageSize)) {
			}

			protected override void SaveInternal(IExtendedList<byte> memoryPage, Stream stream) {
				// Use byte streaming for perf
				var memBuff = (MemoryBuffer)memoryPage;
				using (var writer = new BinaryWriter(stream)) {
					writer.Write(memBuff.AsSpan());
				}
			}

			protected override void LoadInternal(Stream stream, IExtendedList<byte> memoryPage) {
				// Use byte streaming for perf
				var memBuff = (MemoryBuffer)memoryPage;
				memBuff.ExpandTo((int)stream.Length);
				var bytesRead = stream.Read(memBuff.AsSpan());
				Guard.Ensure(bytesRead == stream.Length, "Read less bytes than expected");
			}

			public ReadOnlySpan<byte> ReadSpan(int index, int count)
				=> PagedBufferImplementationHelper.ReadPageSpan(this, (MemoryBuffer)MemoryStore, index, count);

			public void UpdateSpan(int index, ReadOnlySpan<byte> items) {
				int beforeUpdateSize = Size;
				PagedBufferImplementationHelper.UpdatePageSpan(this, MemoryStore as MemoryBuffer, index, items);
				EndPosition += Size - beforeUpdateSize;
			}

			public bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow) {
				var fittedCompletely = PagedBufferImplementationHelper.AppendPageSpan(this, MemoryStore as MemoryBuffer, items, out overflow);
				EndPosition += items.Length - overflow.Length;
				return fittedCompletely;
			}
		}
	}
}