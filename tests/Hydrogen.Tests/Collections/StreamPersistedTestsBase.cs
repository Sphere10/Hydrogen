// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using NUnit.Framework.Internal;
//using Hydrogen.NUnit;

//namespace Hydrogen.Tests {

//	public class StreamPersistedTestsBase {

//        protected IDisposable CreateStream(StorageType storageType, int estimatedMaxByteSize, out Stream stream) {
//            var disposables = new Disposables();

//            switch (storageType) {
//                case StorageType.MemoryStream:
//                    stream = new MemoryStream();
//                    break;
//                case StorageType.MemoryBuffer:
//                    stream = new ExtendedMemoryStream(new MemoryBuffer());
//                    break;
//                case StorageType.BinaryFile_1Page_1InMem:
//                    var tmpFile = Tools.FileSystem.GetTempFileName(false);
//                    stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize), 1* Math.Max(1, estimatedMaxByteSize)));
//                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
//                    break;
//                case StorageType.BinaryFile_2Page_1InMem:
//                    tmpFile = Tools.FileSystem.GetTempFileName(false);
//                    stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize / 2), 2* Math.Max(1, estimatedMaxByteSize / 2)));
//                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
//                    break;
//                case StorageType.BinaryFile_10Page_5InMem:
//                    tmpFile = Tools.FileSystem.GetTempFileName(false);
//                    stream = new ExtendedMemoryStream(new FileMappedBuffer(tmpFile, Math.Max(1, estimatedMaxByteSize / 10), 5* Math.Max(1, estimatedMaxByteSize / 10)));
//                    disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
//                    break;
//                case StorageType.TransactionalBinaryFile_1Page_1InMem:
//                    var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//                    var fileName = Path.Combine(baseDir, "File.dat");
//                    stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Math.Max(1, estimatedMaxByteSize), 1* Math.Max(1, estimatedMaxByteSize), autoLoad:true));
//                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
//                    break;
//                case StorageType.TransactionalBinaryFile_2Page_1InMem:
//                    baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//                    fileName = Path.Combine(baseDir, "File.dat");
//                    stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 2), 2* Math.Max(1, estimatedMaxByteSize / 2), autoLoad: true));
//                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
//                    break;

//                case StorageType.TransactionalBinaryFile_10Page_5InMem:
//                    baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
//                    fileName = Path.Combine(baseDir, "File.dat");
//                    stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 10), 5* Math.Max(1, estimatedMaxByteSize / 10), autoLoad: true));
//                    disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
//            }
//            return disposables;
//        }

//        public class TestObject {

//			public TestObject(Random random) {
//				A = random.NextString(random.Next(0, 101));
//				B = random.Next(0, 1000);
//				C = random.NextBool();
//			}

//			public TestObject(string a, int b, bool c) {
//				A = a;
//				B = b;
//				C = c;
//			}

//			public string A { get; set; }

//			public int B { get; set; }

//			public bool C { get; set; }

//			public override string ToString() => $"[TestObject] A: '{A}', B: {B}, C: {C}";

//        }

//        public class TestObjectSerializer : ItemSerializerBase<TestObject> {
//			private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.UTF8);

//			public override int CalculateSize(TestObject item) 
//				=> _stringSerializer.CalculateSize(item.A) + sizeof(int) + sizeof(bool);


//			public override bool TrySerialize(TestObject item, EndianBinaryWriter writer, out int bytesWritten) {
//				int stringBytesCount = _stringSerializer.Serialize(item.A, writer);
//				writer.Write(item.B);
//				writer.Write(item.C);

//				bytesWritten = stringBytesCount + sizeof(int) + sizeof(bool);
//				return true;
//			}

//			public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TestObject item) {
//				int stringSize = byteSize - sizeof(int) - sizeof(bool);
//				item = new(_stringSerializer.Deserialize(stringSize, reader), reader.ReadInt32(), reader.ReadBoolean());
//				return true;
//			}

//		}

//        public class TestObjectEqualityComparer : IEqualityComparer<TestObject> {
//			public bool Equals(TestObject x, TestObject y) {
//				if (ReferenceEquals(x, y))
//					return true;
//				if (ReferenceEquals(x, null))
//					return false;
//				if (ReferenceEquals(y, null))
//					return false;
//				if (x.GetType() != y.GetType())
//					return false;
//				return x.A == y.A && x.B == y.B && x.C == y.C;
//			}
//			public int GetHashCode(TestObject obj) {
//				return HashCode.Combine(obj.A, obj.B, obj.C);
//			}
//		}

//        public enum StorageType {
//			MemoryStream,
//			MemoryBuffer,
//			BinaryFile_1Page_1InMem,
//			BinaryFile_2Page_1InMem,
//			BinaryFile_10Page_5InMem,
//			TransactionalBinaryFile_1Page_1InMem,
//			TransactionalBinaryFile_2Page_1InMem,
//			TransactionalBinaryFile_10Page_5InMem
//		}
//	}
//}



