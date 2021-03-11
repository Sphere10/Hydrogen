//-----------------------------------------------------------------------
// <copyright file="TransactionalBinaryFileTests.cs" company="Sphere 10 Software">
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
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Sphere10.Framework;
using Sphere10.Framework.Collections.StreamMapped;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class StreamMappedListTests {
		
		[Test]
		public void V1_Add_1([Values(1, 111)] int pageSize) {
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

			list.Add("item1");
			Assert.AreEqual(1, list.Count);
		}
		
		[Test]
		public void V1_Update_1([Values(1, 111)] int pageSize) {
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

			list.Add("item1");
			list.Update(0, "item2");
			
			Assert.AreEqual("item2", list[0]);
		}

		[Test]
		public void V1_Add_2([Values(1, 2, 111)] int pageSize) {
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

			list.Add("item1");
			list.Add("the second item");
			Assert.AreEqual(2, list.Count);
		}

		[Test]
		public void V1_Add_3([Values(1, 2, 111)] int pageSize) {
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

			list.Add("item1");
			list.AddRange("the second item", "33333333333333333333333333");
			Assert.AreEqual(3, list.Count);
		}

		[Test]
		public void V1_Read_1([Values(1, 2)] int pageSize) {
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

			list.Add("item1");
			Assert.AreEqual("item1", list[0]);
		}

		[Test]
		public void V1_Read_2([Values(1, 2)] int pageSize) {
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

			list.AddRange("item1", "item2");
			Assert.AreEqual("item1", list[0]);
			Assert.AreEqual("item2", list[1]);
		}

		[Test]
		public void V1_Integration_SimpleRun([Values(1,2,3,5)] int pageSize, [Values] StorageType storage) {

			using (CreateStream(storage, 14, out var stream)) {
				var list = new StreamMappedList<string>(pageSize, new StringSerializer(Encoding.ASCII), stream);

				var len0 = stream.Length;

				list.Add("test");
				Assert.AreEqual(1, list.Count);
				Assert.AreEqual("test", list[0]);

				var len1 = stream.Length;

				list.Add("test2");
				Assert.AreEqual(2, list.Count);
				Assert.AreEqual("test", list[0]);
				Assert.AreEqual("test2", list[1]);

				var len2 = stream.Length;

				list.Add("test33");
				Assert.AreEqual(3, list.Count);
				Assert.AreEqual("test", list[0]);
				Assert.AreEqual("test33", list[2]);
				Assert.AreEqual("test2", list[1]);

				var len3 = stream.Length;

				// Illegal removes
				Assert.That(() => list.RemoveAt(0), Throws.Exception);
				Assert.That(() => list.RemoveAt(1), Throws.Exception);

				// Remove tip
				list.RemoveAt(2);
				Assert.AreEqual(2, list.Count);
				Assert.AreEqual("test", list[0]);
				Assert.AreEqual("test2", list[1]);

				Assert.AreEqual(len2, stream.Length);

				// Illegal remove
				Assert.That(() => list.RemoveAt(0), Throws.Exception);

				// Remove rest
				list.RemoveAt(1);
				Assert.AreEqual(1, list.Count);
				Assert.AreEqual("test", list[0]);

				Assert.AreEqual(len1, stream.Length);

				list.RemoveAt(0);
				Assert.AreEqual(0, list.Count);

				Assert.AreEqual(len0, stream.Length);
			}
		}

		[Test]
		public void V1_Integration_FixedSize()
		{
			var serializer = new IntSerializer();
			int pageSize = 1000;
			int capacity = serializer.FixedSize / pageSize;

			var RNG = new Random(1231);
			var list = new StreamMappedList<int>(pageSize, serializer, new MemoryStream())
			{
				IncludeListHeader = false
			};

			var expected = new List<int>();
			for (var i = 0; i < 100; i++)
			{

				// add a random amount
				var remainingCapacity = capacity - list.Count;
				var newItemsCount = RNG.Next(0, remainingCapacity + 1);
				IEnumerable<int> newItems = RNG.NextInts(newItemsCount);
				list.AddRange(newItems);
				expected.AddRange(newItems);
				Assert.AreEqual(expected, list);

				if (list.Count > 0)
				{
					// update a random amount
					var range = RNG.RandomRange(list.Count);
					newItems = RNG.NextInts(range.End - range.Start + 1);
					list.UpdateRange(range.Start, newItems);
					expected.UpdateRangeSequentially(range.Start, newItems);
					Assert.AreEqual(expected, list);

					// shuffle a random amount
					range = RNG.RandomRange(list.Count);
					newItems = list.ReadRange(range.Start, range.End - range.Start + 1);
					var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);
					range = RNG.RandomSegment(list.Count, newItems.Count());
					expected.UpdateRangeSequentially(range.Start, expectedNewItems);
					list.UpdateRange(range.Start, newItems);

					Assert.AreEqual(expected.Count, list.Count);
					Assert.AreEqual(expected, list);

					// remove a random amount
					range = RNG.RandomRange(list.Count);
					list.RemoveRange(range.Start, range.End - range.Start + 1);
					expected.RemoveRange(range.Start, range.End - range.Start + 1);
					Assert.AreEqual(expected, list);
				}
			}
		}
		
		[Test]
		public void V1_Integration_FixedSizeWithHeader()
		{
			var serializer = new IntSerializer();
			int pageSize = 1000;
			int capacity = serializer.FixedSize / pageSize;

			var RNG = new Random(1231);
			var list = new StreamMappedList<int>(pageSize, serializer, new MemoryStream())
			{
				IncludeListHeader = true
			};

			var expected = new List<int>();
			for (var i = 0; i < 100; i++)
			{
				// add a random amount
				var remainingCapacity = capacity - list.Count;
				var newItemsCount = RNG.Next(0, remainingCapacity + 1);
				IEnumerable<int> newItems = RNG.NextInts(newItemsCount);
				list.AddRange(newItems);
				expected.AddRange(newItems);
				Assert.AreEqual(expected, list);

				if (list.Count > 0)
				{
					// update a random amount
					var range = RNG.RandomRange(list.Count);
					newItems = RNG.NextInts(range.End - range.Start + 1);
					list.UpdateRange(range.Start, newItems);
					expected.UpdateRangeSequentially(range.Start, newItems);
					Assert.AreEqual(expected, list);

					// shuffle a random amount
					range = RNG.RandomRange(list.Count);
					newItems = list.ReadRange(range.Start, range.End - range.Start + 1);
					var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);
					range = RNG.RandomSegment(list.Count, newItems.Count());
					expected.UpdateRangeSequentially(range.Start, expectedNewItems);
					list.UpdateRange(range.Start, newItems);

					Assert.AreEqual(expected.Count, list.Count);
					Assert.AreEqual(expected, list);

					// remove a random amount
					range = RNG.RandomRange(list.Count);
					list.RemoveRange(range.Start, range.End - range.Start + 1);
					expected.RemoveRange(range.Start, range.End - range.Start + 1);
					Assert.AreEqual(expected, list);
				}
			}
		}


		[Test]
		public void V1_IncludeListHeaderThrowsAfterInit()
		{
			var stream = new MemoryStream();
			var list = new StreamMappedList<string>(100, new StringSerializer(Encoding.ASCII), stream)
			{
				IncludeListHeader = false
			};

			Assert.DoesNotThrow(() => list.IncludeListHeader = true);
			list.Add("baz");

			Assert.Throws<InvalidOperationException>(() => list.IncludeListHeader = true);
		}

		public enum StorageType {
			MemoryStream,
			List,
			ExtendedList,
			MemoryBuffer,
			BinaryFile_1Page_1InMem,
			BinaryFile_2Page_1InMem,
			BinaryFile_10Page_5InMem,
			TransactionalBinaryFile_1Page_1InMem,
			TransactionalBinaryFile_2Page_1InMem,
			TransactionalBinaryFile_10Page_5InMem
		}

		private IDisposable CreateStream(StorageType storageType, int estimatedMaxByteSize, out Stream stream) {
			var disposables = new Disposables();

			switch (storageType) {
				case StorageType.MemoryStream:
					stream = new MemoryStream();
					break;
				case StorageType.List:
					stream = new ExtendedMemoryStream(new ExtendedListAdapter<byte>(new List<byte>()));
					break;
				case StorageType.ExtendedList:
					stream = new ExtendedMemoryStream(new ExtendedList<byte>());
					break;
				case StorageType.MemoryBuffer:
					stream = new ExtendedMemoryStream(new MemoryBuffer());
					break;
				case StorageType.BinaryFile_1Page_1InMem:
					var tmpFile = Tools.FileSystem.GetTempFileName(false);
					stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize), 1));
					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
					break;
				case StorageType.BinaryFile_2Page_1InMem:
					tmpFile = Tools.FileSystem.GetTempFileName(false);
					stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize / 2), 2));
					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
					break;
				case StorageType.BinaryFile_10Page_5InMem:
					tmpFile = Tools.FileSystem.GetTempFileName(false);
					stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize / 10), 5));
					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
					break;
				case StorageType.TransactionalBinaryFile_1Page_1InMem:
					var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
					var fileName = Path.Combine(baseDir, "File.dat");
					stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Guid.NewGuid(), Math.Max(1, estimatedMaxByteSize), 1));
					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
					break;
				case StorageType.TransactionalBinaryFile_2Page_1InMem:
					baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
					fileName = Path.Combine(baseDir, "File.dat");
					stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Guid.NewGuid(), Math.Max(1, estimatedMaxByteSize / 2), 2));
					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
					break;

				case StorageType.TransactionalBinaryFile_10Page_5InMem:
					baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
					fileName = Path.Combine(baseDir, "File.dat");
					stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Guid.NewGuid(), Math.Max(1, estimatedMaxByteSize / 10), 5));
					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
			}
			return disposables;
		}
	}
}
