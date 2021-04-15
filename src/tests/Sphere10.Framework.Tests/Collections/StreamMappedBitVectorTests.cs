using System.IO;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {
	public class StreamMappedBitVectorTests {

		[Test]
		public void AddRange() {
			using var memoryStream = new MemoryStream();
			var array = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { true, true, true, true, true, true, true, true, false };
			array.AddRange(inputs);

			Assert.AreEqual(9, array.Count);
			Assert.AreEqual(2, memoryStream.Position);
		}

		[Test]
		public void ReadRange() {
			using var memoryStream = new MemoryStream();
			var array = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { true, true, true, true, true, true, true, true, false };
			array.AddRange(inputs);

			Assert.AreEqual(inputs, array.ReadRange(0, 9));
		}

		[Test]
		public void IndexOfRange() {
			using var memoryStream = new MemoryStream();
			var array = new StreamMappedBitVector(memoryStream);

			var inputs = new[] { false, false, false, false, false, false, false, false, true };
			array.AddRange(inputs);

			Assert.AreEqual(new[] { 8, 8, 8 }, array.IndexOfRange(new[] { true, true, true }));
			Assert.AreEqual(new[] { 7 }, array.IndexOfRange(new[] { false }));
		}
	}
}
