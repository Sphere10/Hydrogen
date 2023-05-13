namespace Hydrogen {
	public abstract class DynamicMerkleTreeDecorator<TMerkleTree> : MerkleTreeDecorator<TMerkleTree>, IDynamicMerkleTree where TMerkleTree : IDynamicMerkleTree {
		protected DynamicMerkleTreeDecorator(TMerkleTree internalMerkleTree)
			: base(internalMerkleTree) {
		}
		public IExtendedList<byte[]> Leafs => InternalMerkleTree.Leafs;
	}

	public abstract class DynamicMerkleTreeDecorator : DynamicMerkleTreeDecorator<IDynamicMerkleTree> {
		protected DynamicMerkleTreeDecorator(IDynamicMerkleTree internalMerkleTree)
			: base(internalMerkleTree) {
		}
	}
}