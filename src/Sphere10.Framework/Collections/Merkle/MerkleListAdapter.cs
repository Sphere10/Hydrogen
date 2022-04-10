
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
	private readonly IItemHasher<TItem> _hasher;
	private readonly IUpdateableMerkleTree _merkleTree;

	public MerkleListAdapter(TList internalList)
		: this(internalList, CHF.SHA2_256) {
	}

	public MerkleListAdapter(TList internalList, CHF hashAlgorithm)
		: this(internalList, ItemSerializer<TItem>.Default, hashAlgorithm) {
	}

	public MerkleListAdapter(TList internalList, IItemSerializer<TItem> serializer, CHF hashAlgorithm)
		: this(internalList, new ItemHasher<TItem>(hashAlgorithm, serializer), new FlatMerkleTree(hashAlgorithm)) {
	}

	public MerkleListAdapter(TList internalList, IItemHasher<TItem> hasher, IUpdateableMerkleTree merkleTreeImpl)
		: base(internalList) {
		_hasher = hasher;
		_merkleTree = merkleTreeImpl;
	}

	public IMerkleTree MerkleTree => _merkleTree;

	public override void Add(TItem item) {
		_merkleTree.Leafs.Add(_hasher.Hash(item));
		base.Add(item);
	}

	public override void AddRange(IEnumerable<TItem> items) {
		var itemsArr = items as TItem[] ?? items.ToArray();
		_merkleTree.Leafs.AddRange(itemsArr.Select(_hasher.Hash));
		base.AddRange(itemsArr);
	}

	public override void Update(int index, TItem item) {
		_merkleTree.Leafs.Update(index, _hasher.Hash(item));
		base.Update(index, item);
	}

	public override void UpdateRange(int fromIndex, IEnumerable<TItem> leafs) {
		var leafsArr = leafs as TItem[] ?? leafs.ToArray();
		_merkleTree.Leafs.UpdateRange(fromIndex, leafsArr.Select(_hasher.Hash));
		base.UpdateRange(fromIndex, leafsArr);
	}

	public override void Insert(int index, TItem item) {
		_merkleTree.Leafs.Insert(index, _hasher.Hash(item));
		base.Insert(index, item);
	}

	public override void InsertRange(int index, IEnumerable<TItem> leafs) {
		var leafsArr = leafs as TItem[] ?? leafs.ToArray();
		_merkleTree.Leafs.InsertRange(index, leafsArr.Select(_hasher.Hash));
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
		_merkleTree.Leafs.RemoveAt(index);
		base.RemoveAt(index);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items)
		=> items.Select(Remove).ToArray();

	public override void RemoveRange(int fromIndex, int count) {
		_merkleTree.Leafs.RemoveRange(fromIndex, count);
		base.RemoveRange(fromIndex, count);
	}

	public override void Clear() {
		base.Clear();
		_merkleTree.Leafs.Clear();
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

	public MerkleListAdapter(IExtendedList<TItem> internalList, IItemHasher<TItem> hasher, IUpdateableMerkleTree merkleTreeImpl)
		: base(internalList, hasher, merkleTreeImpl) {
	}
}