namespace Hydrogen {
	public abstract class EditableMerkleTreeDecorator<TMerkleTree> : MerkleTreeDecorator<TMerkleTree>, IEditableMerkleTree where TMerkleTree : IEditableMerkleTree {
		protected EditableMerkleTreeDecorator(TMerkleTree internalMerkleTree)
			: base(internalMerkleTree) {
		}
		public IExtendedList<byte[]> Leafs => InternalMerkleTree.Leafs;
	}

	public abstract class EditableMerkleTreeDecorator : EditableMerkleTreeDecorator<IEditableMerkleTree> {
		protected EditableMerkleTreeDecorator(IEditableMerkleTree internalMerkleTree)
			: base(internalMerkleTree) {
		}
	}
}