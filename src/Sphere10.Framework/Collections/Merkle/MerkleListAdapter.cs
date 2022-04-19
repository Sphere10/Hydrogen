
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework;

/// <summary>
/// Adapts a <see cref="TList"/> into an <see cref="IMerkleList{TItem}"/>.
/// A <see cref="MerkleListAdapter{TItem}"/> can also be used stand-alone in-memory as a merkleized list of items.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class MerkleListAdapter<TItem, TList> : ExtendedListDecorator<TItem, TList>, IMerkleList<TItem> 
	where TList : IExtendedList<TItem> {
	protected readonly IItemHasher<TItem> ItemHasher;
	protected readonly IEditableMerkleTree InternalMerkleTree;

	public MerkleListAdapter(TList internalList)
		: this(internalList, CHF.SHA2_256) {
	}

	public MerkleListAdapter(TList internalList, CHF hashAlgorithm)
		: this(internalList, ItemSerializer<TItem>.Default, hashAlgorithm) {
	}

	public MerkleListAdapter(TList internalList, IItemSerializer<TItem> serializer, CHF hashAlgorithm)
		: this(internalList, new ItemHasher<TItem>(hashAlgorithm, serializer), new FlatMerkleTree(hashAlgorithm)) {
	}

	public MerkleListAdapter(TList internalList, IItemHasher<TItem> hasher, IEditableMerkleTree merkleTreeImpl)
		: base(internalList) {
		ItemHasher = hasher.WithNullHash(Tools.Array.Gen<byte>(Hashers.GetDigestSizeBytes(merkleTreeImpl.HashAlgorithm), 0));
		InternalMerkleTree = merkleTreeImpl;
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

	public override void Update(int index, TItem item) {
		InternalMerkleTree.Leafs.Update(index, ItemHasher.Hash(item));
		base.Update(index, item);
	}

	public override void UpdateRange(int fromIndex, IEnumerable<TItem> leafs) {
		var leafsArr = leafs as TItem[] ?? leafs.ToArray();
		InternalMerkleTree.Leafs.UpdateRange(fromIndex, leafsArr.Select(ItemHasher.Hash));
		base.UpdateRange(fromIndex, leafsArr);
	}

	public override void Insert(int index, TItem item) {
		InternalMerkleTree.Leafs.Insert(index, ItemHasher.Hash(item));
		base.Insert(index, item);
	}

	public override void InsertRange(int index, IEnumerable<TItem> leafs) {
		var leafsArr = leafs as TItem[] ?? leafs.ToArray();
		InternalMerkleTree.Leafs.InsertRange(index, leafsArr.Select(ItemHasher.Hash));
		base.InsertRange(index, leafsArr);
	}

	public override bool Remove(TItem item) {
		var ix = IndexOf(item);
		if (ix < 0)
			return false;
		this.RemoveAt(ix);
		return true;
	}

	public override void RemoveAt(int index) {
		InternalMerkleTree.Leafs.RemoveAt(index);
		base.RemoveAt(index);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items)
		=> items.Select(Remove).ToArray();

	public override void RemoveRange(int fromIndex, int count) {
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
public class MerkleListAdapter<TItem> : MerkleListAdapter<TItem, IExtendedList<TItem>>{
	
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
		: base(internalList, new ItemHasher<TItem>(hashAlgorithm, serializer), new FlatMerkleTree(hashAlgorithm)) {
	}

	public MerkleListAdapter(IExtendedList<TItem> internalList, IItemHasher<TItem> hasher, IEditableMerkleTree merkleTreeImpl)
		: base(internalList, hasher, merkleTreeImpl) {
	}
}