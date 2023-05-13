// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;

namespace Hydrogen.Tests {

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
            var range = rng.NextRange(1, rangeLength:1);
            Assert.AreEqual(0, range.Start);
            Assert.AreEqual(0, range.End);
        }

        [Test]
        public void Range_2() {
            var rng = new Random(31337);
            var range = rng.NextRange(2, true, rangeLength:2);
            Assert.AreEqual(0, range.Start);
            Assert.AreEqual(1, range.End);

            range = rng.NextRange(2, false, rangeLength:2);
            Assert.AreEqual(0, range.Start);
            Assert.AreEqual(1, range.End);

        }

        [Test]
        public void Range_Empty() {
	        var rng = new Random(31337);
	        var range = rng.NextRange(1, rangeLength: 0);
	        Assert.AreEqual(0, range.Start);
	        Assert.AreEqual(0, range.End);
        }

	}
}
