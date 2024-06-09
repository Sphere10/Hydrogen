// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
//
// NOTE: This file is part of the reference implementation for Dynamic Merkle-Trees. Read the paper at:
// Web: https://sphere10.com/tech/dynamic-merkle-trees
// e-print: https://vixra.org/abs/2305.0087

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Merkle-tree implementation that maintains all perfect nodes in flat array allocated in a contiguous block of memory.
/// Read Complexity: O(1)
/// Append Complexity: O(LogN)   (leaf and antecedent nodes are updated)
/// Update Complexity: O(LogN)   (leaf and antecedent nodes need updating)
/// Insert Complexity: O(N)      
/// Update Complexity: O(N)
/// Memory Complexity: O(N LogN)
/// </summary>
public class FlatMerkleTree : IDynamicMerkleTree {
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

	public FlatMerkleTree(CHF hashAlgorithm, long initialLeafCapacity, long leafGrowthCapacity)
		: this(hashAlgorithm, initialLeafCapacity, leafGrowthCapacity, DefaultMaxLeaf) {
	}

	public FlatMerkleTree(CHF hashAlgorithm, long initialLeafCapacity, long leafGrowthCapacity, long maxCapacity)
		: this(hashAlgorithm, initialLeafCapacity, leafGrowthCapacity, maxCapacity, Enumerable.Empty<byte[]>()) {
	}

	public FlatMerkleTree(CHF hashAlgorithm, long initialLeafCapacity, long leafGrowthCapacity, long maxLeafCapacity, IEnumerable<byte[]> initialLeafs)
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

	public FlatMerkleTree(CHF hashAlgorithm, IBuffer nodeBuffer, long? precomputedLeafCount = null) {
		Guard.Argument(hashAlgorithm != CHF.ConcatBytes, nameof(hashAlgorithm), "Must be digest size CHF");
		HashAlgorithm = hashAlgorithm;

		_digestSize = Hashers.GetDigestSizeBytes(hashAlgorithm);
		Guard.Argument(_digestSize > 0, nameof(hashAlgorithm), "Unsupported CHF");

		// Determine the leaf count
		var leafCount = precomputedLeafCount ?? MerkleMath.SlowCalculateLeafCountFromFlatNodes(nodeBuffer.Count / _digestSize);

		// Check to ensure buffer has all the flat nodes
		var expectedStoredNodes = checked((long)MerkleMath.CountFlatNodes(leafCount));
		var actualStoredNodes = nodeBuffer.Count / _digestSize;
		Guard.Ensure( expectedStoredNodes == actualStoredNodes, $"Corrupted merkle-tree: expected {expectedStoredNodes} flat-nodes but found {actualStoredNodes}.");

		Leafs = new LeafList(this);
		AttachBuffer(nodeBuffer, leafCount);
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
			return GetValue(childNodes.Left); // Bubble-up left
		}
		// Aggregation node (left is always a sub-root)
		return MerkleMath.NodeHash(HashAlgorithm, GetValue(childNodes.Left), GetValue(childNodes.Right));
	}

	public void SetLeafDirty(long index, bool value) {
		SetDirty((int)MerkleMath.ToFlatIndex(MerkleCoordinate.LeafAt(index)), value);
	}

	public void EnsureComputed() {
		if (_size.LeafCount == 0)
			return;

		var rootCoord =  MerkleCoordinate.Root(_size.LeafCount);
		EnsureComputed(rootCoord, checked((long)MerkleMath.ToFlatIndex(rootCoord)));
	}

	public byte[] ToBytes() {
		EnsureComputed();
		return _nodeBuffer.ToArray();
	}

	private void EnsureComputed(MerkleCoordinate coordinate, long flatIndex) {
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

	internal void AttachBuffer(IBuffer buffer, long leafCount) {
		Guard.Argument(buffer.Count % _digestSize == 0, nameof(buffer), "Size was not a multiple of digest length");
		_nodeBuffer = buffer;
		var flatNodeCount = buffer.Count / _digestSize;
		var flatNodeCountI = Tools.Collection.CheckNotImplemented64bitAddressingLength(flatNodeCount);
		_dirtyNodes = new BitArray(flatNodeCountI);
		_size = MerkleSize.FromLeafCount(leafCount);
		//Guard.Ensure((ulong)flatNodeCount == MerkleMath.CountFlatNodes(_size.LeafCount), $"Inconsistent buffer size (flatNodes: {flatNodeCount}, leafCount: {leafCount})");
	}

	private bool IsDirty(long flatIndex) {
		var flatIndexI = Tools.Collection.CheckNotImplemented64bitAddressingLength(flatIndex);
		return _dirtyNodes.Get(flatIndexI);
	}

	private void SetDirty(long index, bool value) {
		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingLength(index);
		_dirtyNodes.Set(indexI, value);
	}

	private void SetDirty(long from, long count, bool value) {
		for (var i = 0L; i < count; i++) {
			var indexI = Tools.Collection.CheckNotImplemented64bitAddressingLength(from + i);
			_dirtyNodes.Set(indexI, value);
		}
	}


	private class LeafList : RangedListBase<byte[]> {
		private readonly FlatMerkleTree _parent;

		public LeafList(FlatMerkleTree parent) {
			_parent = parent;
		}

		public override long Count => _parent._size.LeafCount;

		public override void AddRange(IEnumerable<byte[]> items) {
			var itemsArr = items as byte[][] ?? items.ToArray();
			Guard.Argument(itemsArr.All(x => x.Length == _parent._digestSize), nameof(items), "Improper digest size(s)");

			// Expand node buffer for new tree nodes
			var newLeafCount = _parent._size.LeafCount + itemsArr.Length;
			var oldTotalFlatNodes = _parent._nodeBuffer.Count / _parent._digestSize;
			var newTotalFlatNodes = checked((long)MerkleMath.CountFlatNodes(newLeafCount));
			var newFlatNodes = newTotalFlatNodes - oldTotalFlatNodes;
			_parent._nodeBuffer.ExpandBy(newFlatNodes * _parent._digestSize);

			// Mark all those new nodes dirty
			var newFlatNodesI = Tools.Collection.CheckNotImplemented64bitAddressingLength(newFlatNodes);
			_parent._dirtyNodes.Length += newFlatNodesI;
			_parent.SetDirty(oldTotalFlatNodes, newFlatNodes, true);

			// Add each leaf node
			foreach (var (item, i) in itemsArr.WithIndex()) {
				var flatIndex = MerkleCoordinate.From(0, _parent._size.LeafCount + i).ToFlatIndex();
				_parent._nodeBuffer.UpdateRange(flatIndex * _parent._digestSize, item);
				_parent.SetDirty(flatIndex, false); // leaf node marked not dirty
			}
			_parent._size = MerkleSize.FromLeafCount(_parent._size.LeafCount + itemsArr.Length);
		}

		public override IEnumerable<long> IndexOfRange(IEnumerable<byte[]> items) {
			throw new NotImplementedException();
		}

		public override IEnumerable<byte[]> ReadRange(long index, long count) {
			Guard.ArgumentInRange(index, 0, _parent.Size.LeafCount, nameof(index));
			Guard.ArgumentInRange(count, 0, _parent.Size.LeafCount - index, nameof(count));
			for (var i = index; i < index + count; i++) {
				yield return _parent._nodeBuffer.ReadSpan(MerkleCoordinate.LeafAt(i).ToFlatIndex() * _parent._digestSize, _parent._digestSize).ToArray();
			}
		}

		public override void UpdateRange(long index, IEnumerable<byte[]> items) {
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
				foreach (var node in MerkleMath.CalculatePathToRoot(_parent.Size, leaf, false).Skip(1)) {
					// note: skip start node
					var nodeIX = node.ToFlatIndex();
					if (nodeIX < flatNodes)
						_parent.SetDirty(nodeIX, true);
				}
			}
		}

		public override void InsertRange(long index, IEnumerable<byte[]> items) {
			var itemsArr = items as byte[][] ?? items.ToArray();
			Guard.Argument(itemsArr.All(x => x.Length == _parent._digestSize), nameof(items), "Improper digest size(s)");

			// Expand node buffer for new tree nodes
			var oldLeafCount = _parent.Size.LeafCount;
			var newLeafCount = oldLeafCount + itemsArr.Length;
			var movedLeafCount = oldLeafCount - index;
			var oldTotalFlatNodes = _parent.FlatNodeCount;
			var newTotalFlatNodes = (int)MerkleMath.CountFlatNodes(newLeafCount);
			var newFlatNodes = newTotalFlatNodes - oldTotalFlatNodes;

			// Grow buffer to accomodate new nodes
			_parent._nodeBuffer.ExpandBy(newFlatNodes * _parent._digestSize);

			// Backup nodes after insert which are moved forward
			var movedLeafs = new byte[movedLeafCount * _parent._digestSize];
			var movedLeafCountI = Tools.Collection.CheckNotImplemented64bitAddressingLength(movedLeafCount);
			var movedLeafsDirtyBit = new BitArray(movedLeafCountI);
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

		public override void RemoveRange(long index, long count) {
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
			var movedLeafCountI = Tools.Collection.CheckNotImplemented64bitAddressingLength(movedLeafCount);
			var movedLeafDirtyBit = new BitArray(movedLeafCountI);
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
