// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ListAdapterTests {

	[Test]
	public void IntegrationTests(
		[Values(10, 793, 2000)] int maxCapacity) {
		var list = new ExtendedListAdapter<int>(new List<int>(maxCapacity));
		AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
	}
}
