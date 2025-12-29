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
/// Captures the leaf count and height for a merkle tree.
/// </summary>
public record MerkleSize {
	/// <summary>
	/// Gets the number of leaves represented by the tree.
	/// </summary>
	public long LeafCount;
	/// <summary>
	/// Gets the height of the tree in levels.
	/// </summary>
	public int Height;

	/// <summary>
	/// Builds a merkle size from a leaf count.
	/// </summary>
	/// <param name="leafCount">The number of leaves.</param>
	public static MerkleSize FromLeafCount(long leafCount) {
		Guard.ArgumentInRange(leafCount, 0, long.MaxValue, nameof(leafCount));
		return new MerkleSize {
			LeafCount = leafCount,
			Height = MerkleMath.CalculateHeight(leafCount)
		};
	}

	/// <summary>
	/// Returns a concise string representation of the size.
	/// </summary>
	public override string ToString() {
		return $"(H:{Height}, LC:{LeafCount})";
	}

}
