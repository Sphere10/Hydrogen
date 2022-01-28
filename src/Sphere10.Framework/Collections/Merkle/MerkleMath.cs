using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Sphere10.Framework;

namespace Sphere10.Framework {

	public static class MerkleMath {

		private static readonly ulong[] PerfectLeftMostIndices;

		static MerkleMath() {
			PerfectLeftMostIndices = new ulong[63];
			for (var i = 1; i < 64; i++) {
				PerfectLeftMostIndices[i - 1] = (1UL << i) - 1;
			}
		}

		#region Base math

		public static int CalculateHeight(int leafCount) {
			if (leafCount == 0)
				return 0;
			return (int)Math.Ceiling(Tools.Maths.EpsilonTrunc(Math.Log(leafCount, 2))) + 1;
		}

		public static int CalculateLevelLength(int leafCount, int level) {
			if (level == 0)
				return leafCount;
			// note: below follows from theorem: ceil(ceil(x/m)/n) = ceil(x/(m*n))
			return (int)Math.Ceiling(leafCount / (double)(1U << level));
		}

		#endregion

		#region Tree Navigation

		public static bool IsLeaf(MerkleCoordinate node) => node.Level == 0;

		public static MerkleNodeTraits GetTraits(MerkleSize size, MerkleCoordinate node) {
			MerkleNodeTraits traits = 0;

			traits.SetFlags(MerkleNodeTraits.Leaf, node.Level == 0);
			traits.SetFlags(MerkleNodeTraits.Perfect, IsPerfectNode(size, node));
			traits.SetFlags(MerkleNodeTraits.HasLeftChild, node.Level > 0);

			if (node.IsRoot(size)) {
				traits.SetFlags(MerkleNodeTraits.Root, true);
				traits.SetFlags(MerkleNodeTraits.IsLeftChild, false);
				traits.SetFlags(MerkleNodeTraits.IsRightChild, false);
				traits.SetFlags(MerkleNodeTraits.BubblesUp, false);
				traits.SetFlags(MerkleNodeTraits.BubbledUp, false);
				traits.SetFlags(MerkleNodeTraits.HasRightChild, size.Height > 1);
			} else {
				traits.SetFlags(MerkleNodeTraits.Root, false);
				var levelLength = CalculateLevelLength(size.LeafCount, node.Level);
				if (levelLength % 2 != 0 && node.Index == levelLength - 1) {
					traits.SetFlags(MerkleNodeTraits.BubblesUp, true);
					traits.SetFlags(MerkleNodeTraits.IsLeftChild, false);
					traits.SetFlags(MerkleNodeTraits.IsRightChild, false);
				} else {
					var isLeftChild = node.Index % 2 == 0;
					traits.SetFlags(MerkleNodeTraits.IsLeftChild, isLeftChild);
					traits.SetFlags(MerkleNodeTraits.IsRightChild, !isLeftChild);
				}

				if (node.Level > 0) {
					var childLevel = node.Level - 1;
					var leftChildIndex = node.Index * 2;
					var childLevelLength = MerkleMath.CalculateLevelLength(size.LeafCount, childLevel);
					var isLast = leftChildIndex == childLevelLength - 1;
					traits.SetFlags(MerkleNodeTraits.BubbledUp, isLast);
					traits.SetFlags(MerkleNodeTraits.HasRightChild, !isLast);
				} else {
					traits.SetFlags(MerkleNodeTraits.BubbledUp, false);
					traits.SetFlags(MerkleNodeTraits.HasRightChild, false);
				}
			}
			return traits;
		}

		public static MerkleCoordinate GetParent(MerkleSize size, MerkleCoordinate node) {
			if (node.IsRoot(size)) {
				return MerkleCoordinate.Null;
			}
			return MerkleCoordinate.From(node.Level + 1, node.Index / 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (MerkleCoordinate Left, MerkleCoordinate Right) GetChildren(MerkleSize size, MerkleCoordinate node)
			=> GetDescendants(size, node, 1, false);

		public static (MerkleCoordinate Left, MerkleCoordinate Right) GetDescendants(MerkleSize size, MerkleCoordinate node, int levelsBelow = 1, bool assumePerfectRightChild = false) {
			Guard.Argument(node != MerkleCoordinate.Null, nameof(node), "Undefined");
			Guard.ArgumentInRange(levelsBelow, 1, int.MaxValue, nameof(levelsBelow));

			// Case: leaf-nodes
			if (node.Level == 0)
				return (MerkleCoordinate.Null, MerkleCoordinate.Null);

			// Case: non-leaf
			var level = node.Level - levelsBelow;
			var left = MerkleCoordinate.From(level, (1 << levelsBelow) * node.Index);

			var rightIX = (1 << levelsBelow) * (node.Index + 1) - 1;
			MerkleCoordinate right;
			if (!assumePerfectRightChild) {
				rightIX = Math.Min(rightIX, CalculateLevelLength(size.LeafCount, level) - 1);
				right = rightIX > left.Index ? MerkleCoordinate.From(level, rightIX) : MerkleCoordinate.Null;
			} else right = MerkleCoordinate.From(level, rightIX);
			return (left, right);
		}

		public static MerkleCoordinate GetLogicalParent(MerkleSize size, MerkleCoordinate node) {
			// The "logical parent" is the parent node which actually consumes the child hash. In most cases
			// the "parent" is the "logical parent" except when the node "bubbles up" because it lacks a sibling.
			// Sometimes a node can bubble all the way up a tree. When constructing proofs, we don't care about 
			// parent nodes which are merely bubbled up child nodes, we care about the node which will consume
			// the child node as either a left or right child. The logical parent denotes this parent node.
			MerkleCoordinate prev;
			do {
				prev = node;
				node = GetParent(size, node);
			} while (node != MerkleCoordinate.Null && GetTraits(size, node).HasFlag(MerkleNodeTraits.BubbledUp));
			return node;
		}

		public static MerkleCoordinate GetLogicalLeftChild(MerkleSize size, MerkleCoordinate node) {
			do {
				node = GetChildren(size, node).Left;
			} while (GetTraits(size, node).HasFlag(MerkleNodeTraits.BubbledUp));
			return node;
		}

		public static MerkleCoordinate GetLogicalSibling(MerkleSize size, MerkleCoordinate node, out MerkleCoordinate logicalParent) {
			// The "logical sibling" is the other node that forms the logical parent hash.
			// If a node has bubbled up, it has no sibling. If a sibling is a bubbled up node, then we
			// need to drill-down until we find source node that bubbles up, since it contains the hash we want.

			logicalParent = GetLogicalParent(size, node);
			var nodeTraits = GetTraits(size, node);
			var siblingIsLeft = !nodeTraits.HasFlag(MerkleNodeTraits.IsLeftChild);
			if (siblingIsLeft)
				return GetChildren(size, logicalParent).Left;
			return GetLogicalRightChild(size, logicalParent);
		}

		public static MerkleCoordinate GetLogicalRightChild(MerkleSize size, MerkleCoordinate node) {
			node = GetChildren(size, node).Right;
			if (GetTraits(size, node).HasFlag(MerkleNodeTraits.BubbledUp))
				node = GetLogicalLeftChild(size, node); // bubbles up from left, so drill down 
			return node;
		}

		#endregion

		#region Tree Aggregation
		
		public static byte[] AggregateSubRoots(CHF chf, IEnumerable<byte[]> subRoots) {
			return Hashers.Aggregate(chf, subRoots.Reverse(), true);
		}

		/// <summary>
		/// Adds a leaf node to a set of sub-roots.
		/// </summary>
		public static void AddLeaf(CHF chf, IList<MerkleSubRoot> subRoots, byte[] leaf) {
			var newNode = new MerkleSubRoot(0, leaf);
			while (true) {
				if (subRoots.Count == 0 || subRoots[^1].Height > newNode.Height) {
					subRoots.Add(newNode);
					return;
				}
				var tip = subRoots[^1];
				subRoots.RemoveAt(subRoots.Count - 1);
				newNode = new MerkleSubRoot(tip.Height + 1, Hashers.JoinHash(chf, tip.Hash, newNode.Hash));
			}
		}

		#endregion

		#region Flat Coordinates

		public static MerkleCoordinate FromFlatIndex(ulong flatIndex) {
			flatIndex++; // algorithm below based on 1-based indexing
			var rootLevel = Array.BinarySearch(PerfectLeftMostIndices, flatIndex);
			if (rootLevel < 0)
				rootLevel = ~rootLevel; // didn't find, so take next larger index

			var index = 0;
			var rootFlatIX = PerfectLeftMostIndices[rootLevel];
			while (flatIndex != rootFlatIX) {
				var isRight = flatIndex > (rootFlatIX >> 1);
				index = 2 * index + (isRight ? 1 : 0);
				rootFlatIX = PerfectLeftMostIndices[--rootLevel];
				if (isRight)
					flatIndex -= rootFlatIX;
			}
			return MerkleCoordinate.From(rootLevel, (int)index);
		}

		public static ulong ToFlatIndex(MerkleCoordinate coordinate) {
			// Step 1: Find the closest perfect root ancestor
			var numNodesBefore = (1UL << (coordinate.Level + 1)) * ((ulong)coordinate.Index + 1) - 1;
			var rootLevel = Array.BinarySearch(PerfectLeftMostIndices, numNodesBefore);
			if (rootLevel < 0)
				rootLevel = ~rootLevel;
			var perfectRoot = MerkleCoordinate.From(rootLevel, 0);
			Debug.Assert(perfectRoot.Level >= coordinate.Level);

			// Step 2: calculate the path to root, tracking left/right turns
			var flags = new int[perfectRoot.Level - coordinate.Level];
			for (var i = 0; i < flags.Length; i++) {
				flags[i] = coordinate.Index % 2;
				coordinate = MerkleCoordinate.From(coordinate.Level + 1, coordinate.Index / 2);
			}

			// Step 2: Traverse from root down to the node, adjusting the flat index along the way
			var flatIX = PerfectLeftMostIndices[rootLevel];
			for (var i = 0; i < flags.Length; i++) {
				if (flags[flags.Length - i - 1] == 0)
					// moving to left child, so flat index decreases by the difference between the corresponding roots.
					flatIX -= PerfectLeftMostIndices[rootLevel - i] - PerfectLeftMostIndices[rootLevel - i - 1];
				else
					flatIX--;  // moving to right child, so flat index decreases by one
			}
			return flatIX - 1;  // decrease by one, since 0-based indexing
		}

		public static ulong CountFlatNodes(int leafCount) {
			return (ulong)Pow2.CalculatePow2Partition(leafCount).Sum(x => (1L << (x + 1)) - 1);
		}

		#endregion

		#region Tree Iteration
		
		public static bool IsPerfectTree(MerkleSize size) {
			if (size.Height == 0)
				return false;

			return size.LeafCount == 1U << (size.Height - 1);
		}

		public static bool IsPerfectNode(MerkleSize size, MerkleCoordinate coordinate) {
			// A node is perfect if it's perfect right-most leaf descendant exists
			return (1U << coordinate.Level) * (coordinate.Index + 1) - 1 < size.LeafCount;
		}

		public static IEnumerable<MerkleCoordinate> CalculateAggregationNodes(MerkleSize size) {
			return CalculateAggregationNodes(size, Pow2.CalculatePow2Partition(size.LeafCount));
		}

		public static IEnumerable<MerkleCoordinate> CalculateAggregationNodes(MerkleSize size, IEnumerable<int> subRootLevels) {
			foreach (var srLevel in subRootLevels) {
				if (srLevel == size.Height - 1) {
					yield return MerkleCoordinate.From(srLevel, 0); // root-node
					yield break; // perfect tree has no more aggregated nodes
				}
				yield return MerkleCoordinate.Last(size, srLevel + 1);
			}
		}

		public static IEnumerable<MerkleCoordinate> CalculateSubRoots(int leafCount) {
			return CalculateSubRoots(Pow2.CalculatePow2Partition(leafCount));
		}

		public static IEnumerable<MerkleCoordinate> CalculateSubRoots(IEnumerable<int> subRootLevels) {
			var totalLeafs = 0U;
			foreach (var subRootLevel in subRootLevels) {
				var subTreeLeafCount = 1U << subRootLevel;  // 2^subRootLevel
				totalLeafs += subTreeLeafCount;
				yield return MerkleCoordinate.From(subRootLevel, (int)Math.Ceiling(totalLeafs / (decimal)subTreeLeafCount) - 1);
			}
		}

		public static MerkleCoordinate CalculateRightSubRoot(MerkleSize size) {
			// Optimized to avoid calc all sub-roots
			// Empty tree has no perfect right subtree
			if (size.LeafCount <= 0)
				return MerkleCoordinate.Null;
			return MerkleCoordinate.Last(size, Pow2.CalculatePow2Partition(size.LeafCount).Last());
		}

		#endregion

		#region Security Proofs

		public static IEnumerable<bool> CalculateDirFlags(MerkleSize size, IEnumerable<MerkleCoordinate> path) {
			var pathArr = path as MerkleCoordinate[] ?? path.ToArray();
			return pathArr.Select(p => !GetTraits(size, p).HasFlag(MerkleNodeTraits.IsLeftChild));
		}

		public static IEnumerable<MerkleCoordinate> CalculatePathToRoot(MerkleSize size, MerkleCoordinate node, bool logicalNodesOnly) {
			do {
				yield return node;
			} while ((node = logicalNodesOnly ? GetLogicalParent(size, node) : GetParent(size, node)) != MerkleCoordinate.Null);
		}

		public static IEnumerable<MerkleCoordinate> CalculateExistenceProofPath(MerkleSize size, MerkleCoordinate node) {
			// First item is skipped (verifier needs this)
			// return node sibling, then parents sibling, then grandparents sibling, et al to root
			while (!node.IsRoot(size)) {
				yield return GetLogicalSibling(size, node, out var logicalParent);
				node = logicalParent;
			}
			// Last item, the root node, is skipped (verifier has this)
		}

		public static IEnumerable<MerkleCoordinate> CalculateConsistencyProofPath(int m, int n, out int[] oldRootProofPath) {
			Guard.Argument(n >= m, nameof(n), $"Must be greater than {nameof(m)}");
			// When appending to a merkle tree, the append proof contains the minimum hash path
			// that can be used to reconstruct both the old root and the new root.
			// Typically, this path is simply the  "existence proof" of the right-most perfect
			// sub-tree of the old root. This node is guaranteed to exist in all appended trees,
			// and walks the path to rebuild the old root as well as the new.

			// If old tree was empty, then append proof is empty
			if (m == 0) {
				oldRootProofPath = new int[0]; // old root does not exist since old tree was empty
				return Enumerable.Empty<MerkleCoordinate>();
			}

			var oldSize = MerkleSize.FromLeafCount(m);
			var newSize = MerkleSize.FromLeafCount(n);

			// The append-proof is the existence-proof of old tree's perfect right-most subtree within new tree
			var startCoordinate = CalculateRightSubRoot(oldSize);
			var appendProofPath = new[] { startCoordinate }.Concat(CalculateExistenceProofPath(newSize, startCoordinate));

			// But, to rebuild the old root the verifier must walk back using coordinates of
			// the old tree, which are calculated here. 
			oldRootProofPath = CalculateExistenceProofPath(oldSize, startCoordinate).Select(x => appendProofPath.EnumeratedIndexOf(x)).ToArray();

			return appendProofPath;

		}

		public static IEnumerable<MerkleCoordinate> CalculateContainsProofPath(MerkleSize size, IEnumerable<int> leafsIndices) {
			var leafs = leafsIndices.Select(MerkleCoordinate.LeafAt).ToArray();
			var path = leafs.Aggregate(
				Enumerable.Empty<MerkleCoordinate>(),
				(aggr, leaf) => aggr.Union(CalculateExistenceProofPath(size, leaf))
			);
			return path.Except(leafs);
		}

		public static IEnumerable<MerkleCoordinate> CalculateUpdateProofPath(MerkleSize size, IEnumerable<int> leafsIndices) {
			var leafs = leafsIndices.Select(MerkleCoordinate.LeafAt).ToArray();
			var path = leafs.Aggregate(
				Enumerable.Empty<MerkleCoordinate>(),
				(aggr, leaf) => aggr.Union(CalculateExistenceProofPath(size, leaf))
			);
			return path.Union(leafs);
		}
	
		public static IEnumerable<byte[]> GenerateProof(IMerkleTree merkleTree, IEnumerable<MerkleCoordinate> path, out IEnumerable<bool> flags) {
			var pathArr = path as MerkleCoordinate[] ?? path.ToArray();
			flags = CalculateDirFlags(merkleTree.Size, pathArr);
			return pathArr.Select(x => merkleTree.GetValue(x).ToArray());
		}

		public static IEnumerable<byte[]> GenerateExistenceProof(IMerkleTree merkleTree, MerkleCoordinate node, out IEnumerable<bool> flags)
			=> GenerateProof(merkleTree, CalculateExistenceProofPath(merkleTree.Size, node), out flags);

		public static IEnumerable<byte[]> GenerateConsistencyProof(IMerkleTree tree, int priorLeafCount, out IEnumerable<bool> flags)
			=> GenerateProof(tree, CalculateConsistencyProofPath(priorLeafCount, tree.Size.LeafCount, out _), out flags);

		public static IEnumerable<byte[]> GenerateContainsProof(IMerkleTree tree, IEnumerable<int> leafIndices, out IEnumerable<bool> flags)
			=> GenerateProof(tree, CalculateContainsProofPath(tree.Size, leafIndices), out flags);

		public static IEnumerable<byte[]> GenerateUpdateProof(IMerkleTree tree, IEnumerable<int> leafIndices, out IEnumerable<bool> flags)
			=> GenerateProof(tree, CalculateUpdateProofPath(tree.Size, leafIndices), out flags);

		public static IEnumerable<byte[]> GenerateAppendProof(IMerkleTree tree) 
			=> GenerateProof(tree, CalculateSubRoots(tree.Size.LeafCount), out _);

		public static IEnumerable<byte[]> GenerateDeleteProof(IMerkleTree tree, int deletedLeafCount, out IEnumerable<bool> flags)
			=> GenerateConsistencyProof(tree, tree.Size.LeafCount - deletedLeafCount, out flags);

		public static bool VerifyProof(CHF hashAlgorithm, ReadOnlySpan<byte> startHash, IEnumerable<byte[]> proof, IEnumerable<bool> flags, ReadOnlySpan<byte> expectedHash) {
			Guard.Argument(!startHash.IsEmpty, nameof(startHash), "Invalid value");
			Guard.ArgumentNotNull(proof, nameof(proof));
			Guard.ArgumentNotNull(flags, nameof(flags));
			Guard.Argument(!expectedHash.IsEmpty, nameof(expectedHash), "Invalid value");
			var proofArr = proof as byte[][] ?? proof.ToArray();
			var flagsArr = flags as bool[] ?? flags.ToArray();
			if (proofArr.Length != flagsArr.Length)
				return false;

			var result = startHash.ToArray();
			for (var i = 0; i < flagsArr.Length; i++) {
				var hash = proofArr[i];
				var flag = flagsArr[i];
				ReadOnlySpan<byte> left, right;
				if (!flag) {
					left = hash;
					right = result;
				} else {
					left = result;
					right = hash;
				}
				result = Hashers.JoinHash(hashAlgorithm, left, right); // TODO: use pre-alloc result, but need to size for concat and irregular initial digests
			}
			return result.AsSpan().SequenceEqual(expectedHash);
		}

		public static bool VerifyExistenceProof(CHF hashAlgorithm, ReadOnlySpan<byte> root, MerkleSize treeSize, int leafIndex, byte[] leafHash, IEnumerable<byte[]> proof)
			=> VerifyExistenceProof(hashAlgorithm, root, treeSize, MerkleCoordinate.LeafAt(leafIndex), leafHash, proof);

		public static bool VerifyExistenceProof(CHF hashAlgorithm, ReadOnlySpan<byte> root, MerkleSize treeSize, MerkleCoordinate nodeCoordinate, ReadOnlySpan<byte> nodeHash, IEnumerable<byte[]> proof) {
			var proofArr = proof as byte[][] ?? proof.ToArray();
			if (treeSize.LeafCount > 0 && nodeCoordinate.Equals(MerkleCoordinate.Null) && !proofArr.Any())
				return true; // proof-of-null (emptiness exists is something)

			// Verify the hash-proof, fetch direction flags by rebuilding proof-path
			return VerifyProof(hashAlgorithm, nodeHash, proofArr, CalculateDirFlags(treeSize, CalculateExistenceProofPath(treeSize, nodeCoordinate)), root);
		}

		public static bool VerifyConsistencyProof(CHF hashAlgorithm, ReadOnlySpan<byte> oldRoot, int oldLeafCount, ReadOnlySpan<byte> newRoot, int newLeafCount, IEnumerable<byte[]> proof) {
			var proofPath = CalculateConsistencyProofPath(oldLeafCount, newLeafCount, out var oldRootPathIX).ToArray();
			var proofArr = proof.ToArray();
			var rightSubRootCoord = proofPath.Length > 0 ? proofPath[0] : MerkleCoordinate.Null;
			var rightSubRoot = proofArr.Length > 0 ? proofArr[0] : default;
			if (oldLeafCount > 0) {
				var oldRootProof = VerifyExistenceProof(hashAlgorithm, oldRoot, MerkleSize.FromLeafCount(oldLeafCount), rightSubRootCoord, rightSubRoot, oldRootPathIX.Select(i => proofArr[i]));
				var newRootProof = VerifyExistenceProof(hashAlgorithm, newRoot, MerkleSize.FromLeafCount(newLeafCount), rightSubRootCoord, rightSubRoot, proofArr.Skip(1));
				return oldRootProof && newRootProof;
			}
			Debug.Assert(!(oldLeafCount == 0 && proofArr.Length > 0));  // append to empty should always be empty proof
			return true;
		}

		public static bool VerifyContainsProof(CHF hashAlgorithm, ReadOnlySpan<byte> root, MerkleSize treeSize, int index, IEnumerable<byte[]> leafs, IEnumerable<byte[]> proof) {
			Guard.ArgumentNotNull(leafs, nameof(leafs));
			var leafsArr = leafs as byte[][] ?? leafs.ToArray();
			return VerifyContainsProof(hashAlgorithm, root, treeSize, Enumerable.Range(index, leafsArr.Length).Zip(leafsArr, Tuple.Create), proof);
		}
			
		public static bool VerifyContainsProof(CHF hashAlgorithm, ReadOnlySpan<byte> root, MerkleSize treeSize, IEnumerable<Tuple<int,byte[]>> leafs, IEnumerable<byte[]> proof) {
			Guard.ArgumentNotNull(proof, nameof(proof));
			Guard.ArgumentNotNull(leafs, nameof(leafs));
			var proofArr = proof as byte[][] ?? proof.ToArray();
			var leafsArr = leafs as Tuple<int, byte[]>[] ?? leafs.ToArray();
			var leafIndices = leafsArr.Select(x => x.Item1).ToArray();

			// Rebuild the range proof path
			var rangeProofPath = CalculateContainsProofPath(treeSize, leafIndices).ToArray();
			Guard.Argument(proofArr.Length == rangeProofPath.Length, nameof(proof), "Proof is inconsistently sized");

			// Build the relevant tree subset using the hash proof and the recalculated path
			var partialTree = new Dictionary<MerkleCoordinate, byte[]>(
				rangeProofPath.Zip(proofArr, (c, v) => new KeyValuePair<MerkleCoordinate, byte[]>(c, v))
				.Concat(leafsArr.Select((leaf, i) => new KeyValuePair<MerkleCoordinate, byte[]>(MerkleCoordinate.LeafAt(leaf.Item1), leaf.Item2)))
			);

			// For each leaf, verify it exists in tree
			foreach (var leaf in leafsArr) {
				// verify each item exists within the proof
				var leafCoord = MerkleCoordinate.LeafAt(leaf.Item1);
				var leafExistencePath = CalculateExistenceProofPath(treeSize, leafCoord).ToArray();
				var flags = CalculateDirFlags(treeSize, leafExistencePath);
				if (!VerifyProof(hashAlgorithm, leaf.Item2, leafExistencePath.Select(c => partialTree[c]), flags, root))
					return false;
			}
			return true;
		}

		public static bool VerifyUpdateProof(CHF hashAlgorithm, ReadOnlySpan<byte> oldRoot, MerkleSize treeSize, ReadOnlySpan<byte> newRoot, int index, IEnumerable<byte[]> updatedLeafs, IEnumerable<byte[]> proof) {
			Guard.ArgumentNotNull(updatedLeafs, nameof(updatedLeafs));
			var updatedLeafsArr = updatedLeafs as byte[][] ?? updatedLeafs.ToArray();
			return VerifyUpdateProof(hashAlgorithm, oldRoot, treeSize, newRoot, Enumerable.Range(index, updatedLeafsArr.Length).Zip(updatedLeafsArr), proof);
		}
	
		public static bool VerifyUpdateProof(CHF hashAlgorithm, ReadOnlySpan<byte> oldRoot, MerkleSize treeSize, ReadOnlySpan<byte> newRoot, IEnumerable<Tuple<int, byte[]>> updatedLeafs, IEnumerable<byte[]> proof) {
			Guard.ArgumentNotNull(proof, nameof(proof));
			Guard.ArgumentNotNull(updatedLeafs, nameof(updatedLeafs));
			var proofArr = proof as byte[][] ?? proof.ToArray();
			var updatedLeafsArr = updatedLeafs as Tuple<int, byte[]>[] ?? updatedLeafs.ToArray();
			var updatedLeafIndices = updatedLeafsArr.Select(x => x.Item1).ToArray();

			// Rebuild the range proof path
			var rangeProofPath = CalculateUpdateProofPath(treeSize, updatedLeafIndices).ToArray();
			Guard.Argument(proofArr.Length == rangeProofPath.Length, nameof(proof), "Proof is inconsistently sized");

			// Build the relevant tree subset using the hash proof and the recalculated path
			var partialTree = new Dictionary<MerkleCoordinate, byte[]>(
				rangeProofPath.Zip(proofArr, (c, v) => new KeyValuePair<MerkleCoordinate, byte[]>(c, v))
			);

			// 1: To verify the update proof commits to the old root, we extract the old leafs from the proof and 
			//    verify it as a contains proof
			var leafCoords = updatedLeafIndices.Select(MerkleCoordinate.LeafAt).ToArray();
			var oldLeafs = leafCoords.Select(c => partialTree[c]).ToArray();
			var containsProof = rangeProofPath.Except(leafCoords).Select(c => partialTree[c]).ToArray();
			if (!VerifyContainsProof(hashAlgorithm, oldRoot, treeSize, updatedLeafIndices.Zip(oldLeafs, Tuple.Create), containsProof))
				return false;

			// If no leafs, nothing to rebuild
			if (updatedLeafsArr.Length == 0)
				return true;

			// 2: Add each new leaf and rebuild the partial tree incrementally
			foreach (var (leafIX, leafHash) in updatedLeafsArr) {
				var leafCoord = MerkleCoordinate.LeafAt(leafIX);

				// Replace the old leaf with new leaf
				if (!partialTree.Remove(leafCoord))
					return false; // internal error
				partialTree[leafCoord] = leafHash;

				// Rebuild parent nodes
				var logicalPathToRoot = MerkleMath.CalculatePathToRoot(treeSize, leafCoord, true).Skip(1);
				foreach (var parent in logicalPathToRoot) {
					var leftCoord = GetLogicalLeftChild(treeSize, parent);
					var rightCoord = GetLogicalRightChild(treeSize, parent);
					var left = partialTree[leftCoord];
					var right = partialTree[rightCoord];
					partialTree[parent] = Hashers.JoinHash(hashAlgorithm, left, right); 
				}
			}
			// Root node was added during rebuild
			var resultRoot = partialTree[MerkleCoordinate.Root(treeSize)];
			return newRoot.SequenceEqual(resultRoot);

		}
		
		public static bool VerifyAppendProof(CHF hashAlgorithm, ReadOnlySpan<byte> oldRoot, ReadOnlySpan<byte> newRoot, MerkleSize treeSize, IEnumerable<byte[]> leafs, IEnumerable<byte[]> proof) {
			Guard.ArgumentNotNull(leafs, nameof(leafs));
			Guard.ArgumentNotNull(proof, nameof(proof));
			var leafsArr = leafs as byte[][] ?? leafs.ToArray();
			var proofArr = proof as byte[][] ?? proof.ToArray();

			// Case: Append empty to empty always true
			if (oldRoot.IsEmpty && newRoot.IsEmpty && !proofArr.Any() && leafsArr.Length == 0)
				return true;

			// Case: failed to provide old root
			if (treeSize.LeafCount > 0 && oldRoot.IsEmpty)
				return false;

			// Case: proof not correct size
			var pow2Sum = Pow2.CalculatePow2Partition(treeSize.LeafCount).ToArray();
			if (proofArr.Length != pow2Sum.Length)
				return false;

			// Step 1. Verify proof gives old root
			if (treeSize.LeafCount > 0 && !oldRoot.SequenceEqual(AggregateSubRoots(hashAlgorithm, proofArr)))
				return false;

			// Step 2. Iteratively transform the proof (which are the sub-roots) for each appended leaf
			var subRoots = pow2Sum.Zip(proofArr, (height, value) => new MerkleSubRoot(height, value)).ToList();
			foreach(var leaf in leafsArr) {
				AddLeaf(hashAlgorithm, subRoots, leaf);
			}

			// Step 3. Aggregate the transformed proof (sub-roots) to find the new root, and match
			return newRoot.SequenceEqual(AggregateSubRoots(hashAlgorithm, subRoots.Select(x => x.Hash)));
		}

		public static bool VerifyDeleteProof(CHF hashAlgorithm, ReadOnlySpan<byte> oldRoot, int leafCount, ReadOnlySpan<byte> newRoot, int deletedLeafs, IEnumerable<byte[]> proof) {
			Guard.Argument(deletedLeafs <= leafCount, nameof(deletedLeafs), $"Cannot delete more than {nameof(leafCount)}");
			// note: delete is a consistency proof in reverse
			return VerifyConsistencyProof(hashAlgorithm, newRoot, leafCount - deletedLeafs, oldRoot, leafCount, proof);
		}

		#endregion
	}

}