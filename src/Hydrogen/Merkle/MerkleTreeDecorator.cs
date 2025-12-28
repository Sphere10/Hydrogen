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

	protected MerkleTreeDecorator(TMerkleTree internalMerkleTree) {
		Guard.ArgumentNotNull(internalMerkleTree, nameof(internalMerkleTree));
		InternalMerkleTree = internalMerkleTree;
	}

	protected TMerkleTree InternalMerkleTree { get; }

	public virtual CHF HashAlgorithm => InternalMerkleTree.HashAlgorithm;

	public virtual byte[] Root => InternalMerkleTree.Root;

	public virtual MerkleSize Size => InternalMerkleTree.Size;

	public virtual ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) => InternalMerkleTree.GetValue(coordinate);
}

/// <summary>
/// Non-generic convenience decorator base for merkle trees.
/// </summary>
public abstract class MerkleTreeDecorator : MerkleTreeDecorator<IMerkleTree> {
	protected MerkleTreeDecorator(IMerkleTree internalMerkleTree) : base(internalMerkleTree) {
	}
}
