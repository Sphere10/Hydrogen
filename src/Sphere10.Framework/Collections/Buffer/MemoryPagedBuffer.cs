using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public class MemoryPagedBuffer : MemoryPagedListBase<byte>, IMemoryPagedBuffer {

        public MemoryPagedBuffer(int pageSize, long maxMemory) 
			: base(pageSize, maxMemory) {
		}

		internal new IReadOnlyList<IBufferPage> Pages => new ReadOnlyListDecorator<IPage<byte>, IBufferPage>(InternalPages);

		IReadOnlyList<IBufferPage> IMemoryPagedBuffer.Pages => this.Pages;

		protected override IPage<byte> NewPageInstance(int pageNumber) {
			return new BufferPage(PageSize);
		}

		protected override IPage<byte>[] LoadPages() {
			throw new NotSupportedException("Pages are not loadable across runtime sessions in this implementation. See BinaryFile class.");
		}

        public ReadOnlySpan<byte> ReadSpan(int index, int count) => PagedBufferImplementationHelper.ReadRange(CreateFriendDelegate(), index,  count);

        public void AddRange(ReadOnlySpan<byte> span) => PagedBufferImplementationHelper.AddRange(CreateFriendDelegate(), span);

        public void UpdateRange(int index, ReadOnlySpan<byte> items) => PagedBufferImplementationHelper.UpdateRange(CreateFriendDelegate(), index, items);

        public void InsertRange(int index, ReadOnlySpan<byte> items) =>  PagedBufferImplementationHelper.InsertRange(CreateFriendDelegate(), Count,  index, items);

        public Span<byte> AsSpan(int index, int count) => PagedBufferImplementationHelper.AsSpan(CreateFriendDelegate(), index, count);

		/// <summary>
		/// The page is mapped to it's own page file.
		/// </summary>
		public sealed class BufferPage : FileSwappedMemoryPage<byte>, IBufferPage {

			public BufferPage(int pageSize)
				: base(pageSize, new FixedSizeItemtSizer<byte>(sizeof(byte)), new MemoryBuffer(0, pageSize, pageSize)) {
			}

			public ReadOnlySpan<byte> ReadSpan(int index, int count) 
				=> PagedBufferImplementationHelper.ReadPageSpan(this, MemoryStore as MemoryBuffer, index, count);

			public bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow)
				=> PagedBufferImplementationHelper.AppendPageSpan(this, MemoryStore as MemoryBuffer,  items, out overflow);

			public void UpdateSpan(int index, ReadOnlySpan<byte> items) =>
				PagedBufferImplementationHelper.UpdatePageSpan(this, MemoryStore as MemoryBuffer, index, items);

			protected override void SaveInternal(IExtendedList<byte> memoryPage, Stream stream) {
				var memBuff = (MemoryBuffer)memoryPage;
				if (stream.Length != memBuff.Count)
					stream.SetLength(memBuff.Count); // eliminate unused end-space in page file (happens when add, save, remove, save)
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