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

namespace Hydrogen;

/// <summary>
/// An <see cref="IDynamicMerkleTree"/> that maintains only a subset of the nodes. This is useful for constructing
/// and verifying multi-item proofs.
/// </summary>
public sealed class PartialMerkleTree : Dictionary<MerkleCoordinate, byte[]>, IDynamicMerkleTree {

	public PartialMerkleTree(CHF hashAlgorithm, int leafCount, IEnumerable<KeyValuePair<MerkleCoordinate, byte[]>> nodes) {
		HashAlgorithm = hashAlgorithm;
		Size = MerkleSize.FromLeafCount(leafCount);
		nodes.ForEach(x => Add(x.Key, x.Value));
		Leafs = new LeafList(this);
	}

	public CHF HashAlgorithm { get; }

	public byte[] Root => GetValue(MerkleCoordinate.Root(Size.LeafCount)).ToArray();

	public MerkleSize Size { get; }

	public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
		if (!TryGetValue(coordinate, out var result))
			throw new ArgumentException($"Node '{coordinate}' was not tracked in the partial node list", nameof(coordinate));
		return GetValue(coordinate);
	}

	public IExtendedList<byte[]> Leafs { get; }


	// This list does nothing except return a count
	private sealed class LeafList : SingularListBase<byte[]> {
		private readonly PartialMerkleTree _parent;
		public LeafList(PartialMerkleTree parent) {
			_parent = parent;
		}

		public override void CopyTo(byte[][] array, int arrayIndex) => throw new NotSupportedException();

		public override IEnumerator<byte[]> GetEnumerator() => throw new NotSupportedException();

		public override bool Remove(byte[] item) => throw new NotSupportedException();

		public override bool Contains(byte[] item) => throw new NotImplementedException();

		public override long Count => _parent.Size.LeafCount;

		public override void Add(byte[] item) => throw new NotSupportedException();

		public override void Clear() => throw new NotSupportedException();

		public override bool IsReadOnly => true;

		public override long IndexOfL(byte[] item) => throw new NotSupportedException();

		public override void Insert(long index, byte[] item) => throw new NotSupportedException();

		public override void RemoveAt(long index) => throw new NotSupportedException();

		public override byte[] Read(long index) => throw new NotImplementedException();

		public override void Update(long index, byte[] item) => throw new NotImplementedException();
	}
}
