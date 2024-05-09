// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Hydrogen.NUnit;
using Hydrogen;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class FlatMerkleTreeTests {

	[Test]
	public void Integration_AddRange_Single([Values(CHF.SHA2_256)] CHF chf) {
		var rng = new Random(31337);
		var reference = new SimpleMerkleTree(chf);
		var impl = new FlatMerkleTree(chf);
		ClassicAssert.AreEqual(reference.Root, impl.Root);
		var newItems = rng.NextBytes(Hashers.GetDigestSizeBytes(chf));
		reference.Leafs.AddRange(newItems);
		impl.Leafs.AddRange(newItems);
		ClassicAssert.AreEqual(reference.Root, impl.Root);
		ClassicAssert.AreEqual(reference.Leafs.Count, impl.Leafs.Count);
		ClassicAssert.AreEqual(reference.Size, impl.Size);
	}

	[Test]
	public void Integration_AddRange_Simple([Values(CHF.SHA2_256)] CHF chf) {
		const int Items = 100;
		var rng = new Random(31337);
		var reference = new SimpleMerkleTree(chf);
		var impl = new FlatMerkleTree(chf);
		for (var i = 0; i < Items; i++) {
			var newItems = rng.NextBytes(Hashers.GetDigestSizeBytes(chf));
			reference.Leafs.AddRange(newItems);
			impl.Leafs.AddRange(newItems);
			ClassicAssert.AreEqual(reference.Root, impl.Root);
			ClassicAssert.AreEqual(impl.Leafs.Count, impl.Size.LeafCount);
		}
	}

	[Test]
	public void Integration_AddRange_Complex([Values(CHF.SHA2_256)] CHF chf) {
		const int MaxIterations = 1000;
		const int MinItemsAddPerIter = 0;
		const int MaxItemsAddPerIter = 10;

		var rng = new Random(31337);
		var reference = new SimpleMerkleTree(chf);
		var impl = new FlatMerkleTree(chf);
		ClassicAssert.AreEqual(reference.Root, impl.Root);
		for (var i = 0; i < MaxIterations; i++) {
			var newItems =
				Tools.Collection.Generate(() => rng.NextBytes(Hashers.GetDigestSizeBytes(chf))).Take(rng.Next(MinItemsAddPerIter, MaxItemsAddPerIter + 1)).ToArray();

			reference.Leafs.AddRange(newItems);
			impl.Leafs.AddRange(newItems);
			if (Tools.Maths.Gamble(0.1)) // force root calculation 10% of the time
				ClassicAssert.AreEqual(reference.Root, impl.Root);

			ClassicAssert.AreEqual(impl.Leafs.Count, impl.Size.LeafCount);

		}
		ClassicAssert.AreEqual(reference.Root, impl.Root);
	}

	[Test]
	public void IntegrationTests_1([Values(11, 111, 555)] int maxLeafs) {
		var RNG = new Random(1231 * maxLeafs);
		var tree = new FlatMerkleTree(CHF.SHA2_256, 0, 256, maxLeafs);
		var expected = new SimpleMerkleTree(CHF.SHA2_256);
		for (var i = 0; i < 100; i++) {
			// add a random amount
			var remainingCapacity = maxLeafs - tree.Leafs.Count;
			var newItemsCount = RNG.Next(0, (int)remainingCapacity + 1);
			var newItems = RNG.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), newItemsCount);
			tree.Leafs.AddRange(newItems);
			expected.Leafs.AddRange(newItems);
			AssertEx.AreEqual(expected, tree);

			if (tree.Leafs.Count > 0) {
				// update a random amount
				var range = RNG.NextRange((int)tree.Leafs.Count);
				newItems = RNG.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), range.End - range.Start + 1);
				tree.Leafs.UpdateRange(range.Start, newItems);
				expected.Leafs.UpdateRangeSequentially(range.Start, newItems);

				AssertEx.AreEqual(expected, tree);

				// remove a random amount
				range = RNG.NextRange((int)tree.Leafs.Count);
				tree.Leafs.RemoveRange(range.Start, range.End - range.Start + 1);
				expected.Leafs.RemoveRange(range.Start, range.End - range.Start + 1);

				AssertEx.AreEqual(expected, tree);
			}

			// insert a random amount
			remainingCapacity = maxLeafs - tree.Leafs.Count;
			newItems = RNG.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), 0, (int)remainingCapacity + 1);
			var insertIX = RNG.Next(0, (int)tree.Leafs.Count);
			tree.Leafs.InsertRange(insertIX, newItems);
			expected.Leafs.InsertRange(insertIX, newItems);

			AssertEx.AreEqual(expected, tree);

		}
	}

	[Test]
	public void IntegrationTests_2([Values(5555)] int maxLeafs) {
		var RNG = new Random(1231 * maxLeafs);
		var tree = new FlatMerkleTree(CHF.SHA2_256, 0, 256, maxLeafs);
		var expected = new SimpleMerkleTree(CHF.SHA2_256);
		for (var i = 0; i < 100; i++) {
			// add a random amount
			var remainingCapacity = maxLeafs - tree.Leafs.Count;
			var newItemsCount = RNG.Next(0, (int)remainingCapacity + 1);
			var newItems = RNG.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), newItemsCount);
			tree.Leafs.AddRange(newItems);
			expected.Leafs.AddRange(newItems);

			if (tree.Leafs.Count > 0) {
				// update a random amount
				var range = RNG.NextRange((int)tree.Leafs.Count);
				newItems = RNG.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), range.End - range.Start + 1);
				tree.Leafs.UpdateRange(range.Start, newItems);
				expected.Leafs.UpdateRangeSequentially(range.Start, newItems);

				// remove a random amount
				range = RNG.NextRange((int)tree.Leafs.Count);
				tree.Leafs.RemoveRange(range.Start, range.End - range.Start + 1);
				expected.Leafs.RemoveRange(range.Start, range.End - range.Start + 1);
			}

			// insert a random amount
			remainingCapacity = maxLeafs - tree.Leafs.Count;
			newItems = RNG.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), 0, (int)remainingCapacity + 1);
			var insertIX = RNG.Next(0, (int)tree.Leafs.Count);
			tree.Leafs.InsertRange(insertIX, newItems);
			expected.Leafs.InsertRange(insertIX, newItems);
		}

		// Test at end only
		AssertEx.AreEqual(expected, tree);
	}

	[Test]
	public void Insert_1() {
		var rng = new Random(31337);
		var list = new FlatMerkleTree(CHF.SHA2_256);
		var expected = new List<byte[]>();

		var digest = rng.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), 1)[0];
		expected.Insert(0, digest);
		list.Leafs.Insert(0, digest);
		ClassicAssert.AreEqual(expected.ToArray(), list.Leafs.ToArray());
	}

	[Test]
	public void Insert_2() {
		var rng = new Random(31337);
		var list = new FlatMerkleTree(CHF.SHA2_256);
		var expected = new List<byte[]>();

		var digest = rng.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), 2);
		expected.Insert(0, digest[0]);
		list.Leafs.Insert(0, digest[0]);

		expected.Insert(0, digest[1]);
		list.Leafs.Insert(0, digest[1]);

		ClassicAssert.AreEqual(expected.ToArray(), list.Leafs.ToArray());
	}

	[Test]
	public void Insert_3() {
		var rng = new Random(31337);
		var list = new FlatMerkleTree(CHF.SHA2_256);
		var expected = new List<byte[]>();

		var digest = rng.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), 3);
		expected.Insert(0, digest[0]);
		list.Leafs.Insert(0, digest[0]);

		expected.Insert(0, digest[2]);
		list.Leafs.Insert(0, digest[2]);

		expected.Insert(1, digest[1]);
		list.Leafs.Insert(1, digest[1]);

		ClassicAssert.AreEqual(expected.ToArray(), list.Leafs.ToArray());
	}

	[Test]
	public void RemoveAt_1([Values(3, 4)] int leafCount) {
		var rng = new Random(31337);
		var list = new FlatMerkleTree(CHF.SHA2_256);
		var expected = new List<byte[]>();

		var digests = rng.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), leafCount);
		expected.AddRange(digests);
		list.Leafs.AddRange(digests);

		expected.RemoveAt(1);
		list.Leafs.RemoveAt(1);

		ClassicAssert.AreEqual(expected.ToArray(), list.Leafs.ToArray());
	}


	[Test]
	public void RemoveAt_2([Values(3, 4)] int leafCount) {
		var rng = new Random(31337);
		var list = new FlatMerkleTree(CHF.SHA2_256);
		var expected = new List<byte[]>();

		var digests = rng.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), leafCount);
		expected.AddRange(digests);
		list.Leafs.AddRange(digests);

		expected.RemoveAt(2);
		list.Leafs.RemoveAt(2);

		ClassicAssert.AreEqual(expected.ToArray(), list.Leafs.ToArray());
	}

	[Test]
	public void TestNodes_BottomUp(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(0, 1, 11, 33, 1000)] int leafCount) {
		var rng = new Random(31337);
		var leafs = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);
		var expected = new SimpleMerkleTree(chf, leafs);
		var actual = new FlatMerkleTree(chf, leafs);

		for (var i = 0; i < MerkleMath.CalculateHeight(leafCount); i++) {
			for (var j = 0; j < MerkleMath.CalculateLevelLength(leafCount, i); j++) {
				var e = expected.GetValue(MerkleCoordinate.From(i, j)).ToArray();
				var a = actual.GetValue(MerkleCoordinate.From(i, j)).ToArray();
				ClassicAssert.AreEqual(e, a);
			}
		}
	}

	[Test]
	public void TestNodes_TopDown(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(0, 1, 11, 33, 1000)] int leafCount) {
		var rng = new Random(31337);
		var leafs = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);
		var expected = new SimpleMerkleTree(chf, leafs);
		var actual = new FlatMerkleTree(chf, leafs);

		for (var i = MerkleMath.CalculateHeight(leafCount) - 1; i >= 0; i--) {
			for (var j = MerkleMath.CalculateLevelLength(leafCount, i) - 1; j >= 0; j--) {
				var e = expected.GetValue(MerkleCoordinate.From(i, j)).ToArray();
				var a = actual.GetValue(MerkleCoordinate.From(i, j)).ToArray();
				ClassicAssert.AreEqual(e, a);
			}
		}
	}

	[Test]
	public void TestBug_1() {
		var rng = new Random(31337);
		var items = rng.NextByteArrays(Hashers.GetDigestSizeBytes(CHF.SHA2_256), 2);
		var tree = new FlatMerkleTree(CHF.SHA2_256, items);
		tree.Leafs.RemoveRange(2, 0);
		Assert.DoesNotThrow(() => {
			var _ = tree.Root;
		});
	}

	[Test]
	public void EmptyRootDoesntThrow() {
		var flatMerkle = new FlatMerkleTree(CHF.SHA2_256);
		Assert.That(() => flatMerkle.Root, Throws.Nothing);
	}

	[Test]
	public void EmptyRootIsNull() {
		var flatMerkle = new FlatMerkleTree(CHF.SHA2_256);
		Assert.That(flatMerkle.Root, Is.Null);
	}
}
