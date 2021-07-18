using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Sphere10.Framework.Tests.Values
{
    public class VarIntTests
    {
        [Test]
        [TestCase((ulong)0xFC, 1)]
        [TestCase((ulong)0xFFFF, 3)]
        [TestCase((ulong)0xFFFFFFFF, 5)]
        [TestCase((ulong)0xFFFFFFFFFFFFFFFF, 9)]
        public void WriteAndRead(ulong value, int expectedByteLength)
        {
            var stream = new MemoryStream();
            VarInt a = new VarInt(value);
            stream.Write(a.ToBytes());
            Assert.AreEqual(stream.Length, expectedByteLength);

            stream.Seek(0, SeekOrigin.Begin);
            ulong b = VarInt.FromStream(stream);
            b.Should().Be(a).And.Be(value);
        }
    }
}