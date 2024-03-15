// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hydrogen.Collections;

namespace Hydrogen;

/// <summary>
/// Used to store keys of an item in an <see cref="ObjectStream"/>. Used primarily for <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>"/> which
/// stores only the value part in the objectStream, the keys are stored in these (mapped to a reserved stream).
/// </summary>
/// <remarks>Unlike <see cref="KeyIndex{TItem,TKey}"/> which automatically extracts the key from the item and stores it, this is used as a primary storage for the key itself. Thus it is not an index, it is a pure store.</remarks>
internal class MerkleTreeStore : MetaDataStoreBase<byte[]> {

	public event EventHandlerEx<byte[], byte[]> RootChanged;

	private readonly CHF _hashAlgorithm;
	private IMerkleTree _readOnlyMerkleTree;
	private IDynamicMerkleTree _merkleTree;
	private StreamMappedProperty<byte[]> _merkleRootProperty;
	private bool _dirtyRoot;
	private bool _isFirstLoad;

	// Migrate from MerkleTreeIndex stuff into here
	public MerkleTreeStore(ClusteredStreams streams, long reservedStreamIndex, CHF hashAlgorithm, bool isFirstLoad) 
		: base(streams, reservedStreamIndex) {
		_hashAlgorithm = hashAlgorithm;
		_dirtyRoot = false;
		_isFirstLoad = isFirstLoad;
	}

	/// <summary>
	/// Generates a merkle-tree storage of <see cref="leafValues"/> and returns raw byte form.
	/// </summary>
	public static byte[] GenerateBytes(CHF chf, IEnumerable<byte[]> leafValues) {
		var merkleTree = new FlatMerkleTree(chf);
		leafValues.ForEach(merkleTree.Leafs.Add);
		var result = merkleTree.ToBytes();
		return result;
	}

	public IMerkleTree MerkleTree => _readOnlyMerkleTree;

	public override long Count => _merkleTree.Size.LeafCount;

	public override byte[] Read(long index) 
		=> ReadBytes(index);

	public override byte[] ReadBytes(long index) 
		=> _merkleTree.Leafs.Read(index);
	
	public override void Add(long index, byte[] data) {
		Guard.ArgumentEquals(index, Count, nameof(index), $"Can only add at end of list");
		using var _ = Streams.EnterAccessScope();
		_merkleTree.Leafs.Add(data);
		_dirtyRoot = true;
	}

	public override void Update(long index, byte[] data) {
		using var _ = Streams.EnterAccessScope();
		_merkleTree.Leafs.Update(index, data);
		_dirtyRoot = true;
	}

	public override void Insert(long index, byte[] data) {
		using var _ = Streams.EnterAccessScope();
		_merkleTree.Leafs.Insert(index, data);
		_dirtyRoot = true;
	}

	public override void Remove(long index) {
		using var _ = Streams.EnterAccessScope();
		_merkleTree.Leafs.RemoveAt(index);
		_dirtyRoot = true;
	}

	public override void Reap(long index) {
		using var _ = Streams.EnterAccessScope();
		var digest = Hashers.ZeroHash(_hashAlgorithm);
		_merkleTree.Leafs.Update(index, digest);
		_dirtyRoot = true;
	}

	public override void Clear() {
		 _merkleTree.Leafs.Clear();
		 _dirtyRoot = true;
	}

	public void ClearListeners() => _merkleRootProperty.ClearListeners();

	public override void Flush() {
		using (Streams.EnterAccessScope())
			EnsureTreeCalculated();
	}

	protected override void AttachInternal() {
		var flatTreeData = new StreamMappedBuffer(AttachmentStream);
		var itemCount = Streams.Count - Streams.Header.ReservedStreams;
		_merkleTree = new FlatMerkleTree(_hashAlgorithm, flatTreeData, itemCount);
		_readOnlyMerkleTree = new StreamLockingMerkleTree(this, _merkleTree, Streams);
		var hashSize = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		using (Streams.EnterAccessScope()) {
			_merkleRootProperty = Streams.Header.MapExtensionProperty(
				0, 
				hashSize, 
				new ConstantSizeByteArraySerializer(hashSize).WithNullSubstitution(Hashers.ZeroHash(_hashAlgorithm), ByteArrayEqualityComparer.Instance)
			);

			// When stream mapped root is changed, fire the root changed event (ensures not excessively triggered during dirty writes)
			_merkleRootProperty.ValueChanged += (oldValue, newValue) => RootChanged?.Invoke(oldValue, newValue);

			// Integrity Check: ensure root property value matches actual merkle-root (or set it on first load)
			if (_isFirstLoad) {
				_merkleRootProperty.Value = _merkleTree.Root;
			} 
		}
	}

	protected override void VerifyIntegrity() {
		var errorHeader = $"{nameof(MerkleTreeStore)} (reserved stream: {this.ReservedStreamIndex}) integrity failure";

		// Verify leaf-count matches item count
		var itemCount = Streams.Count - Streams.Header.ReservedStreams;
		Guard.Ensure(_merkleTree.Size.LeafCount == itemCount, $"{errorHeader}: item count {itemCount} mismatch with tree leaf-count {_merkleTree.Size.LeafCount}");

		// Verify merkle-root of header matches that of the tree
		Guard.Ensure(
			ByteArrayEqualityComparer.Instance.Equals(_merkleTree.Root, _merkleRootProperty.Value), 
			$"{errorHeader}: header root '{_merkleRootProperty.Value?.ToHexString()}', computed root '{_merkleTree.Root?.ToHexString()}'."
		);
	}

	protected override void DetachInternal() {
		// Ensure tree is calculated 
		_merkleTree = null;
		_readOnlyMerkleTree = null;
		_merkleRootProperty = null;
	}

	private void EnsureTreeCalculated() {
		Guard.Ensure(Streams.IsLocked, $"Merkle-tree store must be locked before calling '{nameof(EnsureTreeCalculated)}' method");

		if (!_dirtyRoot)
			return;

		_merkleRootProperty.Value = _merkleTree.Root;
		_dirtyRoot = false;
	}

	private class StreamLockingMerkleTree : MerkleTreeDecorator  {
		private readonly MerkleTreeStore _merkleTreeStore;
		private readonly ClusteredStreams _streams;

		public StreamLockingMerkleTree(MerkleTreeStore merkleTreeStore, IMerkleTree internalMerkleTree, ClusteredStreams streams) 
			: base(internalMerkleTree) {
			_merkleTreeStore = merkleTreeStore;
			_streams = streams;
		}

		public override byte[] Root { 
			get {
				// Locks the streams
				using var _ = _streams.EnterAccessScope();

				// This call ensures that a dirty root is fully evaluated
				_merkleTreeStore.EnsureTreeCalculated();

				// Now that root is evaluated, we can fetch/return the root
				var root = base.Root;;

				// Sanity check: ensure the merkle-root of the tree matches the merkle-root property mapped onto the clustered stream header
				Debug.Assert(ByteArrayEqualityComparer.Instance.Equals(root, _merkleTreeStore._merkleRootProperty.Value));

				return base.Root;
			}
		}

		public override ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
			using var _ = _streams.EnterAccessScope();
			_merkleTreeStore.Flush();
			return base.GetValue(coordinate);
		}

	}
}