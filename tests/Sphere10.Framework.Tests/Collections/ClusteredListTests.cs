using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ClusteredListTests : StreamPersistedTestsBase {

		private IDisposable CreateList(out ClusteredList<TestObject> clusteredList) {
			var stream = new MemoryStream();
			clusteredList = new ClusteredList<TestObject>(stream, 32, new TestObjectSerializer());
			return stream;
		}

		[Test]
		public void AddOneTest() {
			var rng = new Random(31337);
			using (CreateList(out var clusteredList)) {
				var obj = new TestObject(rng);
				clusteredList.Add(obj);
				Assert.That(clusteredList.Count, Is.EqualTo(1));
				Assert.That(clusteredList[0], Is.EqualTo(obj).Using(new TestObjectComparer()));
			}
		}

		[Test]
		public void AddOneRepeat([Values(100)] int iterations) {
			var rng = new Random(31337);
			using (CreateList(out var clusteredList)) {
				for (var i = 0; i < iterations; i++) {
					var obj = new TestObject(rng);
					clusteredList.Add(obj);
					Assert.That(clusteredList.Count, Is.EqualTo(i + 1));
					Assert.That(clusteredList[i], Is.EqualTo(obj).Using(new TestObjectComparer()));
				}
			}
		}
	
		[Test]
		public void ConstructorArgumentsAreGuarded() {
			Assert.Throws<ArgumentNullException>(() => new ClusteredList<int>(null, 1, new PrimitiveSerializer<int>()));
			Assert.Throws<ArgumentNullException>(() => new ClusteredList<int>(new MemoryStream(), 1,  null));
			Assert.Throws<ArgumentOutOfRangeException>(() => new ClusteredList<int>(new MemoryStream(), 0, null));
		}


		[Test]
		public void ReadRange() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(0, random.Next(5, 10)).Select(x => random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32, new StringSerializer(Encoding.UTF8));

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
			var list = new ClusteredList<int>(stream, 1,  new PrimitiveSerializer<int>());
			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(-1, 1).ToList());
			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(0, 5).ToList());
		}

		[Test]
		public void ReadRangeEmpty() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<int>(stream, 1,  new PrimitiveSerializer<int>());
			list.AddRange(999, 1000, 1001, 1002);
			Assert.IsEmpty(list.ReadRange(0, 0));

			list.Clear();
			Assert.IsEmpty(list.ReadRange(0, 0));
		}

		[Test]
		public void AddRangeEmptyNullStrings() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32,  new StringSerializer(Encoding.UTF8));
			string[] input = { string.Empty, null, string.Empty, null };
			list.AddRange(input);
			Assert.AreEqual(4, list.Count);

			var read = list.ReadRange(0, 4);
			Assert.AreEqual(input, read);
		}

		[Test]
		public void AddRangeNullEmptyCollections() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32,  new StringSerializer(Encoding.UTF8));
			Assert.Throws<ArgumentNullException>(() => list.AddRange(null));
			Assert.DoesNotThrow(() => list.AddRange(new string[0]));
		}

		[Test]
		public void UpdateRange() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32,  new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);

			string update = random.NextString(0, 100);
			list.UpdateRange(0, new[] { update });
			var value = list.Read(0);
			Assert.AreEqual(update, value);
			Assert.AreEqual(inputs.Length, list.Count);
		}

		[Test]
		public void UpdateRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<int>(stream, 32,  new PrimitiveSerializer<int>());

			list.AddRange(999, 1000, 1001, 1002);
			list.UpdateRange(0, new[] { 998 });
			int read = list[0];

			Assert.AreEqual(998, read);
			Assert.AreEqual(4, list.Count);
		}

		[Test]
		public void RemoveRange() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);
			list.RemoveRange(0, 1);

			Assert.AreEqual(inputs[1], list[0]);
			Assert.AreEqual(inputs.Length - 1, list.Count);
		}

		[Test]
		public void RemoveRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<int>(stream, 32, new PrimitiveSerializer<int>());

			list.Add(999);

			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(1, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(-1, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveRange(0, -1));

			Assert.DoesNotThrow(() => list.RemoveRange(0, 0));
		}

		[Test]
		public void IndexOf() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(1, 10).Select(x => random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32, new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);

			IEnumerable<int> indexes = list.IndexOfRange(inputs[..5]);

			Assert.AreEqual(new[] { 0, 1, 2, 3, 4 }, indexes);
		}

		[Test]
		public void IndexOfInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<int>(stream, 32,  new PrimitiveSerializer<int>());
			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentNullException>(() => list.IndexOfRange(null));
			Assert.DoesNotThrow(() => list.IndexOfRange(Array.Empty<int>()));
		}

		[Test]
		public void Count() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32,  new StringSerializer(Encoding.UTF8));

			Assert.AreEqual(0, list.Count);
			list.AddRange(inputs);

			Assert.AreEqual(inputs.Length, list.Count);
		}

		[Test]
		public void InsertRange() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(1, random.Next(1, 5)).Select(x => random.NextString(1, 5)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32,  new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);
			list.InsertRange(0, new[] { random.NextString(1, 100) });

			Assert.AreEqual(inputs.Length + 1, list.Count);
			Assert.AreEqual(inputs[0], list[1]);
		}

		[Test]
		public void InsertRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<int>(stream, 32,  new PrimitiveSerializer<int>());

			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentNullException>(() => list.InsertRange(0, null));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(5, new int[] { 1 }));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(-1, new int[] { 1 }));
			Assert.DoesNotThrow(() => list.InsertRange(0, Array.Empty<int>()));
		}

		[Test]
		public void Clear() {
			var random = new Random(31337);
			string[] inputs = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32,  new StringSerializer(Encoding.UTF8));

			list.AddRange(inputs);
			list.Clear();
			Assert.AreEqual(0, list.Count);

			list.AddRange(inputs);
			Assert.AreEqual(inputs, list);
		}

		[Test]
		public void IntegrationTests_String() {
			using var stream = new MemoryStream();
			var list = new ClusteredList<string>(stream, 32, new StringSerializer(Encoding.UTF8));
			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i)
					.Select(x => rng.NextString(1, 100))
					.ToArray());
		}


		[Test]
		public void LoadAndUseExistingStream([Values(1, 100)] int iterations) {
			var random = new Random(31337 + iterations);
			var input = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(0, 100)).ToArray();
			var fileName = Tools.FileSystem.GetTempFileName(true);
			using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new ClusteredList<string>(fileStream, 32, new StringSerializer(Encoding.UTF8));
					list.AddRange(input);
				}

				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new ClusteredList<string>(fileStream, 32, new StringSerializer(Encoding.UTF8));
					Assert.AreEqual(input.Length, list.Count);
					Assert.AreEqual(input, list);

					var secondInput = Enumerable.Range(0, random.Next(1, 100)).Select(x => random.NextString(1, 100)).ToArray();
					list.AddRange(secondInput);
					Assert.AreEqual(input.Concat(secondInput), list.ReadRange(0, list.Count));
					list.RemoveRange(0, list.Count);
					Assert.IsEmpty(list);
				}
			}
		}

		[Test]
		public void IntegrationTests([Values] StorageType storage) {
			var rng = new Random(31337);
			using (CreateStream(storage, 5000, out Stream stream)) {
				var list = new ClusteredList<TestObject>(stream, 100, new TestObjectSerializer(), new TestObjectComparer());
				AssertEx.ListIntegrationTest(list,
					100,
					(rng, i) => Enumerable.Range(0, i).Select(x => new TestObject(rng)).ToArray(),
					false,
					10,
					itemComparer: new TestObjectComparer()
				);
			}
		}

	}
}
