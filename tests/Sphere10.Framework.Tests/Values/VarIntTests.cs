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
        public void WriteAndReadStream(ulong value, int expectedByteLength)
        {
            var stream = new MemoryStream();
            var a = new VarInt(value);
            a.Write(stream);
            Assert.AreEqual(expectedByteLength, stream.Length);

            stream.Seek(0, SeekOrigin.Begin);
            ulong b = VarInt.Read(stream);
            b.Should().Be(a).And.Be(value);
        }

        [Test]
        [TestCase(ulong.MinValue, 1)]
        [TestCase((ulong) 0xFC, 1)]
        [TestCase((ulong) 0xFD, 3)]
        [TestCase((ulong) 0xFFFF, 3)]
        [TestCase((ulong) 0xFFFFF, 5)]
        [TestCase((ulong) 0xFFFFFFFF, 5)]
        [TestCase((ulong) 0xFFFFFFFFF, 9)]
        [TestCase(ulong.MaxValue, 9)]
        public void ToFromBytes(ulong value, int expectedByteLength)
        {
            VarInt a = new VarInt(value);
            var bytes = a.ToBytes();
            bytes.Length.Should().Be(expectedByteLength);

            ulong b = new VarInt(bytes);
            b.Should().Be(a);
        }

        [Test]
        public void ArithmeticOperatorOverloads()
        {
            ((ulong)(new VarInt(1) + new VarInt(1))).Should().Be(2);
            ((ulong)(new VarInt(1) + 1)).Should().Be(2);
            ((ulong)(new VarInt(1) - new VarInt(1))).Should().Be(0);
            ((ulong)(new VarInt(1) - 1)).Should().Be(0);
            ((ulong)(new VarInt(10) / new VarInt(3))).Should().Be(3);
            ((ulong)(new VarInt(10) / 3)).Should().Be(3);
            ((ulong)(new VarInt(1) * 2)).Should().Be(2);
        }
    }
}