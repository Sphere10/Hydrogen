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

namespace Hydrogen;

/// <summary>
/// Denotes the coordinate of a node within an merkle-tree.
/// </summary>
public record MerkleCoordinate : IEquatable<MerkleCoordinate> {
	public readonly int Level;
	public readonly long Index;

	private MerkleCoordinate(int level, long index) {
		Level = level;
		Index = index;
	}

	public bool IsRoot(MerkleSize size) {
		return Equals(Root(size));
	}

	public static MerkleCoordinate Null =>
		new MerkleCoordinate(-1, -1);

	public static MerkleCoordinate LeafAt(long index) {
		return From(0, index);
	}

	public static MerkleCoordinate From(int level, long index) {
		Guard.ArgumentInRange(level, 0, int.MaxValue, nameof(level));
		Guard.ArgumentInRange(index, 0, long.MaxValue, nameof(index));
		return new MerkleCoordinate(level, index);
	}

	public static MerkleCoordinate Root(long leafCount) {
		return Root(MerkleSize.FromLeafCount(leafCount));
	}

	public static MerkleCoordinate Root(MerkleSize size) {
		return size.Height == 0 ? Null : From(size.Height - 1, 0);
	}

	public static IEnumerable<MerkleCoordinate> SubRoots(long leafCount)
		=> MerkleMath.CalculateSubRoots(leafCount);

	public static MerkleCoordinate Next(MerkleCoordinate coordinate) {
		return From(coordinate.Level, coordinate.Index + 1);
	}

	public static MerkleCoordinate Last(MerkleSize size, int level) {
		return From(level, MerkleMath.CalculateLevelLength(size.LeafCount, level) - 1);
	}

	public int ToFlatIndex() {
		return (int)MerkleMath.ToFlatIndex(this);
	}

	public override int GetHashCode() {
		unchecked {
			return Level * 397 ^ unchecked((int)Index);
		}
	}

	public override string ToString() {
		return $"(L:{Level}, IX:{Index})";
	}

}
