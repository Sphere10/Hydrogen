using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A collection of bytes that is mapped over a file and accessed via intelligent memory-mapped paging.
	/// <remarks>
	/// By varying the pageSize and in inMemoryPages arguments the performance of this list can by tuned to meet the specific use case.
	/// In cases where appending is more demanded, the page sizes should be larger so they are swapped less often.
	/// In cases where random accessing the list, the page sizes should be smaller so they can be loaded faster.
	/// In general, the more pages allowed in memory the less frequently they will be swapped to storage.
	/// </remarks>
	/// </summary>
	public sealed class BinaryFile : FileMappedList<byte, BinaryFile.Page> {
		
		public BinaryFile(string filename, int pageSize, int maxOpenPages, bool readOnly = false) 
			: base(filename, pageSize, maxOpenPages, CacheCapacityPolicy.CapacityIsMaxOpenPages, readOnly) {
		}

		protected override Page[] LoadPages() {
			var fileLength = (int)Stream.Length;
			if (fileLength == 0)
				return new Page[0];

			var numPages = (int)Math.Ceiling(fileLength / (double)PageSize);
			var lastPageSize = (int)(fileLength - (numPages - 1) * PageSize);
			return
				Enumerable
				.Range(0, numPages - 1)
				.Select((x, i) =>
				   new Page(Stream, i, PageSize) {
					   Number = i,
					   StartPosition = i * sizeof(byte) * PageSize,
					   StartIndex = i * PageSize,
					   EndIndex = (i + 1) * PageSize - 1,
					   EndPosition = (i+1) * sizeof(byte) * PageSize - 1,
					   Count = PageSize,
					   Size = PageSize,
					   State = PageState.Unloaded,
				   }
				).Concat(
					// Last page
					new Page(Stream, numPages - 1, PageSize) { 
						Number = numPages - 1,
						StartPosition = (numPages - 1) * sizeof(byte) * PageSize,
						StartIndex = (numPages - 1) * PageSize,
						EndIndex = fileLength - 1,
						EndPosition = fileLength - 1,
						Count = lastPageSize,
						Size = lastPageSize,
						State = PageState.Unloaded,
					}
				)
				.ToArray();
		}

		protected override void OnPageSaving(Page page) {
			// Always ensure file-stream is long enough to save this page
			base.OnPageSaving(page);
			var requiredLen = page.EndIndex + 1;
			if (Stream.Length < requiredLen)
				Stream.SetLength(requiredLen);
		}

		protected override void OnPageSaved(Page page) {
			// Always ensure file-stream is never larger than last page
			base.OnPageSaved(page);
			if (page.Number == _pages.Count - 1) {
				TruncateFile();				
			}
		}

		protected override Page NewPageInstance(int pageNumber) {
			return new Page(Stream, pageNumber, PageSize);
		}

		protected override void OnPageDeleted(Page page) {
			base.OnPageDeleted(page);
			if (Disposing)
				return;
			if (!IsReadOnly && (page.Number == 0 || page.Number == _pages.Count)) {
				TruncateFile();
			}
		}

		public class Page : FilePageBase<byte> {

			public Page(Stream stream, int pageNumber, int pageSize)
				: base(stream, new ConstantObjectSizer<byte>(sizeof(byte)), pageNumber, pageSize, new MemoryBuffer(0, pageSize, pageSize)) {
			}

			protected override void SaveInternal(IEnumerable<byte> items, Stream stream) {
				// Use byte streaming for perf
				using (var writer = new BinaryWriter(stream)) {
					var itemsArr = items as byte[] ?? items.ToArray();
					writer.Write(itemsArr);
				}
			}

			protected override IEnumerable<byte> LoadInternal(Stream stream) {
				// Use byte streaming for perf
				var buff = new byte[stream.Length];
				var bytesRead = stream.Read(buff, 0, (int)stream.Length);
				if (bytesRead != buff.Length)
					Array.Resize(ref buff, bytesRead);
				return buff;
			}
		}
	}
}