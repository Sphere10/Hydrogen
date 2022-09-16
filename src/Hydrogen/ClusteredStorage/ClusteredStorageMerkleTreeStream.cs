using System;
using System.Collections.Generic;
using Hydrogen;

namespace Hydrogen.Collections;

/// <summary>
/// Maps a <see cref="IEditableMerkleTree"/> onto a <see cref="Stream"/> within a <see cref="IClusteredStorage"/>. The tree data 
/// is stored as a <see cref="FlatMerkleTree"/> and mapped to reserved <see cref="ClusteredStreamRecord"/> within the <see cref="IClusteredStorage"/>.
/// </summary>
/// <remarks>This class is intended for building <see cref="IMerkleTree"/>'s of items stored in a <see cref="IClusteredStorage"/>-based containers.</remarks>
internal sealed class ClusteredStorageMerkleTreeStream : IEditableMerkleTree {

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

	private IDisposable EnterAccessMerkleTreeScope(out IEditableMerkleTree merkleTree) {
		var disposables = new Disposables(false);
		var streamScope = _storage.Open(_flatTreeStreamRecord);
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

		public override int Count => _parent._storage.Count - _parent._storage.Header.ReservedRecords;

		public override void AddRange(IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.AddRange(items);
			}
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				return merkleTree.Leafs.IndexOfRange(items);
			}
		}

		public override IEnumerable<byte[]> ReadRange(int index, int count) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				return merkleTree.Leafs.ReadRange(index, count);
			}
		}

		public override void UpdateRange(int index, IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.UpdateRange(index, items);
			}
		}

		public override void InsertRange(int index, IEnumerable<byte[]> items) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.InsertRange(index, items);
			}
		}

		public override void RemoveRange(int index, int count) {
			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
				merkleTree.Leafs.RemoveRange(index, count);
			}
		}
	}

}
