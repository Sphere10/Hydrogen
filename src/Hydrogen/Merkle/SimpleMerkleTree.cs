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
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Merkle-tree implementation that maintains all leaf nodes and lazily computes parent nodes when needed.
/// Streams is O(2N), root calculation is O(log(n)) all leaf mutations available at O(1). 
///  
/// Not suitable for "large" trees. Not thread-safe.
/// </summary>
public class SimpleMerkleTree : IDynamicMerkleTree {
	private readonly IExtendedList<IExtendedList<byte[]>> _levels;
	private readonly MerkleSize _size;

	public SimpleMerkleTree(CHF hasher, IEnumerable<byte[]> leafs = null) {
		Leafs = new ObservableExtendedList<byte[]>(new ExtendedList<byte[]>());
		if (leafs != null)
			Leafs.AddRange(leafs);
		_levels = new ExtendedList<IExtendedList<byte[]>> { Leafs }; // level 0 is the leaf-level
		HashAlgorithm = hasher;
		_size = new MerkleSize { Height = 0, LeafCount = 0 };
		Dirty = Leafs.Count > 0;

		// When leaf-layer updated, mark tree dirty
		if (HashAlgorithm != CHF.ConcatBytes) {
			var digestSize = Hashers.GetDigestSizeBytes(HashAlgorithm);
			((ObservableExtendedList<byte[]>)Leafs).Adding += (o, e) => e.CallArgs.Items?.ForEach(a => Guard.Ensure(a.Length == digestSize, "Invalid leaf digest size"));
		}
		((ObservableExtendedList<byte[]>)Leafs).Mutated += (o, trait) => Dirty = true;
		((ObservableExtendedList<byte[]>)Leafs).EventFilter = EventTraits.Write;
	}

	public CHF HashAlgorithm { get; }

	public bool Dirty { get; private set; }

	public byte[] Root {
		get {
			EnsureComputed();
			return _levels[0].Count > 0 ? _levels.Last().Single() : null;
		}
	}

	public MerkleSize Size {
		get {
			EnsureComputed();
			return _size;
		}
	}

	public IExtendedList<byte[]> Leafs { get; }

	public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
		EnsureComputed();
		Guard.ArgumentInRange(coordinate.Level, 0, _size.Height - 1, nameof(coordinate.Level));
		Guard.ArgumentInRange(coordinate.Level, 0, _size.LeafCount - 1, nameof(coordinate.Index));
		return _levels[coordinate.Level][coordinate.Index];
	}

	protected virtual void EnsureComputed() {
		if (Dirty) {
			RecomputeTree();
			Dirty = false;
		}
	}

	protected virtual void RecomputeTree() {
		// keep leaf level 0 and clear computed levels
		_levels.RemoveRange(1, _levels.Count - 1);

		// Re-compute levels are based on previous level
		var level = _levels[0];
		while ((level = ComputeNextLevel(level)).Count > 0) {
			_levels.Add(level);
		}
		_size.Height = checked((int)(_levels.Count > 1 ? _levels.Count : _levels[0].Count > 0 ? 1 : 0));
		_size.LeafCount = _size.Height > 0 ? _levels[0].Count : 0;
	}

	protected virtual IExtendedList<byte[]> ComputeNextLevel(IExtendedList<byte[]> level) {
		var nextLevel = new ExtendedList<byte[]>();
		if (level.Count == 1) // root
			return nextLevel;

		for (var i = 0; i < level.Count; i += 2) {
			// last element by itself, bubble it up
			if (i == level.Count - 1) {
				nextLevel.Add(level[i]);
				break;
			}
			var parentHash = MerkleMath.NodeHash(HashAlgorithm, level[i], level[i + 1]);
			nextLevel.Add(parentHash);
		}
		return nextLevel;
	}

}
