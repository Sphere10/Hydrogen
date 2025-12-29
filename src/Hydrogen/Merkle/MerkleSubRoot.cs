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
/// Represents a sub-root hash at a given height within a merkle tree.
/// </summary>
public record MerkleSubRoot {
	/// <summary>
	/// Gets the height of the subtree represented by this sub-root.
	/// </summary>
	public readonly int Height;
	/// <summary>
	/// Gets the hash of the sub-root.
	/// </summary>
	public readonly byte[] Hash;

	/// <summary>
	/// Initializes a sub-root from a height and hash.
	/// </summary>
	/// <param name="height">The subtree height.</param>
	/// <param name="hash">The sub-root hash.</param>
	public MerkleSubRoot(int height, byte[] hash) {
		Height = height;
		Hash = hash;
	}
}
