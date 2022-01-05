//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using NUnit.Framework;
//using Sphere10.Framework.NUnit;

//namespace Sphere10.Framework.Tests {

//	[TestFixture]
//	[Parallelizable(ParallelScope.Children)]
//	public class StaticClusteredListTests : ClusteredListTestsBase {

//		protected override IDisposable CreateList(out ClusteredList<TestObject> clusteredList) {
//			var stream = new MemoryStream();
//			clusteredList = new ClusteredList<TestObject>(32, 100, 30000, stream, new TestObjectSerializer());
//			return stream;
//		}


//		[Test]
//		public void ConstructorArgumentsAreGuarded() {
//			Assert.Throws<ArgumentNullException>(() =>
//				new ClusteredList<int>(32, 100, 40, null, new IntSerializer()));
//			Assert.Throws<ArgumentNullException>(() =>
//				new ClusteredList<int>(32, 100, 40, new MemoryStream(), null));
//			Assert.Throws<ArgumentOutOfRangeException>(() =>
//				new ClusteredList<int>(0, 100, 40, new MemoryStream(), new IntSerializer()));
//		}

//		[Test]
//		public void ReadRange() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);

//			var read = list.ReadRange(0, 3).ToArray();

//			Assert.AreEqual(999, read[0]);
//			Assert.AreEqual(1000, read[1]);
//			Assert.AreEqual(1001, read[2]);
//		}

//		[Test]
//		public void ReadRangeInvalidArguments() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);

//			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(-1, 1).ToList());
//			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(0, 5).ToList());
//		}

//		[Test]
//		public void ReadRangeEmpty() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);
//			Assert.IsEmpty(list.ReadRange(0, 0));
//			;
//			list.Clear();
//			Assert.IsEmpty(list.ReadRange(0, 0));
//		}

//		[Test]
//		public void AddRangeEmptyNullStrings() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<string>(32, 100, 4000, stream, new StringSerializer(Encoding.UTF8));
//			string[] input = { string.Empty, null, string.Empty, null };
//			list.AddRange(input);
//			Assert.AreEqual(4, list.Count);
//			var read = list.ReadRange(0, 4);
//			Assert.AreEqual(input, read);
//		}

//		[Test]
//		public void AddRangeNullEmptyCollections() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<string>(32, 100, 4000, stream,
//				new StringSerializer(Encoding.UTF8));
//			Assert.Throws<ArgumentNullException>(() => list.AddRange(null));
//			Assert.DoesNotThrow(() => list.AddRange(new string[0]));
//		}

//		[Test]
//		public void UpdateRange() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);
//			list.UpdateRange(0, new[] { 998 });
//			int read = list[0];

//			Assert.AreEqual(998, read);
//			Assert.AreEqual(4, list.Count);
//		}

//		[Test]
//		public void UpdateRangeInvalidArguments() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);

//			Assert.Throws<ArgumentNullException>(() => list.UpdateRange(0, null));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.UpdateRange(4, new int[1]));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.UpdateRange(3, new int[2]));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.UpdateRange(-1, new int[2]));
//		}

//		[Test]
//		public void RemoveRange() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.Add(999);
//			list.Add(1000);
//			list.RemoveRange(0, 1);

//			Assert.AreEqual(1000, list[0]);
//		}

//		[Test]
//		public void RemoveRangeInvalidArguments() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.Add(999);

//			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(1, 1));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(-1, 1));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(0, -1));

//			Assert.DoesNotThrow(() => list.RemoveRange(0, 0));
//		}

//		[Test]
//		public void IndexOf() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);

//			IEnumerable<int> indexes = list.IndexOfRange(new[] { 1000, 1001 });

//			Assert.AreEqual(new[] { 1, 2 }, indexes);
//		}

//		[Test]
//		public void IndexOfInvalidArguments() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());
//			list.AddRange(999, 1000, 1001, 1002);

//			Assert.Throws<ArgumentNullException>(() => list.IndexOfRange(null));
//			Assert.DoesNotThrow(() => list.IndexOfRange(Array.Empty<int>()));
//		}

//		[Test]
//		public void Count() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			Assert.AreEqual(0, list.Count);
//			list.AddRange(999, 1000, 1001, 1002);

//			Assert.AreEqual(4, list.Count);
//		}

//		[Test]
//		public void InsertRange() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);
//			list.InsertRange(2, new[] { 1003 });

//			Assert.AreEqual(5, list.Count);
//			Assert.AreEqual(1001, list[3]);
//		}

//		[Test]
//		public void InsertRangeInvalidArguments() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

//			list.AddRange(999, 1000, 1001, 1002);

//			Assert.Throws<ArgumentNullException>(() => list.InsertRange(0, null));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(5, new int[0]));
//			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(-1, new int[0]));
//			Assert.DoesNotThrow(() => list.InsertRange(0, Array.Empty<int>()));
//		}

//		[Test]
//		public void LoadAndUseExistingStream([Values(1, 100)] int iterations) {
//			var random = new Random(31337 + iterations);
//			var input = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(0, 100)).ToArray();
//			var fileName = Tools.FileSystem.GetTempFileName(true);

//			using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
//				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
//					new ClusteredList<string>(32, 100, 40000, fileStream, new StringSerializer(Encoding.Default))
//						.AddRange(input);
//				}

//				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
//					var list = new ClusteredList<string>(32, 100, 40000, fileStream, new StringSerializer(Encoding.Default));

//					list.Load();

//					Assert.AreEqual(input.Length, list.Count);

//					list.Add("hello");
//					Assert.AreEqual(input.Length + 1, list.Count);
//					Assert.AreEqual(input, list.ReadRange(0, input.Length));
//					Assert.AreEqual(list[^1], "hello");
//				}
//			}
//		}

//		[Test]
//		public void ObjectIntegrationTest([Values] StorageType storage) {
//			var rng = new Random(31337);
//			using (CreateStream(storage, 5000, out Stream stream)) {
//				var list = new ClusteredList<TestObject>(
//					32,
//					100,
//					30000,
//					stream,
//					new TestObjectSerializer(),
//					new TestObjectComparer()
//				);

//				AssertEx.ListIntegrationTest(
//					list,
//					100,
//					(_, i) => 
//						Enumerable
//							.Range(0, i)
//							.Select(_ => new TestObject(rng))
//							.ToArray(),
//					false,
//					10,
//					null,
//					new TestObjectComparer()
//				);
//			}
//		}

//		[Test]
//		public void IntegrationTestsFixedItemSize() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<int>(16, 100, 4000, stream, new IntSerializer());
//			AssertEx.ListIntegrationTest(list, 100, (rng, i) => rng.NextInts(i));
//		}

//		[Test]
//		public void IntegrationTestsDynamicItemSize() {
//			using var stream = new MemoryStream();
//			var list = new ClusteredList<string>(64, 1000, 1000000, stream, new StringSerializer(Encoding.UTF8));
//			AssertEx.ListIntegrationTest(
//				list,
//				100,
//				(rng, i) => 
//					Enumerable
//						.Range(0, i)
//						.Select(x => rng.NextString(0, 100))
//						.ToArray()
//			);
//		}


//	}


//}