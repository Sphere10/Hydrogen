// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Hydrogen.NUnit;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

public class BitVectorTests {

	[Test]
	public void InsertRangeEnd() {
		var random = new Random(31337);
		using var memoryStream = new MemoryStream();
		var list = new BitVector(memoryStream);

		var inputs = Enumerable.Repeat(true, 20).ToArray();
		list.AddRange(inputs);

		var insert = Enumerable.Repeat(false, 20).ToArray();
		list.InsertRange(20, insert);

		ClassicAssert.AreEqual(inputs.Concat(insert), list);
	}

	[Test]
	public void ReadRange() {
		var random = new Random(31337);
		using var memoryStream = new MemoryStream();
		var list = new BitVector(memoryStream);

		var inputs = random.NextBools(16);
		list.AddRange(inputs);
		ClassicAssert.AreEqual(inputs, list);

		var range = list.ReadRange(9, 7)
			.ToList();

		ClassicAssert.AreEqual(inputs[9..], range);
	}

	[Test]
	public void IndexOfRange() {
		var random = new Random(31337);
		using var memoryStream = new MemoryStream();
		var list = new BitVector(memoryStream);

		var inputs = new[] { false, false, false, false, false, false, false, false, true };
		list.AddRange(inputs);

		ClassicAssert.AreEqual(new[] { 8, 8, 8 }, list.IndexOfRange(new[] { true, true, true }));
		ClassicAssert.AreEqual(new[] { 7 }, list.IndexOfRange(new[] { false }));
	}

	[Test]
	public void RemoveRange() {
		using var memoryStream = new MemoryStream();
		var list = new BitVector(memoryStream);

		var inputs = new[] { false, false, false, false, false, false, false, false, true };

		list.AddRange(inputs);
		list.RemoveRange(8, 1);
		ClassicAssert.AreEqual(8, list.Count);
		ClassicAssert.AreEqual(inputs[..^1], list);

		list.RemoveRange(0, list.Count);
		ClassicAssert.AreEqual(0, list.Count);
	}

	[Test]
	public void UpdateRange() {
		var random = new Random(31337);
		using var memoryStream = new MemoryStream();
		var list = new BitVector(memoryStream);
		var expected = new ExtendedList<bool>();

		var inputs = random.NextBools(100);
		var update = random.NextBools(inputs.Length);

		list.AddRange(inputs);
		expected.AddRange(inputs);

		list.UpdateRange(0, update);
		expected.UpdateRange(0, update);

		ClassicAssert.AreEqual(expected, list);

		int randomIndex = random.Next(0, (int)list.Count - 1);
		var randomUpdate = random.NextBools((int)list.Count - randomIndex);

		list.UpdateRange(randomIndex, randomUpdate);
		expected.UpdateRange(randomIndex, randomUpdate);

		ClassicAssert.AreEqual(expected, list);
	}

	[Test]
	public void IntegrationTest() {
		using var memoryStream = new MemoryStream();
		var list = new BitVector(memoryStream);
		AssertEx.ListIntegrationTest(list, 1000, (Random, i) => Random.NextBools(i), true);
	}
}
