using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {

	public interface IMerkleTree {
		CHF HashAlgorithm { get; }
		byte[] Root { get; }
		MerkleSize Size { get; }
		ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate);
	}

	public static class IMerkleTreeExtensions {

		public static MerkleNode GetNodeAt(this IMerkleTree tree, MerkleCoordinate coordinate) {
			return new MerkleNode(coordinate, tree.GetValue(coordinate).ToArray());
		}

		public static MerkleNode GetRootNode(this IMerkleTree tree) {
			return tree.GetNodeAt(MerkleCoordinate.Root(tree.Size));
		}

		public static byte[] CalculateOldRoot(this IMerkleTree tree, int priorLeafCount)
			=> MerkleMath.AggregateSubRoots(tree.HashAlgorithm, MerkleMath.CalculateSubRoots(priorLeafCount).Select(c => tree.GetValue(c).ToArray()));
			
		public static (MerkleNode Left, MerkleNode Right) GetChildren(this IMerkleTree tree, MerkleCoordinate node) {
			Guard.Argument(!MerkleMath.IsLeaf(node), nameof(node), "Leaf node has no descendant node(s)");
			var childCoords = MerkleMath.GetChildren(tree.Size, node);
			var left = new MerkleNode(childCoords.Left, tree.GetValue(childCoords.Left).ToArray());
			var right = childCoords.Right != MerkleCoordinate.Null ?  new MerkleNode(childCoords.Right, tree.GetValue(childCoords.Right).ToArray()) : null;
			return (left, right);
		}


		public static IEnumerable<MerkleNode> GetPathToRoot(this IMerkleTree tree, int leafIndex, bool logicalNodesOnly) =>
			tree.GetPathToRoot(MerkleCoordinate.LeafAt(leafIndex), logicalNodesOnly);

		public static IEnumerable<MerkleNode> GetPathToRoot(this IMerkleTree tree, MerkleCoordinate nodeCoordinate, bool logicalNodesOnly) =>
			MerkleMath
				.CalculatePathToRoot(tree.Size, nodeCoordinate, logicalNodesOnly)
				.Select(tree.GetNodeAt);

		public static IEnumerable<byte[]> GenerateExistenceProof(this IMerkleTree tree, int leafIndex) =>
			MerkleMath.GenerateExistenceProof(tree, MerkleCoordinate.LeafAt(leafIndex), out _);

		public static IEnumerable<byte[]> GenerateConsistencyProof(this IMerkleTree tree, int priorLeafCount) =>
			MerkleMath.GenerateConsistencyProof(tree, priorLeafCount, out _);

		public static IEnumerable<byte[]> GenerateContainsProof(this IMerkleTree tree, int index, int count) =>
			tree.GenerateContainsProof(Enumerable.Range(index, count));

		public static IEnumerable<byte[]> GenerateContainsProof(this IMerkleTree tree, IEnumerable<int> leafIndices) =>
			MerkleMath.GenerateContainsProof(tree, leafIndices, out _);

		public static IEnumerable<byte[]> GenerateUpdateProof(this IMerkleTree tree, int index, int count) =>
			tree.GenerateUpdateProof(Enumerable.Range(index, count));

		public static IEnumerable<byte[]> GenerateUpdateProof(this IMerkleTree tree, IEnumerable<int> leafIndices) =>
			MerkleMath.GenerateUpdateProof(tree, leafIndices, out _);

		public static IEnumerable<byte[]> GenerateAppendProof(this IMerkleTree tree)
			=> MerkleMath.GenerateAppendProof(tree);

		public static IEnumerable<byte[]> GenerateDeleteProof(this IMerkleTree tree, int deletedLeafCount)
			=> MerkleMath.GenerateDeleteProof(tree, deletedLeafCount, out _);

	}

}