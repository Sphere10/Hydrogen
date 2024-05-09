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
using System.Text;
using Hydrogen;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MerkleProofTests {

	[Test]
	public void ExistenceProof() {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		var tree = new SimpleMerkleTree(CHF.ConcatBytes);

		foreach (var elem in elems) {
			tree.Leafs.Add(Encoding.ASCII.GetBytes(elem));
			for (var i = 0; i < tree.Leafs.Count; i++) {
				var proof = tree.GenerateExistenceProof(i).ToArray();
				var verify = MerkleMath.VerifyExistenceProof(tree.HashAlgorithm, tree.Root, tree.Size, i, Encoding.ASCII.GetBytes(elems[i]), proof);
				ClassicAssert.IsTrue(verify);
			}
		}

		// Verify last proof items 
		var lastProof = tree.GenerateExistenceProof(tree.Leafs.Count - 1).ToArray();
		ClassicAssert.AreEqual(3, lastProof.Length);
		ClassicAssert.AreEqual(lastProof[0], Encoding.ASCII.GetBytes("Y"));
		ClassicAssert.AreEqual(lastProof[1], Encoding.ASCII.GetBytes("QRSTUVWX"));
		ClassicAssert.AreEqual(lastProof[2], Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP"));
	}


	[Test]
	public void ConsistencyProof([Values(CHF.ConcatBytes, CHF.SHA2_256)] CHF chf) {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };


		for (var i = 0; i < elems.Length; i++) {
			var oldTree = new SimpleMerkleTree(chf);
			oldTree.Leafs.AddRange(elems.Take(i).Select(Encoding.ASCII.GetBytes));

			for (var j = i + 1; j < elems.Length; j++) {
				// Copy old tree
				var newTree = new SimpleMerkleTree(chf);
				newTree.Leafs.AddRange(oldTree.Leafs);

				// Add new item
				newTree.Leafs.AddRange(elems.Skip(i).Take(j - i).Select(Encoding.ASCII.GetBytes));

				var consistencyProof = newTree.GenerateConsistencyProof(i);
				var verifyProof = MerkleMath.VerifyConsistencyProof(chf, oldTree.Root, i, newTree.Root, i + (j - i), consistencyProof);

				ClassicAssert.IsTrue(verifyProof);
			}

		}
	}

	[Test]
	public void UpdateProof_Single([Values(CHF.SHA2_256)] CHF chf) {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

		var tree = new SimpleMerkleTree(chf);
		var stringHasher = new ActionHasher<string>( s => Hashers.Hash(chf,  Encoding.ASCII.GetBytes(s)), Hashers.GetDigestSizeBytes(chf));

		tree.Leafs.AddRange(elems.Select(stringHasher.Hash));

		var oldRoot = tree.Root;
		var proof = MerkleMath.GenerateUpdateProof(tree, new [] { 0L }, out var flags).ToArray();
		tree.Leafs.Update(0, stringHasher.Hash("A1"));


		var result = MerkleMath.VerifyUpdateProof(chf, oldRoot, tree.Size, tree.Root, new [] { Tuple.Create (0L, tree.Leafs[0]) }, proof);

		Assert.That(result, Is.True);
	}

	[Test]
	public void UpdateProof_Adjacent([Values(CHF.SHA2_256)] CHF chf) {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

		var tree = new SimpleMerkleTree(chf);
		var stringHasher = new ActionHasher<string>( s => Hashers.Hash(chf,  Encoding.ASCII.GetBytes(s)), Hashers.GetDigestSizeBytes(chf));

		tree.Leafs.AddRange(elems.Select(stringHasher.Hash));

		var oldRoot = tree.Root;
		var proof = MerkleMath.GenerateUpdateProof(tree, new [] { 0L, 1 }, out var flags).ToArray();
		tree.Leafs.Update(0, stringHasher.Hash("A1"));
		tree.Leafs.Update(1, stringHasher.Hash("B2"));
		

		var result = MerkleMath.VerifyUpdateProof(chf, oldRoot, tree.Size, tree.Root, new [] { Tuple.Create (0L, tree.Leafs[0]), Tuple.Create (1L, tree.Leafs[1]) }, proof);

		Assert.That(result, Is.True);
	}


	[Test]
	public void UpdateProof_Complex_AllPermutations([Values(CHF.SHA2_256)] CHF chf, [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12)] int leafCount) {
		var rng = new Random(31337);
		var stringHasher = new ActionHasher<string>(s => Hashers.Hash(chf, Encoding.ASCII.GetBytes(s)), Hashers.GetDigestSizeBytes(chf));

		var originalLeafs = Enumerable.Range(0, leafCount).Select(_ => rng.NextString(10)).Select(stringHasher.Hash).ToArray();
		var modifiedLeafs = originalLeafs.Select(x => "_" + x + "!").ToArray();

		for(var updatedLeafCount = 0; updatedLeafCount <= leafCount; updatedLeafCount++) {
			foreach(var updatedLeafIndices in Tools.Maths.IterateIndexPermutationsMemoryEfficiently(leafCount, updatedLeafCount)) {
				var tree = new SimpleMerkleTree(chf);
				tree.Leafs.AddRange(originalLeafs);	

				// generate the proof (before tree updated)
				var proof = tree.GenerateUpdateProof(updatedLeafIndices).ToArray();
				var oldRoot = tree.Root;
				
				// update tree 
				foreach(var updatedIndex in updatedLeafIndices) {
					tree.Leafs.Update(updatedIndex, stringHasher.Hash( modifiedLeafs[updatedIndex]));
				}

				// verify update proof
				var result = MerkleMath.VerifyUpdateProof(chf, oldRoot, tree.Size, tree.Root, updatedLeafIndices.Select(x => Tuple.Create(x, tree.Leafs[x])), proof); 

				Assert.That(result, Is.True);
			}

		}


	}
}
