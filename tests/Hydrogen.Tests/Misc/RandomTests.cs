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
public class RandomTests {

	[Test]
	public void Range_0() {
		var rng = new Random(31337);
		Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextRange(0, false));
		Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextRange(0, true));
		Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextRange(0));
	}

	[Test]
	public void Range_1() {
		var rng = new Random(31337);
		var range = rng.NextRange(1, rangeLength: 1);
		ClassicAssert.AreEqual(0, range.Start);
		ClassicAssert.AreEqual(0, range.End);
	}

	[Test]
	public void Range_2() {
		var rng = new Random(31337);
		var range = rng.NextRange(2, true, rangeLength: 2);
		ClassicAssert.AreEqual(0, range.Start);
		ClassicAssert.AreEqual(1, range.End);

		range = rng.NextRange(2, false, rangeLength: 2);
		ClassicAssert.AreEqual(0, range.Start);
		ClassicAssert.AreEqual(1, range.End);

	}

	[Test]
	public void Range_Empty() {
		var rng = new Random(31337);
		var range = rng.NextRange(1, rangeLength: 0);
		ClassicAssert.AreEqual(0, range.Start);
		ClassicAssert.AreEqual(0, range.End);
	}

	//[Test]
	//public void RandomLong([Values(100000)] int iterations) {
	//	var rng = new Random(31337);
	//	var expected = Enumerable.Range(19, 8).Select(x => (long)x).ToArray();
	//	var tally19 = 0L;
	//	var tally20 = 0L;
	//	var tally21 = 0L;
	//	var tally22 = 0L;
	//	var tally23 = 0L;
	//	var tally24 = 0L;
	//	var tally25 = 0L;
	//	var tally26 = 0L;
	//	for (var i = 0; i < iterations; i++) {
	//		var actual = rng.NextLong(19, 27);
	//		Assert.That(expected, Contains.Item(actual));
	//		switch (actual) {
	//			case 19:
	//				tally19++;
	//				break;
	//			case 20:
	//				tally20++;
	//				break;
	//			case 21:
	//				tally21++;
	//				break;
	//			case 22:
	//				tally22++;
	//				break;
	//			case 23:
	//				tally23++;
	//				break;
	//			case 24:
	//				tally24++;
	//				break;
	//			case 25:
	//				tally25++;
	//				break;
	//			case 26:
	//				tally26++;
	//				break;
	//			default:
	//				throw new InvalidOperationException();
	//		}

	//	}
	//}
}
