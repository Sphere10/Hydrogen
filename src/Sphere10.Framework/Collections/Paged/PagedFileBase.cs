using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public abstract class PagedFileBase<TItem, TPage> : MemoryPagedListBase<TItem, TPage>
		where TPage : PagedFileBase<TItem, TPage>.FilePageBase {

		protected PagedFileBase(string filename, int pageSize, int maxCacheCapacity, CacheCapacityPolicy cachePolicy, bool readOnly = false)
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

		public abstract class FilePageBase : MemoryPageBase {

			protected FilePageBase(Stream stream, IObjectSizer<TItem> sizer, int pageNumber, int pageSize, IExtendedList<TItem> memoryStore)
				: base(pageSize, sizer, memoryStore) {
				Stream = new BoundedStream(stream, (long)pageNumber * pageSize, (long)(pageNumber + 1) * pageSize - 1);
			}

			protected BoundedStream Stream { get; }

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
				Stream.Seek(Stream.MinPosition, SeekOrigin.Begin);
				return new NonClosingStream(Stream);
			}

			protected override Stream OpenWriteStream() {
				Stream.Seek(Stream.MinPosition, SeekOrigin.Begin);
				return new NonClosingStream(Stream);
			}
		}

	}
}