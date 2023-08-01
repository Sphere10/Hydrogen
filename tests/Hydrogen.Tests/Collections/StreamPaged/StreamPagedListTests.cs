// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamPagedListTests {

	[Test]
	public void Add_1([Values(1, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		Assert.AreEqual(1, list.Count);
	}

	[Test]
	public void Update_1([Values(1, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		list.Update(0, "item2");

		Assert.AreEqual("item2", list[0]);
	}

	[Test]
	public void Add_2([Values(1, 2, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		list.Add("the second item");
		Assert.AreEqual(2, list.Count);
	}

	[Test]
	public void Add_3([Values(1, 2, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		list.AddRange("the second item", "33333333333333333333333333");
		Assert.AreEqual(3, list.Count);
	}

	[Test]
	public void Read_1([Values(1, 2)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		Assert.AreEqual("item1", list[0]);
	}

	[Test]
	public void Read_2([Values(1, 2)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.AddRange("item1", "item2");
		Assert.AreEqual("item1", list[0]);
		Assert.AreEqual("item2", list[1]);
	}

	[Test]
	public void FixedSize_Read_NoHeader() {
		StreamPagedList<int> list;
		using var stream = new MemoryStream();
		list = new StreamPagedList<int>(new PrimitiveSerializer<int>(), stream) { IncludeListHeader = false };
		var added = new[] { 1, 2, 3, 4 };
		list.AddRange(added);
		var read = list.ReadRange(2, 2);
		Assert.AreEqual(added[2..], read);
	}

	[Test]
	public void FixedSize_Update() {
		using var stream = new MemoryStream();
		StreamPagedList<int> list = new StreamPagedList<int>(new PrimitiveSerializer<int>(), stream) { IncludeListHeader = false };
		var added = new[] { 1, 2, 3, 4 };
		list.AddRange(added);
		var read = list.ReadRange(0, added.Length);

		list.UpdateRange(0, read);

		Assert.AreEqual(added, list);
	}

	[Test]
	[TestCase(StreamPagedListType.Static, int.MaxValue)]
	[TestCase(StreamPagedListType.Dynamic, 12)]
	public void ReadItemRaw(StreamPagedListType type, int pageSize) {
		var random = new Random(31337);
		using var stream = new MemoryStream();
		var mappedList = new StreamPagedList<int>(type, new PrimitiveSerializer<int>(), stream, pageSize);

		mappedList.AddRange(random.NextInts(10));
		int read = mappedList.ReadItemBytes(1, 1, 3, out var span);

		Assert.AreEqual(read, span.Length);
		Assert.AreEqual(3, span.Length);
		Assert.AreEqual(BitConverter.GetBytes(mappedList[1])[1..], span.ToArray());
	}

	[Test]
	public void ReadItemRawInvalidIndex() {
		var random = new Random(31337);
		using var stream = new MemoryStream();
		var mappedList = new StreamPagedList<int>(StreamPagedListType.Static, new PrimitiveSerializer<int>(), stream, int.MaxValue);

		mappedList.AddRange(random.NextInts(1));
		Assert.Throws<ArgumentOutOfRangeException>(() => mappedList.ReadItemBytes(5, 1, 1, out var span));

	}

	[Test]
	public void V1_Integration_SimpleRun([Values(1, 2, 3, 5)] int pageSize, [Values] StorageType storage) {

		using (CreateStream(storage, 14, out var stream)) {
			var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

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
	public void V1_IntegrationTests([Values(0, 1, 17, 1000, 2213)] int maxCapacity, [Values] bool includeListHeader) {
		using (var stream = new MemoryStream()) {
			var list = new StreamPagedList<int>(new PrimitiveSerializer<int>(), stream) { IncludeListHeader = includeListHeader };
			AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i), mutateFromEndOnly: true);
		}
	}


	private struct TestStruct {
		public int X;
		public int Y;
		public int Z;
		public byte U;
	}


	public enum StorageType {
		MemoryStream,
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
			case StorageType.MemoryBuffer:
				stream = new ExtendedMemoryStream(new MemoryBuffer());
				break;
			case StorageType.BinaryFile_1Page_1InMem:
				var tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize), 1 * Math.Max(1, estimatedMaxByteSize)));
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_2Page_1InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize / 2), 2 * Math.Max(1, estimatedMaxByteSize / 2)));
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_10Page_5InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize / 10), 5 * Math.Max(1, estimatedMaxByteSize / 10)));
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.TransactionalBinaryFile_1Page_1InMem:
				var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				var fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Math.Max(1, estimatedMaxByteSize), 1 * Math.Max(1, estimatedMaxByteSize), autoLoad: true));
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			case StorageType.TransactionalBinaryFile_2Page_1InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 2), 2 * Math.Max(1, estimatedMaxByteSize / 2), autoLoad: true));
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;

			case StorageType.TransactionalBinaryFile_10Page_5InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 10), 5 * Math.Max(1, estimatedMaxByteSize / 10), autoLoad: true));
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
		}
		return disposables;
	}
}
