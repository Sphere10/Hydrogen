using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	public class StreamMappedFixedClusteredListTests {

		[Test]
		public void ConstructorArgumentsAreGuarded() {

			Assert.Throws<ArgumentNullException>(() => new StreamMappedFixedClusteredList<int>(32, 100, 40, null, new IntSerializer()));
			Assert.Throws<ArgumentNullException>(() => new StreamMappedFixedClusteredList<int>(32, 100, 40, new MemoryStream(), null));
			Assert.Throws<ArgumentOutOfRangeException>(() => new StreamMappedFixedClusteredList<int>(0, 100, 40, new MemoryStream(), new IntSerializer()));
		}
		
		[Test]
		public void ReadRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);

			var read = list.ReadRange(0, 3).ToArray();

			Assert.AreEqual(999, read[0]);
			Assert.AreEqual(1000, read[1]);
			Assert.AreEqual(1001, read[2]);
		}

		[Test]
		public void ReadRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(-1, 1).ToList());
			Assert.Throws<ArgumentOutOfRangeException>(() => _ = list.ReadRange(0,5).ToList());
		}

		[Test]
		public void ReadRangeEmpty() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);
			Assert.IsEmpty(list.ReadRange(0 , 0));;
			list.Clear();
			Assert.IsEmpty(list.ReadRange(0, 0));
		}

		[Test]
		public void AddRangeEmptyNullStrings() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<string>(32, 100, 4000, stream, new StringSerializer(Encoding.UTF8));
			string[] input = new[] { string.Empty, null, string.Empty, null };
			list.AddRange(input);
			Assert.AreEqual(4, list.Count);

			var read = list.ReadRange(0, 4);
			Assert.AreEqual(input, read);
		}

		[Test]
		public void AddRangeNullEmptyCollections() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<string>(32, 100, 4000, stream, new StringSerializer(Encoding.UTF8));
			Assert.Throws<ArgumentNullException>(() => list.AddRange(null));
			Assert.DoesNotThrow(() => list.AddRange(new string[0]));
		}

		[Test]
		public void UpdateRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);
			list.UpdateRange(0, new[] { 998 });
			int read = list[0];

			Assert.AreEqual(998, read);
			Assert.AreEqual(4, list.Count);
		}

		[Test]
		public void UpdateRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);
			
			Assert.Throws<ArgumentNullException>(() => list.UpdateRange(0, null));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.UpdateRange(4, new int[1]));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.UpdateRange(3, new int[2]));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.UpdateRange(-1, new int[2]));
		}

		[Test]
		public void RemoveRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.Add(999);
			list.Add(1000);
			list.RemoveRange(0, 1);

			Assert.AreEqual(1000, list[0]);
		}

		[Test]
		public void RemoveRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.Add(999);
			
			Assert.Throws<ArgumentOutOfRangeException>(() =>list.RemoveRange(1, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() =>list.RemoveRange(-1, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() =>list.RemoveRange(0, -1));
			
			Assert.DoesNotThrow(() =>list.RemoveRange(0, 0));
		}

		[Test]
		public void IndexOf() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);

			IEnumerable<int> indexes = list.IndexOfRange(new[] { 1000, 1001 });

			Assert.AreEqual(new[] { 1, 2 }, indexes);
		}

		[Test]
		public void IndexOfInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());
			list.AddRange(999, 1000, 1001, 1002);

			Assert.Throws<ArgumentNullException>(() => list.IndexOfRange(null));
			Assert.DoesNotThrow(() => list.IndexOfRange(Array.Empty<int>()));
		}

		[Test]
		public void Count() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			Assert.AreEqual(0, list.Count);
			list.AddRange(999, 1000, 1001, 1002);

			Assert.AreEqual(4, list.Count);
		}

		[Test]
		public void InsertRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);
			list.InsertRange(2, new[] { 1003 });

			Assert.AreEqual(5, list.Count);
			Assert.AreEqual(1001, list[3]);
		}

		[Test]
		public void InsertRangeInvalidArguments() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, stream, new IntSerializer());

			list.AddRange(999, 1000, 1001, 1002);
			
			Assert.Throws<ArgumentNullException>(() => list.InsertRange(0, null));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(5, new int[0]));
			Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(-1, new int[0]));
			Assert.DoesNotThrow(() => list.InsertRange(0, Array.Empty<int>()));
		}
		
		[Test]
		public void LoadAndUseExistingStream() {
			var fileName = Tools.FileSystem.GetTempFileName(true);

			using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					new StreamMappedFixedClusteredList<int>(32, 100, 4000, fileStream, new IntSerializer())
						.Add(999);
				}

				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedFixedClusteredList<int>(32, 100, 4000, fileStream, new IntSerializer());

					list.Load();

					Assert.AreEqual(1, list.Count);
					Assert.AreEqual(999, list[0]);

					list.Add(1000);
					Assert.AreEqual(2, list.Count);
					Assert.AreEqual(1000, list[1]);
					Assert.AreEqual(list.ReadRange(0, 2), new[] { 999, 1000 });
					list.RemoveAt(1);
					list.RemoveAt(0);

					Assert.IsEmpty(list);
				}
			}
		}
		
		[Test]
		public void ObjectIntegrationTest() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<StreamMappedDynamicClusteredListTests.TestObject>(100, 100, 50000, stream, new StreamMappedDynamicClusteredListTests.TestObjectSerializer(), new StreamMappedDynamicClusteredListTests.TestObjectComparer());
			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i).Select(x => new StreamMappedDynamicClusteredListTests.TestObject()).ToArray(),
				false,
				100,
				null,
				new StreamMappedDynamicClusteredListTests.TestObjectComparer());
		}
		
		[Test]
		public void IntegrationTestsFixedItemSize() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(16, 100, 4000, stream, new IntSerializer());

			AssertEx.ListIntegrationTest(list, 100, (rng, i) => rng.NextInts(i));
		}

		[Test]
		public void IntegrationTestsDynamicItemSize() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<string>(64, 1000, 1000000, stream, new StringSerializer(Encoding.ASCII));
			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i)
					.Select(x => rng.NextString(1, 100))
					.ToArray());
		}
	}
	
	internal class IntSerializer : FixedSizeObjectSerializer<int> {
		public IntSerializer() : base(4) {
		}

		public override int Serialize(int @object, EndianBinaryWriter writer) {
			writer.Write(BitConverter.GetBytes(@object));
			return sizeof(int);
		}

		public override int Deserialize(int size, EndianBinaryReader reader) {
			return reader.ReadInt32();
		}
	}
}
