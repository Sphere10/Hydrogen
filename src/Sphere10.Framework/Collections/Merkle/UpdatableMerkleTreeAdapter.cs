
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Sphere10.Framework {

	
//	/// <summary>
//	/// Implements a merkle-tree from a given set of leaf digests. This is useful when the the client
//	/// code already has the leaf set but wants to build a merkle-tree out of it.
//	/// </summary>
//	/// <typeparam name="TItem"></typeparam>
//	public class UpdatableMerkleTreeAdapter : MerkleTreeAdapter, IUpdateableMerkleTree {
//		private readonly MerkleTreeLeafGetter _leafGetter;
//		private readonly MerkleTreeLeafCounter _leafCounter;
//		private readonly int _digestLength;
//		private readonly List<byte[]> _subRoots;

//		public UpdatableMerkleTreeAdapter(CHF chf, IList<byte[]> leafList)
//			: this(chf, x => leafList[x], () => leafList.Count) {
//		}

//		public UpdatableMerkleTreeAdapter(CHF chf, MerkleTreeLeafGetter leafGetter, MerkleTreeLeafCounter leafCounter) {
//			Guard.ArgumentNotNull(leafGetter, nameof(leafGetter));
//			Guard.ArgumentNotNull(leafCounter, nameof(leafCounter));
//			_leafGetter = leafGetter;
//			_leafCounter = leafCounter;
//			_digestLength = Hashers.GetDigestSizeBytes(chf);
//			_subRoots = new List<byte[]>();
//			HashAlgorithm = chf;
//		}

//		public CHF HashAlgorithm { get; }

//		public byte[] Root => MerkleMath.AggregateSubRoots(HashAlgorithm, _subRoots);

//		public MerkleSize Size => MerkleSize.FromLeafCount(_leafCounter());

//		public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
//			var size = Size;
//			if (MerkleMath.IsLeaf(coordinate)) {
//				var leaf = _leafGetter(coordinate.Index);
//				Guard.Ensure(leaf.Length == _digestLength, $"Leaf {coordinate.Index} digest was incorrectly sized (expected: {_digestLength} was {leaf.Length}).");
//				return leaf;
//			}

//			if (MerkleMath.IsPerfectNode(size, coordinate))
//				return _subRoots[coordinate.Level];

//			var (left, right) = MerkleMath.GetChildren(size, coordinate);
//			if (right != null)
//				return MerkleMath.NodeHash(HashAlgorithm, GetValue(left), GetValue(right));

//			return GetValue(left);
//		}

//		public IExtendedList<byte[]> Leafs { get; }
//	}
//}