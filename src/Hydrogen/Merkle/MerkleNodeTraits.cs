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
/// Flags describing structural properties of a merkle node within a tree.
/// </summary>
[Flags]
public enum MerkleNodeTraits : ushort {
	/// <summary>
	/// Node is the root of the tree.
	/// </summary>
	Root = 1 << 0,
	/// <summary>
	/// Node is a leaf.
	/// </summary>
	Leaf = 1 << 1,
	/// <summary>
	/// Node belongs to a perfect subtree.
	/// </summary>
	Perfect = 1 << 2,
	/// <summary>
	/// Node is a left child.
	/// </summary>
	IsLeftChild = 1 << 3,
	/// <summary>
	/// Node is a right child.
	/// </summary>
	IsRightChild = 1 << 4,
	/// <summary>
	/// Node value is bubbled up from its left child.
	/// </summary>
	BubblesUp = 1 << 5,
	/// <summary>
	/// Node has a left child.
	/// </summary>
	HasLeftChild = 1 << 6,
	/// <summary>
	/// Node has a right child.
	/// </summary>
	HasRightChild = 1 << 7,
	/// <summary>
	/// Node is the result of a bubble-up operation.
	/// </summary>
	BubbledUp = 1 << 8,
}
