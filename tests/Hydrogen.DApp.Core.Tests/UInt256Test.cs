// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using Hydrogen;
//using Hydrogen.DApp.Core.Encoding;
//using Hydrogen.DApp.Core.Maths;

//namespace VelocityNET.Processing.Tests.Core {

//    public class UInt256Test {

//        [Test]
//        public void Standard() {
//            var v = new UInt256("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
//            var v2 = new UInt256("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
//            var vless = new UInt256("00000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffe");
//            var vplus = new UInt256("00000001ffffffffffffffffffffffffffffffffffffffffffffffffffffffff");

//            ClassicAssert.IsTrue(HexEncoding.IsValid("0xff"));

//            ClassicAssert.AreEqual("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff", v.ToString());
//            ClassicAssert.AreEqual(new UInt256("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"), v);
//            ClassicAssert.AreEqual(new UInt256("0x00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"), v);
//            ClassicAssert.AreEqual(UInt256.Parse("0x00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"), v);
//            ClassicAssert.IsTrue(v < vplus);
//            ClassicAssert.IsTrue(v > vless);
//            UInt256 unused;
//            ClassicAssert.IsTrue(UInt256.TryParse("0x00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff", out unused));
//            ClassicAssert.IsTrue(UInt256.TryParse("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff", out unused));
//            ClassicAssert.IsTrue(UInt256.TryParse("00000000ffffFFfFffffffffffffffffffffffffffffffffffffffffffffffff", out unused));
//            ClassicAssert.IsFalse(UInt256.TryParse("00000000gfffffffffffffffffffffffffffffffffffffffffffffffffffffff", out unused));
//            ClassicAssert.IsFalse(UInt256.TryParse("100000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff", out unused));
//            ClassicAssert.IsFalse(UInt256.TryParse("1100000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff", out unused));
//            Assert.Throws<FormatException>(() => UInt256.Parse("1100000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
//            Assert.Throws<FormatException>(() => UInt256.Parse("100000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
//            UInt256.Parse("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
//            Assert.Throws<FormatException>(() => UInt256.Parse("000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));

//            ClassicAssert.IsTrue(v >= v2);
//            ClassicAssert.IsTrue(v <= v2);
//            ClassicAssert.IsFalse(v < v2);
//            ClassicAssert.IsFalse(v > v2);

//            ClassicAssert.IsTrue(v.ToBytes()[0] == 0xFF);
//            ClassicAssert.IsTrue(v.ToBytes(false)[0] == 0x00);

//            AssertEquals(v, new UInt256(v.ToBytes()));
//            AssertEquals(v, new UInt256(v.ToBytes(false), false));

//            ClassicAssert.AreEqual(0xFF, v.GetByte(0));
//            ClassicAssert.AreEqual(0x00, v.GetByte(31));
//            ClassicAssert.AreEqual(0x39, new UInt256("39000001ffffffffffffffffffffffffffffffffffffffffffffffffffffffff").GetByte(31));
//            Assert.Throws<ArgumentOutOfRangeException>(() => v.GetByte(32));
//        }

//        [Test]
//        public void Sortable() {
//            SortedDictionary<UInt256, UInt256> values = new SortedDictionary<UInt256, UInt256>();
//            values.Add(UInt256.Zero, UInt256.Zero);
//            values.Add(UInt256.One, UInt256.One);
//            ClassicAssert.AreEqual(UInt256.Zero, values.First().Key);
//            ClassicAssert.AreEqual(UInt256.One, values.Skip(1).First().Key);
//            ClassicAssert.AreEqual(-1, UInt256.Zero.CompareTo(UInt256.One));
//            ClassicAssert.AreEqual(1, UInt256.One.CompareTo(UInt256.Zero));
//            ClassicAssert.AreEqual(1, UInt256.One.CompareTo(null as object));
//            ClassicAssert.AreEqual(1, UInt256.Zero.CompareTo(null as object));

//            ClassicAssert.IsTrue(null < UInt256.Zero);
//            ClassicAssert.IsTrue(UInt256.Zero > (null as UInt256));
//            ClassicAssert.IsTrue((null as UInt256) >= (null as UInt256));
//            ClassicAssert.IsTrue((null as UInt256) == (null as UInt256));

//            var values2 = new SortedDictionary<UInt160, UInt160> {
//                { UInt160.Zero, UInt160.Zero }, 
//                { UInt160.One, UInt160.One }
//            };
//            ClassicAssert.AreEqual(UInt160.Zero, values2.First().Key);
//            ClassicAssert.AreEqual(UInt160.One, values2.Skip(1).First().Key);

//            ClassicAssert.AreEqual(-1, UInt160.Zero.CompareTo(UInt160.One));
//            ClassicAssert.AreEqual(1, UInt160.One.CompareTo(UInt160.Zero));
//            ClassicAssert.AreEqual(1, UInt160.One.CompareTo(null as object));
//            ClassicAssert.AreEqual(1, UInt160.Zero.CompareTo(null as object));

//            ClassicAssert.IsTrue((null as UInt160) < UInt160.Zero);
//            ClassicAssert.IsTrue(UInt160.Zero > (null as UInt160));
//            ClassicAssert.IsTrue((null as UInt160) >= (null as UInt160));
//            ClassicAssert.IsTrue((null as UInt160) == (null as UInt160));
//        }

//        //[Test]
//        //public void spanUintSerializationTests() {
//        //    var v = new UInt256(RandomUtils.GetBytes(32));
//        //    Assert.Equal(v, new UInt256(v.ToBytes()));
//        //    AssertEx.CollectionEquals(v.ToBytes(), v.AsBitcoinSerializable().ToBytes());
//        //    UInt256.MutableUInt256 mutable = new UInt256.MutableUInt256();
//        //    mutable.ReadWrite(v.ToBytes(), Network.Main);
//        //    Assert.Equal(v, mutable.Value);
//        //}

//        //[Test]
//        //public void uitnSerializationTests() {
//        //    MemoryStream ms = new MemoryStream();
//        //    BitcoinStream stream = new BitcoinStream(ms, true);

//        //    var v = new UInt256("00000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
//        //    var vless = new UInt256("00000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffe");
//        //    var vplus = new UInt256("00000001ffffffffffffffffffffffffffffffffffffffffffffffffffffffff");

//        //    stream.ReadWrite(ref v);
//        //    ClassicAssert.NotNull(v);

//        //    ms.Position = 0;
//        //    stream = new BitcoinStream(ms, false);

//        //    UInt256 v2 = UInt256.Zero;
//        //    stream.ReadWrite(ref v2);
//        //    Assert.Equal(v, v2);

//        //    v2 = null;
//        //    ms.Position = 0;
//        //    stream.ReadWrite(ref v2);
//        //    Assert.Equal(v, v2);

//        //    List<UInt256> vs = new List<UInt256>()
//        //    {
//        //        v,vless,vplus
//        //    };

//        //    ms = new MemoryStream();
//        //    stream = new BitcoinStream(ms, true);
//        //    stream.ReadWrite(ref vs);
//        //    ClassicAssert.IsTrue(vs.Count == 3);

//        //    ms.Position = 0;
//        //    stream = new BitcoinStream(ms, false);
//        //    List<UInt256> vs2 = new List<UInt256>();
//        //    stream.ReadWrite(ref vs2);
//        //    ClassicAssert.IsTrue(vs2.SequenceEqual(vs));

//        //    ms.Position = 0;
//        //    vs2 = null;
//        //    stream.ReadWrite(ref vs2);
//        //    ClassicAssert.IsTrue(vs2.SequenceEqual(vs));
//        //}

//        private void AssertEquals(UInt256 a, UInt256 b) {
//            ClassicAssert.AreEqual(a, b);
//            ClassicAssert.AreEqual(a.GetHashCode(), b.GetHashCode());
//            ClassicAssert.IsTrue(a == b);
//        }
//    }

//}


