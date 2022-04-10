using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;

namespace Tools;

public static class MerkleTree {

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items)
		=> ComputeMerkleRoot(items, CHF.SHA2_256);

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, CHF chf, IItemSerializer<TItem> serializer = null)
		=> ComputeMerkleRoot(items, new ItemHasher<TItem>(chf, serializer ?? ItemSerializer<TItem>.Default), chf);

	public static byte[] ComputeMerkleRoot<TItem>(IEnumerable<TItem> items, IItemHasher<TItem> hasher, CHF chf)
		=> ComputeMerkleRoot(items.Select(hasher.Hash), chf);

	public static byte[] ComputeMerkleRoot(IEnumerable<byte[]> leafs, CHF chf) {
		var simpleMerkleTree = new SimpleMerkleTree(chf, leafs);
		return simpleMerkleTree.Root;
	}

}
