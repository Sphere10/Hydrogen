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

using System;

namespace Hydrogen;

[Flags]
public enum MerkleNodeTraits : ushort {
	Root = 1 << 0,
	Leaf = 1 << 1,
	Perfect = 1 << 2,
	IsLeftChild = 1 << 3,
	IsRightChild = 1 << 4,
	BubblesUp = 1 << 5,
	HasLeftChild = 1 << 6,
	HasRightChild = 1 << 7,
	BubbledUp = 1 << 8,
}
