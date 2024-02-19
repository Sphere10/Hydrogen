// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using Hydrogen.Collections;

namespace Hydrogen;

/// <summary>
/// Used to store keys of an item in an <see cref="ObjectStream"/>. Used primarily for <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>"/> which
/// stores only the value part in the objectStream, the keys are stored in these (mapped to a reserved stream).
/// </summary>
/// <remarks>Unlike <see cref="KeyIndex{TItem,TKey}"/> which automatically extracts the key from the item and stores it, this is used as a primary storage for the key itself. Thus it is not an index, it is a pure store.</remarks>
internal class MerkleTreeStore : MetaDataStoreBase<byte[]> {
	private readonly CHF _hashAlgorithm;
	private IMerkleTree _readOnlyMerkleTree;
	private IDynamicMerkleTree _merkleTree;
	private StreamMappedProperty<byte[]> _merkleRootProperty;
	private bool _dirtyRoot;

	// Migrate from MerkleTreeIndex stuff into here
	public MerkleTreeStore(ClusteredStreams streams, long reservedStreamIndex, CHF hashAlgorithm) 
		: base(streams, reservedStreamIndex) {
		_hashAlgorithm = hashAlgorithm;
		_dirtyRoot = false;
	}

	public IMerkleTree MerkleTree => _readOnlyMerkleTree;

	public override long Count => _merkleTree.Size.LeafCount;

	public override byte[] Read(long index) 
		=> ReadBytes(index);

	public override byte[] ReadBytes(long index) 
		=> _merkleTree.Leafs.Read(index);
	
	public override void Add(long index, byte[] data) {
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

	protected override void AttachInternal() {
		var flatTreeData = new StreamMappedBuffer(AttachmentStream);
		_merkleTree = new FlatMerkleTree(_hashAlgorithm, flatTreeData, Streams.Count);
		_readOnlyMerkleTree = new ContainerLockingMerkleTree(this, _merkleTree, Streams);
		var hashSize = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		using (Streams.EnterAccessScope()) {
			_merkleRootProperty = Streams.Header.MapExtensionProperty(
				0, 
				hashSize, 
				new ConstantSizeByteArraySerializer(hashSize).WithNullSubstitution(Hashers.ZeroHash(_hashAlgorithm), ByteArrayEqualityComparer.Instance)
			);
		}
	}

	protected override void DetachInternal() {
		// Ensure tree is calculated 
		_merkleTree = null;
		_readOnlyMerkleTree = null;
		_merkleRootProperty = null;
	}

	private void EnsureTreeCalculated() {
		// TODO: Guard ensure access scope is entered

		if (!_dirtyRoot) 
			return;

		_merkleRootProperty.Value = _merkleTree.Root;
		_dirtyRoot = true;
	}

	private class ContainerLockingMerkleTree : MerkleTreeDecorator  {
		private readonly MerkleTreeStore _merkleTreeStore;
		private readonly ClusteredStreams _container;

		public ContainerLockingMerkleTree(MerkleTreeStore merkleTreeStore, IMerkleTree internalMerkleTree, ClusteredStreams container) 
			: base(internalMerkleTree) {
			_merkleTreeStore = merkleTreeStore;
			_container = container;
		}

		public override byte[] Root { 
			get {
				using var _ = _container.EnterAccessScope();
				_merkleTreeStore.EnsureTreeCalculated();
				var root = base.Root;;
				Debug.Assert(ByteArrayEqualityComparer.Instance.Equals(root, _merkleTreeStore._merkleRootProperty.Value));
				return base.Root;
			}
		}

		public override ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
			using var _ = _container.EnterAccessScope();
			_merkleTreeStore.EnsureTreeCalculated();
			return base.GetValue(coordinate);
		}
	}

}