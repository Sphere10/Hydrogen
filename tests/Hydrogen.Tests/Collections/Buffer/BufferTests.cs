// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;
using Hydrogen.Collections;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class BufferTests {
	[Test]
	public void SinglePage(
		[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)]
		StorageType storageType,
		[Values(1, 2, 10, 57, 173)] int pageSize,
		[Values(1, 2, int.MaxValue)] int maxOpenPages) {
		using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages * (long)pageSize, out var buffer)) {
			buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));
			// Check page
			Assert.AreEqual(1, buffer.Pages.Count());
			Assert.AreEqual(0, buffer.Pages[0].Number);
			Assert.AreEqual(pageSize, buffer.Pages[0].MaxSize);
			Assert.AreEqual(0, buffer.Pages[0].StartIndex);
			Assert.AreEqual(pageSize, buffer.Pages[0].Count);
			Assert.AreEqual(pageSize - 1, buffer.Pages[0].EndIndex);
			Assert.AreEqual(pageSize, buffer.Pages[0].Size);
			Assert.IsTrue(buffer.Pages[0].Dirty);

			// Check value
			Assert.AreEqual(10, buffer[0]);
			Assert.AreEqual(pageSize, buffer.Count);

		}
	}

	[Test]
	public void TwoPages(
		[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)]
		StorageType storageType,
		[Values(1, 2, 10, 57, 173)] int pageSize) {

		using (CreateMemPagedBuffer(storageType, pageSize, 1 * pageSize, out var buffer)) {
			buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));

			// Check Page 1
			Assert.AreEqual(1, buffer.Pages.Count());
			Assert.AreEqual(PageState.Loaded, buffer.Pages[0].State);
			Assert.AreEqual(0, buffer.Pages[0].Number);
			Assert.AreEqual(pageSize, buffer.Pages[0].MaxSize);
			Assert.AreEqual(0, buffer.Pages[0].StartIndex);
			Assert.AreEqual(pageSize, buffer.Pages[0].Count);
			Assert.AreEqual(pageSize - 1, buffer.Pages[0].EndIndex);
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
			Assert.AreEqual(pageSize - 1, buffer.Pages[0].EndIndex);
			Assert.AreEqual(pageSize, buffer.Pages[0].Size);


			Assert.AreEqual(PageState.Loaded, buffer.Pages[1].State);
			Assert.AreEqual(1, buffer.Pages[1].Number);
			Assert.AreEqual(pageSize, buffer.Pages[1].MaxSize);
			Assert.AreEqual(pageSize, buffer.Pages[1].StartIndex);
			Assert.AreEqual(pageSize, buffer.Pages[1].Count);
			Assert.AreEqual(pageSize * 2 - 1, buffer.Pages[1].EndIndex);
			Assert.AreEqual(pageSize, buffer.Pages[1].Size);
			Assert.IsTrue(buffer.Pages[1].Dirty);

			// Check values
			Assert.AreEqual(10, buffer[0]);
			Assert.AreEqual(20, buffer[pageSize]);

		}
	}

	[Test]
	public void RemoveAll(
		[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)]
		StorageType storageType,
		[Values(1, 2, 10, 57, 173)] int pageSize,
		[Values(1, 2, int.MaxValue)] int maxOpenPages) {
		using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages * (long)pageSize, out var buffer)) {
			buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));
			buffer.RemoveRange(0, buffer.Count);
			Assert.AreEqual(0, buffer.Pages.Count);
			Assert.AreEqual(0, buffer.Count);
			Assert.AreEqual(Enumerable.Empty<byte>(), buffer);
		}
	}

	[Test]
	public void RemoveAllExcept1(
		[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)]
		StorageType storageType,
		[Values(2, 10, 57, 173)] int pageSize,
		[Values(1, 2, int.MaxValue)] int maxOpenPages) {
		using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages * (long)pageSize, out var buffer)) {
			buffer.AddRange(Tools.Array.Gen<byte>(pageSize, 10));
			buffer.RemoveRange(1, buffer.Count - 1);
			Assert.AreEqual(1, buffer.Pages.Count);
			Assert.AreEqual(1, buffer.Count);
			Assert.AreEqual(new byte[] { 10 }, buffer);
		}
	}

	[Test]
	public void Rewind(
		[Values(StorageType.MemoryPagedBuffer, StorageType.BinaryFile, StorageType.TransactionalBinaryFile)]
		StorageType storageType,
		[Values(1, 2, 111)] int pageSize,
		[Values(1, 2, int.MaxValue)] int maxOpenPages) {
		var expected = new byte[] { 127, 17, 18, 19 };
		using (CreateMemPagedBuffer(storageType, pageSize, maxOpenPages * (long)pageSize, out var buffer)) {
			buffer.AddRange<byte>(127, 16, 15, 14, 13);
			buffer.RemoveRange(1, 4);
			Assert.AreEqual(1, buffer.Count);
			Assert.AreEqual(127, buffer[0]);
			buffer.AddRange<byte>(17, 18, 19);
			Assert.AreEqual(4, buffer.Count);
			Assert.AreEqual(127, buffer[0]);
			Assert.AreEqual(17, buffer[1]);
			Assert.AreEqual(18, buffer[2]);
			Assert.AreEqual(19, buffer[3]);
			Assert.AreEqual(expected, buffer);
		}
	}

	[Test]
	public void IntegrationTests([Values] StorageType storageType, [Values(1, 10, 57, 173, 1111)] int pageSize, [Values(1, 2, 100)] int maxOpenPages) {
		var expected = new List<byte>();
		var maxCapacity = pageSize * maxOpenPages * 2;
		using (CreateBuffer(storageType, pageSize, maxOpenPages * pageSize, out var buffer)) {
			var mutateFromEndOnly = buffer is not MemoryBuffer && buffer is not StreamMappedBuffer;
			AssertEx.BufferIntegrationTest(buffer, maxCapacity, mutateFromEndOnly);
		}
	}


	public enum StorageType {
		MemoryBuffer,
		MemoryPagedBuffer,
		BinaryFile,
		TransactionalBinaryFile,
		StreamMappedBuffer,
	}


	private IDisposable CreateBuffer(StorageType storageType, int pageSize, long maxMemory, out IBuffer buffer) {
		switch (storageType) {
			case StorageType.MemoryBuffer:
				buffer = new MemoryBuffer(0, pageSize);
				return new Disposables();
			case StorageType.StreamMappedBuffer:
				buffer = new StreamMappedBuffer(new MemoryStream());
				return new Disposables();
		}

		var result = CreateMemPagedBuffer(storageType, pageSize, maxMemory, out var memBuffer);
		if (memBuffer.RequiresLoad)
			memBuffer.Load();
		buffer = memBuffer;
		return result;
	}

	private IDisposable CreateMemPagedBuffer(StorageType storageType, int pageSize, long maxMemory, out IMemoryPagedBuffer buffer) {
		var disposables = new Disposables();

		switch (storageType) {
			case StorageType.MemoryBuffer:
				throw new InvalidOperationException();
			case StorageType.MemoryPagedBuffer:
				buffer = new MemoryPagedBuffer(pageSize, maxMemory);
				break;
			case StorageType.BinaryFile:
				var tmpFile = Tools.FileSystem.GetTempFileName(false);
				buffer = new FileMappedBuffer(tmpFile, pageSize, maxMemory);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.TransactionalBinaryFile:
				var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				var fileName = Path.Combine(baseDir, "File.dat");
				buffer = new TransactionalFileMappedBuffer(fileName, baseDir, pageSize, maxMemory);
				if (buffer.RequiresLoad)
					buffer.Load();
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
		}
		return disposables;
	}

}
