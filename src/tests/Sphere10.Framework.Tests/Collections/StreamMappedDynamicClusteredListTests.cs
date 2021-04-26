using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {
	public class StreamMappedDynamicClusteredListTests {

		private Random Random { get; } = new(31337);

		[Test]
		public void ConstructorArgumentsAreGuarded() {
			Assert.Throws<ArgumentNullException>(() => new StreamMappedDynamicClusteredList<int>(1, null, new IntSerializer()));
			Assert.Throws<ArgumentNullException>(() => new StreamMappedDynamicClusteredList<int>(1, new MemoryStream(), null));
			Assert.Throws<ArgumentOutOfRangeException>(() => new StreamMappedDynamicClusteredList<int>(0, new MemoryStream(), null));
		}

		[Test]
		public void RequiresLoad_1() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			Assert.IsFalse(list.RequiresLoad);

			var secondList = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			Assert.IsFalse(secondList.RequiresLoad); // since nothing was written, should still be empty
		}

		[Test]
		public void RequiresLoad_2() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			Assert.IsFalse(list.RequiresLoad);
			list.Add("data");

			var secondList = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			Assert.IsTrue(secondList.RequiresLoad);

			secondList.Load();
			Assert.IsFalse(secondList.RequiresLoad);
		}


		[Test]
		public void ReadRange() {
			string[] inputs = Enumerable.Range(0, Random.Next(5, 10)).Select(x => Random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);

			var read = list.ReadRange(0, 3)
				.ToArray();

			Assert.AreEqual(inputs[0], read[0]);
			Assert.AreEqual(inputs[1], read[1]);
			Assert.AreEqual(inputs[2], read[2]);
		}

		[Test]
		public void ReadRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<int>(1, stream, new IntSerializer());
			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(-1, 1).ToList());
			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(0, 5).ToList());
		}

		[Test]
		public void ReadRangeEmpty() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<int>(1, stream, new IntSerializer());
			list.AddRange(999, 1000, 1001, 1002);
			Assert.IsEmpty(list.ReadRange(0, 0));
			;
			list.Clear();
			Assert.IsEmpty(list.ReadRange(0, 0));
		}

		[Test]
		public void AddRangeEmptyNullStrings() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			string[] input = { string.Empty, null, string.Empty, null };
			list.AddRange(input);
			Assert.AreEqual(4, list.Count);

			var read = list.ReadRange(0, 4);
			Assert.AreEqual(input, read);
		}

		[Test]
		public void AddRangeNullEmptyCollections() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			Assert.Throws<ArgumentNullException>(() => list.AddRange(null));
			Assert.DoesNotThrow(() => list.AddRange(new string[0]));
		}

		[Test]
		public void UpdateRange() {
			string[] inputs = Enumerable.Range(0, Random.Next(1, 100)).Select(x => Random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);

			string update = Random.NextString(0, 100);
			list.UpdateRange(0, new[] { update });

			Assert.AreEqual(update, list[0]);
			Assert.AreEqual(inputs.Length, list.Count);
		}

		[Test]
		public void UpdateRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<int>(32, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);
			list.UpdateRange(0, new[] { 998 });
			int read = list[0];

			Assert.AreEqual(998, read);
			Assert.AreEqual(4, list.Count);
		}


		[Test]
		public void RemoveRange() {
			string[] inputs = Enumerable.Range(0, Random.Next(1, 100)).Select(x => Random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);
			list.RemoveRange(0, 1);

			Assert.AreEqual(inputs[1], list[0]);
			Assert.AreEqual(inputs.Length - 1, list.Count);
		}

		[Test]
		public void RemoveRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<int>(32, stream, new IntSerializer());

			list.Add(999);

			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(1, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(-1, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(0, -1));

			Assert.DoesNotThrow(() => list.RemoveRange(0, 0));
		}

		[Test]
		public void IndexOf() {
			string[] inputs = Enumerable.Range(1, 10).Select(x => Random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);

			IEnumerable<int> indexes = list.IndexOfRange(inputs[..5]);

			Assert.AreEqual(new[] { 0, 1, 2, 3, 4 }, indexes);
		}

		[Test]
		public void IndexOfInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<int>(32, stream, new IntSerializer());
			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentNullException>(() => list.IndexOfRange(null));
			Assert.DoesNotThrow(() => list.IndexOfRange(Array.Empty<int>()));
		}

		[Test]
		public void Count() {
			string[] inputs = Enumerable.Range(0, Random.Next(1, 100)).Select(x => Random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			Assert.AreEqual(0, list.Count);
			list.AddRange(inputs);

			Assert.AreEqual(inputs.Length, list.Count);
		}

		[Test]
		public void InsertRange() {
			string[] inputs = Enumerable.Range(1, Random.Next(1, 5)).Select(x => Random.NextString(1, 5)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);
			list.InsertRange(0, new[] { Random.NextString(1, 100) });

			Assert.AreEqual(inputs.Length + 1, list.Count);
			Assert.AreEqual(inputs[0], list[1]);
		}

		[Test]
		public void InsertRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<int>(32, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentNullException>(() => list.InsertRange(0, null));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(5, new int[0]));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(-1, new int[0]));
			Assert.DoesNotThrow(() => list.InsertRange(0, Array.Empty<int>()));
		}

		[Test]
		public void Clear() {
			string[] inputs = Enumerable.Range(0, Random.Next(1, 100)).Select(x => Random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);
			list.Clear();
			Assert.AreEqual(0, list.Count);

			list.AddRange(inputs);
			Assert.AreEqual(inputs, list);
		}

		[Test]
		public void IntegrationTests() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, stream, new StringSerializer(Encoding.UTF8));
			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i)
					.Select(x => rng.NextString(1, 100))
					.ToArray());
		}

		[Test]
		public void ObjectIntegrationTest() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<TestObject>(100, stream, new TestObjectSerializer(), new TestObjectComparer());
			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i).Select(x => new TestObject()).ToArray(),
				false,
				100,
				null,
				new TestObjectComparer());
		}

		[Test]
		public void LoadAndUseExistingStream() {
			var input = Enumerable.Range(0, Random.Next(1, 100)).Select(x => Random.NextString(1, 100)).ToArray();
			var fileName = Tools.FileSystem.GetTempFileName(true);
			using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedDynamicClusteredList<string>(32, fileStream, new StringSerializer(Encoding.UTF8));
					list.AddRange(input);
				}

				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedDynamicClusteredList<string>(32, fileStream, new StringSerializer(Encoding.UTF8));
					list.Load();
					Assert.AreEqual(input.Length, list.Count);
					Assert.AreEqual(input, list);

					var secondInput = Enumerable.Range(0, Random.Next(1, 100)).Select(x => Random.NextString(1, 100)).ToArray();
					list.AddRange(secondInput);
					Assert.AreEqual(input.Concat(secondInput), list.ReadRange(0, list.Count));
					list.RemoveRange(0, list.Count);
					Assert.IsEmpty(list);
				}
			}
		}


		internal class TestObject {

			public TestObject() {
				var random = new Random();
				A = random.NextString(100);
				B = random.Next(0, 1000);
				C = random.NextBool();
			}

			public TestObject(string a, int b, bool c) {
				A = a;
				B = b;
				C = c;
			}

			public string A { get; }

			public int B { get; }

			public bool C { get; }

		}


		internal class TestObjectSerializer : IObjectSerializer<TestObject> {

			private IObjectSerializer<string> StringSerializer { get; } = new StringSerializer(Encoding.UTF8);
			public bool IsFixedSize => false;
			public int FixedSize => -1;

			public int CalculateTotalSize(IEnumerable<TestObject> items, bool calculateIndividualItems, out int[] itemSizes) {
				var sizes = new List<int>();

				if (calculateIndividualItems) {
					foreach (var testObject in items) {
						sizes.Add(CalculateSize(testObject));
					}
				}

				itemSizes = sizes.ToArray();
				return sizes.Sum(x => x);
			}

			public int CalculateSize(TestObject item) =>
				StringSerializer.CalculateSize(item.A) + sizeof(int) + sizeof(bool);

			public int Serialize(TestObject @object, EndianBinaryWriter writer) {
				int stringBytesCount = StringSerializer.Serialize(@object.A, writer);
				writer.Write(@object.B);
				writer.Write(@object.C);

				return stringBytesCount + sizeof(int) + sizeof(bool);
			}

			public TestObject Deserialize(int size, EndianBinaryReader reader) {
				int stringSize = size - sizeof(int) - sizeof(bool);
				return new(StringSerializer.Deserialize(stringSize, reader), reader.ReadInt32(), reader.ReadBoolean());
			}
		}
		
		internal class TestObjectComparer : IEqualityComparer<TestObject> {
			public bool Equals(TestObject x, TestObject y) {
				if (ReferenceEquals(x, y))
					return true;
				if (ReferenceEquals(x, null))
					return false;
				if (ReferenceEquals(y, null))
					return false;
				if (x.GetType() != y.GetType())
					return false;
				return x.A == y.A && x.B == y.B && x.C == y.C;
			}
			public int GetHashCode(TestObject obj) {
				return HashCode.Combine(obj.A, obj.B, obj.C);
			}
		}
	}
}
