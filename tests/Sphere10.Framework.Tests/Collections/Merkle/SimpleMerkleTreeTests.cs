using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Maths;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class SimpleMerkleTreeTests {

		[Test]
		public void Add() {
			var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
			var expectedTrees =
				Enumerable.Range(0, elems.Count())
				.Select(x => new SimpleMerkleTree(CHF.ConcatBytes, elems.Take(x).Select(Encoding.ASCII.GetBytes)));

			var testTree = new SimpleMerkleTree(CHF.ConcatBytes);
			foreach (var (newLeaf, expectedTree) in new string[] { null }.Concat(elems).Zip(expectedTrees, Tuple.Create)) {
				if (newLeaf != null)
					testTree.Leafs.Add(Encoding.ASCII.GetBytes(newLeaf));
				Assert.AreEqual(expectedTree.Root, testTree.Root);
			}
		}

		[Test]
		public void Insert() {
			var expected = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
			var test = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
			Assert.AreNotEqual(expected.Root, test.Root);
			test.Leafs.InsertRange(3, new[] { "D", "E", "F" }.Select(Encoding.ASCII.GetBytes));
			Assert.AreEqual(expected.Root, test.Root);
		}

		[Test]
		public void Update() {
			var expected = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
			var test = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "Z", "Z", "Z", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
			Assert.AreNotEqual(expected.Root, test.Root);
			test.Leafs.UpdateRange(3, new[] { "D", "E", "F" }.Select(Encoding.ASCII.GetBytes));
			Assert.AreEqual(expected.Root, test.Root);
		}

		[Test]
		public void Remove() {
			var expected = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
			var test = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
			Assert.AreNotEqual(expected.Root, test.Root);
			test.Leafs.RemoveRange(3, 3);
			Assert.AreEqual(expected.Root, test.Root);
		}


		[Test]
		public void ExistenceProof() {
			var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
			var tree = new SimpleMerkleTree(CHF.ConcatBytes);

			foreach (var elem in elems) {
				tree.Leafs.Add(Encoding.ASCII.GetBytes(elem));
				for (var i = 0; i < tree.Leafs.Count; i++) {
					var proof = tree.GenerateExistenceProof(i).ToArray();
					var verify = MerkleMath.VerifyExistenceProof(tree.HashAlgorithm, tree.Root, tree.Size, i, Encoding.ASCII.GetBytes(elems[i]), proof);
					Assert.IsTrue(verify);
				}
			}

			// Verify last proof items 
			var lastProof = tree.GenerateExistenceProof(tree.Leafs.Count - 1).ToArray();
			Assert.AreEqual(3, lastProof.Length);
			Assert.AreEqual(lastProof[0], Encoding.ASCII.GetBytes("Y"));
			Assert.AreEqual(lastProof[1], Encoding.ASCII.GetBytes("QRSTUVWX"));
			Assert.AreEqual(lastProof[2], Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP"));
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
					var verifyProof = MerkleMath.VerifyConsistencyProof(chf, oldTree.Root, i, newTree.Root, i + (j - i),  consistencyProof);

					Assert.IsTrue(verifyProof);
				}

			}
		}




	}
}
