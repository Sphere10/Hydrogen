//-----------------------------------------------------------------------
// <copyright file="LargebinaryFileTests.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class BufferTests {
		[Test]
		public void SinglePage(
			[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)] StorageType storageType,
			[Values(1, 2, 10, 57, 173)] int pageSize,
			[Values(1, 2, int.MaxValue)] int maxOpenPages) {
			using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages, out var buffer)) {
				buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));
				// Check page
				Assert.AreEqual(1, buffer.Pages.Count());
				Assert.AreEqual(0, buffer.Pages[0].Number);
				Assert.AreEqual(pageSize, buffer.Pages[0].MaxSize);
				Assert.AreEqual(0, buffer.Pages[0].StartIndex);
				Assert.AreEqual(pageSize, buffer.Pages[0].Count);
				Assert.AreEqual(pageSize-1, buffer.Pages[0].EndIndex);
				Assert.AreEqual(pageSize, buffer.Pages[0].Size);
				Assert.IsTrue(buffer.Pages[0].Dirty);

				// Check value
				Assert.AreEqual(10, buffer[0]);
				Assert.AreEqual(pageSize, buffer.Count);
			
			}
		}

		[Test]
		public void TwoPages(
			[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)] StorageType storageType,
			[Values(1, 2, 10, 57, 173)] int pageSize) {

			using (CreateMemPagedBuffer(storageType, pageSize, 1, out var buffer)) {
				buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));

				// Check Page 1
				Assert.AreEqual(1, buffer.Pages.Count());
				Assert.AreEqual(PageState.Loaded, buffer.Pages[0].State);
				Assert.AreEqual(0, buffer.Pages[0].Number);
				Assert.AreEqual(pageSize, buffer.Pages[0].MaxSize);
				Assert.AreEqual(0, buffer.Pages[0].StartIndex);
				Assert.AreEqual(pageSize, buffer.Pages[0].Count);
				Assert.AreEqual(pageSize-1, buffer.Pages[0].EndIndex);
				Assert.AreEqual(pageSize, buffer.Pages[0].Size);
				Assert.IsTrue(buffer.Pages[0].Dirty);

				// Add new page
				buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 20));

				// Check pages 1 & 2
				Assert.AreEqual(2, buffer.Pages.Count());
				Assert.AreEqual(PageState.Unloaded, buffer.Pages[0].State);
				Assert.AreEqual(0, buffer.Pages[0].Number);
				Assert.AreEqual(pageSize, buffer.Pages[0].MaxSize);
				Assert.AreEqual(0, buffer.Pages[0].StartIndex);
				Assert.AreEqual(pageSize, buffer.Pages[0].Count);
				Assert.AreEqual(pageSize-1, buffer.Pages[0].EndIndex);
				Assert.AreEqual(pageSize, buffer.Pages[0].Size);


				Assert.AreEqual(PageState.Loaded, buffer.Pages[1].State);
				Assert.AreEqual(1, buffer.Pages[1].Number);
				Assert.AreEqual(pageSize, buffer.Pages[1].MaxSize);
				Assert.AreEqual(pageSize, buffer.Pages[1].StartIndex);
				Assert.AreEqual(pageSize, buffer.Pages[1].Count);
				Assert.AreEqual(pageSize*2-1, buffer.Pages[1].EndIndex);
				Assert.AreEqual(pageSize, buffer.Pages[1].Size);
				Assert.IsTrue(buffer.Pages[1].Dirty);

				// Check values
				Assert.AreEqual(10, buffer[0]);
				Assert.AreEqual(20, buffer[pageSize]);
				
			}
		}
		
		[Test]
		public void RemoveAll(
			[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)] StorageType storageType,
			[Values(1, 2, 10, 57, 173)] int pageSize,
			[Values(1, 2, int.MaxValue)] int maxOpenPages) {
			using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages, out var buffer)) {
				buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));
				buffer.RemoveRange(0, buffer.Count);
				Assert.AreEqual(0, buffer.Pages.Count);
				Assert.AreEqual(0, buffer.Count);
				Assert.AreEqual(Enumerable.Empty<byte>(), buffer);
			}
		}

		[Test]
		public void RemoveAllExcept1(
			[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)] StorageType storageType,
			[Values(2, 10, 57, 173)] int pageSize,
			[Values(1, 2, int.MaxValue)] int maxOpenPages) {
			using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages, out var buffer)) {
				buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));
				buffer.RemoveRange(1, buffer.Count-1);
				Assert.AreEqual(1, buffer.Pages.Count);
				Assert.AreEqual(1, buffer.Count);
				Assert.AreEqual(new byte[] { 10 }, buffer);
			}
		}

		[Test]
		public void Rewind(
			[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)] StorageType storageType,
			[Values(1, 2, 111)] int pageSize,
			[Values(1, 2, int.MaxValue)] int maxOpenPages) {
			var expected = new byte[] { 127, 17, 18, 19 };
			using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages, out var buffer)) {
				buffer.AddRange<byte>(127, 16, 15, 14, 13);
				buffer.RemoveRange(1, 4);
				Assert.AreEqual(1, buffer.Count);
				Assert.AreEqual(127, buffer[0]);
				buffer.AddRange<byte>(17, 18, 19 );
				Assert.AreEqual(4, buffer.Count);
				Assert.AreEqual(127, buffer[0]);
				Assert.AreEqual(17, buffer[1]);
				Assert.AreEqual(18, buffer[2]);
				Assert.AreEqual(19, buffer[3]);
				Assert.AreEqual(expected, buffer);
			}
		}

		[Test]
		public void IntegrationTests([Values] StorageType storageType, [Values(1, 10, 57, 173, 1111)] int pageSize, [Values(1,2,100)] int maxOpenPages) {
			var expected = new List<byte>();
			var RNG = new Random(31337);
			var maxCapacity = pageSize * maxOpenPages*2;
			using (CreateBuffer(storageType, pageSize, maxOpenPages, out var buffer)) {
				for (var i = 0; i < 100; i++) {

					// add a random amount
					var remainingCapacity = maxCapacity - buffer.Count;
					var newItemsCount = RNG.Next(0, remainingCapacity + 1);
					byte[] newItems = RNG.NextBytes(newItemsCount);
					buffer.AddRange(newItems.AsSpan());
					expected.AddRange(newItems);
					Assert.AreEqual(expected, buffer);

					// update a random amount
					if (buffer.Count > 0) {
						var range = RNG.RandomRange(buffer.Count);
						newItems = RNG.NextBytes(range.End - range.Start + 1);
						expected.UpdateRangeSequentially(range.Start, newItems);
						buffer.UpdateRange(range.Start, newItems.AsSpan());
						Assert.AreEqual(expected, buffer);

						// shuffle a random amount
						range = RNG.RandomRange(buffer.Count);
						newItems = buffer.ReadSpan(range.Start, range.End - range.Start + 1).ToArray();
						var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);

						range = RNG.RandomSegment(buffer.Count, newItems.Count());
						expected.UpdateRangeSequentially(range.Start, expectedNewItems);
						buffer.UpdateRange(range.Start, newItems.AsSpan());

						Assert.AreEqual(expected.Count, buffer.Count);
						Assert.AreEqual(expected, buffer);

						// remove a random amount (FROM END OF LIST)
						range = new ValueRange<int>(RNG.Next(0, buffer.Count), buffer.Count - 1);
						buffer.RemoveRange(range.Start, range.End - range.Start + 1);
						expected.RemoveRange(range.Start, range.End - range.Start + 1);

						var buff = buffer.ToArray();
						Assert.AreEqual(expected, buffer);
					}
				}
			}
		}


		public enum StorageType {
			MemoryBuffer,
			MemoryPagedBuffer,
			BinaryFile,
			TransactionalBinaryFile,
		}

		private IDisposable CreateBuffer(StorageType storageType, int pageSize, int maxOpenPages, out IBuffer buffer) {
			switch (storageType) {
				case StorageType.MemoryBuffer:
					buffer = new MemoryBuffer(0, pageSize);
					return new Disposables();
			}
			
			var result = CreateMemPagedBuffer(storageType, pageSize, maxOpenPages, out var memBuffer);
			buffer = memBuffer;
			return result;
		}

		private IDisposable CreateMemPagedBuffer(StorageType storageType, int pageSize, int maxOpenPages, out IMemoryPagedBuffer buffer) {
			var disposables = new Disposables();

			switch (storageType) {
				case StorageType.MemoryBuffer:
					throw new InvalidOperationException();
				case StorageType.MemoryPagedBuffer:
					buffer = new MemoryPagedBuffer(pageSize, maxOpenPages);
					break;
				case StorageType.BinaryFile:
					var tmpFile = Tools.FileSystem.GetTempFileName(false);
					buffer = new FileMappedBuffer(tmpFile, pageSize, maxOpenPages);
					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
					break;
				case StorageType.TransactionalBinaryFile:
					var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
					var fileName = Path.Combine(baseDir, "File.dat");
					buffer = new TransactionalFileMappedBuffer(fileName, baseDir, Guid.NewGuid(), pageSize, maxOpenPages);
					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
			}
			return disposables;
		}

	}
}
