using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {
	public class StreamMappedDynamicClusteredListTests {

		[Test]
		public void RequiresLoad() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);
			Assert.IsFalse(list.RequiresLoad);

			var secondList = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);
			Assert.IsTrue(secondList.RequiresLoad);

			secondList.Load();
			Assert.IsFalse(secondList.RequiresLoad);
		}

		[Test]
		public void ReadRange() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);

			var read = list.ReadRange(0, 3)
				.ToArray();

			Assert.AreEqual(inputs[0], read[0]);
			Assert.AreEqual(inputs[1], read[1]);
			Assert.AreEqual(inputs[2], read[2]);
		}

		[Test]
		public void UpdateRange() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);

			string update = rand.NextString(0, 100);
			list.UpdateRange(0, new[] { update });

			Assert.AreEqual(update, list[0]);
			Assert.AreEqual(inputs.Length, list.Count);
		}

		[Test]
		public void RemoveRange() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);
			list.RemoveRange(0, 1);

			Assert.AreEqual(inputs[1], list[0]);
			Assert.AreEqual(inputs.Length - 1, list.Count);
		}

		[Test]
		public void IndexOf() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);

			IEnumerable<int> indexes = list.IndexOfRange(inputs[..4]);

			Assert.AreEqual(new[] { 0, 1, 2, 3 }, indexes);
		}

		[Test]
		public void Count() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);

			Assert.AreEqual(inputs.Length, list.Count);
		}

		[Test]
		public void InsertRange() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);
			list.InsertRange(2, new[] { rand.NextString(1, 100) });

			Assert.AreEqual(inputs.Length + 1, list.Count);
			Assert.AreEqual(inputs[2], list[3]);
		}

		[Test]
		public void Clear() {
			var rand = new Random();
			string[] inputs = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);

			list.AddRange(inputs);
			list.Clear();
			Assert.AreEqual(0, list.Count);

			list.AddRange(inputs);
			Assert.AreEqual(inputs, list);
		}

		[Test]
		public void IntegrationTests() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), stream);
			AssertEx.ListIntegrationTest(list,
				100,
				(rng, i) => Enumerable.Range(0, i)
					.Select(x => rng.NextString(1, 100))
					.ToArray());
		}

		[Test]
		public void LoadFromExistingStream() {
			var rand = new Random();
			var input = Enumerable.Range(0, rand.Next(1, 100)).Select(x => rand.NextString(1, 100)).ToArray();
			var fileName = Tools.FileSystem.GetTempFileName(true);
			using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), fileStream);
					list.AddRange(input);
				}

				using (var fileStream = new FileStream(fileName, FileMode.Open)) {
					var list = new StreamMappedDynamicClusteredList<string>(32, new StringSerializer(Encoding.UTF8), fileStream);
					list.Load();
					Assert.AreEqual(input.Length, list.Count);
					Assert.AreEqual(input, list);
				}
			}
		}
	}
}
