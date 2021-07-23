using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Sphere10.Framework.Tests.Values
{
    public class VarIntTests
    {
        [Test]
        [TestCase(ulong.MinValue, 1)]
        [TestCase((ulong)0xFC, 1)]
        [TestCase((ulong)0xFD, 3)]
        [TestCase((ulong)0xFFFF, 3)]
        [TestCase((ulong)0xFFFFF, 5)]
        [TestCase((ulong)0xFFFFFFFF, 5)]
        [TestCase((ulong)0xFFFFFFFFF, 9)]
        [TestCase(ulong.MaxValue, 9)]
        public void WriteAndRead(ulong value, int expectedByteLength)
        {
            var stream = new MemoryStream();
            var a = new VarInt(value);
            stream.Write(a.ToBytes());
            Assert.AreEqual(expectedByteLength, stream.Length);

            stream.Seek(0, SeekOrigin.Begin);
            ulong b = VarInt.FromStream(stream);
            b.Should().Be(a).And.Be(value);
        }

        [Test]
        [TestCase(ulong.MinValue)]
        [TestCase((ulong)0xFC)]
        [TestCase((ulong)0xFD)]
        [TestCase((ulong)0xFFFF)]
        [TestCase((ulong)0xFFFFF)]
        [TestCase((ulong)0xFFFFFFFF)]
        [TestCase((ulong)0xFFFFFFFFF)]
        [TestCase(ulong.MaxValue)]
        public void ConstructFromByteArray(ulong value)
        {
            byte[] bytes = new VarInt(value).ToBytes();
            ulong varint = new VarInt(bytes);
            varint.Should().Be(value);
        }
    }
}