﻿using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// An <see cref="IEditableMerkleTree"/> that maintains only a subset of the nodes. This is useful for constructing
	/// and verifying multi-item proofs.
	/// </summary>
	public sealed class PartialMerkleTree : Dictionary<MerkleCoordinate, byte[]>,  IEditableMerkleTree {

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
			return this.GetValue(coordinate);
		}

		public IExtendedList<byte[]> Leafs {
			get;
		}

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

			public override int Count=> _parent.Size.LeafCount;

			public override void Add(byte[] item) => throw new NotSupportedException();

			public override void Clear() => throw new NotSupportedException();

			public override bool IsReadOnly => true;

			public override int IndexOf(byte[] item) => throw new NotSupportedException();

			public override void Insert(int index, byte[] item) => throw new NotSupportedException();

			public override void RemoveAt(int index) => throw new NotSupportedException();

			public override byte[] Read(int index) => throw new NotImplementedException();

			public override void Update(int index, byte[] item) => throw new NotImplementedException();
		}
	}

}
