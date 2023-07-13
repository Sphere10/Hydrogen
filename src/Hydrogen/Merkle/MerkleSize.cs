// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
//
// NOTE: This file is part of the reference implementation for Dynamic Merkle-Trees. Read the paper at:
// Web: https://sphere10.com/tech/dynamic-merkle-trees
// e-print: https://vixra.org/abs/2305.0087

namespace Hydrogen;

public record MerkleSize {
	public int LeafCount;
	public int Height;

	public static MerkleSize FromLeafCount(int leafCount) {
		Guard.ArgumentInRange(leafCount, 0, int.MaxValue, nameof(leafCount));
		return new MerkleSize {
			LeafCount = leafCount,
			Height = MerkleMath.CalculateHeight(leafCount)
		};
	}

	public override string ToString() {
		return $"(H:{Height}, LC:{LeafCount})";
	}

}
