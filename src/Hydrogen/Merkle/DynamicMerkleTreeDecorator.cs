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

namespace Hydrogen;

/// <summary>
/// Base decorator for dynamic merkle trees, forwarding operations to an inner implementation while exposing dynamic leaf access.
/// </summary>
public abstract class DynamicMerkleTreeDecorator<TMerkleTree> : MerkleTreeDecorator<TMerkleTree>, IDynamicMerkleTree where TMerkleTree : IDynamicMerkleTree {
	/// <summary>
	/// Initializes the decorator with a dynamic merkle tree instance.
	/// </summary>
	/// <param name="internalMerkleTree">The inner merkle tree to wrap.</param>
	protected DynamicMerkleTreeDecorator(TMerkleTree internalMerkleTree)
		: base(internalMerkleTree) {
	}
	/// <summary>
	/// Gets the leaf collection exposed by the decorated tree.
	/// </summary>
	public IExtendedList<byte[]> Leafs => InternalMerkleTree.Leafs;
}


/// <summary>
/// Non-generic convenience wrapper for decorating dynamic merkle trees.
/// </summary>
public abstract class DynamicMerkleTreeDecorator : DynamicMerkleTreeDecorator<IDynamicMerkleTree> {
	/// <summary>
	/// Initializes the decorator with a dynamic merkle tree instance.
	/// </summary>
	/// <param name="internalMerkleTree">The inner merkle tree to wrap.</param>
	protected DynamicMerkleTreeDecorator(IDynamicMerkleTree internalMerkleTree)
		: base(internalMerkleTree) {
	}
}
