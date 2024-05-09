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
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MerklePagedBufferTest {

	[Test]
	public void SingleByte([Values(1, 2, 1 << 18)] int pageSize, [Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values] StorageType storage) {
		using (CreateMerkleBuffer(storage, chf, pageSize, out var list)) {
			var data = new byte[] { 0 };
			list.AddRange(data);
			var dataHash = Hashers.Hash(chf, data);
			ClassicAssert.AreEqual(dataHash, list.MerkleTree.Root);
		}
	}

	[Test]
	public void ThreePages_AddPagesManually([Values(1, 2, 1 << 18)] int pageSize, [Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values] StorageType storage) {
		using (CreateMerkleBuffer(storage, chf, pageSize, out var list)) {
			var rng = new Random(31337);
			var page1Data = rng.NextBytes(pageSize);
			var page2Data = rng.NextBytes(pageSize);
			var page3Data = rng.NextBytes(pageSize);
			list.AddRange(page1Data);
			list.AddRange(page2Data);
			list.AddRange(page3Data);

			// build ref tree
			var refTree = new SimpleMerkleTree(chf);
			refTree.Leafs.Add(Hash(page1Data));
			refTree.Leafs.Add(Hash(page2Data));
			refTree.Leafs.Add(Hash(page3Data));

			ClassicAssert.AreEqual(refTree.Root, list.MerkleTree.Root);
		}

		byte[] Hash(byte[] data) => Hashers.Hash(chf, data);
	}

	[Test]
	public void ThreePages_AddPagesAuto([Values(1, 2, 1 << 18)] int pageSize, [Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values] StorageType storage) {
		using (CreateMerkleBuffer(storage, chf, pageSize, out var list)) {
			var rng = new Random(31337);
			var page1Data = rng.NextBytes(pageSize);
			var page2Data = rng.NextBytes(pageSize);
			var page3Data = rng.NextBytes(pageSize);
			list.AddRange(Tools.Array.Concat<byte>(page1Data, page2Data, page3Data));

			// build ref tree
			var refTree = new SimpleMerkleTree(chf);
			refTree.Leafs.Add(Hash(page1Data));
			refTree.Leafs.Add(Hash(page2Data));
			refTree.Leafs.Add(Hash(page3Data));

			ClassicAssert.AreEqual(refTree.Root, list.MerkleTree.Root);
		}

		byte[] Hash(byte[] data) => Hashers.Hash(chf, data);
	}

	[Test]
	public void ThreePages_UpdatePage([Values(1, 2, 1 << 18)] int pageSize, [Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values] StorageType storage) {
		using (CreateMerkleBuffer(storage, chf, pageSize, out var list)) {
			var rng = new Random(31337);
			var page1Data = rng.NextBytes(pageSize);
			var page2Data_1 = rng.NextBytes(pageSize);
			var page2Data_2 = rng.NextBytes(pageSize);
			var page3Data = rng.NextBytes(pageSize);
			list.AddRange(page1Data);
			list.AddRange(page2Data_1);
			list.AddRange(page3Data);
			list.UpdateRange(page1Data.Length, page2Data_2);

			// build ref tree
			var refTree = new SimpleMerkleTree(chf);
			refTree.Leafs.Add(Hash(page1Data));
			refTree.Leafs.Add(Hash(page2Data_2));
			refTree.Leafs.Add(Hash(page3Data));

			ClassicAssert.AreEqual(refTree.Root, list.MerkleTree.Root);
		}

		byte[] Hash(byte[] data) => Hashers.Hash(chf, data);
	}


	[Test]
	public void IntegrationTests_CheckEnd(
		[Values(1, 2, 1 << 18)] int pageSize,
		[Values(CHF.Blake2b_128, CHF.SHA2_256)]
		CHF chf,
		[Values] StorageType storage) {
		var expected = new List<byte>();
		var RNG = new Random(1231);
		var maxCapacity = pageSize * 11;
		using (CreateMerkleBuffer(storage, chf, pageSize, out var merkleBuffer)) {
			for (var i = 0; i < 10; i++) {
				// add a random amount
				var remainingCapacity = maxCapacity - merkleBuffer.Count;
				var newItemsCount = RNG.Next(0, (int)remainingCapacity + 1);
				IEnumerable<byte> newItems = RNG.NextBytes(newItemsCount);
				merkleBuffer.AddRange(newItems);
				expected.AddRange(newItems);
				ClassicAssert.AreEqual(expected, merkleBuffer);

				// update a random amount
				if (merkleBuffer.Count > 0) {
					var range = RNG.NextRange(expected.Count);
					newItems = RNG.NextBytes(range.End - range.Start + 1);
					expected.UpdateRangeSequentially(range.Start, newItems);
					merkleBuffer.UpdateRange(range.Start, newItems);

					// shuffle a random amount
					range = RNG.NextRange(expected.Count);
					newItems = merkleBuffer.ReadRange(range.Start, range.End - range.Start + 1);
					var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);

					range = RNG.NextRange(expected.Count, rangeLength: newItems.Count());
					expected.UpdateRangeSequentially(range.Start, expectedNewItems);
					merkleBuffer.UpdateRange(range.Start, newItems);

					// remove a random amount (FROM END OF LIST)
					range = new ValueRange<int>(RNG.Next(0, (int)merkleBuffer.Count), (int)merkleBuffer.Count - 1);
					merkleBuffer.RemoveRange(range.Start, range.End - range.Start + 1);
					expected.RemoveRange(range.Start, range.End - range.Start + 1);
				}
			}

			CheckRoot();

			void CheckRoot() {
				// build ref tree
				var refTree = new SimpleMerkleTree(chf);
				foreach (var pageData in expected.Partition(pageSize))
					refTree.Leafs.Add(Hash(pageData.ToArray()));
				ClassicAssert.AreEqual(refTree.Root, merkleBuffer.MerkleTree.Root);
			}

		}

		byte[] Hash(byte[] data) => Hashers.Hash(chf, data);
	}


	public enum StorageType {
		MemoryBuffer,
		BinaryFile_1InMem,
		BinaryFile_2InMem,
		BinaryFile_5InMem,
		TransactionalBinaryFile_1InMem,
		TransactionalBinaryFile_2InMem,
		TransactionalBinaryFile_5InMem
	}


	private IDisposable CreateMerkleBuffer(StorageType storageType, CHF chf, int pageSize, out MerklePagedBuffer merkleBuffer) {
		var disposables = new Disposables();
		switch (storageType) {
			case StorageType.MemoryBuffer:
				merkleBuffer = new MerklePagedBuffer(new MemoryPagedBuffer(pageSize, int.MaxValue), chf);
				break;
			case StorageType.BinaryFile_1InMem:
				var tmpFile = Tools.FileSystem.GetTempFileName(false);
				var fileMappedBuffer = new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, pageSize, 1 * pageSize));
				merkleBuffer = new MerklePagedBuffer(fileMappedBuffer, chf);
				disposables.Add(fileMappedBuffer);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_2InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				fileMappedBuffer = new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, pageSize, 2 * pageSize));
				merkleBuffer = new MerklePagedBuffer(fileMappedBuffer, chf);
				disposables.Add(fileMappedBuffer);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_5InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				fileMappedBuffer = new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, pageSize, 5 * pageSize));
				merkleBuffer = new MerklePagedBuffer(fileMappedBuffer, chf);
				disposables.Add(fileMappedBuffer);
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.TransactionalBinaryFile_1InMem:
				var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				var fileName = Path.Combine(baseDir, "File.dat");
				var txnFileMappedBuffer = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, pageSize, 1 * pageSize));
				merkleBuffer = new MerklePagedBuffer(txnFileMappedBuffer, chf);
				disposables.Add(txnFileMappedBuffer);
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			case StorageType.TransactionalBinaryFile_2InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				txnFileMappedBuffer = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From( fileName, baseDir, pageSize, 2 * pageSize));
				merkleBuffer = new MerklePagedBuffer(txnFileMappedBuffer, chf);
				disposables.Add(txnFileMappedBuffer);
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			case StorageType.TransactionalBinaryFile_5InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				txnFileMappedBuffer = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, pageSize, 5 * pageSize));
				merkleBuffer = new MerklePagedBuffer(txnFileMappedBuffer, chf);
				disposables.Add(txnFileMappedBuffer);
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
		}
		return disposables;
	}


}
