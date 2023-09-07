// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Collections;
using System;

namespace Hydrogen;

/// <summary>
/// Used to maintain a merkle-tree of an <see cref="ObjectContainer"/>'s items. The merkle-tree is stored within a reserved stream within the container.
/// </summary>
/// <remarks>When fetching item bytes for hashing, a key-value-pair with same key but empty/null value will result in the same digest.</remarks>
internal class ObjectContainerMerkleTree : MetaDataProviderBase, IMetaDataMerkleTree {

	private readonly CHF _hashAlgorithm;
	private IMerkleTree _readOnlyMerkleTree;
	private IDynamicMerkleTree _merkleTree;
	private readonly Func<long, byte[]> _itemDigestor;
	private StreamMappedProperty<byte[]> _merkleRootProperty;

	public ObjectContainerMerkleTree(
		ObjectContainer objectContainer,
		Func<long, byte[]> itemDigestor,
		CHF hashAlgorithm,
		long reservedStreamIndex, 
		long offset
	) : base(objectContainer, reservedStreamIndex, offset) {
		_itemDigestor = itemDigestor;
		_hashAlgorithm = hashAlgorithm;
		Container.PostItemOperation += Container_PostItemOperation;
		objectContainer.StreamContainer.RegisterInitAction(Initialize);
	}

	public IMerkleTree MerkleTree => _readOnlyMerkleTree;

	private void Initialize() {
		var hashSize = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		using (Container.StreamContainer.EnterAccessScope()) {
			_merkleRootProperty = Container.StreamContainer.Header.CreateExtensionProperty(
				0, 
				hashSize, 
				new StaticSizeByteArraySerializer(hashSize).WithNullSubstitution(Hashers.ZeroHash(_hashAlgorithm))
			);
		}
	}

	protected override void OnLoaded() {
		base.OnLoaded();
		var flatTreeData = new StreamMappedBuffer(Stream);
		_merkleTree = new FlatMerkleTree(_hashAlgorithm, flatTreeData, Container.Count);
		_readOnlyMerkleTree = new ContainerLockingMerkleTree(_merkleTree, Container);
	}

	private void Container_PostItemOperation(object source, long index, object item, ObjectContainerOperationType operationType) {
		// TODO: proof-building can be done here
		using var _ = Container.EnterAccessScope();
		switch(operationType) {
			case ObjectContainerOperationType.Read:
				// do nothing
				break;
			case ObjectContainerOperationType.Add:
				var digest = _itemDigestor(index);
				_merkleTree.Leafs.Add(digest);
				_merkleRootProperty.Value = _merkleTree.Root;
				break;
			case ObjectContainerOperationType.Insert:
				digest = _itemDigestor(index);
				_merkleTree.Leafs.Insert(index, digest);
				_merkleRootProperty.Value = _merkleTree.Root;
				break;
			case ObjectContainerOperationType.Update:
				digest = _itemDigestor(index);
				_merkleTree.Leafs.Update(index, digest);
				_merkleRootProperty.Value = _merkleTree.Root;
				break;
			case ObjectContainerOperationType.Remove:
				_merkleTree.Leafs.RemoveAt(index);
				_merkleRootProperty.Value = _merkleTree.Root;
				break;				
			case ObjectContainerOperationType.Reap:
				digest = _itemDigestor(index);
				_merkleTree.Leafs.Update(index, digest);
				_merkleRootProperty.Value = _merkleTree.Root;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
		}
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


internal class ObjectContainerMerkleTree<TItem> : ObjectContainerMerkleTree {
	public ObjectContainerMerkleTree(
		ObjectContainer<TItem> objectContainer,
		CHF hashAlgorithm,
		long reservedStreamIndex, 
		long offset
	) : base(
		objectContainer,
		objectContainer.StreamContainer.ReadAll,
		hashAlgorithm,
		reservedStreamIndex, 
		offset
	) {
	}
}