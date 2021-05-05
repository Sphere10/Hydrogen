using System.IO;

namespace Sphere10.Framework {
    public abstract class FileSwappedMemoryPage<TItem> : MemoryPageBase<TItem> {
		private readonly string _file;

		protected FileSwappedMemoryPage(int pageSize, IItemSizer<TItem> sizer, IExtendedList<TItem> memoryStore)
			: this(pageSize, Tools.FileSystem.GetTempFileName(false), sizer, memoryStore) {
		}

		protected FileSwappedMemoryPage(int pageSize, string fileStore, IItemSizer<TItem> sizer, IExtendedList<TItem> memoryStore)
			: base(pageSize, sizer, memoryStore) {
			_file = fileStore;
		}

		public override void Dispose() {
			if (File.Exists(_file))
				File.Delete(_file);
		}

		protected override Stream OpenReadStream() {
			if (!File.Exists(_file))
				return Stream.Null;
			return File.OpenRead(_file);
		}

		protected override Stream OpenWriteStream() {
			return File.Open(_file, FileMode.OpenOrCreate, FileAccess.Write);
		}

	}
}