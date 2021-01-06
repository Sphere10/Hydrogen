using System.IO;

namespace Sphere10.Framework {
    public abstract class FileSwappedMemoryPage<TItem> : MemoryPageBase<TItem> {
		private readonly string _fileStore;

		protected FileSwappedMemoryPage(int pageSize, IObjectSizer<TItem> sizer, IExtendedList<TItem> memoryStore)
			: this(pageSize, Tools.FileSystem.GetTempFileName(false), sizer, memoryStore) {
		}

		protected FileSwappedMemoryPage(int pageSize, string fileStore, IObjectSizer<TItem> sizer, IExtendedList<TItem> memoryStore)
			: base(pageSize, sizer, memoryStore) {
			_fileStore = fileStore;
		}

		public override void Dispose() {
			if (File.Exists(_fileStore))
				File.Delete(_fileStore);
		}

		protected override Stream OpenReadStream() {
			if (!File.Exists(_fileStore))
				return Stream.Null;
			return File.OpenRead(_fileStore);
		}

		protected override Stream OpenWriteStream() {
			return File.Open(_fileStore, FileMode.OpenOrCreate, FileAccess.Write);
		}
	}
}