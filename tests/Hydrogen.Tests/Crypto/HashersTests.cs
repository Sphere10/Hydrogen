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
public class HashersTests {

	[Test]
	public void Aggregate_Empty() {
		Assert.Throws<ArgumentException>(() => Hashers.Aggregate(CHF.SHA2_256, Enumerable.Empty<byte[]>()));
	}

	[Test]
	public void Aggregate([Values(1, 2, 3, 100)] int n) {
		var rng = new Random(31337 * (n + 1));
		var items = Tools.Collection.Generate(() => rng.NextBytes(32)).Take(n).ToArray();

		var expected = items[0];
		foreach (var item in items.Skip(1))
			expected = Hashers.JoinHash(CHF.SHA2_256, expected, item);

		ClassicAssert.AreEqual(expected, Hashers.Aggregate(CHF.SHA2_256, items));
	}

}
