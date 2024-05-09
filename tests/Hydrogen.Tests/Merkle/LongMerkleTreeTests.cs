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
using Hydrogen;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class LongMerkleTreeTests {

	[Test]
	public void Integration_AddRange_Single([Values(CHF.SHA2_256)] CHF chf) {
		var rng = new Random(31337);
		var reference = new SimpleMerkleTree(chf);
		var impl = new LongMerkleTree(chf);
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
		var impl = new LongMerkleTree(chf);
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
		var impl = new LongMerkleTree(chf);
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
	public void GetNodeAt() {
		var rng = new Random(31337);
		var tree = new LongMerkleTree(CHF.SHA2_256);
		for (var i = 0; i < 200; i++) {
			for (var y = 0; y < tree.Size.Height; y++) {
				var levelLength = MerkleMath.CalculateLevelLength(tree.Size.LeafCount, y);
				for (var x = 0; x < levelLength; x++) {
					var coord = MerkleCoordinate.From(y, x);
					if (!MerkleMath.IsPerfectNode(tree.Size, coord) || MerkleMath.CalculateSubRoots(tree.Size.LeafCount).Contains(coord))
						Assert.DoesNotThrow(() => tree.GetNodeAt(coord));
					else
						Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(coord));

				}
			}
			tree.Leafs.Add(rng.NextBytes(32));
		}
	}

	[Test]
	public void GetNodeAt_7() {
		var rng = new Random(31337);
		var tree = new LongMerkleTree(CHF.SHA2_256);
		tree.Leafs.AddRange(rng.NextByteArrays(Hashers.GetDigestSizeBytes(tree.HashAlgorithm), 7));

		// Level 0
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(0, 0)));
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(0, 1)));
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(0, 2)));
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(0, 3)));
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(0, 4)));
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(0, 5)));
		Assert.DoesNotThrow(() => tree.GetNodeAt(MerkleCoordinate.From(0, 6)));

		// Level 1
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(1, 0)));
		Assert.Throws<InvalidOperationException>(() => tree.GetNodeAt(MerkleCoordinate.From(1, 1)));
		Assert.DoesNotThrow(() => tree.GetNodeAt(MerkleCoordinate.From(1, 2)));
		Assert.DoesNotThrow(() => tree.GetNodeAt(MerkleCoordinate.From(1, 3)));

		// Level 2
		Assert.DoesNotThrow(() => tree.GetNodeAt(MerkleCoordinate.From(2, 0)));
		Assert.DoesNotThrow(() => tree.GetNodeAt(MerkleCoordinate.From(2, 1)));


		// Level 3
		Assert.DoesNotThrow(() => tree.GetNodeAt(MerkleCoordinate.From(3, 0)));
	}
}
