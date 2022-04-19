// <copyright file="PageManager.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A collection of bytes that is mapped over a file and accessed via intelligent memory-mapped paging and mutated
	/// in an ACID transactional manner.
	/// <remarks>
	/// By varying the pageSize and in inMemoryPages arguments the performance of this list can by tuned to meet the specific use case.
	/// In cases where appending is more demanded, the page sizes should be larger so they are swapped less often.
	/// In cases where random accessing the list, the page sizes should be smaller so they can be loaded faster.
	/// In general, the more pages allowed in memory the less frequently they will be swapped to storage.
	/// </remarks>
	/// </summary>
	public sealed class TransactionalFileMappedBuffer : TransactionalFileMappedListBase<byte>, IMemoryPagedBuffer {
		private readonly IPagedListDelegate<byte> _friend;
		private readonly ReadOnlyListDecorator<IPage<byte>, IBufferPage> _pagesDecorator;

		public TransactionalFileMappedBuffer(string filename, int pageSize, long maxMemory, bool readOnly = false, bool autoLoad = false)
			: this(filename, System.IO.Path.GetDirectoryName(filename), pageSize, maxMemory, readOnly, autoLoad) {
		}

		public TransactionalFileMappedBuffer(string filename, string uncommittedPageFileDir, int pageSize, long maxMemory, bool readOnly = false, bool autoLoad = false)
			: base(filename, uncommittedPageFileDir, pageSize, maxMemory, readOnly, autoLoad) {
			_friend = CreateFriendDelegate();
			_pagesDecorator = new ReadOnlyListDecorator<IPage<byte>, IBufferPage>(new ReadOnlyListAdapter<IPage<byte>>( base.InternalPages));
		}

		public new IReadOnlyList<IBufferPage> Pages => _pagesDecorator;

		public override TransactionalFileMappedBuffer AsBuffer => this;

		public ReadOnlySpan<byte> ReadSpan(int index, int count) => PagedBufferImplementationHelper.ReadRange(_friend, index, count);

		public void AddRange(ReadOnlySpan<byte> span) => PagedBufferImplementationHelper.AddRange(_friend, span);

		public void UpdateRange(int index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.UpdateRange(_friend, index, items);

		public void InsertRange(int index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.InsertRange(_friend, Count, index, items);

		public Span<byte> AsSpan(int index, int count) => PagedBufferImplementationHelper.AsSpan(_friend, index, count);

		public void ExpandTo(int totalBytes) => PagedBufferImplementationHelper.ExpandTo(_friend, totalBytes);

		public void ExpandBy(int newBytes) => PagedBufferImplementationHelper.ExpandBy(_friend, newBytes);

		protected override IPage<byte>[] LoadPages() {
			var lowestDeletedPageNumber = PageMarkerRepo.LowestDeletedPageNumber ?? int.MaxValue;
			var highestChangedPageNumber = PageMarkerRepo.HighestChangedPageNumber ?? int.MinValue;
			var committedPageCount = GetCommittedPageCount();
			var logicalPageCount = Math.Min(lowestDeletedPageNumber, Math.Max(highestChangedPageNumber + 1, committedPageCount));
			var lastLogicalPageNumber = logicalPageCount - 1;

			if (lastLogicalPageNumber < 0)
				return Array.Empty<IPage<byte>>();

			var lastPageLength = 0;
			if (PageMarkerRepo.Contains(PageMarkerType.UncommittedPage, lastLogicalPageNumber)) {
				lastPageLength = (int)Tools.FileSystem.GetFileSize(PageMarkerRepo.GetMarkerFilename(highestChangedPageNumber, PageMarkerType.UncommittedPage));
			} else {
				var remainder = (int)Stream.Length % PageSize;
				lastPageLength = remainder == 0 ? PageSize : remainder;
			}

			var logicalFileLength = (logicalPageCount - 1) * PageSize + lastPageLength;
			if (logicalFileLength == 0)
				return Array.Empty<IPage<byte>>();

			return
				Enumerable
					.Range(0, logicalPageCount - 1)
					.Select((x, i) =>
						new PageImpl(base.Stream, PageMarkerRepo.GetMarkerFilename(i, PageMarkerType.UncommittedPage), i, PageSize) {
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
						new PageImpl(base.Stream, PageMarkerRepo.GetMarkerFilename(logicalPageCount - 1, PageMarkerType.UncommittedPage), logicalPageCount - 1, PageSize) {
							Number = logicalPageCount - 1,
							StartPosition = (logicalPageCount - 1) * sizeof(byte) * PageSize,
							StartIndex = (logicalPageCount - 1) * PageSize,
							EndIndex = logicalFileLength - 1,
							EndPosition = logicalFileLength - 1,
							Count = lastPageLength,
							Size = lastPageLength,
							State = PageState.Unloaded,
						}
					)
					.Cast<IPage<byte>>()
					.ToArray();
		}

		protected override int GetCommittedPageCount() {
			return (int)Math.Ceiling(Stream.Length / (double)PageSize);
		}

		protected override IPage<byte> NewPageInstance(int pageNumber) {
			return new PageImpl(
				Stream,
				PageMarkerRepo.GetMarkerFilename(pageNumber, PageMarkerType.UncommittedPage),
				pageNumber,
				PageSize
			);
		}

		private class PageImpl : TransactionalFilePageBase<byte>, IBufferPage {

			public PageImpl(FileStream stream, string uncommittedPageFileName, int pageNumber, int pageSize)
				: base(stream, new StaticSizeItemSizer<byte>(sizeof(byte)), uncommittedPageFileName, pageNumber, pageSize, new MemoryBuffer(0, pageSize, pageSize)) {
			}

			public ReadOnlySpan<byte> ReadSpan(int index, int count)
				=> PagedBufferImplementationHelper.ReadPageSpan(this, (MemoryBuffer)MemoryStore, index, count);

			public bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow)
				=> PagedBufferImplementationHelper.AppendPageSpan(this, MemoryStore as MemoryBuffer, items, out overflow);

			public void UpdateSpan(int index, ReadOnlySpan<byte> items) =>
				PagedBufferImplementationHelper.UpdatePageSpan(this, MemoryStore as MemoryBuffer, index, items);

			protected override void SaveInternal(IExtendedList<byte> memoryPage, Stream stream) {
				var memBuff = (MemoryBuffer)memoryPage;
				if (stream.Length != memBuff.Count)
					stream.SetLength(memBuff.Count); // eliminate unused space in fiel
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


		}
	}
}