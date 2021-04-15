using System.IO;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {
	public class StreamMappedBitVectorTests {

		[Test]
		public void AddRange() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { true, true, true, true, true, true, true, true, false };
			list.AddRange(inputs);

			Assert.AreEqual(9, list.Count);
			Assert.AreEqual(2, memoryStream.Position);
		}

		[Test]
		public void ReadRange() {
			using var memoryStream = new MemoryStream();
			var list = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { true, true, true, true, true, true, true, true, false };
			list.AddRange(inputs);

			Assert.AreEqual(inputs, list.ReadRange(0, 9));
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
	}
}
