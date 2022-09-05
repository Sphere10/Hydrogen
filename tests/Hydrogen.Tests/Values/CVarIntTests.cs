using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests.Values {
	public class CVarIntTests {
		[Test]
		[TestCase(ushort.MinValue, sizeof(ushort), 1)]
		[TestCase((ulong)0x7F, sizeof(ushort), 1)]
		[TestCase((ulong)0xFF, sizeof(ushort), 2)]
		[TestCase(ushort.MaxValue, sizeof(ushort), 3)]
		[TestCase(uint.MaxValue, sizeof(uint), 5)]
		[TestCase(ulong.MaxValue, sizeof(ulong), 10)]
		public void WriteAndRead(ulong value, int size, int expectedByteLength) {
			var stream = new MemoryStream();
			var a = new CVarInt(value);
			a.Write(stream);
			Assert.AreEqual(expectedByteLength, stream.Length);

			stream.Seek(0, SeekOrigin.Begin);
			ulong b = CVarInt.Read(size, stream);
			b.Should().Be(a).And.Be(value);
		}

		[Test]
		[TestCase(ushort.MinValue, sizeof(ushort), 1)]
		[TestCase((ulong)0x7F, sizeof(ushort), 1)]
		[TestCase((ulong)0xFF, sizeof(ushort), 2)]
		[TestCase(ushort.MaxValue, sizeof(ushort), 3)]
		[TestCase(uint.MaxValue, sizeof(uint), 5)]
		[TestCase(ulong.MaxValue, sizeof(ulong), 10)]
		public void ToAndFromBytes(ulong value, int size, int expectedByteLength) {
			var stream = new MemoryStream();
			var a = new CVarInt(value);
			stream.Write(a.ToBytes());
			Assert.AreEqual(expectedByteLength, stream.Length);

			ulong b = new CVarInt(stream.ToArray());
			b.Should().Be(a).And.Be(value);
		}

		[TestCase(ushort.MinValue, 1)]
		[TestCase((ulong)0x7F, 1)]
		[TestCase((ulong)0xFF, 2)]
		[TestCase(ushort.MaxValue, 3)]
		[TestCase(uint.MaxValue, 5)]
		[TestCase(ulong.MaxValue, 10)]
		public void SizeOf(ulong value, int expectedByteLength) {
			CVarInt.SizeOf(value).Should().Be(expectedByteLength);
		}

		[Test]
		public void IntegrationTest([Values(1000000)] int iterations) {
			var rng = new Random(31337);
			for (var i = 0; i < iterations; i++) {
				ulong val = (uint)rng.Next() + (uint)rng.Next();
				CVarInt varInt = val;
				Assert.AreEqual(val, varInt.ToLong());
			}
		}
	}
}