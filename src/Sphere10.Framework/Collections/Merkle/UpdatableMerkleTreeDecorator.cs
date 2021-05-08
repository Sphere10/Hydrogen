namespace Sphere10.Framework {
	public abstract class UpdatableMerkleTreeDecorator<TMerkleTree> : MerkleTreeDecorator<TMerkleTree>, IUpdateableMerkleTree where TMerkleTree : IUpdateableMerkleTree {
        protected UpdatableMerkleTreeDecorator(TMerkleTree internalMerkleTree)
            : base(internalMerkleTree) {
        }
        public IExtendedList<byte[]> Leafs => InternalMerkleTree.Leafs;
    }

	public abstract class UpdatableMerkleTreeDecorator : UpdatableMerkleTreeDecorator<IUpdateableMerkleTree> {
		protected UpdatableMerkleTreeDecorator(IUpdateableMerkleTree internalMerkleTree)
			: base(internalMerkleTree) {
		}
	}
}