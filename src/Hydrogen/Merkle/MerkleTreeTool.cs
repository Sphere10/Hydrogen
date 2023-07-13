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

using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Hydrogen;

public static class MerkleTree {

	// NOTE: avoid method signatures with "params TItem[] items" since passed in collections of type C are interpreted as arrays of 1 item containing C
	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items)
		=> ComputeMerkleRoot(items, CHF.SHA2_256);

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, CHF chf, IItemSerializer<TItem> serializer = null)
		=> ComputeMerkleRoot(items, new ItemDigestor<TItem>(chf, serializer ?? ItemSerializer<TItem>.Default), chf);

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, IItemHasher<TItem> hasher, CHF chf) {
		hasher = hasher.WithNullHash(Array.Gen<byte>(Hashers.GetDigestSizeBytes(chf), 0));
		return ComputeMerkleRoot(items.Select(hasher.Hash), chf);
	}

	public static byte[] ComputeMerkleRoot(IEnumerable<byte[]> leafs, CHF chf) {
		var simpleMerkleTree = new SimpleMerkleTree(chf, leafs);
		return simpleMerkleTree.Root;
	}

}
