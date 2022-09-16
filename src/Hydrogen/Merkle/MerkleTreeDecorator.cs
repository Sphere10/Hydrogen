using System;

namespace Hydrogen {
	public abstract class MerkleTreeDecorator<TMerkleTree> : IMerkleTree where TMerkleTree : IMerkleTree {

		protected MerkleTreeDecorator(TMerkleTree internalMerkleTree) {
			Guard.ArgumentNotNull(internalMerkleTree, nameof(internalMerkleTree));
			InternalMerkleTree = internalMerkleTree;
		}

		protected TMerkleTree InternalMerkleTree { get; }

		public virtual CHF HashAlgorithm => InternalMerkleTree.HashAlgorithm;

		public virtual byte[] Root => InternalMerkleTree.Root;

		public virtual MerkleSize Size => InternalMerkleTree.Size;

		public virtual ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) => InternalMerkleTree.GetValue(coordinate);
	}
}