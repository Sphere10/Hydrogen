// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Collections;
using System;
using System.ComponentModel;
using System.IO;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Used to maintain a merkle-tree of an <see cref="ObjectContainer"/>'s items. The merkle-tree is stored within a reserved stream within the container.
/// </summary>
/// <remarks>When fetching item bytes for hashing, a key-value-pair with same key but empty/null value will result in the same digest.</remarks>
internal class MerkleTreeIndex : MetaDataObserverBase {

	private readonly CHF _hashAlgorithm;
	private IMerkleTree _readOnlyMerkleTree;
	private IDynamicMerkleTree _merkleTree;
	private readonly Func<long, byte[]> _itemDigestor;
	private StreamMappedProperty<byte[]> _merkleRootProperty;

	public MerkleTreeIndex(
		ObjectContainer objectContainer,
		Func<long, byte[]> itemDigestor,
		CHF hashAlgorithm,
		long reservedStreamIndex
	) : base(objectContainer, reservedStreamIndex) {
		_itemDigestor = itemDigestor;
		_hashAlgorithm = hashAlgorithm;
	}

	public IMerkleTree MerkleTree => _readOnlyMerkleTree;


	protected override void AttachInternal() {
		// Merkle-tree is stored a flat-tree mapped over the reserved stream
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

	protected override void OnAdded(object item, long index) {
		base.OnAdded(item, index);
		using var _ = Container.EnterAccessScope();
		var digest = _itemDigestor(index);
		_merkleTree.Leafs.Add(digest);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	protected override void OnInserted(object item, long index) {
		base.OnInserted(item, index);
		using var _ = Container.EnterAccessScope();
		var digest = _itemDigestor(index);
		_merkleTree.Leafs.Insert(index, digest);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	protected override void OnUpdated(object item, long index) {
		base.OnUpdated(item, index);
		using var _ = Container.EnterAccessScope();
		var digest = _itemDigestor(index);
		_merkleTree.Leafs.Update(index, digest);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	protected override void OnRemoved(long index) {
		base.OnRemoved(index);
		using var _ = Container.EnterAccessScope();
		_merkleTree.Leafs.RemoveAt(index);
		_merkleRootProperty.Value = _merkleTree.Root;
	}

	protected override void OnReaped(long index) {
		base.OnReaped(index);
		using var _ = Container.EnterAccessScope();
		var digest = _itemDigestor(index);
		_merkleTree.Leafs.Update(index, digest);
		_merkleRootProperty.Value = _merkleTree.Root;
	}


	public class ContainerLockingMerkleTree : MerkleTreeDecorator  {
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
