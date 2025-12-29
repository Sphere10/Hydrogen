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

namespace Hydrogen;

/// <summary>
/// Decorator base for merkle trees, forwarding calls to an inner tree while allowing subclasses to extend behavior.
/// </summary>
public abstract class MerkleTreeDecorator<TMerkleTree> : IMerkleTree where TMerkleTree : IMerkleTree {

	/// <summary>
	/// Initializes the decorator with an inner merkle tree.
	/// </summary>
	/// <param name="internalMerkleTree">The inner merkle tree to wrap.</param>
	protected MerkleTreeDecorator(TMerkleTree internalMerkleTree) {
		Guard.ArgumentNotNull(internalMerkleTree, nameof(internalMerkleTree));
		InternalMerkleTree = internalMerkleTree;
	}

	/// <summary>
	/// Gets the decorated merkle tree instance.
	/// </summary>
	protected TMerkleTree InternalMerkleTree { get; }

	/// <summary>
	/// Gets the hash algorithm used by the decorated tree.
	/// </summary>
	public virtual CHF HashAlgorithm => InternalMerkleTree.HashAlgorithm;

	/// <summary>
	/// Gets the merkle root of the decorated tree.
	/// </summary>
	public virtual byte[] Root => InternalMerkleTree.Root;

	/// <summary>
	/// Gets the size metadata of the decorated tree.
	/// </summary>
	public virtual MerkleSize Size => InternalMerkleTree.Size;

	/// <summary>
	/// Reads the node hash value at the specified coordinate.
	/// </summary>
	/// <param name="coordinate">The coordinate to read.</param>
	public virtual ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) => InternalMerkleTree.GetValue(coordinate);
}

/// <summary>
/// Non-generic convenience decorator base for merkle trees.
/// </summary>
public abstract class MerkleTreeDecorator : MerkleTreeDecorator<IMerkleTree> {
	/// <summary>
	/// Initializes the decorator with an inner merkle tree.
	/// </summary>
	/// <param name="internalMerkleTree">The inner merkle tree to wrap.</param>
	protected MerkleTreeDecorator(IMerkleTree internalMerkleTree) : base(internalMerkleTree) {
	}
}
