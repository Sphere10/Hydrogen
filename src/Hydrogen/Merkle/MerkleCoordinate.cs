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
	/// <summary>
	/// Gets the tree level for the node (0 is leaves).
	/// </summary>
	public readonly int Level;
	/// <summary>
	/// Gets the index within the level.
	/// </summary>
	public readonly long Index;

	private MerkleCoordinate(int level, long index) {
		Level = level;
		Index = index;
	}

	/// <summary>
	/// Returns true when this coordinate is the root for the given size.
	/// </summary>
	/// <param name="size">The merkle tree size.</param>
	public bool IsRoot(MerkleSize size) {
		return Equals(Root(size));
	}

	/// <summary>
	/// Represents a sentinel coordinate for missing nodes.
	/// </summary>
	public static MerkleCoordinate Null =>
		new MerkleCoordinate(-1, -1);

	/// <summary>
	/// Builds a coordinate for a leaf at the given index.
	/// </summary>
	/// <param name="index">The leaf index.</param>
	public static MerkleCoordinate LeafAt(long index) {
		return From(0, index);
	}

	/// <summary>
	/// Builds a coordinate at the given level and index.
	/// </summary>
	/// <param name="level">The tree level.</param>
	/// <param name="index">The index within the level.</param>
	public static MerkleCoordinate From(int level, long index) {
		Guard.ArgumentInRange(level, 0, int.MaxValue, nameof(level));
		Guard.ArgumentInRange(index, 0, long.MaxValue, nameof(index));
		return new MerkleCoordinate(level, index);
	}

	/// <summary>
	/// Gets the root coordinate for the provided leaf count.
	/// </summary>
	/// <param name="leafCount">The number of leaves.</param>
	public static MerkleCoordinate Root(long leafCount) {
		return Root(MerkleSize.FromLeafCount(leafCount));
	}

	/// <summary>
	/// Gets the root coordinate for the provided size.
	/// </summary>
	/// <param name="size">The merkle tree size.</param>
	public static MerkleCoordinate Root(MerkleSize size) {
		return size.Height == 0 ? Null : From(size.Height - 1, 0);
	}

	/// <summary>
	/// Returns the coordinates of sub-roots for the given leaf count.
	/// </summary>
	/// <param name="leafCount">The number of leaves.</param>
	public static IEnumerable<MerkleCoordinate> SubRoots(long leafCount)
		=> MerkleMath.CalculateSubRoots(leafCount);

	/// <summary>
	/// Gets the next coordinate at the same level.
	/// </summary>
	/// <param name="coordinate">The current coordinate.</param>
	public static MerkleCoordinate Next(MerkleCoordinate coordinate) {
		return From(coordinate.Level, coordinate.Index + 1);
	}

	/// <summary>
	/// Gets the last coordinate for the specified level and size.
	/// </summary>
	/// <param name="size">The merkle tree size.</param>
	/// <param name="level">The tree level.</param>
	public static MerkleCoordinate Last(MerkleSize size, int level) {
		return From(level, MerkleMath.CalculateLevelLength(size.LeafCount, level) - 1);
	}

	/// <summary>
	/// Converts this coordinate to its flat index representation.
	/// </summary>
	public int ToFlatIndex() {
		return (int)MerkleMath.ToFlatIndex(this);
	}

	/// <summary>
	/// Returns a hash code for this coordinate.
	/// </summary>
	public override int GetHashCode() {
		unchecked {
			return Level * 397 ^ unchecked((int)Index);
		}
	}

	/// <summary>
	/// Returns a string representation of the coordinate.
	/// </summary>
	public override string ToString() {
		return $"(L:{Level}, IX:{Index})";
	}

}
