using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A buffer whose contents are paged on memory and suitable for arbitrarily large buffers.
	/// </summary>
	/// <remarks>The underlying implementation relies on a <see cref="MemoryPagedList{TItem}"/> whose pages are <see cref="MemoryBuffer"/>'s.</remarks>
	public class MemoryPagedBuffer : MemoryPagedListBase<byte>, IMemoryPagedBuffer {
		private readonly IPagedListDelegate<byte> _friend;
		private readonly ReadOnlyListDecorator<IPage<byte>, IBufferPage> _pagesDecorator;

		public MemoryPagedBuffer(int pageSize, long maxMemory) 
			: base(pageSize, maxMemory) {
			_friend = CreateFriendDelegate();
			_pagesDecorator = new ReadOnlyListDecorator<IPage<byte>, IBufferPage>( new ReadOnlyListAdapter<IPage<byte>>( base.InternalPages));
		}

		public new IReadOnlyList<IBufferPage> Pages => _pagesDecorator;

		protected override IPage<byte> NewPageInstance(int pageNumber) {
			return new BufferPage(PageSize);
		}

		protected override IPage<byte>[] LoadPages() {
			throw new NotSupportedException($"Pages are not loadable across runtime sessions in this implementation. See {nameof(FileMappedBuffer)} class.");
		}

		public override IEnumerable<byte> ReadRange(int index, int count)
			=> ReadSpan(index, count).ToArray();

		public ReadOnlySpan<byte> ReadSpan(int index, int count) 
	        => PagedBufferImplementationHelper.ReadRange(_friend, index,  count);

        public override void AddRange(IEnumerable<byte> items) 
	        => AddRange((items as byte[] ?? items?.ToArray()).AsSpan());

        public void AddRange(ReadOnlySpan<byte> span) 
	        => PagedBufferImplementationHelper.AddRange(_friend, span);

        public override void UpdateRange(int index, IEnumerable<byte> items) 
	        => UpdateRange(index, (items as byte[] ?? items?.ToArray()).AsSpan());

        public void UpdateRange(int index, ReadOnlySpan<byte> items) 
	        => PagedBufferImplementationHelper.UpdateRange(_friend, index, items);

        public override void InsertRange(int index, IEnumerable<byte> items)
	        => InsertRange(index, (items as byte[] ?? items?.ToArray()).AsSpan());

        public void InsertRange(int index, ReadOnlySpan<byte> items) 
	        =>  PagedBufferImplementationHelper.InsertRange(_friend, Count,  index, items);

        public Span<byte> AsSpan(int index, int count) 
	        => PagedBufferImplementationHelper.AsSpan(_friend, index, count);

		/// <summary>
		/// The page is mapped to it's own page file.
		/// </summary>
		public sealed class BufferPage : FileSwappedMemoryPage<byte>, IBufferPage {

			public BufferPage(int pageSize)
				: base(pageSize, new StaticSizeItemSizer<byte>(sizeof(byte)), new MemoryBuffer(0, pageSize, pageSize)) {
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