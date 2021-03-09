using System.IO;

namespace Sphere10.Framework {
    public abstract class FilePageBase<TItem> : MemoryPageBase<TItem>, IFilePage<TItem> {

		protected FilePageBase(Stream stream, IObjectSizer<TItem> sizer, int pageNumber, int pageSize, IExtendedList<TItem> memoryStore)
			: base(pageSize, sizer, memoryStore) {
			Stream = new BoundedStream(stream, (long)pageNumber * pageSize, (long)(pageNumber + 1) * pageSize - 1);
		}

		internal BoundedStream Stream { get; }

		public long StartPosition { get; set; }

		public long EndPosition { get; set; }

		public override void Dispose() {
			// Stream is managed by client		
		}

		protected override int AppendInternal(TItem[] items, out int newItemsSpace) {
			var appendCount = base.AppendInternal(items, out newItemsSpace);
			EndPosition += newItemsSpace;
			return appendCount;
		}

		protected override void UpdateInternal(int index, TItem[] items, out int oldItemsSpace, out int newItemsSpace) {
			base.UpdateInternal(index, items, out oldItemsSpace, out newItemsSpace);
			var spaceDiff = newItemsSpace - oldItemsSpace;
			EndPosition += spaceDiff;
		}

		protected override void EraseFromEndInternal(int count, out int oldItemsSpace) {
			base.EraseFromEndInternal(count, out oldItemsSpace);
			EndPosition -= oldItemsSpace;
		}

		protected override Stream OpenReadStream() {
			Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
			return new NonClosingStream(Stream);
		}

		protected override Stream OpenWriteStream() {
			Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
			return new NonClosingStream(Stream);
		}
	}
}