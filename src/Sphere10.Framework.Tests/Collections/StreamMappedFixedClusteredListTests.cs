using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	public class StreamMappedFixedClusteredListTests {
		[Test]
		public void ReadRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), stream);

			list.AddRange(999, 1000, 1001, 1002);

			var read = list.ReadRange(0, 3).ToArray();

			Assert.AreEqual(999, read[0]);
			Assert.AreEqual(1000, read[1]);
			Assert.AreEqual(1001, read[2]);
		}

		[Test]
		public void LoadFromExistingStream() {
			var fileName = Tools.FileSystem.GetTempFileName(true);
			using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), fileStream);
					list.Add(999);
				}

				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), fileStream);
					Assert.AreEqual(999, list[0]);
				}
			}
		}

		[Test]
		public void UpdateRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), stream);

			list.AddRange(999, 1000, 1001, 1002);
			list.UpdateRange(0, new[] { 998 });
			int read = list[0];

			Assert.AreEqual(998, read);
			Assert.AreEqual(4, list.Count);
		}

		[Test]
		public void RemoveRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), stream);

			list.Add(999);
			list.Add(1000);
			list.RemoveRange(0, 1);

			Assert.AreEqual(1000, list[0]);
		}

		[Test]
		public void IndexOf() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), stream);

			list.AddRange(999, 1000, 1001, 1002);

			IEnumerable<int> indexes = list.IndexOfRange(new[] { 1000, 1001 });

			Assert.AreEqual(new[] { 1, 2 }, indexes);
		}

		[Test]
		public void Count() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100, new IntSerializer(), stream);

			list.AddRange(999, 1000, 1001, 1002);

			Assert.AreEqual(4, list.Count);
		}

		[Test]
		public void InsertRange() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(32, 100,  new IntSerializer(), stream);

			list.AddRange(999, 1000, 1001, 1002);
			list.InsertRange(2, new[] { 1003 });

			Assert.AreEqual(5, list.Count);
			Assert.AreEqual(1001, list[3]);
		}

		[Test]
		[Pairwise]
		public void IntegrationTests() {
			using var stream = new MemoryStream();
			var list = new StreamMappedFixedClusteredList<int>(16, 100, new IntSerializer(), stream);

			AssertEx.ListIntegrationTest(list, 100, (rng, i) => rng.NextInts(i));
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
