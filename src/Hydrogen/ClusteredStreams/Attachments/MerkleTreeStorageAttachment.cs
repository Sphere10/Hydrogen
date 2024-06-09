// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Hydrogen.Collections;

namespace Hydrogen;

/// <summary>
/// Used to store keys of an item in an <see cref="ObjectStream"/>. Used primarily for <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>"/> which
/// stores only the value part in the objectStream, the keys are stored in these (mapped to a reserved stream).
/// </summary>
/// <remarks>Unlike <see cref="ProjectionIndex{TItem,TKey}"/> which automatically extracts the key from the item and stores it, this is used as a primary storage for the key itself. Thus it is not an index, it is a pure store.</remarks>
internal class MerkleTreeStorageAttachment : ClusteredStreamsAttachmentBase, IDynamicMerkleTree {

	public event EventHandlerEx<byte[], byte[]> RootChanged;

	private readonly CHF _hashAlgorithm;
	private FlatMerkleTree _flatTree;
	private StreamLockingList<byte[]> _streamLockingTreeLeafs;
	private StreamMappedProperty<byte[]> _merkleRootProperty;
	private bool _isFirstLoad;

	// Migrate from MerkleTreeIndex stuff into here
	public MerkleTreeStorageAttachment(ClusteredStreams streams, string attachmentID, CHF hashAlgorithm, bool isFirstLoad) 
		: base(streams, attachmentID) {
		_hashAlgorithm = hashAlgorithm;
		_isFirstLoad = isFirstLoad;
	}

	public IExtendedList<byte[]> Leafs { 
		get {
			CheckAttached();
			return _streamLockingTreeLeafs;
		} 
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

	public CHF HashAlgorithm { 
		get {
			CheckAttached();
			return _flatTree.HashAlgorithm;
		}
	}

	public MerkleSize Size {
		get {
			CheckAttached();
			return _flatTree.Size;
		} 
	}

	public byte[] Root { 
		get {
			CheckAttached();

			// Locks the streams
			using var _ = Streams.EnterAccessScope();

			// This call ensures that any dirty root is fully evaluated
			EnsureTreeCalculated();

			// Now that root is evaluated, we can fetch/return the root
			var root = _flatTree.Root;

			// Sanity check: ensure the merkle-root of the tree matches the merkle-root property mapped onto the clustered stream header
			Debug.Assert(ByteArrayEqualityComparer.Instance.Equals(root, _merkleRootProperty.Value));

			return root;
		}
	}

	public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		//EnsureTreeCalculated();  // Flat-tree ensures that coordinate is calculated, even if others aren't
		return _flatTree.GetValue(coordinate);
	}

	public override void Flush() {
		using var _ = Streams.EnterAccessScope();
		EnsureTreeCalculated();
	}

	protected override void AttachInternal() {
		var flatTreeData = new StreamMappedBuffer(AttachmentStream);
		var itemCount = Streams.Count - Streams.Header.ReservedStreams;
		_flatTree = new FlatMerkleTree(_hashAlgorithm, flatTreeData, itemCount);
		_streamLockingTreeLeafs = new StreamLockingList<byte[]>(_flatTree.Leafs, this);
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
				_merkleRootProperty.Value = _flatTree.Root;
			} 
		}
	}

	protected override void VerifyIntegrity() {
		var errorHeader = $"{nameof(MerkleTreeStorageAttachment)} (reserved stream: {this.ReservedStreamIndex}) integrity failure";

		// Verify leaf-count matches item count
		var itemCount = Streams.Count - Streams.Header.ReservedStreams;
		Guard.Ensure(_flatTree.Size.LeafCount == itemCount, $"{errorHeader}: item count {itemCount} mismatch with tree leaf-count {_flatTree.Size.LeafCount}");

		// Verify merkle-root of header matches that of the tree
		Guard.Ensure(
			ByteArrayEqualityComparer.Instance.Equals(_flatTree.Root, _merkleRootProperty.Value), 
			$"{errorHeader}: header root '{_merkleRootProperty.Value?.ToHexString()}', computed root '{_flatTree.Root?.ToHexString()}'."
		);
	}

	protected override void DetachInternal() {
		// Ensure tree is calculated 
		_flatTree = null;
		_streamLockingTreeLeafs = null;
		_merkleRootProperty = null;
	}

	private void EnsureTreeCalculated() {
		Guard.Ensure(Streams.IsLocked, $"{nameof(MerkleTreeStorageAttachment)} must be locked before calling '{nameof(EnsureTreeCalculated)}' method");
		_merkleRootProperty.Value = _flatTree.Root;
	}

}