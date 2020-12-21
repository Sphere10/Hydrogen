//-----------------------------------------------------------------------
// <copyright file="PageManager.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Sphere10.Framework {

	/// <summary>
	/// A collection of bytes that is mapped over a file and accessed via intelligent memory-mapped paging and mutated
	/// in an ACID transactional manner.
	/// <remarks>
	/// By varying the pageSize and in inMemoryPages arguments the performance of this list can by tuned to meet the specific use case.
	/// In cases where appending is more demanded, the page sizes should be larger so they are swapped less often.
	/// In cases where random accessing the list, the page sizes should be smaller so they can be loaded faster.
	/// In general, the more pages allowed in memory the less frequently they will be swapped to storage.
	/// </remarks>
	/// </summary>
	public sealed class TransactionalBinaryFile : TransactionalFileBase<byte, TransactionalBinaryFile.TransactionalPage> {
		
		public TransactionalBinaryFile(string filename, int pageSize, int inMemoryPages, bool readOnly = false)
			: this(filename, Guid.NewGuid(), pageSize, inMemoryPages, readOnly) {
		}

		public TransactionalBinaryFile(string filename, Guid fileID, int pageSize, int inMemoryPages, bool readOnly = false)
			: this(filename, System.IO.Path.GetDirectoryName(filename), fileID, pageSize, inMemoryPages, readOnly) {
		}

		public TransactionalBinaryFile(string filename, string uncomittedPageFileDir, Guid fileID, int pageSize, int inMemoryPages, bool readOnly = false)
			: base(filename, uncomittedPageFileDir, fileID, pageSize, inMemoryPages, CacheCapacityPolicy.CapacityIsMaxOpenPages, readOnly) {
		}

		protected override TransactionalPage[] LoadPages() {
			var lowestDeletedPageNumber = PageMarkerRepo.LowestDeletedPageNumber ?? int.MaxValue;
			var highestChangedPageNumber = PageMarkerRepo.HighestChangedPageNumber ?? int.MinValue;
			var comittedPageCount = GetComittedPageCount();
			var logicalPageCount = Math.Min(lowestDeletedPageNumber, Math.Max(highestChangedPageNumber + 1, comittedPageCount));
			var lastLogicalPageNumber = logicalPageCount - 1;

			if (lastLogicalPageNumber < 0)
				return new TransactionalPage[0];

			var lastPageLength = 0;
			if (PageMarkerRepo.Contains(PageMarkerType.UncommittedPage, lastLogicalPageNumber)) {
				lastPageLength = (int)Tools.FileSystem.GetFileSize(PageMarkerRepo.GetMarkerFilename(highestChangedPageNumber, PageMarkerType.UncommittedPage));
			} else {
				var remainder = (int)Stream.Length % PageSize;
				lastPageLength = remainder == 0 ? PageSize : remainder;
			}

			var logicalFileLength = (logicalPageCount - 1) * PageSize + lastPageLength;
			if (logicalFileLength == 0)
				return new TransactionalPage[0];

			return
				Enumerable
					.Range(0, logicalPageCount - 1)
					.Select((x, i) =>
						new TransactionalPage(base.Stream, PageMarkerRepo.GetMarkerFilename(i, PageMarkerType.UncommittedPage), i, PageSize) {
							Number = i,
							StartPosition = i * sizeof(byte) * PageSize,
							StartIndex = i * PageSize,
							EndIndex = (i + 1) * PageSize - 1,
							EndPosition = (i + 1) * sizeof(byte) * PageSize - 1,
							Count = PageSize,
							Size = PageSize,
							State = PageState.Unloaded,
						}
					).Concat(
						// Last page
						new TransactionalPage(base.Stream, PageMarkerRepo.GetMarkerFilename(logicalPageCount - 1, PageMarkerType.UncommittedPage), logicalPageCount - 1, PageSize) {
							Number = logicalPageCount - 1,
							StartPosition = (logicalPageCount - 1) * sizeof(byte) * PageSize,
							StartIndex = (logicalPageCount - 1) * PageSize,
							EndIndex = logicalFileLength - 1,
							EndPosition = logicalFileLength - 1,
							Count = lastPageLength,
							Size = lastPageLength,
							State = PageState.Unloaded,
						}
					)
					.ToArray();
		}

		protected override int GetComittedPageCount() {
			return (int)Math.Ceiling(Stream.Length / (double)PageSize);
		}

		protected override TransactionalPage NewPageInstance(int pageNumber) {
			return new TransactionalPage(
				Stream,
				PageMarkerRepo.GetMarkerFilename(pageNumber, PageMarkerType.UncommittedPage),
				pageNumber,
				PageSize
			);
		}

		public class TransactionalPage : TransactionalPageBase {

			public TransactionalPage(FileStream stream, string uncommittedPageFileName, int pageNumber, int pageSize)
				: base(stream, new ConstantObjectSizer<byte>(sizeof(byte)), uncommittedPageFileName, pageNumber, pageSize, new MemoryBuffer(0, pageSize, pageSize)) {
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
