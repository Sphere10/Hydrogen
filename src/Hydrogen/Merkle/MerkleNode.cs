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
/// Represents a merkle node with its coordinate and hash value.
/// </summary>
public record MerkleNode : IEquatable<MerkleNode> {
	/// <summary>
	/// Gets the coordinate that identifies this node.
	/// </summary>
	public readonly MerkleCoordinate Coordinate;
	/// <summary>
	/// Gets the node hash value.
	/// </summary>
	public readonly byte[] Hash;

	/// <summary>
	/// Initializes a merkle node with the provided coordinate and hash.
	/// </summary>
	/// <param name="coordinate">The node coordinate.</param>
	/// <param name="hash">The node hash.</param>
	public MerkleNode(MerkleCoordinate coordinate, byte[] hash) {
		Coordinate = coordinate;
		Hash = hash;
	}

	/// <summary>
	/// Returns a hash code for this node.
	/// </summary>
	public override int GetHashCode() {
		unchecked {
			return Coordinate.GetHashCode() * 397 ^ ByteArrayEqualityComparer.Instance.GetHashCode(Hash);
		}
	}
}
