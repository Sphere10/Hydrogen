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
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class MerkleBufferTest {

        [Test]
        public void SingleByte([Values(1, 2, 1 << 18)] int pageSize, [Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values] StorageType storage) {
            using (CreateMerkleBuffer(storage, chf, pageSize, out var list)) {
                var data = new byte[] { 0 };
                list.AddRange(data);
                var dataHash = Hashers.Hash(chf, data);
                Assert.AreEqual(dataHash,  list.MerkleTree.Root);
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

                Assert.AreEqual(refTree.Root, list.MerkleTree.Root);
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

				Assert.AreEqual(refTree.Root, list.MerkleTree.Root);
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

				Assert.AreEqual(refTree.Root, list.MerkleTree.Root);
			}

			byte[] Hash(byte[] data) => Hashers.Hash(chf, data);
		}


        [Test]
        public void IntegrationTests_CheckEnd(
			[Values(1, 2, 1 << 18)] int pageSize, 
			[Values(CHF.Blake2b_128, CHF.SHA2_256)] CHF chf, 
			[Values] StorageType storage) {
            var expected = new List<byte>();
            var RNG = new Random(1231);
			var maxCapacity = pageSize * 11;
            using (CreateMerkleBuffer(storage, chf, pageSize, out var merkleBuffer)) {
                for (var i = 0; i < 10; i++) {
                    // add a random amount
                    var remainingCapacity = maxCapacity - merkleBuffer.Count;
                    var newItemsCount = RNG.Next(0, remainingCapacity + 1);
                    IEnumerable<byte> newItems = RNG.NextBytes(newItemsCount);
                    merkleBuffer.AddRange(newItems);
                    expected.AddRange(newItems);
                    Assert.AreEqual(expected, merkleBuffer);

                    // update a random amount
                    if (merkleBuffer.Count > 0) {
                        var range = RNG.NextRange(merkleBuffer.Count);
                        newItems = RNG.NextBytes(range.End - range.Start + 1);
                        expected.UpdateRangeSequentially(range.Start, newItems);
                        merkleBuffer.UpdateRange(range.Start, newItems);

                        // shuffle a random amount
                        range = RNG.NextRange(merkleBuffer.Count);
                        newItems = merkleBuffer.ReadRange(range.Start, range.End - range.Start + 1);
                        var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);

                        range = RNG.NextRange(merkleBuffer.Count, rangeLength: newItems.Count());
                        expected.UpdateRangeSequentially(range.Start, expectedNewItems);
                        merkleBuffer.UpdateRange(range.Start, newItems);

                        // remove a random amount (FROM END OF LIST)
                        range = new ValueRange<int>(RNG.Next(0, merkleBuffer.Count), merkleBuffer.Count - 1);
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
                    Assert.AreEqual(refTree.Root, merkleBuffer.MerkleTree.Root);
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

        private IDisposable CreateMerkleBuffer(StorageType storageType, CHF chf, int pageSize, out MerkleBuffer merkleBuffer) {
            var disposables = new Disposables();
            switch (storageType) {
                case StorageType.MemoryBuffer:
                    merkleBuffer = new MerkleBuffer(new MemoryPagedBuffer(pageSize, int.MaxValue), chf);
                    break;
                case StorageType.BinaryFile_1InMem:
                    var tmpFile = Tools.FileSystem.GetTempFileName(false);
                    merkleBuffer = new MerkleBuffer(new FileMappedBuffer(tmpFile, pageSize, 1*pageSize), chf);
                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
                    break;
                case StorageType.BinaryFile_2InMem:
                    tmpFile = Tools.FileSystem.GetTempFileName(false);
                    merkleBuffer = new MerkleBuffer(new FileMappedBuffer(tmpFile, pageSize, 2 * pageSize), chf);
                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
                    break;
                case StorageType.BinaryFile_5InMem:
                    tmpFile = Tools.FileSystem.GetTempFileName(false);
                    merkleBuffer = new MerkleBuffer(new FileMappedBuffer(tmpFile, pageSize, 5 * pageSize), chf);
                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
                    break;
                case StorageType.TransactionalBinaryFile_1InMem:
                    var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    var fileName = Path.Combine(baseDir, "File.dat");
                    merkleBuffer = new MerkleBuffer(new TransactionalFileMappedBuffer(fileName, baseDir, pageSize, 1 * pageSize), chf);
                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
                    break;
                case StorageType.TransactionalBinaryFile_2InMem:
                    baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    fileName = Path.Combine(baseDir, "File.dat");
                    merkleBuffer = new MerkleBuffer(new TransactionalFileMappedBuffer(fileName, baseDir, pageSize, 2 * pageSize), chf);
                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
                    break;
                case StorageType.TransactionalBinaryFile_5InMem:
                    baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    fileName = Path.Combine(baseDir, "File.dat");
                    merkleBuffer = new MerkleBuffer(new TransactionalFileMappedBuffer(fileName, baseDir, pageSize, 5 * pageSize), chf);
                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
            }
            return disposables;
        }


    }
}
