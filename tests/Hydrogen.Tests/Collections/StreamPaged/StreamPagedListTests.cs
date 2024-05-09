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
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamPagedListTests {

	[Test]
	public void EmptyString_Bug() {
		using var stream = new MemoryStream();
		using var streams = new ClusteredStreams(stream, reservedStreams: 1, autoLoad: true);
		using var clusteredStream = streams.OpenWrite(0);
		streams.AddBytes(new byte[256]);
		var serializer = new StringSerializer(Encoding.UTF8).AsReferenceSerializer().AsConstantSize(256);
		var size = serializer.ConstantSize;
		var list = new StreamPagedList<string>(serializer, clusteredStream, includeListHeader: false, autoLoad: true);
		list.Add("");
		ClassicAssert.AreEqual(1, list.Count);
		var item = list.Read(0);
		Assert.That(item, Is.EqualTo(string.Empty));
	}

	[Test]
	public void Add_1([Values(1, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		ClassicAssert.AreEqual(1, list.Count);
	}

	[Test]
	public void Update_1([Values(1, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		list.Update(0, "item2");

		ClassicAssert.AreEqual("item2", list[0]);
	}

	[Test]
	public void Add_2([Values(1, 2, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		list.Add("the second item");
		ClassicAssert.AreEqual(2, list.Count);
	}

	[Test]
	public void Add_3([Values(1, 2, 111)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		list.AddRange("the second item", "33333333333333333333333333");
		ClassicAssert.AreEqual(3, list.Count);
	}

	[Test]
	public void Read_1([Values(1, 2)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.Add("item1");
		ClassicAssert.AreEqual("item1", list[0]);
	}

	[Test]
	public void Read_2([Values(1, 2)] int pageSize) {
		using var stream = new MemoryStream();
		var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize);

		list.AddRange("item1", "item2");
		ClassicAssert.AreEqual("item1", list[0]);
		ClassicAssert.AreEqual("item2", list[1]);
	}

	[Test]
	public void FixedSize_Read_NoHeader() {
		StreamPagedList<int> list;
		using var stream = new MemoryStream();
		list = new StreamPagedList<int>(new PrimitiveSerializer<int>(), stream, includeListHeader: false);
		var added = new[] { 1, 2, 3, 4 };
		list.AddRange(added);
		var read = list.ReadRange(2, 2);
		ClassicAssert.AreEqual(added[2..], read);
	}

	[Test]
	public void FixedSize_Update() {
		using var stream = new MemoryStream();
		StreamPagedList<int> list = new StreamPagedList<int>(new PrimitiveSerializer<int>(), stream, includeListHeader: false);
		var added = new[] { 1, 2, 3, 4 };
		list.AddRange(added);
		var read = list.ReadRange(0, added.Length);

		list.UpdateRange(0, read);

		ClassicAssert.AreEqual(added, list);
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

		ClassicAssert.AreEqual(read, span.Length);
		ClassicAssert.AreEqual(3, span.Length);
		ClassicAssert.AreEqual(BitConverter.GetBytes(mappedList[1])[1..], span.ToArray());
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
			var list = new StreamPagedList<string>(new StringSerializer(Encoding.UTF8), stream, pageSize, autoLoad:true);

			var len0 = stream.Length;

			list.Add("test");
			ClassicAssert.AreEqual(1, list.Count);
			ClassicAssert.AreEqual("test", list[0]);

			var len1 = stream.Length;

			list.Add("test2");
			ClassicAssert.AreEqual(2, list.Count);
			ClassicAssert.AreEqual("test", list[0]);
			ClassicAssert.AreEqual("test2", list[1]);

			var len2 = stream.Length;

			list.Add("test33");
			ClassicAssert.AreEqual(3, list.Count);
			ClassicAssert.AreEqual("test", list[0]);
			ClassicAssert.AreEqual("test33", list[2]);
			ClassicAssert.AreEqual("test2", list[1]);

			var len3 = stream.Length;

			// Illegal removes
			Assert.That(() => list.RemoveAt(0), Throws.Exception);
			Assert.That(() => list.RemoveAt(1), Throws.Exception);

			// Remove tip
			list.RemoveAt(2);
			ClassicAssert.AreEqual(2, list.Count);
			ClassicAssert.AreEqual("test", list[0]);
			ClassicAssert.AreEqual("test2", list[1]);

			ClassicAssert.AreEqual(len2, stream.Length);

			// Illegal remove
			Assert.That(() => list.RemoveAt(0), Throws.Exception);

			// Remove rest
			list.RemoveAt(1);
			ClassicAssert.AreEqual(1, list.Count);
			ClassicAssert.AreEqual("test", list[0]);

			ClassicAssert.AreEqual(len1, stream.Length);

			list.RemoveAt(0);
			ClassicAssert.AreEqual(0, list.Count);

			ClassicAssert.AreEqual(len0, stream.Length);
		}
	}

	[Test]
	public void V1_IntegrationTests([Values(0, 1, 17, 1000, 2213)] int maxCapacity, [Values] bool includeListHeader) {
		using (var stream = new MemoryStream()) {
			var list = new StreamPagedList<int>(new PrimitiveSerializer<int>(), stream, includeListHeader: includeListHeader);
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
				stream = new ExtendedMemoryStream(new MemoryBuffer(), true);
				break;
			case StorageType.BinaryFile_1Page_1InMem:
				var tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(PagedFileDescriptor.From( tmpFile, Math.Max(1, estimatedMaxByteSize), 1 * Math.Max(1, estimatedMaxByteSize))), true);
				disposables.Add(stream);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_2Page_1InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, Math.Max(1, estimatedMaxByteSize / 2), 2 * Math.Max(1, estimatedMaxByteSize / 2))), true);
				disposables.Add(stream);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_10Page_5InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, Math.Max(1, estimatedMaxByteSize / 10), 5 * Math.Max(1, estimatedMaxByteSize / 10))), true);
				disposables.Add(stream);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.TransactionalBinaryFile_1Page_1InMem:
				var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				var fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, Math.Max(1, estimatedMaxByteSize), 1 * Math.Max(1, estimatedMaxByteSize))), true);
				disposables.Add(stream);
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			case StorageType.TransactionalBinaryFile_2Page_1InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 2), 2 * Math.Max(1, estimatedMaxByteSize / 2))), true);
				disposables.Add(stream);
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;

			case StorageType.TransactionalBinaryFile_10Page_5InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 10), 5 * Math.Max(1, estimatedMaxByteSize / 10))), true);
				disposables.Add(stream);
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
		}
		return disposables;
	}
}
