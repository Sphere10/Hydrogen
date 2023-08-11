// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Collections;

/// <summary>
/// Maps a <see cref="IDynamicMerkleTree"/> onto a <see cref="Stream"/> within a <see cref="IClusteredStorage"/>. The tree data 
/// is stored as a <see cref="FlatMerkleTree"/> and mapped to reserved <see cref="ClusteredStreamRecord"/> within the <see cref="IClusteredStorage"/>.
/// </summary>
/// <remarks>This class is intended for building <see cref="IMerkleTree"/>'s of items stored in a <see cref="IClusteredStorage"/>-based containers.</remarks>
internal sealed class ClusteredStorageMerkleTreeStream : IDynamicMerkleTree {

	private readonly IClusteredStorage _storage;
	private readonly int _flatTreeStreamRecord;

	public ClusteredStorageMerkleTreeStream(IClusteredStorage clusteredStorage, int merkleTreeStreamIndex, CHF hashAlgorithm) {
		_storage = clusteredStorage;
		_flatTreeStreamRecord = merkleTreeStreamIndex;
		HashAlgorithm = hashAlgorithm;
		Leafs = new LeafList(this);
	}

	public CHF HashAlgorithm { get; }

	public byte[] Root {
		get => _storage.Count - _storage.Header.ReservedRecords > 0 ? _storage.Header.MerkleRoot : null;
		set => _storage.Header.MerkleRoot = value ?? Tools.Array.Gen<byte>(Hashers.GetDigestSizeBytes(HashAlgorithm), 0);
	}

	public MerkleSize Size => MerkleSize.FromLeafCount(_storage.Count - _storage.Header.ReservedRecords);

	public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
		using (EnterAccessMerkleTreeScope(out var merkleTree)) {
			return merkleTree.GetValue(coordinate);
		}
	}
	public IExtendedList<byte[]> Leafs { get; }

	private IDisposable EnterAccessMerkleTreeScope(out IDynamicMerkleTree merkleTree) {
		var disposables = new Disposables(false);
		var streamScope = _storage.OpenWrite(_flatTreeStreamRecord);
		var flatTreeData = new StreamMappedBuffer(streamScope.Stream);
		var flatMerkleTree = new FlatMerkleTree(HashAlgorithm, flatTreeData, _storage.Count - _storage.Header.ReservedRecords);
		merkleTree = flatMerkleTree;
		disposables.Add(() => { Root = flatMerkleTree.Root; }); // When scope finishes, update storage merkle-root in header
		disposables.Add(streamScope); // when scope finishes, dispose the stream scope
		return disposables;
	}


	private sealed class LeafList : RangedListBase<byte[]> {
		private readonly ClusteredStorageMerkleTreeStream _parent;
		public LeafList(ClusteredStorageMerkleTreeStream parent) {
			_parent = parent;
		}

		public override long Count => _parent._storage.Count - _parent._storage.Header.ReservedRecords;

		public override void AddRange(IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.AddRange(items);
			}
		}

		public override IEnumerable<long> IndexOfRange(IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				return merkleTree.Leafs.IndexOfRange(items);
			}
		}

		public override IEnumerable<byte[]> ReadRange(long index, long count) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				return merkleTree.Leafs.ReadRange(index, count);
			}
		}

		public override void UpdateRange(long index, IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.UpdateRange(index, items);
			}
		}

		public override void InsertRange(long index, IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.InsertRange(index, items);
			}
		}

		public override void RemoveRange(long index, long count) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.RemoveRange(index, count);
			}
		}
	}

}
