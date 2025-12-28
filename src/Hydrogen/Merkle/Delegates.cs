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
/// Delegate that retrieves a leaf value by index as a byte span.
/// </summary>
public delegate ReadOnlySpan<byte> MerkleTreeLeafGetter(int index);

/// <summary>
/// Delegate that returns the current number of leaves in a merkle tree.
/// </summary>
public delegate int MerkleTreeLeafCounter();

/// <summary>
/// Delegate that yields sub-roots representing independent merkle subtrees.
/// </summary>
public delegate IEnumerable<MerkleSubRoot> MerkleTreeSubRootsGetter();
