//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using NUnit.Framework;
//using System.IO;

//namespace Hydrogen.Tests;

//[TestFixture]
//[Parallelizable(ParallelScope.Children)]
//public class StreamPagedListTests_Bug1 {


//	[Test]
//	public void ThrowsWhenItemSerializerSerializesIncorrectAmount() {
//		// Tests for a bug found in initial impl of ClusteredDictionary caused by a broken ItemSerializerBase
//		using var stream = new MemoryStream();
//		var list = new StreamPagedList<TestStruct>(new ItemRecordSerializer(true), stream) { IncludeListHeader = true };
//		Assert.That(() => list.Add(default), Throws.Exception);
//	}


//	[Test]
//	public void DoesNotThrowWhenItemSerializerSerializesCorrectAmount() {
//		// Tests for a bug found in initial impl of ClusteredDictionary caused by a broken ItemSerializerBase
//		using var stream = new MemoryStream();
//		var list = new StreamPagedList<TestStruct>(new ItemRecordSerializer(false), stream) { IncludeListHeader = true };
//		Assert.That(() => list.Add(default), Throws.Nothing);
//	}


//	private class ItemRecordSerializer : ConstantLengthItemSizer<TestStruct>, IItemSerializer<TestStruct> {
//		private readonly bool _simulateBug;

//		public ItemRecordSerializer(bool simulateBug)
//			: base(sizeof(int) + sizeof(int) + sizeof(int) + sizeof(byte)) {
//			_simulateBug = simulateBug;
//		}

//		public void Serialize(ItemSerializationContext context, TestStruct item, EndianBinaryWriter writer) {
//			writer.Write(item.X);
//			writer.Write(item.Y);
//			writer.Write(item.Z);
//			writer.Write((byte)item.Z);
//			bytesWritten = sizeof(int) + sizeof(int) + sizeof(int) + (_simulateBug ? 0 : sizeof(byte)); // BUG WAS HERE, returning wrong amount of bytes written (must be detected by StreamPagedList)
//			return true;
//		}

//		public TestStruct Deserialize(ItemSerializationContext context, long byteSize, EndianBinaryReader reader) {
//			item = new TestStruct();
//			item.X = reader.ReadInt32();
//			item.Y = reader.ReadInt32();
//			item.Z = reader.ReadInt32();
//			item.U = reader.ReadByte();
//			return true;
//		}
//	}


//	private struct TestStruct {
//		public int X;
//		public int Y;
//		public int Z;
//		public byte U;
//	}


//}
