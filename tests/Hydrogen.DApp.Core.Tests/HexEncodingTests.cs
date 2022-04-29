using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hydrogen;
using Hydrogen.NUnit;

namespace VelocityNET.Processing.Tests.Core {
    public class HexEncodingTests {

        [Test]
        public void Simple() {
            Assert.AreEqual(new[] { 1, 2, 3 }, HexEncoding.Decode("0x010203"));
            Assert.AreEqual(new[] { 1, 2, 3 }, HexEncoding.Decode("010203"));
            Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x1"));
            Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x012"));
        }

        [Test]
        public void IsValid() {
            Assert.IsTrue(HexEncoding.IsValid("0x0"));
            Assert.IsFalse(HexEncoding.IsValid("0x"));
            Assert.IsTrue(HexEncoding.IsValid("00"));
            Assert.IsFalse(HexEncoding.IsValid("0")); // should be double-digits
        }

        [Test]
        public void ByteLength() {
            Assert.AreEqual(0, HexEncoding.ByteLength("0x0"));
            Assert.AreEqual(1, HexEncoding.ByteLength("0x00"));
            Assert.Throws<FormatException>( () => HexEncoding.ByteLength("0x000"));
        }

    }
}
