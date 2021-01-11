namespace Sphere10.Framework {
    public abstract class UpdatableMerkleTreeDecorator : MerkleTreeDecorator, IUpdateableMerkleTree {

        protected UpdatableMerkleTreeDecorator(IUpdateableMerkleTree internalMerkleTree)
            : base(internalMerkleTree) {
        }

        protected new IUpdateableMerkleTree InternalMerkleTree => (IUpdateableMerkleTree)base.InternalMerkleTree;

        public IExtendedList<byte[]> Leafs => InternalMerkleTree.Leafs;
    }
}