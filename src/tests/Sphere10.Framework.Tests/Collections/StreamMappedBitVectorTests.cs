using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {
	public class StreamMappedBitVectorTests {

		private Random Random { get; } = new();

		[Test]
		public void InsertRangeEnd() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);

			var inputs = Enumerable.Repeat(true, 20);
			list.AddRange(inputs);

			var insert = Enumerable.Repeat(false, 20);
			list.InsertRange(20, insert);

			Assert.AreEqual(inputs.Concat(insert), list);
		}

		[Test]
		public void ReadRange() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);

			var inputs = Random.NextBools(16);
			list.AddRange(inputs);
			Assert.AreEqual(inputs, list);

			var range = list.ReadRange(9, 7)
				.ToList();

			Assert.AreEqual(inputs[9..], range);
		}

		[Test]
		public void IndexOfRange() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { false, false, false, false, false, false, false, false, true };
			list.AddRange(inputs);

			Assert.AreEqual(new[] { 8, 8, 8 }, list.IndexOfRange(new[] { true, true, true }));
			Assert.AreEqual(new[] { 7 }, list.IndexOfRange(new[] { false }));
		}

		[Test]
		public void RemoveRange() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { false, false, false, false, false, false, false, false, true };

			list.AddRange(inputs);
			list.RemoveRange(8, 1);
			Assert.AreEqual(8, list.Count);
			Assert.AreEqual(inputs[..^1], list);

			list.RemoveRange(0, list.Count);
			Assert.AreEqual(0, list.Count);
		}

		[Test]
		public void UpdateRange() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);
			var expected = new ExtendedList<bool>();

			var inputs = Random.NextBools(100);
			var update = Random.NextBools(inputs.Length);

			list.AddRange(inputs);
			expected.AddRange(inputs);

			list.UpdateRange(0, update);
			expected.UpdateRange(0, update);

			Assert.AreEqual(expected, list);

			int randomIndex = Random.Next(0, list.Count - 1);
			var randomUpdate = Random.NextBools(list.Count - randomIndex);

			list.UpdateRange(randomIndex, randomUpdate);
			expected.UpdateRange(randomIndex, randomUpdate);

			Assert.AreEqual(expected, list);
		}

		[Test]
		public void UpdateRangeOverlap() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);
			var expected = new ExtendedList<bool>();

			var inputs = Random.NextBools(935);
			list.AddRange(inputs);
			expected.AddRange(inputs);
			Assert.AreEqual(expected, list);

			var firstUpdate = Random.NextBools(505);
			list.UpdateRange(26, firstUpdate);
			expected.UpdateRangeSequentially(26, firstUpdate);
			Assert.AreEqual(expected, list);

			var copyUpdate = list.ReadRange(15, 901);
			var expectedUpdate = expected.ReadRangeSequentially(15, 901);
			list.UpdateRange(15, copyUpdate);
			expected.UpdateRangeSequentially(15, expectedUpdate);

			Assert.AreEqual(expected, list);
		}

		[Test]
		public void IntegrationTest() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);
			AssertEx.ListIntegrationTest(list, 1000, (rng, i) => rng.NextBools(i), true);
		}
	}
}
