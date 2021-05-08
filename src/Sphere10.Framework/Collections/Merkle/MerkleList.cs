using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public class MerkleList<TItem> : ExtendedListDecorator<TItem>, IMerkleList<TItem> {

		private readonly IItemHasher<TItem> _hasher;
		private readonly IUpdateableMerkleTree _merkleTree;

		public MerkleList(IItemSerializer<TItem> serializer, CHF hashAlgorithm) 
			: this(new ItemHasher<TItem>(hashAlgorithm, serializer), new ExtendedList<TItem>(), new SimpleMerkleTree(hashAlgorithm)) {
		}

		public MerkleList(IItemHasher<TItem> hasher, IExtendedList<TItem> internalList, IUpdateableMerkleTree merkleTreeImpl) 
			: base(internalList) {
			_hasher = hasher;
			_merkleTree = merkleTreeImpl;
		}

		public IMerkleTree MerkleTree => _merkleTree;

		public override void Add(TItem item) {
			base.Add(item);
			_merkleTree.Leafs.Add(_hasher.Hash(item));
		}

		public override void AddRange(IEnumerable<TItem> items) {
			var itemsArr = items as TItem[] ?? items.ToArray();
			base.AddRange(itemsArr);
			_merkleTree.Leafs.AddRange(itemsArr.Select(_hasher.Hash));
		}

		public override void Update(int index, TItem item) {
			base.Update(index, item);
			_merkleTree.Leafs.Update(index, _hasher.Hash(item));
		}

		public override void UpdateRange(int fromIndex, IEnumerable<TItem> leafs) {
			var leafsArr = leafs as TItem[] ?? leafs.ToArray();
			base.UpdateRange(fromIndex, leafsArr);
			_merkleTree.Leafs.UpdateRange(fromIndex, leafsArr.Select(_hasher.Hash));
		}

		public override void Insert(int index, TItem item) {
			base.Insert(index, item);
			_merkleTree.Leafs.Insert(index, _hasher.Hash(item));
		}

		public override void InsertRange(int index, IEnumerable<TItem> leafs) {
			var leafsArr = leafs as TItem[] ?? leafs.ToArray();
			base.InsertRange(index, leafsArr);
			_merkleTree.Leafs.InsertRange(index, leafsArr.Select(_hasher.Hash));
		}

		public override bool Remove(TItem item) {
			var ix = IndexOf(item);
			if (ix < 0)
				return false;
			this.RemoveAt(ix);
			return true;
		}

		public override void RemoveAt(int index) {
			base.RemoveAt(index);
			_merkleTree.Leafs.RemoveAt(index);
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items)
			=> items.Select(Remove).ToArray();

		public override void RemoveRange(int fromIndex, int count) {
			base.RemoveRange(fromIndex, count);
			_merkleTree.Leafs.RemoveRange(fromIndex, count);
		}

		public override void Clear() {
			InternalExtendedList.Clear();
			_merkleTree.Leafs.Clear();
		}

	}
}