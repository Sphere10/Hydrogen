////-----------------------------------------------------------------------
//// <copyright file="TransactionalBinaryFileTests.cs" company="Sphere 10 Software">
////
//// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// <author>Herman Schoenfeld</author>
//// <date>2018</date>
//// </copyright>
////-----------------------------------------------------------------------

//using System;
//using System.CodeDom;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using System.IO;
//using NUnit.Framework.Constraints;
//using Sphere10.Framework;
//using Sphere10.Framework.NUnit;

//namespace Sphere10.Framework.Tests {

//	[TestFixture]
//	[Parallelizable(ParallelScope.Children)]
//	public class MerkleizedBufferTest {
		
		
//		[Test]
//		public void V1_Integration_SimpleRun([Values(1,2,3,5)] int pageSize, [Values] StorageType storage) {

//			using (CreateStorage(storage, 14, out var list)) {

//				var merkleBuff = new MerklizedBuffer(list);

//				var len0 = stream.Length;

//				list.Add("test");
//				Assert.AreEqual(1, list.Count);
//				Assert.AreEqual("test", list[0]);

//				var len1 = stream.Length;

//				list.Add("test2");
//				Assert.AreEqual(2, list.Count);
//				Assert.AreEqual("test", list[0]);
//				Assert.AreEqual("test2", list[1]);

//				var len2 = stream.Length;

//				list.Add("test33");
//				Assert.AreEqual(3, list.Count);
//				Assert.AreEqual("test", list[0]);
//				Assert.AreEqual("test33", list[2]);
//				Assert.AreEqual("test2", list[1]);

//				var len3 = stream.Length;

//				// Illegal removes
//				Assert.That(() => list.RemoveAt(0), Throws.Exception);
//				Assert.That(() => list.RemoveAt(1), Throws.Exception);

//				// Remove tip
//				list.RemoveAt(2);
//				Assert.AreEqual(2, list.Count);
//				Assert.AreEqual("test", list[0]);
//				Assert.AreEqual("test2", list[1]);

//				Assert.AreEqual(len2, stream.Length);

//				// Illegal remove
//				Assert.That(() => list.RemoveAt(0), Throws.Exception);

//				// Remove rest
//				list.RemoveAt(1);
//				Assert.AreEqual(1, list.Count);
//				Assert.AreEqual("test", list[0]);

//				Assert.AreEqual(len1, stream.Length);

//				list.RemoveAt(0);
//				Assert.AreEqual(0, list.Count);

//				Assert.AreEqual(len0, stream.Length);
//			}
//		}

//		public enum StorageType {
//			List,
//			ExtendedList,
//			MemoryBuffer,
//			BinaryFile_1Page_1InMem,
//			BinaryFile_2Page_1InMem,
//			BinaryFile_10Page_5InMem,
//			TransactionalBinaryFile_1Page_1InMem,
//			TransactionalBinaryFile_2Page_1InMem,
//			TransactionalBinaryFile_10Page_5InMem
//		}

//		private IDisposable CreateStorage(StorageType storageType, int estimatedMaxByteSize, out IExtendedList<byte> list) {
//			var disposables = new Disposables();
//			switch (storageType) {
//				case StorageType.List:
//					list = new ExtendedListAdapter<byte>(new List<byte>());
//					break;
//				case StorageType.ExtendedList:
//					list = new ExtendedList<byte>();
//					break;
//				case StorageType.MemoryBuffer:
//					list = new MemoryBuffer();
//					break;
//				case StorageType.BinaryFile_1Page_1InMem:
//					var tmpFile = Tools.FileSystem.GetTempFileName(false);
//					list = new BinaryFile(tmpFile, Math.Max(1, estimatedMaxByteSize), 1);
//					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
//					break;
//				case StorageType.BinaryFile_2Page_1InMem:
//					tmpFile = Tools.FileSystem.GetTempFileName(false);
//					list = new BinaryFile(tmpFile, Math.Max(1, estimatedMaxByteSize / 2), 2);
//					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
//					break;
//				case StorageType.BinaryFile_10Page_5InMem:
//					tmpFile = Tools.FileSystem.GetTempFileName(false);
//					list = new BinaryFile(tmpFile, Math.Max(1, estimatedMaxByteSize / 10), 5);
//					disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
//					break;
//				case StorageType.TransactionalBinaryFile_1Page_1InMem:
//					var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//					var fileName = Path.Combine(baseDir, "File.dat");
//					list = new TransactionalBinaryFile(fileName, baseDir, Guid.NewGuid(), Math.Max(1, estimatedMaxByteSize), 1);
//					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
//					break;
//				case StorageType.TransactionalBinaryFile_2Page_1InMem:
//					baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//					fileName = Path.Combine(baseDir, "File.dat");
//					list = new TransactionalBinaryFile(fileName, baseDir, Guid.NewGuid(), Math.Max(1, estimatedMaxByteSize / 2), 2);
//					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
//					break;
//				case StorageType.TransactionalBinaryFile_10Page_5InMem:
//					baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//					fileName = Path.Combine(baseDir, "File.dat");
//					list = new TransactionalBinaryFile(fileName, baseDir, Guid.NewGuid(), Math.Max(1, estimatedMaxByteSize / 10), 5);
//					disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
//					break;
//				default:
//					throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
//			}
//			return disposables;
//		}


//	}
//}
