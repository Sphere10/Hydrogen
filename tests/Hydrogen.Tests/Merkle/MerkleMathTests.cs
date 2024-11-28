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
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MerkleMathTests {

	private static int[] CalculateTreeDimensionsManual(int leafCount) {
		Guard.ArgumentInRange(leafCount, 0, int.MaxValue, nameof(leafCount));

		// Empty tree
		if (leafCount == 0)
			return new int[0];

		var levels = new List<int> { leafCount }; // Level 0
		while (leafCount > 1) {
			var isOdd = leafCount % 2 != 0;
			leafCount >>= 1;
			if (isOdd)
				leafCount++;
			levels.Add(leafCount);
		}

		return levels.ToArray();
	}

	[Test]
	public void IsPerfect() {
		for (var x = 0; x < 31; x++) {
			ClassicAssert.IsTrue(MerkleMath.IsPerfectTree(MerkleSize.FromLeafCount(1 << x)));
			if (x != 1)
				ClassicAssert.IsFalse(MerkleMath.IsPerfectTree(MerkleSize.FromLeafCount((1 << x) - 1)));
		}
	}

	[Test]
	public void CalculateHeight_Basic() {
		ClassicAssert.AreEqual(0, MerkleMath.CalculateHeight(0));
		ClassicAssert.AreEqual(1, MerkleMath.CalculateHeight(1));
		ClassicAssert.AreEqual(3, MerkleMath.CalculateHeight(3));
		ClassicAssert.AreEqual(32, MerkleMath.CalculateHeight(int.MaxValue));
	}

	[Test]
	public void CalculateHeight_Consistent() {
		for (var i = 0; i < 32; i++) {
			for (var j = 0; j < 1000; j++) {
				var leafCount = i < 31 ? 1 << i : int.MaxValue;
				if (leafCount - j < 0)
					break;

				var height = MerkleMath.CalculateHeight(leafCount);
				var dims = CalculateTreeDimensionsManual(leafCount);
				ClassicAssert.AreEqual(dims.Length, height);
			}
		}
	}

	[Test]
	public void CalculateLevelLength_Consistent() {
		for (var i = 0; i < 32; i++) {
			for (var j = 0; j < 1000; j++) {
				var leafCount = i < 31 ? 1 << i : int.MaxValue;

				if (leafCount - j < 0)
					break;

				var dims = CalculateTreeDimensionsManual(leafCount);
				var treeSize = MerkleSize.FromLeafCount(leafCount);
				for (var k = 0; k < dims.Length; k++) {
					if (k == 31 && leafCount == int.MaxValue) {
						var xxx = 1;
					}
					ClassicAssert.AreEqual(dims[k], MerkleMath.CalculateLevelLength(treeSize.LeafCount, k));
				}
			}
		}
	}

	[Test]
	public void CalculateHeight_Length_RandomSamples() {
		const int NumTests = 10000;

		uint randomInt = int.MaxValue;
		for (var i = 0; i < NumTests; i++) {
			if (randomInt == 0)
				randomInt = 1;
			randomInt = XorShift32.Next(ref randomInt);

			var leafCount = (int)randomInt % int.MaxValue;
			if (leafCount < 0)
				leafCount = -leafCount;

			var dims = CalculateTreeDimensionsManual(leafCount); // note: CalculateTreeDimensions is simple integer based arithmetic
			var treeSize = MerkleSize.FromLeafCount(leafCount);
			ClassicAssert.AreEqual(dims.Length, MerkleMath.CalculateHeight(leafCount));
			for (var j = 0; j < dims.Length; j++) {
				ClassicAssert.AreEqual(dims[j], MerkleMath.CalculateLevelLength(treeSize.LeafCount, j));
			}
		}
	}

	[Test]
	public void CalculateNodeTraits_Single() {
		var size = MerkleSize.FromLeafCount(1);
		ClassicAssert.AreEqual(1, size.Height);
		var traits = MerkleMath.GetTraits(size, MerkleCoordinate.LeafAt(0));
		AssertEx.HasFlags(MerkleNodeTraits.Root, traits);
		AssertEx.HasFlags(MerkleNodeTraits.Leaf, traits);
		AssertEx.NotHasFlags(MerkleNodeTraits.IsLeftChild, traits);
		AssertEx.NotHasFlags(MerkleNodeTraits.IsRightChild, traits);
		AssertEx.NotHasFlags(MerkleNodeTraits.HasLeftChild, traits);
		AssertEx.NotHasFlags(MerkleNodeTraits.HasRightChild, traits);
	}

	[Test]
	public void CalculateNodeTraits_PerfectTree() {
		// Perfect tree with 2**8 leafs
		var size = MerkleSize.FromLeafCount(1 << 8);
		for (var level = 0; level < size.Height - 1; level++) {
			var lastIndex = MerkleMath.CalculateLevelLength(size.LeafCount, level) - 1;
			for (var i = 0; i <= lastIndex; i++) {
				var nodeTraits = MerkleMath.GetTraits(size, MerkleCoordinate.From(level, i));
				var isEven = i % 2 == 0;
				if (isEven) {
					// left-only child check
					AssertEx.HasFlags(MerkleNodeTraits.IsLeftChild, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.IsRightChild, nodeTraits);
				} else {
					// right-only child check
					AssertEx.HasFlags(MerkleNodeTraits.IsRightChild, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.IsLeftChild, nodeTraits);
				}

				if (level > 0) {
					// node check
					AssertEx.NotHasFlags(MerkleNodeTraits.Leaf, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.Root, nodeTraits);
					AssertEx.HasFlags(MerkleNodeTraits.HasLeftChild, nodeTraits);
					AssertEx.HasFlags(MerkleNodeTraits.HasRightChild, nodeTraits);

				} else {
					// leaf check
					AssertEx.HasFlags(MerkleNodeTraits.Leaf, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.Root, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.HasLeftChild, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.HasRightChild, nodeTraits);
				}
			}
		}

		// Root check
		var rootTraits = MerkleMath.GetTraits(size, MerkleCoordinate.From(size.Height - 1, 0));
		AssertEx.HasFlags(MerkleNodeTraits.Root, rootTraits);
		AssertEx.NotHasFlags(MerkleNodeTraits.IsRightChild, rootTraits);
		AssertEx.NotHasFlags(MerkleNodeTraits.IsLeftChild, rootTraits);
		AssertEx.HasFlags(MerkleNodeTraits.HasLeftChild, rootTraits);
		AssertEx.HasFlags(MerkleNodeTraits.HasRightChild, rootTraits);
	}

	[Test]
	public void CalculateNodeTraits_LargePerfectTree() {
		// Perfect tree with 2**31 leafs
		var size = MerkleSize.FromLeafCount(1 << 30);
		for (var level = 0; level < size.Height - 1; level++) {
			var lastIndex = MerkleMath.CalculateLevelLength(size.LeafCount, level) - 1;
			foreach (var i in new[] { 0, lastIndex }) {
				var nodeTraits = MerkleMath.GetTraits(size, MerkleCoordinate.From(level, i));

				var isEven = i % 2 == 0;
				if (isEven) {
					// left-only child check
					AssertEx.HasFlags(MerkleNodeTraits.IsLeftChild, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.IsRightChild, nodeTraits);
				} else {
					// right-only child check
					AssertEx.HasFlags(MerkleNodeTraits.IsRightChild, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.IsLeftChild, nodeTraits);
				}

				// parent check
				if (level > 0) {
					AssertEx.NotHasFlags(MerkleNodeTraits.Leaf, nodeTraits);
					AssertEx.HasFlags(MerkleNodeTraits.HasLeftChild, nodeTraits);
					AssertEx.HasFlags(MerkleNodeTraits.HasRightChild, nodeTraits);
				} else {
					AssertEx.HasFlags(MerkleNodeTraits.Leaf, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.HasLeftChild, nodeTraits);
					AssertEx.NotHasFlags(MerkleNodeTraits.HasRightChild, nodeTraits);
				}
			}
		}

		// Root check
		var rootTraits = MerkleMath.GetTraits(size, MerkleCoordinate.From(size.Height - 1, 0));
		AssertEx.HasFlags(MerkleNodeTraits.Root, rootTraits);
		AssertEx.NotHasFlags(MerkleNodeTraits.IsRightChild, rootTraits);
		AssertEx.NotHasFlags(MerkleNodeTraits.IsLeftChild, rootTraits);
		AssertEx.HasFlags(MerkleNodeTraits.HasLeftChild, rootTraits);
		AssertEx.HasFlags(MerkleNodeTraits.HasRightChild, rootTraits);
	}

	[Test]
	public void CalculateNodeTraits_Root_1() {
		var size = MerkleSize.FromLeafCount(1);
		//          T
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 0)).HasFlag(MerkleNodeTraits.Root));
	}

	[Test]
	public void CalculateNodeTraits_BubbleUp_1() {
		// Perfect tree with 2**31 leafs plus one extra item
		// results in last leaf bubbling up until it becomes right child of root node
		var size = MerkleSize.FromLeafCount((1 << 30) + 1);
		for (var level = 0; level < size.Height - 1; level++) {
			var nodeCoord = MerkleCoordinate.Last(size, level);
			var nodeTraits = MerkleMath.GetTraits(size, nodeCoord);
			AssertEx.HasFlags(MerkleNodeTraits.BubblesUp, nodeTraits);
			if (level > 0)
				AssertEx.HasFlags(MerkleNodeTraits.BubbledUp, nodeTraits);
			else
				AssertEx.NotHasFlags(MerkleNodeTraits.BubbledUp, nodeTraits);
		}
	}

	[Test]
	public void CalculateNodeTraits_BubbleUp_2() {
		// Perfect tree with 2**31 leafs plus two extra items
		// results in last node of second level bubbling up until it becomes right child of root node
		var size = MerkleSize.FromLeafCount((1 << 30) + 2);
		AssertEx.NotHasFlags(MerkleNodeTraits.BubblesUp, MerkleMath.GetTraits(size, MerkleCoordinate.Last(size, 0)));
		for (var level = 1; level < size.Height - 1; level++) {
			var nodeCoord = MerkleCoordinate.Last(size, level);
			var nodeTraits = MerkleMath.GetTraits(size, nodeCoord);
			AssertEx.HasFlags(MerkleNodeTraits.BubblesUp, nodeTraits);
			if (level > 1)
				AssertEx.HasFlags(MerkleNodeTraits.BubbledUp, nodeTraits);
			else
				AssertEx.NotHasFlags(MerkleNodeTraits.BubbledUp, nodeTraits);
		}
	}

	[Test]
	public void CalculateNodeTraits_BubbleUp_3() {
		// Perfect tree with 2**31 leafs plus three extra items
		// results in last leaf bubbling up one level, becomes right child of level 2, bubbles up to right child of root

		var size = MerkleSize.FromLeafCount((1 << 30) + 3);

		// Last leaf
		AssertEx.HasFlags(MerkleNodeTraits.BubblesUp, MerkleMath.GetTraits(size, MerkleCoordinate.Last(size, 0)));

		// Last node of level 1
		AssertEx.HasFlags(MerkleNodeTraits.BubbledUp, MerkleMath.GetTraits(size, MerkleCoordinate.Last(size, 1)));
		AssertEx.NotHasFlags(MerkleNodeTraits.BubblesUp, MerkleMath.GetTraits(size, MerkleCoordinate.Last(size, 1)));

		// Last node of level 2 bubbles up to root right child
		for (var level = 2; level < size.Height - 1; level++) {
			var nodeCoord = MerkleCoordinate.Last(size, level);
			var nodeTraits = MerkleMath.GetTraits(size, nodeCoord);
			AssertEx.HasFlags(MerkleNodeTraits.BubblesUp, nodeTraits);
			if (level > 2)
				AssertEx.HasFlags(MerkleNodeTraits.BubbledUp, nodeTraits);
		}
	}

	[Test]
	public void CalculateNodeTraits_BubbleUp_4() {
		// Previous bug: 5 leafs, yet L^1_2 not bubbles up	
		//
		//
		//              o
		//          o       c 
		//      o       o        b
		//    o   o   o   o   a
		var size = MerkleSize.FromLeafCount(5);
		AssertEx.HasFlags(MerkleNodeTraits.BubblesUp, MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 4))); // a
		AssertEx.HasFlags(MerkleNodeTraits.BubblesUp, MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 2))); // b
		AssertEx.HasFlags(MerkleNodeTraits.BubbledUp, MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 2))); // b (bubbled)
		AssertEx.HasFlags(MerkleNodeTraits.BubbledUp, MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 2))); // c
	}

	[Test]
	public void CalculateNodeTraits_Perfect_1() {
		var size = MerkleSize.FromLeafCount(1);

		//          T
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 0)).HasFlag(MerkleNodeTraits.Perfect));
	}

	[Test]
	public void CalculateNodeTraits_Perfect_4() {
		var size = MerkleSize.FromLeafCount(4);

		//          T
		//      T       T
		//    T   T   T   T     

		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 2)).HasFlag(MerkleNodeTraits.Perfect));
	}

	[Test]
	public void CalculateNodeTraits_Perfect_5() {
		var size = MerkleSize.FromLeafCount(5);
		//                  F
		//          T               F
		//      T       T       F
		//    T   T   T   T   T      

		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(3, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 2)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 2)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 3)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 4)).HasFlag(MerkleNodeTraits.Perfect));

	}

	[Test]
	public void CalculateNodeTraits_Perfect_6() {
		var size = MerkleSize.FromLeafCount(6);
		//                  F
		//          T               F
		//      T       T       T       
		//    T   T   T   T   T   T         

		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(3, 0)).HasFlag(MerkleNodeTraits.Perfect));

		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 1)).HasFlag(MerkleNodeTraits.Perfect));

		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 2)).HasFlag(MerkleNodeTraits.Perfect));


		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 2)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 3)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 4)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 5)).HasFlag(MerkleNodeTraits.Perfect));

	}

	[Test]
	public void CalculateNodeTraits_Perfect_7() {
		var size = MerkleSize.FromLeafCount(7);
		//                  F
		//          T               F
		//      T       T       T       F
		//    T   T   T   T   T   T   T      

		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(3, 0)).HasFlag(MerkleNodeTraits.Perfect));

		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(2, 1)).HasFlag(MerkleNodeTraits.Perfect));

		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 2)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsFalse(MerkleMath.GetTraits(size, MerkleCoordinate.From(1, 3)).HasFlag(MerkleNodeTraits.Perfect));

		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 0)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 1)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 2)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 3)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 4)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 5)).HasFlag(MerkleNodeTraits.Perfect));
		ClassicAssert.IsTrue(MerkleMath.GetTraits(size, MerkleCoordinate.From(0, 6)).HasFlag(MerkleNodeTraits.Perfect));

	}

	[Test]
	public void GetChildren_3() {
		//          a
		//      b       c
		//    d   e   f        

		var a = MerkleCoordinate.From(2, 0);
		var b = MerkleCoordinate.From(1, 0);
		var c = MerkleCoordinate.From(1, 1);
		var d = MerkleCoordinate.From(0, 0);
		var e = MerkleCoordinate.From(0, 1);
		var f = MerkleCoordinate.From(0, 2);

		var size = MerkleSize.FromLeafCount(3);


		var (left, right) = MerkleMath.GetChildren(size, a);
		ClassicAssert.AreEqual(b, left);
		ClassicAssert.AreEqual(c, right);

		(left, right) = MerkleMath.GetChildren(size, b);
		ClassicAssert.AreEqual(d, left);
		ClassicAssert.AreEqual(e, right);

		(left, right) = MerkleMath.GetChildren(size, c);
		ClassicAssert.AreEqual(f, left);
		ClassicAssert.AreEqual(MerkleCoordinate.Null, right);

	}

	[Test]
	public void FromFlatCoordinate() {
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 0), MerkleMath.FromFlatIndex(0));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 1), MerkleMath.FromFlatIndex(1));
		ClassicAssert.AreEqual(MerkleCoordinate.From(1, 0), MerkleMath.FromFlatIndex(2));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 2), MerkleMath.FromFlatIndex(3));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 3), MerkleMath.FromFlatIndex(4));
		ClassicAssert.AreEqual(MerkleCoordinate.From(1, 1), MerkleMath.FromFlatIndex(5));
		ClassicAssert.AreEqual(MerkleCoordinate.From(2, 0), MerkleMath.FromFlatIndex(6));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 4), MerkleMath.FromFlatIndex(7));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 5), MerkleMath.FromFlatIndex(8));
		ClassicAssert.AreEqual(MerkleCoordinate.From(1, 2), MerkleMath.FromFlatIndex(9));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 6), MerkleMath.FromFlatIndex(10));
		ClassicAssert.AreEqual(MerkleCoordinate.From(0, 7), MerkleMath.FromFlatIndex(11));
		ClassicAssert.AreEqual(MerkleCoordinate.From(1, 3), MerkleMath.FromFlatIndex(12));
		ClassicAssert.AreEqual(MerkleCoordinate.From(2, 1), MerkleMath.FromFlatIndex(13));
		ClassicAssert.AreEqual(MerkleCoordinate.From(3, 0), MerkleMath.FromFlatIndex(14));
	}

	[Test]
	public void ToFlatCoordinate() {
		ClassicAssert.AreEqual(0, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 0)));
		ClassicAssert.AreEqual(1, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 1)));
		ClassicAssert.AreEqual(2, MerkleMath.ToFlatIndex(MerkleCoordinate.From(1, 0)));
		ClassicAssert.AreEqual(3, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 2)));
		ClassicAssert.AreEqual(4, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 3)));
		ClassicAssert.AreEqual(5, MerkleMath.ToFlatIndex(MerkleCoordinate.From(1, 1)));
		ClassicAssert.AreEqual(6, MerkleMath.ToFlatIndex(MerkleCoordinate.From(2, 0)));
		ClassicAssert.AreEqual(7, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 4)));
		ClassicAssert.AreEqual(8, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 5)));
		ClassicAssert.AreEqual(9, MerkleMath.ToFlatIndex(MerkleCoordinate.From(1, 2)));
		ClassicAssert.AreEqual(10, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 6)));
		ClassicAssert.AreEqual(11, MerkleMath.ToFlatIndex(MerkleCoordinate.From(0, 7)));
		ClassicAssert.AreEqual(12, MerkleMath.ToFlatIndex(MerkleCoordinate.From(1, 3)));
		ClassicAssert.AreEqual(13, MerkleMath.ToFlatIndex(MerkleCoordinate.From(2, 1)));
		ClassicAssert.AreEqual(14, MerkleMath.ToFlatIndex(MerkleCoordinate.From(3, 0)));
	}

	[Test]
	public void FlatCoordinate_Consistency() {
		const ulong Max = 100000;
		for (var i = 0UL; i < Max; i++) {
			var coord = MerkleMath.FromFlatIndex(i);
			var result = MerkleMath.ToFlatIndex(coord);
			ClassicAssert.AreEqual(i, result);
		}
	}

	[Test]
	public void FlatCoordinate_Uniqueness() {
		const ulong Max = 100000;
		var set = new HashSet<MerkleCoordinate>();
		for (var i = 0UL; i < Max; i++) {
			var coord = MerkleMath.FromFlatIndex(i);
			ClassicAssert.IsFalse(set.Contains(coord));
			set.Add(coord);
		}
	}
}
