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
/// Specifies the traversal order when walking a merkle tree.
/// </summary>
public enum TreeTraversalType {
	/// <summary>
	/// Visit the node before its children.
	/// </summary>
	PreOrder,
	/// <summary>
	/// Visit the node after its children.
	/// </summary>
	PostOrder,
	/// <summary>
	/// Visit the node between its left and right children.
	/// </summary>
	InOrder,
	/// <summary>
	/// Visit nodes level by level from the root.
	/// </summary>
	LevelOrder
}
