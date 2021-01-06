using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	/// <summary>
	/// A list whose items are mapped onto pages of a file and file pages are cached in memory.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TPage"></typeparam>
    public abstract class FileMappedList<TItem, TPage> : MemoryPagedList<TItem, TPage>, IFilePagedList<TItem, TPage>
		where TPage : IFilePage<TItem> {

		protected FileMappedList(string filename, int pageSize, int maxCacheCapacity, CacheCapacityPolicy cachePolicy, bool readOnly = false)
			: base(pageSize, maxCacheCapacity, cachePolicy) {
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
			RequiresLoad = fileExists;
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

		protected abstract override TPage[] LoadPages();

		protected override void OnPageCreated(TPage page) {
			base.OnPageCreated(page);
			if (page.Number == 0) {
				page.StartPosition = 0;
				page.EndPosition = -1;
			} else {
				var lastPage = _pages[page.Number - 1];
				page.StartPosition = lastPage.EndPosition + 1;
				page.EndPosition = lastPage.EndPosition;
			}
		}

		protected virtual void TruncateFile() {
			// This ensures file is exact size of it's content, excluding any pre-allocated append buffer
			var pages = _pages;
			var streamLength = pages.Count > 0 ? pages.Last().EndPosition + 1 : 0;
			if (Stream.Length != streamLength) {
				Stream.SetLength(streamLength);
			}
		}

	

	}
}