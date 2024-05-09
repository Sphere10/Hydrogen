// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class EnumerableExtensionTests {

	[Test]
	public void TestOrderByAll() {
		var arr2d = new[] {
			new object[] { "B", 1 },
			new object[] { "B", 3 },
			new object[] { "C", 1 },
			new object[] { "A", 3 },
			new object[] { "A", 1 },
			new object[] { "A", 2 },
			new object[] { "C", 2 },
			new object[] { "C", 3 },
			new object[] { "B", 2 },
		};

		var expected = new[] {
			new object[] { "A", 1 },
			new object[] { "A", 2 },
			new object[] { "A", 3 },
			new object[] { "B", 1 },
			new object[] { "B", 2 },
			new object[] { "B", 3 },
			new object[] { "C", 1 },
			new object[] { "C", 2 },
			new object[] { "C", 3 },
		};

		var actual = arr2d.OrderByAll();

		for (var i = 0; i < 6; i++) {
			for (var j = 0; j < 2; j++) {
				ClassicAssert.AreEqual(expected[i][j], actual.ElementAt(i).ElementAt(j));
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

		ClassicAssert.AreEqual(expectedParts, parts.Count());
		ClassicAssert.AreEqual(input, parts.Aggregate((x, y) => x.Concat(y)));
	}
}
