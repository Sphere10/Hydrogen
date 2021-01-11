using System;

namespace Sphere10.Framework {
    public abstract class MerkleTreeDecorator : IMerkleTree {

        protected MerkleTreeDecorator(IMerkleTree internalMerkleTree) {
            Guard.ArgumentNotNull(internalMerkleTree, nameof(internalMerkleTree));
            InternalMerkleTree = internalMerkleTree;
        }

        protected IMerkleTree InternalMerkleTree { get; }

        public virtual CHF HashAlgorithm => InternalMerkleTree.HashAlgorithm;

        public virtual byte[] Root => InternalMerkleTree.Root;

        public virtual MerkleSize Size => InternalMerkleTree.Size;

        public virtual ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) => InternalMerkleTree.GetValue(coordinate);
    }
}