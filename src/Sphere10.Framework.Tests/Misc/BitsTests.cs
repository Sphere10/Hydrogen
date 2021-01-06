//-----------------------------------------------------------------------
// <copyright file="UrlToolTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using NUnit.Framework;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class BitsTests {

        [Test]
        public void CopyBits_L2R() {
            var rng = new Random(31337);
            var bytes = rng.NextBytes(100);
            var dest = new byte[100];
            var bitLength = bytes.Length * 8;
            Bits.CopyBits(bytes, 0, dest, 0, bitLength);
            Assert.AreEqual(bytes, dest);
        }

		[Test]
		public void CopyBits_R2L() {
			var rng = new Random(31337);
			var bytes = rng.NextBytes(100);
			var dest = new byte[100];
			var bitLength = bytes.Length * 8;
			Bits.CopyBits(bytes, bitLength - 1, dest, bitLength - 1, bitLength, IterateDirection.RightToLeft, IterateDirection.RightToLeft);
			Assert.AreEqual(bytes, dest);
		}


		[Test]
		public void CopyBits_Integration_1() {
			var rng = new Random(31337);
			var bytes = rng.NextBytes(100);
			var tmp = new byte[100];
			var dest = new byte[100];
			var bitLength = bytes.Length * 8;
			Bits.CopyBits(bytes, 0, tmp, bitLength - 1, bitLength, IterateDirection.LeftToRight, IterateDirection.RightToLeft);
			Bits.CopyBits(tmp, bitLength - 1, dest, 0, bitLength, IterateDirection.RightToLeft, IterateDirection.LeftToRight);
			Assert.AreEqual(bytes, dest);
		}

        [Test]
		public void CopyBits_Integration_2() {
            const int Iterations = 100;
            const int Rounds = 100;
			var rng = new Random(31337);
            for (var i = 0; i < Iterations; i++) {
				var arr1 = rng.NextBytes(rng.Next(1, 1024));
                var arr2 = Tools.Array.Clone(arr1);
                var bitLength = arr1.Length * 8;
                for (var j = 0; j < Rounds; j++) {
                    var range = rng.RandomRange(bitLength);
                    var dir = rng.NextBool() ? IterateDirection.LeftToRight : IterateDirection.RightToLeft;
                    Bits.CopyBits(arr1, range.Start, arr2, range.Start, range.End - range.Start + 1, dir, dir);

					range = rng.RandomRange(bitLength);
					dir = rng.NextBool() ? IterateDirection.LeftToRight : IterateDirection.RightToLeft;
					Bits.CopyBits(arr2, range.Start, arr1, range.Start, range.End - range.Start + 1, dir, dir);

                }
                Assert.AreEqual(arr1, arr2);
            }
		}

		[Test]
		public void ReadWriteNumber_Integration_1() {
			const int Iterations = 100;
			const int Rounds = 100;
			var rng = new Random(31337);
			for (var i = 0; i < Iterations; i++) {
				var bytes = rng.NextBytes(rng.Next(1, 1024));
				var bitLength = bytes.Length * 8;
				for (var j = 0; j < Rounds; j++) {
					var bitsInNumber = rng.Next(16) + 1;
					var segment = rng.RandomSegment(bitLength, bitsInNumber);
					var dir = rng.NextBool() ? IterateDirection.LeftToRight : IterateDirection.RightToLeft;
					var offset = dir == IterateDirection.LeftToRight ? segment.Start : segment.End;
					
					// Read the number
					var number = Bits.ReadBinaryNumber(bytes, offset, bitsInNumber, dir);
					
					// Check not bigger than possible (this can happen if reading bits wrong)
					Assert.LessOrEqual(number, 1 << bitsInNumber);

					// Rebuild the array copy surrounding bits and writing the number where it was read
					var copy = new byte[bytes.Length];  //Tools.Array.Clone(bytes);
					Bits.CopyBits(bytes, 0, copy, 0, segment.Start, IterateDirection.LeftToRight, IterateDirection.LeftToRight);
					Bits.CopyBits(bytes, bitLength - 1, copy, bitLength - 1, bitLength - segment.End - 1, IterateDirection.RightToLeft, IterateDirection.RightToLeft);
					Bits.WriteBinaryNumber(number, copy, offset, bitsInNumber, dir);

					// Check unchanged
					Assert.AreEqual(bytes, copy);
				}
			}

        }

    }
}
