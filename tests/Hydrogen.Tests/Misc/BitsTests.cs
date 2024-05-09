// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class BitsTests {

	[Test]
	public void CopyBits_L2R() {
		var rng = new Random(31337);
		var bytes = rng.NextBytes(100);
		var dest = new byte[100];
		var bitLength = bytes.Length * 8;
		Bits.CopyBits(bytes, 0, dest, 0, bitLength);
		ClassicAssert.AreEqual(bytes, dest);
	}

	[Test]
	public void CopyBits_R2L() {
		var rng = new Random(31337);
		var bytes = rng.NextBytes(100);
		var dest = new byte[100];
		var bitLength = bytes.Length * 8;
		Bits.CopyBits(bytes, bitLength - 1, dest, bitLength - 1, bitLength, IterateDirection.RightToLeft, IterateDirection.RightToLeft);
		ClassicAssert.AreEqual(bytes, dest);
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
		ClassicAssert.AreEqual(bytes, dest);
	}

	[Test]
	public void SetBit_1() {

		var buffer = new byte[2];
		Bits.SetBit(buffer, 7, true);
		Bits.SetBit(buffer, 15, true);

		ClassicAssert.AreEqual(1, buffer[0]);
		ClassicAssert.AreEqual(1, buffer[1]);
	}

	[Test]
	public void SetBit_2() {
		var buffer = new byte[] { 1, 128 };
		Bits.SetBit(buffer, 7, false);
		Bits.SetBit(buffer, 0, true);

		Bits.SetBit(buffer, 8, false);
		Bits.SetBit(buffer, 15, true);

		ClassicAssert.AreEqual(128, buffer[0]);
		ClassicAssert.AreEqual(1, buffer[1]);
	}

	[Test]
	public void SetBit_3() {
		var buffer = new byte[1];
		Bits.SetBit(buffer, 0, true);
		ClassicAssert.AreEqual(128, (int)buffer[0]);
	}

	[Test]
	public void ReadBit_1() {
		var buffer = new byte[] { 129, 128, 255 };

		ClassicAssert.IsTrue(Bits.ReadBit(buffer, 0));
		ClassicAssert.IsTrue(Bits.ReadBit(buffer, 7));
		ClassicAssert.IsTrue(Bits.ReadBit(buffer, 8));
		ClassicAssert.IsFalse(Bits.ReadBit(buffer, 15));

		bool allTrue = Enumerable.Range(16, 7)
			.Select(x => Bits.ReadBit(buffer, x))
			.All(x => x);

		ClassicAssert.IsTrue(allTrue);
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
				var range = rng.NextRange(bitLength);
				var dir = rng.NextBool() ? IterateDirection.LeftToRight : IterateDirection.RightToLeft;
				Bits.CopyBits(arr1, range.Start, arr2, range.Start, range.End - range.Start + 1, dir, dir);

				range = rng.NextRange(bitLength);
				dir = rng.NextBool() ? IterateDirection.LeftToRight : IterateDirection.RightToLeft;
				Bits.CopyBits(arr2, range.Start, arr1, range.Start, range.End - range.Start + 1, dir, dir);

			}
			ClassicAssert.AreEqual(arr1, arr2);
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
				var segment = rng.NextRange(bitLength, rangeLength: bitsInNumber);
				var dir = rng.NextBool() ? IterateDirection.LeftToRight : IterateDirection.RightToLeft;
				var offset = dir == IterateDirection.LeftToRight ? segment.Start : segment.End;

				// Read the number
				var number = Bits.ReadBinaryNumber(bytes, offset, bitsInNumber, dir);

				// Check not bigger than possible (this can happen if reading bits wrong)
				ClassicAssert.LessOrEqual(number, 1 << bitsInNumber);

				// Rebuild the array copy surrounding bits and writing the number where it was read
				var copy = new byte[bytes.Length]; //Tools.Array.Clone(bytes);
				Bits.CopyBits(bytes, 0, copy, 0, segment.Start, IterateDirection.LeftToRight, IterateDirection.LeftToRight);
				Bits.CopyBits(bytes, bitLength - 1, copy, bitLength - 1, bitLength - segment.End - 1, IterateDirection.RightToLeft, IterateDirection.RightToLeft);
				Bits.WriteBinaryNumber(number, copy, offset, bitsInNumber, dir);

				// Check unchanged
				ClassicAssert.AreEqual(bytes, copy);
			}
		}

	}

}
