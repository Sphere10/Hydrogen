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

            using (CreateMerkleList(storage, chf, pageSize, out var list)) {
                var data = new byte[] { 0 };
                list.AddRange(data);

                var dataHash = Hashers.Hash(chf, data);

                Assert.AreEqual(dataHash,  list.MerkleTree.Root);
            }
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

        private IDisposable CreateMerkleList(StorageType storageType, CHF chf, int pageSize, out IMerkleList<byte> list) {
            var disposables = new Disposables();
            switch (storageType) {
                case StorageType.MemoryBuffer:
                    list = new MerkleBuffer<MemoryBufferPage>(new LargeBuffer(pageSize, int.MaxValue), chf);
                    break;
                case StorageType.BinaryFile_1InMem:
                    var tmpFile = Tools.FileSystem.GetTempFileName(false);
                    list = new MerkleBuffer<BinaryFile.Page>(new BinaryFile(tmpFile, pageSize, 1), chf);
                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
                    break;
                case StorageType.BinaryFile_2InMem:
                    tmpFile = Tools.FileSystem.GetTempFileName(false);
                    list = new MerkleBuffer<BinaryFile.Page>(new BinaryFile(tmpFile, pageSize, 2), chf);
                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
                    break;
                case StorageType.BinaryFile_5InMem:
                    tmpFile = Tools.FileSystem.GetTempFileName(false);
                    list = new MerkleBuffer<BinaryFile.Page>(new BinaryFile(tmpFile, pageSize, 5), chf);
                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
                    break;
                case StorageType.TransactionalBinaryFile_1InMem:
                    var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    var fileName = Path.Combine(baseDir, "File.dat");
                    list = new MerkleBuffer<TransactionalBinaryFile.TransactionalPage>(new TransactionalBinaryFile(fileName, baseDir, Guid.NewGuid(), pageSize, 1), chf);
                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
                    break;
                case StorageType.TransactionalBinaryFile_2InMem:
                    baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    fileName = Path.Combine(baseDir, "File.dat");
                    list = new MerkleBuffer<TransactionalBinaryFile.TransactionalPage>(new TransactionalBinaryFile(fileName, baseDir, Guid.NewGuid(), pageSize, 2), chf);
                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
                    break;
                case StorageType.TransactionalBinaryFile_5InMem:
                    baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    fileName = Path.Combine(baseDir, "File.dat");
                    list = new MerkleBuffer<TransactionalBinaryFile.TransactionalPage>(new TransactionalBinaryFile(fileName, baseDir, Guid.NewGuid(), pageSize, 5), chf);
                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
            }
            return disposables;
        }


    }
}
