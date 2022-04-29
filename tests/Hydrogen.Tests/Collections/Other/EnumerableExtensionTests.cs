//-----------------------------------------------------------------------
// <copyright file="EnumerableExtensionTests.cs" company="Sphere 10 Software">
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
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Linq;
using Sphere10.Framework;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class EnumerableExtensionTests {

        [Test]
        public void TestOrderByAll() {
            var arr2d = new[] {
                new object[] {"B", 1},
                new object[] {"B", 3},
                new object[] {"C", 1},
                new object[] {"A", 3},
                new object[] {"A", 1},                
                new object[] {"A", 2},
                new object[] {"C", 2},                
                new object[] {"C", 3},                
                new object[] {"B", 2},
            };

            var expected = new[] {
                new object[] {"A", 1},
                new object[] {"A", 2},
                new object[] {"A", 3},
                new object[] {"B", 1},
                new object[] {"B", 2},                
                new object[] {"B", 3},
                new object[] {"C", 1},                
                new object[] {"C", 2},                
                new object[] {"C", 3},
            };

            var actual = arr2d.OrderByAll();

            for (var i = 0; i < 6; i++) {
                for (var j = 0; j < 2; j++) {
                    Assert.AreEqual(expected[i][j], actual.ElementAt(i).ElementAt(j));
                }
            }

        }

		[Test]
		[TestCase(1, 1, 1)]
		[TestCase(10, 3, 4)]
		public void PartitionBySize(int length, int partSize, int expectedParts) {
			var rando = new Random(31337);

			var input = rando.NextBytes(length);
			var parts = input.PartitionBySize(x => 1, partSize);
			
			Assert.AreEqual(expectedParts, parts.Count());
			Assert.AreEqual(input, parts.Aggregate((x, y) => x.Concat(y)));
		}
    }
}
