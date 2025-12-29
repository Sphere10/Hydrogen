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

/// <summary>
/// Represents a merkle tree capable of reporting its root, size, and node values by coordinate.
/// </summary>
public interface IMerkleTree {
	/// <summary>
	/// Gets the hash algorithm used for node aggregation and leaf hashing.
	/// </summary>
	CHF HashAlgorithm { get; }

	/// <summary>
	/// Gets the current merkle root for this tree.
	/// </summary>
	byte[] Root { get; }

	/// <summary>
	/// Gets the size metadata describing leaf count and height.
	/// </summary>
	MerkleSize Size { get; }

	/// <summary>
	/// Retrieves the hash value stored at a specific coordinate.
	/// </summary>
	/// <param name="coordinate">The coordinate of the node to read.</param>
	ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate);
}

/// <summary>
/// Extension helpers for reading nodes and producing proofs from merkle trees.
/// </summary>
public static class IMerkleTreeExtensions {

	/// <summary>
	/// Reads a node at the given coordinate and returns its coordinate/hash pair.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="coordinate">The coordinate to read.</param>
	public static MerkleNode GetNodeAt(this IMerkleTree tree, MerkleCoordinate coordinate) {
		return new MerkleNode(coordinate, tree.GetValue(coordinate).ToArray());
	}

	/// <summary>
	/// Reads the root node for the current tree size.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	public static MerkleNode GetRootNode(this IMerkleTree tree) {
		return tree.GetNodeAt(MerkleCoordinate.Root(tree.Size));
	}

	/// <summary>
	/// Returns all leaf hashes in order.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	public static IEnumerable<byte[]> GetLeafs(this IMerkleTree tree) {
		return Tools.Collection.RangeL(0, tree.Size.LeafCount).Select(i => tree.GetValue(MerkleCoordinate.LeafAt(i)).ToArray());
	}

	/// <summary>
	/// Calculates the merkle root that would have existed at a prior leaf count.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="priorLeafCount">The prior leaf count to evaluate.</param>
	public static byte[] CalculateOldRoot(this IMerkleTree tree, int priorLeafCount)
		=> MerkleMath.AggregateSubRoots(tree.HashAlgorithm, MerkleMath.CalculateSubRoots(priorLeafCount).Select(c => tree.GetValue(c).ToArray()));

	/// <summary>
	/// Retrieves the left and right children for a non-leaf node.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="node">The node coordinate.</param>
	/// <exception cref="ArgumentException">Thrown when the node is a leaf.</exception>
	public static (MerkleNode Left, MerkleNode Right) GetChildren(this IMerkleTree tree, MerkleCoordinate node) {
		Guard.Argument(!MerkleMath.IsLeaf(node), nameof(node), "Leaf node has no descendant node(s)");
		var childCoords = MerkleMath.GetChildren(tree.Size, node);
		var left = new MerkleNode(childCoords.Left, tree.GetValue(childCoords.Left).ToArray());
		var right = childCoords.Right != MerkleCoordinate.Null ? new MerkleNode(childCoords.Right, tree.GetValue(childCoords.Right).ToArray()) : null;
		return (left, right);
	}


	/// <summary>
	/// Returns the node path from the specified leaf to the root.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="leafIndex">The leaf index.</param>
	/// <param name="logicalNodesOnly">Whether to skip bubbled nodes.</param>
	public static IEnumerable<MerkleNode> GetPathToRoot(this IMerkleTree tree, int leafIndex, bool logicalNodesOnly) =>
		tree.GetPathToRoot(MerkleCoordinate.LeafAt(leafIndex), logicalNodesOnly);

	/// <summary>
	/// Returns the node path from the specified coordinate to the root.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="nodeCoordinate">The starting coordinate.</param>
	/// <param name="logicalNodesOnly">Whether to skip bubbled nodes.</param>
	public static IEnumerable<MerkleNode> GetPathToRoot(this IMerkleTree tree, MerkleCoordinate nodeCoordinate, bool logicalNodesOnly) =>
		MerkleMath
			.CalculatePathToRoot(tree.Size, nodeCoordinate, logicalNodesOnly)
			.Select(tree.GetNodeAt);

	/// <summary>
	/// Generates a proof that a leaf exists at the given index.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="leafIndex">The leaf index.</param>
	public static IEnumerable<byte[]> GenerateExistenceProof(this IMerkleTree tree, long leafIndex) =>
		MerkleMath.GenerateExistenceProof(tree, MerkleCoordinate.LeafAt(leafIndex), out _);

	/// <summary>
	/// Generates a consistency proof against a prior leaf count.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="priorLeafCount">The prior leaf count.</param>
	public static IEnumerable<byte[]> GenerateConsistencyProof(this IMerkleTree tree, long priorLeafCount) =>
		MerkleMath.GenerateConsistencyProof(tree, priorLeafCount, out _);

	/// <summary>
	/// Generates a proof that a contiguous range of leaves is contained in the tree.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="index">The starting leaf index.</param>
	/// <param name="count">The number of leaves.</param>
	public static IEnumerable<byte[]> GenerateContainsProof(this IMerkleTree tree, long index, long count) =>
		tree.GenerateContainsProof(Tools.Collection.RangeL(index, count));

	/// <summary>
	/// Generates a proof that specific leaf indices are contained in the tree.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="leafIndices">The leaf indices.</param>
	public static IEnumerable<byte[]> GenerateContainsProof(this IMerkleTree tree, IEnumerable<long> leafIndices) =>
		MerkleMath.GenerateContainsProof(tree, leafIndices, out _);

	/// <summary>
	/// Generates a proof for updating a contiguous range of leaves.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="index">The starting leaf index.</param>
	/// <param name="count">The number of leaves.</param>
	public static IEnumerable<byte[]> GenerateUpdateProof(this IMerkleTree tree, long index, long count) =>
		tree.GenerateUpdateProof(Tools.Collection.RangeL(index, count));

	/// <summary>
	/// Generates a proof for updating specific leaf indices.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="leafIndices">The leaf indices.</param>
	public static IEnumerable<byte[]> GenerateUpdateProof(this IMerkleTree tree, IEnumerable<long> leafIndices) =>
		MerkleMath.GenerateUpdateProof(tree, leafIndices, out _);

	/// <summary>
	/// Generates a proof for appending new leaves to the tree.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	public static IEnumerable<byte[]> GenerateAppendProof(this IMerkleTree tree)
		=> MerkleMath.GenerateAppendProof(tree);

	/// <summary>
	/// Generates a proof for deleting a number of leaves from the end of the tree.
	/// </summary>
	/// <param name="tree">The merkle tree to read from.</param>
	/// <param name="deletedLeafCount">The number of deleted tail leaves.</param>
	public static IEnumerable<byte[]> GenerateDeleteProof(this IMerkleTree tree, int deletedLeafCount)
		=> MerkleMath.GenerateDeleteProof(tree, deletedLeafCount, out _);

}
