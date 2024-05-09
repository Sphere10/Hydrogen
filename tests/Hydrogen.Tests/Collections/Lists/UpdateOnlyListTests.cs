// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;
using NUnit.Framework;
using Hydrogen.NUnit;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

public class UpdateOnlyListTests {

	[Test]
	public void AddRange() {
		var list = new UpdateOnlyList<int>(10, () => default);
		ClassicAssert.IsTrue(list.All(x => x == default));

		list.AddRange(Enumerable.Range(0, 5));
		list.AddRange(Enumerable.Range(5, 5));

		ClassicAssert.AreEqual(Enumerable.Range(0, 10).ToList(), list);
	}

	[Test]
	public void Insert_Basic() {
		var store = new ExtendedList<int>(new[] { 0, 0, 0 });
		var list = new UpdateOnlyList<int>(store, 0, PreAllocationPolicy.Fixed, 3, () => default);
		list.AddRange(new[] { 1, 3 });
		list.Insert(1, 2);
		ClassicAssert.AreEqual(new[] { 1, 2, 3 }, list);
	}

	[Test]
	public void InsertAtIndex() {
		int[] input = Enumerable.Repeat(0, 10).ToArray();
		ExtendedList<int> list = new ExtendedList<int>(input);
		UpdateOnlyList<int> preallocatedList = new UpdateOnlyList<int>(list, 0, PreAllocationPolicy.Fixed, 10, () => default);
		preallocatedList.InsertRange(0, input.Reverse());
		ClassicAssert.AreEqual(input.Reverse(), list);
	}

	[Test]
	public void RemoveAtIndex() {
		int[] expected = Enumerable.Repeat(0, 10).ToArray();

		ExtendedList<int> list = new ExtendedList<int>(expected);
		UpdateOnlyList<int> preallocatedList = new UpdateOnlyList<int>(list, 0, PreAllocationPolicy.Fixed, 10, () => default);

		preallocatedList.RemoveRange(0, preallocatedList.Count);

		ClassicAssert.IsTrue(preallocatedList.All(x => x == default));
	}

	[Test]
	public void IntegrationTests_Fixed([Values(1, 793, 2000)] int maxCapacity) {
		var fixedStore = new ExtendedList<int>(Tools.Array.Gen(maxCapacity, 0));
		var list = new UpdateOnlyList<int>(fixedStore, 0, PreAllocationPolicy.Fixed, maxCapacity, () => default);
		AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
	}

	[Test]
	public void IntegrationTests_ByBlock([Values(0, 1, 793, 2000)] int maxCapacity) {
		var list = new UpdateOnlyList<int>(PreAllocationPolicy.ByBlock, 5, () => default);
		AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
	}

	[Test]
	public void IntegrationTests_MinimumRequired([Values(0, 1, 793, 2000)] int maxCapacity) {
		var list = new UpdateOnlyList<int>(PreAllocationPolicy.MinimumRequired, 0, () => default);
		AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
	}

}
