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

namespace Hydrogen;

/// <summary>
/// A collection of bytes that is mapped over a file and accessed via intelligent memory-mapped paging and mutated
/// in an ACID transactional manner.
/// <remarks>
/// By varying the pageSize and in inMemoryPages arguments the performance of this list can by tuned to meet the specific use case.
/// In cases where appending is more demanded, the page sizes should be larger so they are swapped less often.
/// In cases where random accessing the list, the page sizes should be smaller so they can be loaded faster.
/// In general, the more pages allowed in memory the less frequently they will be swapped to streams.
/// </remarks>
/// </summary>
public sealed class TransactionalFileMappedBuffer : TransactionalFileMappedListBase<byte>, IMemoryPagedBuffer {
	private readonly IPagedListDelegate<byte> _friend;

	public TransactionalFileMappedBuffer(TransactionalFileDescriptor fileDescriptor, FileAccessMode accessMode = FileAccessMode.Default)
		: base(fileDescriptor, accessMode) {
		_friend = CreateFriendDelegate();
		Pages = InternalPages.AsReadOnly().WithProjection(x => (IBufferPage)x);
	}

	public new IReadOnlyList<IBufferPage> Pages { get; }

	public override TransactionalFileMappedBuffer AsBuffer => this;

	public ReadOnlySpan<byte> ReadSpan(long index, long count) => PagedBufferImplementationHelper.ReadRange(_friend, index, count);

	public void AddRange(ReadOnlySpan<byte> span) => PagedBufferImplementationHelper.AddRange(_friend, span);

	public void UpdateRange(long index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.UpdateRange(_friend, index, items);

	public void InsertRange(long index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.InsertRange(_friend, Count, index, items);

	public Span<byte> AsSpan(long index, long count) => PagedBufferImplementationHelper.AsSpan(_friend, index, count);

	public void ExpandTo(long totalBytes) => PagedBufferImplementationHelper.ExpandTo(_friend, totalBytes);

	public void ExpandBy(long newBytes) => PagedBufferImplementationHelper.ExpandBy(_friend, newBytes);

	protected override IPage<byte>[] LoadPages() {
		var lowestDeletedPageNumber = PageMarkerRepo.LowestDeletedPageNumber ?? int.MaxValue;
		var highestChangedPageNumber = PageMarkerRepo.HighestChangedPageNumber ?? int.MinValue;
		var committedPageCount = GetCommittedPageCount();
		var logicalPageCount = Math.Min(lowestDeletedPageNumber, Math.Max(highestChangedPageNumber + 1, committedPageCount));
		var lastLogicalPageNumber = logicalPageCount - 1;

		if (lastLogicalPageNumber < 0)
			return Array.Empty<IPage<byte>>();

		var lastPageLength = 0L;
		if (PageMarkerRepo.Contains(PageMarkerType.UncommittedPage, lastLogicalPageNumber)) {
			lastPageLength = Tools.FileSystem.GetFileSize(PageMarkerRepo.GetMarkerFilename(highestChangedPageNumber, PageMarkerType.UncommittedPage));
		} else {
			var remainder = (int)Stream.Length % PageSize;
			lastPageLength = remainder == 0 ? PageSize : remainder;
		}

		var logicalFileLength = (logicalPageCount - 1) * PageSize + lastPageLength;
		if (logicalFileLength == 0)
			return Array.Empty<IPage<byte>>();

		return
			Tools.Collection
				.RangeL(0, logicalPageCount - 1)
				.Select((x, i) =>
					new PageImpl(Stream, PageMarkerRepo.GetMarkerFilename(i, PageMarkerType.UncommittedPage), i, PageSize) {
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
					new PageImpl(Stream, PageMarkerRepo.GetMarkerFilename(logicalPageCount - 1, PageMarkerType.UncommittedPage), logicalPageCount - 1, PageSize) {
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

	protected override long GetCommittedPageCount() {
		return (int)Math.Ceiling(Stream.Length / (double)PageSize);
	}

	protected override IPage<byte> NewPageInstance(long pageNumber) {
		return new PageImpl(
			Stream,
			PageMarkerRepo.GetMarkerFilename(pageNumber, PageMarkerType.UncommittedPage),
			pageNumber,
			PageSize
		);
	}


	private class PageImpl : TransactionalFilePageBase<byte>, IBufferPage {

		public PageImpl(FileStream stream, string uncommittedPageFileName, long pageNumber, long pageSize)
			: base(stream, new ConstantLengthItemSizer<byte>(sizeof(byte), false), uncommittedPageFileName, pageNumber, pageSize, new MemoryBuffer(0, pageSize, pageSize)) {
		}

		public ReadOnlySpan<byte> ReadSpan(long index, long count)
			=> PagedBufferImplementationHelper.ReadPageSpan(this, (MemoryBuffer)MemoryStore, index, count);

		public bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow)
			=> PagedBufferImplementationHelper.AppendPageSpan(this, MemoryStore as MemoryBuffer, items, out overflow);

		public void UpdateSpan(long index, ReadOnlySpan<byte> items) =>
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
