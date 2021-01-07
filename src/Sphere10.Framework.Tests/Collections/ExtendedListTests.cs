//-----------------------------------------------------------------------
// <copyright file="LargeCollectionTests.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Sphere10.Framework;

namespace Sphere10.Framework.Tests {


    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ExtendedListTests {

        [Test]
        [Pairwise]
        public void IntegrationTests(
            [Values(0, 3,  111)] int startCapacity,
            [Values(19, 7, 13)] int growCapacity,
            [Values(10, 793, 2000)] int maxCapacity) {
            var RNG = new Random(1231);
            var list = new ExtendedList<int>(startCapacity, growCapacity, maxCapacity);
            var expected = new List<int>();
            for (var i = 0; i < 100; i++) {

                // add a random amount
                var remainingCapacity = list.MaxCapacity - list.Count;
                var newItemsCount = RNG.Next(0, remainingCapacity + 1);
                IEnumerable<int> newItems = RNG.NextInts(newItemsCount);
                list.AddRange(newItems);
                expected.AddRange(newItems);
                Assert.AreEqual(expected, list);

                if (list.Count > 0) {
                    // update a random amount
                    var range = RNG.RandomRange(list.Count);
                    newItems = RNG.NextInts(range.End - range.Start + 1);
                    list.UpdateRange(range.Start, newItems);
                    expected.UpdateRangeSequentially(range.Start, newItems);
                    Assert.AreEqual(expected, list);

                    // shuffle a random amount
                    range = RNG.RandomRange(list.Count);
                    newItems = list.ReadRange(range.Start, range.End - range.Start + 1);
                    var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);
                    range = RNG.RandomSegment(list.Count, newItems.Count());
                    expected.UpdateRangeSequentially(range.Start, expectedNewItems);
                    list.UpdateRange(range.Start, newItems);

                    Assert.AreEqual(expected.Count, list.Count);
                    Assert.AreEqual(expected, list);

                    // remove a random amount
                    range = RNG.RandomRange(list.Count);
                    list.RemoveRange(range.Start, range.End - range.Start + 1);
                    expected.RemoveRange(range.Start, range.End - range.Start + 1);
                    Assert.AreEqual(expected, list);
                }

                // insert a random amount
                remainingCapacity = list.MaxCapacity - list.Count;
                newItemsCount = RNG.Next(0, remainingCapacity + 1);
                newItems = RNG.NextInts(newItemsCount);
                var insertIX = RNG.Next(0, list.Count);
                list.InsertRange(insertIX, newItems);
                expected.InsertRange(insertIX, newItems);
                Assert.AreEqual(expected, list);
            }
        }
    }
}
