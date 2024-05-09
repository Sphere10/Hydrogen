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
using System.Text;
using Hydrogen;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MerkleTreeTests {

	[Test]
	public void NullRoot(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat, MerkleTreeImpl.Long)]
		MerkleTreeImpl impl) {
		var tree = CreateMerkleTree(impl, chf);
		ClassicAssert.AreEqual(MerkleSize.FromLeafCount(0), tree.Size);
		ClassicAssert.AreEqual(null, tree.Root);
	}

	[Test]
	public void SingleRoot(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat, MerkleTreeImpl.Long)]
		MerkleTreeImpl impl) {
		var rng = new Random(31337);
		var tree = CreateMerkleTree(impl, chf);
		var data = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), 1)[0];
		tree.Leafs.Add(data);
		ClassicAssert.AreEqual(data, tree.Root);
	}

	[Test]
	public void PastRoots(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl) {
		const int Items = 1000;
		var rng = new Random(31337);
		var tree = CreateMerkleTree(impl, chf);
		var roots = new List<byte[]>();

		for (var i = 0; i < Items; i++) {
			tree.Leafs.Add(rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), 1)[0]);
			roots.Add(tree.Root);
		}

		for (var i = 0; i < Items; i++) {
			ClassicAssert.AreEqual(roots[i], tree.CalculateOldRoot(i + 1));
		}
	}

	[Test]
	public void Update_1(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl) {
		var tree = CreateMerkleTree(impl, chf);
		var rng = new Random(31337);
		// add item
		tree.Leafs.Add(Hashers.Hash(chf, rng.NextBytes(100)));
		ClassicAssert.AreEqual(tree.Leafs[0], tree.Root);

		// update it
		var datum = Hashers.Hash(chf, rng.NextBytes(100));
		tree.Leafs.Update(0, datum);
		ClassicAssert.AreEqual(datum, tree.Leafs[0]);
		ClassicAssert.AreEqual(datum, tree.Root);
	}

	[Test]
	public void ExistenceProof_2(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl) {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		var elemsHash = elems.Select(Encoding.ASCII.GetBytes).Select(x => Hashers.Hash(chf, x)).ToArray();
		var tree = CreateMerkleTree(impl, chf);

		foreach (var elem in elemsHash) {
			tree.Leafs.Add(elem);
			for (var i = 0; i < tree.Leafs.Count; i++) {
				var proof = tree.GenerateExistenceProof(i).ToArray();
				ClassicAssert.IsTrue(MerkleMath.VerifyExistenceProof(chf, tree.Root, tree.Size, i, elemsHash[i], proof));
			}
		}

	}
	[Test]
	public void ConsistencyProof(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl,
		[Values(0, 1, 11, 55)] int leafCount) {
		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		for (var i = 0; i < elems.Length; i++) {
			var oldTree = CreateMerkleTree(impl, chf);
			oldTree.Leafs.AddRange(elems.Take(i));

			// Add ranges of all combinations
			for (var j = i + 1; j < elems.Length; j++) {
				// duplicate old tree
				var newTree = CreateMerkleTree(impl, chf);
				newTree.Leafs.AddRange(oldTree.Leafs);

				// Add new (j-i) items
				newTree.Leafs.AddRange(elems.Skip(i).Take(j - i));

				var consistencyProof = newTree.GenerateConsistencyProof(i);
				var verifyProof = MerkleMath.VerifyConsistencyProof(chf, oldTree.Root, i, newTree.Root, i + (j - i), consistencyProof);

				ClassicAssert.IsTrue(verifyProof);
			}
		}
	}

	[Test]
	public void ContainsProof_Range(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl,
		[Values(0, 1, 11, 33)] int leafCount) {
		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		var tree = CreateMerkleTree(impl, chf);
		tree.Leafs.AddRange(elems.Take(leafCount));

		for (var i = 0; i < leafCount; i++) {
			// Add ranges of all combinations
			for (var j = i; j < leafCount; j++) {
				var leafs = tree.Leafs.Skip(i).Take(j - i).ToArray();
				var proof = tree.GenerateContainsProof(i, j - i);
				var verify = MerkleMath.VerifyContainsProof(chf, tree.Root, tree.Size, i, leafs, proof);

				ClassicAssert.IsTrue(verify);
			}
		}
	}

	[Test]
	public void ContainsProof_Random(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl,
		[Values(0, 1, 11, 33)] int leafCount) {
		const int Samples = 100;
		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		var tree = CreateMerkleTree(impl, chf);
		tree.Leafs.AddRange(elems.Take(leafCount));

		for (var i = 0; i < Samples; i++) {
			var indices = Enumerable.Range(0, rng.Next(0, leafCount)).Select(x => (long)rng.Next(0, leafCount)).Distinct().ToArray();
			var leafs = indices.Select(i => elems[i]).ToArray();
			var proof = tree.GenerateContainsProof(indices);
			var verify = MerkleMath.VerifyContainsProof(chf, tree.Root, tree.Size, indices.Zip(leafs, Tuple.Create), proof);
			ClassicAssert.IsTrue(verify);
		}
	}

	[Test]
	public void UpdateProof_Range(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl,
		[Values(0, 1, 11, 33)] int leafCount) {
		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		var oldTree = CreateMerkleTree(impl, chf);
		oldTree.Leafs.AddRange(elems);

		for (var i = 0; i < elems.Length; i++) {
			// update ranges of all combinations
			for (var j = i; j < elems.Length; j++) {
				// duplicate old tree
				var newTree = CreateMerkleTree(impl, chf);
				newTree.Leafs.AddRange(oldTree.Leafs);

				// update j-i items
				var updatedItems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), j - i);
				newTree.Leafs.UpdateRange(i, updatedItems);
				var newRoot = newTree.Root;

				var updateProof = oldTree.GenerateUpdateProof(i, j - i);
				var verifyProof = MerkleMath.VerifyUpdateProof(chf, oldTree.Root, oldTree.Size, newRoot, i, updatedItems, updateProof);

				ClassicAssert.IsTrue(verifyProof);
			}
		}
	}

	[Test]
	public void UpdateProof_Random(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl,
		[Values(0, 1, 11, 33)] int leafCount) {
		const int Samples = 100;
		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		var oldTree = CreateMerkleTree(impl, chf);
		oldTree.Leafs.AddRange(elems);

		for (var i = 0; i < Samples; i++) {
			var indices = Enumerable.Range(0, rng.Next(0, leafCount)).Select(x => (long)rng.Next(0, leafCount)).Distinct().ToArray();
			var oldLeafs = indices.Select(i => elems[i]).ToArray();
			var newLeafs = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), oldLeafs.Length);

			// duplicate old tree
			var newTree = CreateMerkleTree(impl, chf, oldTree.Leafs);
			indices.Zip(newLeafs).ForEach(x => newTree.Leafs.Update(x.Item1, x.Item2));

			// Gen proof
			var updateProof = oldTree.GenerateUpdateProof(indices);
			var verifyProof = MerkleMath.VerifyUpdateProof(chf, oldTree.Root, oldTree.Size, newTree.Root, indices.Zip(newLeafs), updateProof);
			ClassicAssert.IsTrue(verifyProof);
		}
	}

	[Test]
	public void AppendProof(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat, MerkleTreeImpl.Long)]
		MerkleTreeImpl impl,
		[Values(0, 1, 11, 33, 1000)] int leafCount,
		[Values(0, 1, 11, 33, 1000)] int appendSize) {

		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		var oldTree = CreateMerkleTree(impl, chf);
		oldTree.Leafs.AddRange(elems);

		// duplicate old tree
		var newTree = CreateMerkleTree(impl, chf, oldTree.Leafs);
		var newLeafs = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), appendSize);
		newTree.Leafs.AddRange(newLeafs);

		// Gen proof
		var appendProof = oldTree.GenerateAppendProof();
		var verifyProof = MerkleMath.VerifyAppendProof(chf, oldTree.Root, newTree.Root, oldTree.Size, newLeafs, appendProof);
		ClassicAssert.IsTrue(verifyProof);
	}

	[Test]
	public void DeleteProof(
		[Values(CHF.SHA2_256)] CHF chf,
		[Values(MerkleTreeImpl.Simple, MerkleTreeImpl.Flat)]
		MerkleTreeImpl impl,
		[Values(0, 1, 2, 11, 33, 1000)] int leafCount) {

		var rng = new Random(31337);
		var elems = rng.NextByteArrays(Hashers.GetDigestSizeBytes(chf), leafCount);

		var oldTree = CreateMerkleTree(impl, chf);
		oldTree.Leafs.AddRange(elems);

		for (var i = 0; i < leafCount; i++) {
			// duplicate old tree
			var newTree = CreateMerkleTree(impl, chf, oldTree.Leafs);

			// Gen proof
			var deleteProof = oldTree.GenerateDeleteProof(i);
			newTree.Leafs.RemoveRange(newTree.Leafs.Count - i, i);
			var verifyProof = MerkleMath.VerifyDeleteProof(chf, oldTree.Root, leafCount, newTree.Root, i, deleteProof);
			ClassicAssert.IsTrue(verifyProof);
		}

	}


	private IDynamicMerkleTree CreateReferenceTree(CHF chf, IEnumerable<byte[]> leafs = null)
		=> CreateMerkleTree(MerkleTreeImpl.Simple, chf);


	private IDynamicMerkleTree CreateMerkleTree(MerkleTreeImpl impl, CHF chf, IEnumerable<byte[]> leafs = null) {
		switch (impl) {
			case MerkleTreeImpl.Simple:
				return new SimpleMerkleTree(chf, leafs);
			case MerkleTreeImpl.Flat:
				return new FlatMerkleTree(chf, leafs);
			case MerkleTreeImpl.Long:
				return new LongMerkleTree(chf, leafs);
			case MerkleTreeImpl.Adapter:
				throw new NotImplementedException();
			default:
				throw new ArgumentOutOfRangeException(nameof(impl), impl, null);
		}
	}


	public enum MerkleTreeImpl {
		Simple,
		Flat,
		Long,
		Adapter
	}
}
