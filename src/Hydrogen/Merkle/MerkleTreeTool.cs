using System.Collections.Generic;
using System.Linq;
using Hydrogen;
using Tools;

namespace Hydrogen;

public static class MerkleTree {

	// NOTE: avoid method signatures with "params TItem[] items" since passed in collections of type C are interpreted as arrays of 1 item containing C
	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items)
		=> ComputeMerkleRoot(items, CHF.SHA2_256);

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, CHF chf, IItemSerializer<TItem> serializer = null)
		=> ComputeMerkleRoot(items, new ItemHasher<TItem>(chf, serializer ?? ItemSerializer<TItem>.Default), chf);

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, IItemHasher<TItem> hasher, CHF chf) {
		hasher = hasher.WithNullHash(Array.Gen<byte>(Hashers.GetDigestSizeBytes(chf), 0));
		return ComputeMerkleRoot(items.Select(hasher.Hash), chf);
	}

	public static byte[] ComputeMerkleRoot(IEnumerable<byte[]> leafs, CHF chf) {
		var simpleMerkleTree = new SimpleMerkleTree(chf, leafs);
		return simpleMerkleTree.Root;
	}

}
