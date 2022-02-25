using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	/// <summary>
	/// A list whose items are mapped onto pages of a file and file pages are cached in memory.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public abstract class FilePagedListBase<TItem> : MemoryPagedListBase<TItem>, IFilePagedList<TItem> {

		protected FilePagedListBase(string filename, int pageSize, long maxMemory, bool readOnly = false)
			: base(pageSize, maxMemory) {
			IsReadOnly = readOnly;
			var fileExists = File.Exists(filename);
			if (readOnly) {
				if (!fileExists)
					throw new FileNotFoundException(filename);
				FlushOnDispose = false;
				Stream = File.Open(filename, FileMode.Open, FileAccess.Read);
			} else {
				FlushOnDispose = true;
				Stream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
			RequiresLoad = fileExists && Tools.FileSystem.GetFileSize(filename) > 0;
		}

		public override bool IsReadOnly { get; }

		public string Path => Tools.FileSystem.GetCaseCorrectFilePath(Stream?.Name);

		internal FileStream Stream { get; }
	
		public override int Count {
			get {
				CheckRequiresLoad();
				return base.Count;
			}
		}

		public override void Flush() {
			base.Flush();
			if (!IsReadOnly)
				Stream.Flush(true);
		}

		public override void Dispose() {
			base.Dispose();
			Stream.Dispose();
		}

		// Force sub-class to implement
		protected abstract override IPage<TItem>[] LoadPages();

		protected override void OnPageCreated(IPage<TItem> page) {
			base.OnPageCreated(page);
			var filePage = (IFilePage<TItem>)page;
			if (filePage.Number == 0) {
				filePage.StartPosition = 0;
				filePage.EndPosition = -1;
			} else {
				var lastPage = (IFilePage<TItem>)InternalPages[filePage.Number - 1];
				filePage.StartPosition = lastPage.EndPosition + 1;
				filePage.EndPosition = lastPage.EndPosition;
			}
		}

		protected virtual void TruncateFile() {
			// This ensures file is exact size of it's content, excluding any pre-allocated append buffer
			var pages = InternalPages;
			var streamLength = pages.Count > 0 ? ((IFilePage<TItem>)pages.Last()).EndPosition + 1 : 0;
			if (Stream.Length != streamLength) {
				Stream.SetLength(streamLength);
			}
		}

	}
}