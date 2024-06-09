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
/// A buffer whose contents are paged on memory and suitable for arbitrarily large buffers.
/// </summary>
/// <remarks>The underlying implementation relies on a <see cref="MemoryPagedList{TItem}"/> whose pages are <see cref="MemoryBuffer"/>'s.</remarks>
public class MemoryPagedBuffer : MemoryPagedListBase<byte>, IMemoryPagedBuffer {
	private readonly IPagedListDelegate<byte> _friend;

	public MemoryPagedBuffer(long pageSize, long maxMemory)
		: base(pageSize, maxMemory) {
		_friend = CreateFriendDelegate();
		Pages = InternalPages.AsReadOnly().WithProjection(x => (IBufferPage)x);
	}

	public new IReadOnlyList<IBufferPage> Pages { get; }

	protected override IPage<byte> NewPageInstance(long pageNumber) {
		return new BufferPage(PageSize);
	}

	protected override IPage<byte>[] LoadPages() {
		throw new NotSupportedException($"Pages are not loadable across runtime sessions in this implementation. See {nameof(FileMappedBuffer)} class.");
	}

	public override IEnumerable<byte> ReadRange(long index, long count)
		=> ReadSpan(index, count).ToArray();

	public ReadOnlySpan<byte> ReadSpan(long index, long count)
		=> PagedBufferImplementationHelper.ReadRange(_friend, index, count);

	public override void AddRange(IEnumerable<byte> items)
		=> AddRange((items as byte[] ?? items?.ToArray()).AsSpan());

	public void AddRange(ReadOnlySpan<byte> span)
		=> PagedBufferImplementationHelper.AddRange(_friend, span);

	public override void UpdateRange(long index, IEnumerable<byte> items)
		=> UpdateRange(index, (items as byte[] ?? items?.ToArray()).AsSpan());

	public void UpdateRange(long index, ReadOnlySpan<byte> items)
		=> PagedBufferImplementationHelper.UpdateRange(_friend, index, items);

	public override void InsertRange(long index, IEnumerable<byte> items)
		=> InsertRange(index, (items as byte[] ?? items?.ToArray()).AsSpan());

	public void InsertRange(long index, ReadOnlySpan<byte> items)
		=> PagedBufferImplementationHelper.InsertRange(_friend, Count, index, items);

	public Span<byte> AsSpan(long index, long count)
		=> PagedBufferImplementationHelper.AsSpan(_friend, index, count);

	public void ExpandTo(long totalBytes) {
		throw new NotImplementedException();
	}

	public void ExpandBy(long newBytes) {
		throw new NotImplementedException();
	}


	/// <summary>
	/// The page is mapped to it's own page file.
	/// </summary>
	public sealed class BufferPage : FileSwappedMemoryPage<byte>, IBufferPage {

		public BufferPage(long pageSize)
			: base(pageSize, new ConstantLengthItemSizer<byte>(sizeof(byte), false), new MemoryBuffer(0, pageSize, pageSize)) {
		}

		public ReadOnlySpan<byte> ReadSpan(long index, long count)
			=> PagedBufferImplementationHelper.ReadPageSpan(this, MemoryStore as MemoryBuffer, index, count);

		public bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow)
			=> PagedBufferImplementationHelper.AppendPageSpan(this, MemoryStore as MemoryBuffer, items, out overflow);

		public void UpdateSpan(long index, ReadOnlySpan<byte> items) =>
			PagedBufferImplementationHelper.UpdatePageSpan(this, MemoryStore as MemoryBuffer, index, items);

		protected override void SaveInternal(IExtendedList<byte> memoryPage, Stream stream) {
			var memBuff = (MemoryBuffer)memoryPage;
			if (stream.Length != memBuff.Count)
				stream.SetLength(memBuff.Count); // eliminate unused end-space in page file (happens when add, save, remove, save)
			stream.Write(memBuff.AsSpan());
		}

		protected override void LoadInternal(Stream stream, IExtendedList<byte> memoryPage) {
			// Use byte streaming for perf
			var memBuff = (MemoryBuffer)memoryPage;
			memBuff.ExpandTo((int)stream.Length); // note: underlying allocation is still done in chunks of page-size
			var bytesRead = stream.Read(memBuff.AsSpan());
			Guard.Ensure(bytesRead == stream.Length, "Read less bytes than expected");
		}
	}
}
