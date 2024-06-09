// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
//
// NOTE: This file is part of the reference implementation for Dynamic Merkle-Trees. Read the paper at:
// Web: https://sphere10.com/tech/dynamic-merkle-trees
// e-print: https://vixra.org/abs/2305.0087

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

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

	public static IEnumerable<byte[]> GetLeafs(this IMerkleTree tree) {
		return Tools.Collection.RangeL(0, tree.Size.LeafCount).Select(i => tree.GetValue(MerkleCoordinate.LeafAt(i)).ToArray());
	}

	public static byte[] CalculateOldRoot(this IMerkleTree tree, int priorLeafCount)
		=> MerkleMath.AggregateSubRoots(tree.HashAlgorithm, MerkleMath.CalculateSubRoots(priorLeafCount).Select(c => tree.GetValue(c).ToArray()));

	public static (MerkleNode Left, MerkleNode Right) GetChildren(this IMerkleTree tree, MerkleCoordinate node) {
		Guard.Argument(!MerkleMath.IsLeaf(node), nameof(node), "Leaf node has no descendant node(s)");
		var childCoords = MerkleMath.GetChildren(tree.Size, node);
		var left = new MerkleNode(childCoords.Left, tree.GetValue(childCoords.Left).ToArray());
		var right = childCoords.Right != MerkleCoordinate.Null ? new MerkleNode(childCoords.Right, tree.GetValue(childCoords.Right).ToArray()) : null;
		return (left, right);
	}


	public static IEnumerable<MerkleNode> GetPathToRoot(this IMerkleTree tree, int leafIndex, bool logicalNodesOnly) =>
		tree.GetPathToRoot(MerkleCoordinate.LeafAt(leafIndex), logicalNodesOnly);

	public static IEnumerable<MerkleNode> GetPathToRoot(this IMerkleTree tree, MerkleCoordinate nodeCoordinate, bool logicalNodesOnly) =>
		MerkleMath
			.CalculatePathToRoot(tree.Size, nodeCoordinate, logicalNodesOnly)
			.Select(tree.GetNodeAt);

	public static IEnumerable<byte[]> GenerateExistenceProof(this IMerkleTree tree, long leafIndex) =>
		MerkleMath.GenerateExistenceProof(tree, MerkleCoordinate.LeafAt(leafIndex), out _);

	public static IEnumerable<byte[]> GenerateConsistencyProof(this IMerkleTree tree, long priorLeafCount) =>
		MerkleMath.GenerateConsistencyProof(tree, priorLeafCount, out _);

	public static IEnumerable<byte[]> GenerateContainsProof(this IMerkleTree tree, long index, long count) =>
		tree.GenerateContainsProof(Tools.Collection.RangeL(index, count));

	public static IEnumerable<byte[]> GenerateContainsProof(this IMerkleTree tree, IEnumerable<long> leafIndices) =>
		MerkleMath.GenerateContainsProof(tree, leafIndices, out _);

	public static IEnumerable<byte[]> GenerateUpdateProof(this IMerkleTree tree, long index, long count) =>
		tree.GenerateUpdateProof(Tools.Collection.RangeL(index, count));

	public static IEnumerable<byte[]> GenerateUpdateProof(this IMerkleTree tree, IEnumerable<long> leafIndices) =>
		MerkleMath.GenerateUpdateProof(tree, leafIndices, out _);

	public static IEnumerable<byte[]> GenerateAppendProof(this IMerkleTree tree)
		=> MerkleMath.GenerateAppendProof(tree);

	public static IEnumerable<byte[]> GenerateDeleteProof(this IMerkleTree tree, int deletedLeafCount)
		=> MerkleMath.GenerateDeleteProof(tree, deletedLeafCount, out _);

}
