// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen.Collections;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Used to store keys of an item in an <see cref="ObjectContainer"/>. Used primarily for <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>"/> which
/// stores only the value part in the container, the keys are stored in these (mapped to a reserved stream).
/// </summary>
/// <remarks>Unlike <see cref="KeyIndex{TItem,TKey}"/> which automatically extracts the key from the item and stores it, this is used as a primary storage for the key itself. Thus it is not an index, it is a pure store.</remarks>
/// <typeparam name="TKey"></typeparam>
internal class MerkleTreeStore : MetaDataStoreBase<byte[]> {
	private readonly CHF _hashAlgorithm;
	private IMerkleTree _readOnlyMerkleTree;
	private IDynamicMerkleTree _merkleTree;
	private StreamMappedProperty<byte[]> _merkleRootProperty;

	// Migrate from MerkleTreeIndex stuff into here
	public MerkleTreeStore(ObjectContainer container, long reservedStreamIndex, CHF hashAlgorithm) 
		: base(container, reservedStreamIndex) {
		_hashAlgorithm = hashAlgorithm;
	}

	public IMerkleTree MerkleTree => _readOnlyMerkleTree;

	protected override void AttachInternal() {
		var flatTreeData = new StreamMappedBuffer(Stream);
		_merkleTree = new FlatMerkleTree(_hashAlgorithm, flatTreeData, Container.Count);
		_readOnlyMerkleTree = new ContainerLockingMerkleTree(_merkleTree, Container);
		var hashSize = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		using (Container.StreamContainer.EnterAccessScope()) {
			_merkleRootProperty = Container.StreamContainer.Header.CreateExtensionProperty(
				0, 
				hashSize, 
				new ConstantSizeByteArraySerializer(hashSize).WithNullSubstitution(Hashers.ZeroHash(_hashAlgorithm))
			);
		}
	}

	protected override void DetachInternal() {
		_merkleTree = null;
		_readOnlyMerkleTree = null;
		_merkleRootProperty = null;
	}

	public override long Count => _merkleTree.Size.LeafCount;

	public override byte[] Read(long index) => ReadBytes(index);

	public override byte[] ReadBytes(long index) 
		=> _merkleTree.Leafs.Read(index);
	
	public override void Add(long index, byte[] data) {
		using var _ = Container.EnterAccessScope();
		_merkleTree.Leafs.Insert(index, data);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	public override void Update(long index, byte[] data) {
		using var _ = Container.EnterAccessScope();
		_merkleTree.Leafs.Update(index, data);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	public override void Insert(long index, byte[] data) {
		using var _ = Container.EnterAccessScope();
		_merkleTree.Leafs.Insert(index, data);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	public override void Remove(long index) {
		using var _ = Container.EnterAccessScope();
		_merkleTree.Leafs.RemoveAt(index);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	public override void Reap(long index) {
		using var _ = Container.EnterAccessScope();
		var digest = Hashers.ZeroHash(_hashAlgorithm);
		_merkleTree.Leafs.Update(index, digest);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	public override void Clear() 
		=> _merkleTree.Leafs.Clear();

	private class ContainerLockingMerkleTree : MerkleTreeDecorator  {
		private readonly ObjectContainer _container;
		public ContainerLockingMerkleTree(IMerkleTree internalMerkleTree, ObjectContainer container) : base(internalMerkleTree) {
			_container = container;
		}

		public override byte[] Root { 
			get {
				using var _ = _container.EnterAccessScope();
				return base.Root;
			}
		}

		public override ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
			using var _ = _container.EnterAccessScope();
			return base.GetValue(coordinate);
		}
	}


}