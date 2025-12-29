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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Merkle tree optimized for very large datasets by keeping only sub-root hashes in memory; supports append and proof verification scenarios.
/// </summary>
public class LongMerkleTree : IDynamicMerkleTree {
	private readonly List<MerkleSubRoot> _subRoots;
	private List<MerkleCoordinate> _subRootCoords;
	private readonly ObservableExtendedList<byte[]> _observableLeafs;
	private MerkleSize _size;
	private byte[] _root;
	private bool _dirty;

	/// <summary>
	/// Initializes an empty long merkle tree with a default leaf list.
	/// </summary>
	/// <param name="hashAlgorithm">The hash function to use.</param>
	/// <param name="leafs">Optional initial leaves.</param>
	public LongMerkleTree(CHF hashAlgorithm, IEnumerable<byte[]> leafs = null)
		: this(hashAlgorithm, new ExtendedList<byte[]>(), leafs) {
	}

	/// <summary>
	/// Initializes a long merkle tree over the provided leaf list instance.
	/// </summary>
	/// <param name="hashAlgorithm">The hash function to use.</param>
	/// <param name="leafListInstance">The leaf list instance to wrap.</param>
	/// <param name="leafs">Optional initial leaves.</param>
	public LongMerkleTree(CHF hashAlgorithm, IExtendedList<byte[]> leafListInstance, IEnumerable<byte[]> leafs = null) {
		Guard.ArgumentNotNull(leafListInstance, nameof(leafListInstance));
		HashAlgorithm = hashAlgorithm;
		_subRoots = new List<MerkleSubRoot>();
		_subRootCoords = new List<MerkleCoordinate>();
		_observableLeafs = new ObservableExtendedList<byte[]>(leafListInstance);
		_observableLeafs.Mutated += (o, traits) => _dirty = true;
		_observableLeafs.Added += (o, args) => args.CallArgs.Items.ForEach(hash => MerkleMath.AddLeaf(HashAlgorithm, _subRoots, hash));
		_observableLeafs.Inserting += (o, args) => throw new NotSupportedException();
		_observableLeafs.RemovingRange += (o, args) => throw new NotSupportedException();
		_observableLeafs.Updating += (o, args) => throw new NotSupportedException();
		if (leafs != null) {
			Leafs.AddRange(leafs);
		}
		CalculateTree();
	}

	/// <summary>
	/// Gets the hash algorithm used for the tree.
	/// </summary>
	public CHF HashAlgorithm { get; }

	/// <summary>
	/// Gets the merkle root, recalculating if leaf data changed.
	/// </summary>
	public byte[] Root {
		get {
			if (_dirty)
				CalculateTree();
			return _root;
		}
	}

	/// <summary>
	/// Gets the size metadata, recalculating if leaf data changed.
	/// </summary>
	public MerkleSize Size {
		get {
			if (_dirty)
				CalculateTree();
			return _size;
		}
	}

	/// <summary>
	/// Enumerates the sub-root nodes that are retained in memory.
	/// </summary>
	public IEnumerable<MerkleNode> SubRoots {
		get {
			if (_dirty)
				CalculateTree();
			return _subRootCoords.Zip(_subRoots, (c, r) => new MerkleNode(c, r.Hash));
		}
	}

	/// <summary>
	/// Gets the observable list of leaf hashes.
	/// </summary>
	public IExtendedList<byte[]> Leafs => _observableLeafs;

	/// <summary>
	/// Reads the hash value at the specified coordinate, computing it on demand.
	/// </summary>
	/// <param name="coordinate">The coordinate to read.</param>
	public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
		if (_dirty)
			CalculateTree();

		// perfect nodes exist within a perfect tree and are maintained
		if (MerkleMath.IsPerfectNode(Size, coordinate)) {
			for (var i = 0; i < _subRootCoords.Count; i++) {
				var subRootCoord = _subRootCoords[i];
				// Case 1: it is a sub-root
				if (coordinate == subRootCoord)
					return _subRoots[i].Hash;
				Debug.Assert(coordinate.Level < subRootCoord.Level);
				// Case 2: it's a descendant of a sub-root
				var (left, right) = MerkleMath.GetDescendants(_size, subRootCoord, subRootCoord.Level - coordinate.Level, true);
				if (left.Index <= coordinate.Index && coordinate.Index <= right.Index) {
					throw new InvalidOperationException("Cannot fetch untracked node");
				}
			}
		}

		// Case 3: It's imperfect, calculate on-the-fly
		var childNodes = MerkleMath.GetChildren(_size, coordinate);
		if (childNodes.Right == MerkleCoordinate.Null) {
			return GetValue(childNodes.Left); // Bubble-up left
		}
		// Aggregation node (left is always a sub-root)
		return MerkleMath.NodeHash(HashAlgorithm, GetValue(childNodes.Left), GetValue(childNodes.Right));
	}

	private void CalculateTree() {
		// note: _subRoots is always up to date
		_subRootCoords = MerkleMath.CalculateSubRoots(_subRoots.Select(x => x.Height)).ToList();
		_size = MerkleSize.FromLeafCount(_subRoots.Aggregate(0, (sum, subRoot) => sum + (1 << subRoot.Height)));
		_root = _size.LeafCount > 0 ? Hashers.Aggregate(HashAlgorithm, _subRoots.Select(x => x.Hash).Reverse(), true) : null;
		_dirty = false;
	}

}
