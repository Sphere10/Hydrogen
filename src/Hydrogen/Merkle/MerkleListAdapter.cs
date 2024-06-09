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

namespace Hydrogen;

/// <summary>
/// Merkleizes an <see cref="IExtendedList{T}"/> by hashing it's items and maintaining those hashes in an <see cref="IDynamicMerkleTree"/> .
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class MerkleListAdapter<TItem, TList> : ExtendedListDecorator<TItem, TList>, IMerkleList<TItem>
	where TList : IExtendedList<TItem> {
	protected readonly IItemHasher<TItem> ItemHasher;
	protected readonly IDynamicMerkleTree InternalMerkleTree;

	public MerkleListAdapter(TList internalList)
		: this(internalList, CHF.SHA2_256) {
	}

	public MerkleListAdapter(TList internalList, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness)
		: this(internalList, ItemSerializer<TItem>.Default, hashAlgorithm, endianness) {
	}

	public MerkleListAdapter(TList internalList, IItemSerializer<TItem> serializer, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness)
		: this(internalList, new ItemDigestor<TItem>(hashAlgorithm, serializer, endianness), new FlatMerkleTree(hashAlgorithm)) {
	}

	public MerkleListAdapter(TList internalList, IItemHasher<TItem> hasher, IDynamicMerkleTree internalMerkleTree)
		: base(internalList) {
		ItemHasher = hasher is not WithNullValueItemHasher<TItem> ? hasher.WithNullHash(internalMerkleTree.HashAlgorithm) : hasher;
		InternalMerkleTree = internalMerkleTree;
	}

	public IMerkleTree MerkleTree => InternalMerkleTree;

	public override void Add(TItem item) {
		InternalMerkleTree.Leafs.Add(ItemHasher.Hash(item));
		base.Add(item);
	}

	public override void AddRange(IEnumerable<TItem> items) {
		var itemsArr = items as TItem[] ?? items.ToArray();
		InternalMerkleTree.Leafs.AddRange(itemsArr.Select(ItemHasher.Hash));
		base.AddRange(itemsArr);
	}

	public override void Update(long index, TItem item) {
		InternalMerkleTree.Leafs.Update(index, ItemHasher.Hash(item));
		base.Update(index, item);
	}

	public override void UpdateRange(long fromIndex, IEnumerable<TItem> leafs) {
		var leafsArr = leafs as TItem[] ?? leafs.ToArray();
		InternalMerkleTree.Leafs.UpdateRange(fromIndex, leafsArr.Select(ItemHasher.Hash));
		base.UpdateRange(fromIndex, leafsArr);
	}

	public override void Insert(long index, TItem item) {
		InternalMerkleTree.Leafs.Insert(index, ItemHasher.Hash(item));
		base.Insert(index, item);
	}

	public override void InsertRange(long index, IEnumerable<TItem> leafs) {
		var leafsArr = leafs as TItem[] ?? leafs.ToArray();
		InternalMerkleTree.Leafs.InsertRange(index, leafsArr.Select(ItemHasher.Hash));
		base.InsertRange(index, leafsArr);
	}

	public override bool Remove(TItem item) {
		var ix = IndexOf(item);
		if (ix < 0)
			return false;
		RemoveAt(ix);
		return true;
	}

	public override void RemoveAt(long index) {
		InternalMerkleTree.Leafs.RemoveAt(index);
		base.RemoveAt(index);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items)
		=> items.Select(Remove).ToArray();

	public override void RemoveRange(long fromIndex, long count) {
		InternalMerkleTree.Leafs.RemoveRange(fromIndex, count);
		base.RemoveRange(fromIndex, count);
	}

	public override void Clear() {
		InternalMerkleTree.Leafs.Clear();
		base.Clear();
	}

}


/// <summary>
/// Converts a given <see cref="IExtendedList{T}"/> into an <see cref="IMerkleList{TItem}"/>.
/// A <see cref="MerkleListAdapter{TItem}"/> can also be used stand-alone in-memory as a merkleized list of items.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class MerkleListAdapter<TItem> : MerkleListAdapter<TItem, IExtendedList<TItem>> {

	public MerkleListAdapter()
		: this(new ExtendedList<TItem>()) {
	}

	public MerkleListAdapter(CHF chf)
		: this(new ExtendedList<TItem>(), chf) {
	}
	public MerkleListAdapter(IExtendedList<TItem> internalList)
		: base(internalList, CHF.SHA2_256) {
	}

	public MerkleListAdapter(IExtendedList<TItem> internalList, CHF hashAlgorithm)
		: base(internalList, ItemSerializer<TItem>.Default, hashAlgorithm) {
	}
	public MerkleListAdapter(IItemSerializer<TItem> serializer, CHF hashAlgorithm)
		: this(new ExtendedList<TItem>(), serializer, hashAlgorithm) {
	}

	public MerkleListAdapter(IExtendedList<TItem> internalList, IItemSerializer<TItem> serializer, CHF hashAlgorithm)
		: base(internalList, new ItemDigestor<TItem>(hashAlgorithm, serializer), new FlatMerkleTree(hashAlgorithm)) {
	}

	public MerkleListAdapter(IExtendedList<TItem> internalList, IItemHasher<TItem> hasher, IDynamicMerkleTree merkleTree)
		: base(internalList, hasher, merkleTree) {
	}
}
