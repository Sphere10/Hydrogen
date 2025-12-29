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

using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Hydrogen;

/// <summary>
/// Helper methods for computing merkle roots from collections of items or pre-hashed leaves.
/// </summary>
public static class MerkleTree {

	// NOTE: avoid method signatures with "params TItem[] items" since passed in collections of type C are interpreted as arrays of 1 item containing C
	/// <summary>
	/// Computes a merkle root using SHA2-256 over the provided items.
	/// </summary>
	/// <param name="items">The items to hash.</param>
	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items)
		=> ComputeMerkleRoot(items, CHF.SHA2_256);

	/// <summary>
	/// Computes a merkle root using the provided hash algorithm and serializer.
	/// </summary>
	/// <param name="items">The items to hash.</param>
	/// <param name="chf">The hash function to use.</param>
	/// <param name="serializer">The serializer for items.</param>
	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, CHF chf, IItemSerializer<TItem> serializer = null)
		=> ComputeMerkleRoot(items, new ItemDigestor<TItem>(chf, serializer ?? ItemSerializer<TItem>.Default), chf);

	/// <summary>
	/// Computes a merkle root using the supplied item hasher.
	/// </summary>
	/// <param name="items">The items to hash.</param>
	/// <param name="hasher">The hasher to apply.</param>
	/// <param name="chf">The hash function to use.</param>
	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, IItemHasher<TItem> hasher, CHF chf) {
		hasher = hasher.WithNullHash(Array.Gen<byte>(Hashers.GetDigestSizeBytes(chf), 0));
		return ComputeMerkleRoot(items.Select(hasher.Hash), chf);
	}

	/// <summary>
	/// Computes a merkle root from pre-hashed leaf values.
	/// </summary>
	/// <param name="leafs">The leaf hashes.</param>
	/// <param name="chf">The hash function to use.</param>
	public static byte[] ComputeMerkleRoot(IEnumerable<byte[]> leafs, CHF chf) {
		var simpleMerkleTree = new SimpleMerkleTree(chf, leafs);
		return simpleMerkleTree.Root;
	 }

}
