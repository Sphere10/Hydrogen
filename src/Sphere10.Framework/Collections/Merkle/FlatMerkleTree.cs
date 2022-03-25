using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;

namespace Sphere10.Framework {

	/// <summary>
	/// Merkle-tree implementation that maintains all perfect nodes in flat array allocated in a contiguous block of memory.
	/// Read Complexity: O(1)
	/// Append Complexity: O(LogN)   (leaf and antecedent nodes are updated)
	/// Update Complexity: O(LogN)   (leaf and antecedent nodes need updating)
	/// Insert Complexity: O(N)      
	/// Update Complexity: O(N)
	/// Memory Complexity: O(N LogN)
	/// </summary>
	public class FlatMerkleTree : IUpdateableMerkleTree {
		public const int DefaultLeafGrowth = 4096;
		public const int DefaultMaxLeaf = 1 << 24;
		private IBuffer _nodeBuffer;
		private BitArray _dirtyNodes;
		private readonly int _digestSize;
		private MerkleSize _size;

		public FlatMerkleTree(CHF hashAlgorithm) : this(hashAlgorithm, 0, DefaultLeafGrowth) {
		}

		public FlatMerkleTree(CHF hashAlgorithm, IEnumerable<byte[]> initialLeafs) 
			: this(hashAlgorithm, 0, DefaultLeafGrowth, DefaultMaxLeaf, initialLeafs) {
		}

		public FlatMerkleTree(CHF hashAlgorithm, int initialLeafCapacity, int leafGrowthCapacity)
			: this(hashAlgorithm, initialLeafCapacity, leafGrowthCapacity, DefaultMaxLeaf) {
		}

		public FlatMerkleTree(CHF hashAlgorithm, int initialLeafCapacity, int leafGrowthCapacity, int maxCapacity)
			: this(hashAlgorithm, initialLeafCapacity, leafGrowthCapacity, maxCapacity, Enumerable.Empty<byte[]>()) {
		}

		public FlatMerkleTree(CHF hashAlgorithm, int initialLeafCapacity, int leafGrowthCapacity, int maxLeafCapacity, IEnumerable<byte[]> initialLeafs) 
			: this(
				  hashAlgorithm, 
				  new MemoryBuffer(
					(int)MerkleMath.CountFlatNodes(initialLeafCapacity) * Hashers.GetDigestSizeBytes(hashAlgorithm),
					(int)MerkleMath.CountFlatNodes(leafGrowthCapacity) * Hashers.GetDigestSizeBytes(hashAlgorithm),
					(int)MerkleMath.CountFlatNodes(maxLeafCapacity) * Hashers.GetDigestSizeBytes(hashAlgorithm)
				  )
				) {
			Leafs.AddRange(initialLeafs ?? Enumerable.Empty<byte[]>());
		}

		public FlatMerkleTree(CHF hashAlgorithm, IBuffer nodeBuffer) {
			Guard.Argument(hashAlgorithm != CHF.ConcatBytes, nameof(hashAlgorithm), "Must be digest size CHF");
			HashAlgorithm = hashAlgorithm;
			_digestSize = Hashers.GetDigestSizeBytes(hashAlgorithm);
			Guard.Argument(_digestSize > 0, nameof(hashAlgorithm), "Unsupported CHF");
			Leafs = new LeafList(this);
			AttachBuffer(nodeBuffer);
		}

		public CHF HashAlgorithm { get; }

		public byte[] Root => _size.LeafCount > 0 ? GetValue(MerkleCoordinate.Root(Size)).ToArray() : null;

		public MerkleSize Size => _size;

		public IExtendedList<byte[]> Leafs { get; }

		public int FlatNodeCount => _dirtyNodes.Length;

		public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
			// perfect nodes exist within a perfect tree and are maintained
			if (MerkleMath.IsPerfectNode(_size, coordinate)) {
				var index = coordinate.ToFlatIndex();
				EnsureComputed(coordinate, index);
				return _nodeBuffer.ReadSpan(index * _digestSize, _digestSize);
			}
			// it's an imperfect node, so calculate it on-the-fly
			var childNodes = MerkleMath.GetChildren(_size, coordinate);
			if (childNodes.Right == MerkleCoordinate.Null) {
				return GetValue(childNodes.Left);  // Bubble-up left
			}
			// Aggregation node (left is always a sub-root)
			return MerkleMath.NodeHash(HashAlgorithm, GetValue(childNodes.Left), GetValue(childNodes.Right));
		}

		public void SetLeafDirty(int index, bool value) {
			SetDirty((int)MerkleMath.ToFlatIndex(MerkleCoordinate.LeafAt(index)), value);
		}

		private void EnsureComputed(MerkleCoordinate coordinate, int flatIndex) {
			// leafs case
			if (coordinate.Level == 0)
				if (IsDirty(flatIndex))
					throw new InvalidOperationException("Leaf node was marked dirty");
				else return;

			if (!IsDirty(flatIndex))
				return;

			var childs = MerkleMath.GetChildren(_size, coordinate);
			if (childs.Right == MerkleCoordinate.Null)
				throw new InternalErrorException("Imperfect nodes can't be marked dirty");
			var leftIX = childs.Left.ToFlatIndex();
			var rightIX = childs.Right.ToFlatIndex();

			if (childs.Left.Level > 0) {
				EnsureComputed(childs.Left, leftIX);
				EnsureComputed(childs.Right, rightIX);
			}

			_nodeBuffer.UpdateRange(
				flatIndex * _digestSize,
				MerkleMath.NodeHash(
					HashAlgorithm,
					_nodeBuffer.ReadSpan(leftIX * _digestSize, _digestSize),
					_nodeBuffer.ReadSpan(rightIX * _digestSize, _digestSize)
				)
			);
			SetDirty(flatIndex, false);
		}

		internal void AttachBuffer(IBuffer buffer) {
			Guard.Argument(buffer.Count % _digestSize == 0, nameof(buffer), "Size was not a multiple of digest length");
			_nodeBuffer = buffer;
			_dirtyNodes = new BitArray(0);
			_size = MerkleSize.FromLeafCount(buffer.Count / _digestSize);
		}

		private bool IsDirty(int flatIndex) {
			return _dirtyNodes.Get(flatIndex);
		}

		private void SetDirty(int index, bool value) {
			_dirtyNodes.Set(index, value);
		}

		private void SetDirty(int from, int count, bool value) {
			for (var i = 0; i < count; i++)
				_dirtyNodes.Set(from + i , value);
		}

		private class LeafList : RangedListBase<byte[]> {
			private readonly FlatMerkleTree _parent;

			public LeafList(FlatMerkleTree parent) {
				_parent = parent;
			}

			public override int Count => _parent._size.LeafCount;

			public override void AddRange(IEnumerable<byte[]> items) {
				var itemsArr = items as byte[][] ?? items.ToArray();
				Guard.Argument(itemsArr.All(x => x.Length == _parent._digestSize), nameof(items), "Improper digest size(s)");

				// Expand node buffer for new tree nodes
				var newLeafCount = _parent._size.LeafCount + itemsArr.Length;
				var oldTotalFlatNodes = _parent._nodeBuffer.Count / _parent._digestSize;
				var newTotalFlatNodes = (int)MerkleMath.CountFlatNodes(newLeafCount); 
				var newFlatNodes = newTotalFlatNodes - oldTotalFlatNodes;
				_parent._nodeBuffer.ExpandBy(newFlatNodes * _parent._digestSize);

				// Mark all those new nodes dirty
				_parent._dirtyNodes.Length += newFlatNodes;
				_parent.SetDirty(oldTotalFlatNodes, newFlatNodes, true);

				// Add each leaf node
				foreach (var (item, i) in itemsArr.WithIndex()) {
					var flatIndex = MerkleCoordinate.From(0, _parent._size.LeafCount + i).ToFlatIndex();
					_parent._nodeBuffer.UpdateRange(flatIndex * _parent._digestSize, item);
					_parent.SetDirty(flatIndex, false); // leaf node marked not dirty
				}
				_parent._size = MerkleSize.FromLeafCount(_parent._size.LeafCount + itemsArr.Length);
			}

			public override IEnumerable<int> IndexOfRange(IEnumerable<byte[]> items) {
				throw new NotImplementedException();
			}

			public override IEnumerable<byte[]> ReadRange(int index, int count) {
				Guard.ArgumentInRange(index, 0, _parent.Size.LeafCount, nameof(index));
				Guard.ArgumentInRange(count, 0, _parent.Size.LeafCount - index, nameof(count));
				for (var i = index; i < index + count; i++) {
					yield return _parent._nodeBuffer.ReadSpan(MerkleCoordinate.LeafAt(i).ToFlatIndex() * _parent._digestSize, _parent._digestSize).ToArray();
				}
			}

			public override void UpdateRange(int index, IEnumerable<byte[]> items) {
				Guard.ArgumentNotNull(items, nameof(items));
				var itemsArr = items as byte[][] ?? items.ToArray();
				Guard.Argument(itemsArr.All(x => x.Length == _parent._digestSize), nameof(items), "Improper digest size(s)");
				Guard.ArgumentInRange(index, 0, _parent.Size.LeafCount, nameof(index));
				Guard.ArgumentInRange(itemsArr.Length, 0, _parent.Size.LeafCount - index, nameof(items), "Too many items will result in overflow update");
				var flatNodes = _parent._dirtyNodes.Length;
				for (var i = 0; i < itemsArr.Length; i++) {
					var leaf = MerkleCoordinate.LeafAt(index + i);
					var z = leaf.ToFlatIndex();
					_parent._nodeBuffer.UpdateRange(z * _parent._digestSize, itemsArr[i]);
					_parent.SetDirty(z, false); // mark leaf clean
					// mark all leaf parents dirty
					foreach (var node in MerkleMath.CalculatePathToRoot(_parent.Size, leaf, false).Skip(1)) { // note: skip start node
						var nodeIX = node.ToFlatIndex();
						if (nodeIX < flatNodes)
							_parent.SetDirty(nodeIX, true);
					}
				}
			}

			public override void InsertRange(int index, IEnumerable<byte[]> items) {
				var itemsArr = items as byte[][] ?? items.ToArray();
				Guard.Argument(itemsArr.All(x => x.Length == _parent._digestSize), nameof(items), "Improper digest size(s)");

				// Expand node buffer for new tree nodes
				var oldLeafCount = _parent.Size.LeafCount;
				var newLeafCount = oldLeafCount + itemsArr.Length;
				int movedLeafCount = oldLeafCount - index;
				var oldTotalFlatNodes = _parent.FlatNodeCount;
				var newTotalFlatNodes = (int)MerkleMath.CountFlatNodes(newLeafCount);
				var newFlatNodes = newTotalFlatNodes - oldTotalFlatNodes;
				
				// Grow buffer to accomodate new nodes
				_parent._nodeBuffer.ExpandBy(newFlatNodes * _parent._digestSize);

				// Backup nodes after insert which are moved forward
				var movedLeafs = new byte[movedLeafCount * _parent._digestSize];
				var movedLeafsDirtyBit = new BitArray(movedLeafCount);
				for (var i = 0; i < movedLeafCount; i++) {
					var movedFlatIX = MerkleCoordinate.LeafAt(index + i).ToFlatIndex();
					_parent
						._nodeBuffer
						.ReadSpan(movedFlatIX * _parent._digestSize, _parent._digestSize)
						.CopyTo(movedLeafs.AsSpan(i * _parent._digestSize, _parent._digestSize));
					movedLeafsDirtyBit[i] = _parent._dirtyNodes[movedFlatIX];
				}

				// Mark all the flat nodes from insertion point to end as dirty
				var flatIX = MerkleCoordinate.LeafAt(index).ToFlatIndex();
				_parent._dirtyNodes.Length += newFlatNodes;
				_parent.SetDirty(flatIX, newTotalFlatNodes - flatIX, true);

				// Add the inserted nodes
				for (var i = 0; i < itemsArr.Length; i++) {
					var flatIndex = MerkleCoordinate.LeafAt(index + i).ToFlatIndex();
					_parent._nodeBuffer.UpdateRange(flatIndex * _parent._digestSize, itemsArr[i]);
					_parent.SetDirty(flatIndex, false); // leaf node marked not dirty
				}

				// Add the moved nodes
				for (var i = 0; i < movedLeafCount; i++) {
					var flatIndex = MerkleCoordinate.LeafAt(index + itemsArr.Length + i).ToFlatIndex();
					_parent._nodeBuffer.UpdateRange(flatIndex * _parent._digestSize, movedLeafs.AsSpan(i * _parent._digestSize, _parent._digestSize));
					_parent.SetDirty(flatIndex, movedLeafsDirtyBit[i]); // leaf node marked not dirty
				}

				_parent._size = MerkleSize.FromLeafCount(newLeafCount);
			}

			public override void RemoveRange(int index, int count) {
				Guard.ArgumentInRange(index, 0, _parent.Size.LeafCount, nameof(index));
				Guard.ArgumentInRange(count, 0, _parent.Size.LeafCount - index, nameof(count));

				// Expand node buffer for new tree nodes
				var oldLeafCount = _parent.Size.LeafCount;
				var newLeafCount = _parent._size.LeafCount - count;
				var movedLeafCount = oldLeafCount - (index + count);
				var oldTotalFlatNodes = _parent.FlatNodeCount;
				var newTotalFlatNodes = (int)MerkleMath.CountFlatNodes(newLeafCount);
				var removedFlatNodes = oldTotalFlatNodes - newTotalFlatNodes;

				// Backup nodes that will be moved after remove
				var movedLeafs = new byte[movedLeafCount * _parent._digestSize];
				var movedLeafDirtyBit = new BitArray(movedLeafCount);
				for (var i = 0; i < movedLeafCount; i++) {
					var movedFlatIX = MerkleCoordinate.LeafAt(index + count + i).ToFlatIndex();
					_parent
						._nodeBuffer
						.ReadSpan(movedFlatIX * _parent._digestSize, _parent._digestSize)
						.CopyTo(movedLeafs.AsSpan(i * _parent._digestSize, _parent._digestSize));
					movedLeafDirtyBit[i] = _parent._dirtyNodes[movedFlatIX];
				}

				// Remove from index to end
				_parent._nodeBuffer.RemoveRange(_parent._nodeBuffer.Count - removedFlatNodes * _parent._digestSize, removedFlatNodes * _parent._digestSize);

				// Mark all the flat nodes from index to end as dirty
				var flatIX = MerkleCoordinate.LeafAt(index).ToFlatIndex();
				_parent._dirtyNodes.Length -= removedFlatNodes;
				_parent.SetDirty(flatIX, newTotalFlatNodes - flatIX, true);

				// Add the moved nodes
				for (var i = 0; i < movedLeafCount; i++) {
					var flatIndex = MerkleCoordinate.LeafAt(index + i).ToFlatIndex();
					_parent._nodeBuffer.UpdateRange(flatIndex * _parent._digestSize, movedLeafs.AsSpan(i * _parent._digestSize, _parent._digestSize));
					_parent.SetDirty(flatIndex, movedLeafDirtyBit[i]);
				}

				_parent._size = MerkleSize.FromLeafCount(newLeafCount);
			}
		}
	}
}
