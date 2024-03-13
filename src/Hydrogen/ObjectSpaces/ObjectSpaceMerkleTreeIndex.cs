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
using System.Linq;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Maintains a <see cref="IMerkleTree"/> of an <see cref="ObjectSpace"/>. It does this by adopting the merkle-roots of the underlying <see cref="ObjectSpace"/>'s dimensions <see cref="IMerkleTree"/> as leaves in it's own tree.
/// When a dimension merkle-tree root is changed, it triggers a leaf-update on this tree.  
/// </summary>
/// <remarks>A dimension of an object space is a <see cref="ObjectStream"/> of a specific type of object.</remarks>
internal class ObjectSpaceMerkleTreeIndex : IClusteredStreamsAttachment {
	private readonly ObjectSpace _objectSpace;
	private readonly MerkleTreeStore _objectSpaceTreeStore;
	private readonly IList<(MerkleTreeStore, EventHandlerEx<byte[], byte[]>)> _collectionTreeListeners;

	public ObjectSpaceMerkleTreeIndex(ObjectSpace objectSpace, long reservedStreamIndex, CHF chf, bool isFirstLoad) {
		Guard.ArgumentNotNull(objectSpace, nameof(objectSpace));
		_objectSpace = objectSpace;
		_objectSpaceTreeStore = new MerkleTreeStore(objectSpace.InternalStreams, reservedStreamIndex, chf, isFirstLoad);
		_collectionTreeListeners = new List<(MerkleTreeStore, EventHandlerEx<byte[], byte[]>)>();
	}

	public ClusteredStreams Streams => _objectSpaceTreeStore.Streams;

	public MerkleTreeStore MerkleTreeStore => _objectSpaceTreeStore;

	public long ReservedStreamIndex => _objectSpaceTreeStore.ReservedStreamIndex;

	public bool IsAttached => _objectSpaceTreeStore.IsAttached;

	public void Attach() {
		Guard.Ensure(!IsAttached, "Already attached");
		_objectSpaceTreeStore.Attach();

		// Ensure when dimension added/removed, that spatial tree listens
		SubscribeToDimensionMutationEvents();
	}

	public void Detach() {
		Guard.Ensure(IsAttached, "Not attached");
		UnsubscribeToDimensionMutationEvents();
		_objectSpaceTreeStore.Detach();
	}

	public void Flush() => _objectSpaceTreeStore.Flush();

	private void SubscribeToDimensionMutationEvents() {
		Streams.StreamAdded += HandleDimensionAdded;
		Streams.StreamRemoved += HandleDimensionRemoved;
		Streams.StreamInserted += HandleDimensionInserted;
		Streams.StreamSwapped += HandleDimensionSwapped;
		Streams.Cleared += HandleDimensionsCleared;

		// NOTE: rather than listen to Streams.StreamUpdated event, 
		// we simply listen to that dimensions merkle root property changed event
		// which allows us to lazily update
		// For dimensions already existing, subscribe to their merkle-tree change events
		for(var i = 0; i < _objectSpace.Dimensions; i++)
			SubscribeToDimensionTreeChanges(i);
	}

	private void SubscribeToDimensionTreeChanges(int i) {
		var dimension = _objectSpace.GetDimension(i);
		var dimensionMerkleTree = dimension.ObjectStream.Streams.FindAttachment<MerkleTreeIndex>();
		var dimensionRoot = dimensionMerkleTree.MerkleTree.Root;

		// Deprecate below check since merkle-tree must always be at correct leaf-count
		//// If object space tree isn't tracking this container yet, add it to tree now
		//if (_objectSpaceTreeStore.Count <= i)
		//	_objectSpaceTreeStore.Add(i, dimensionRoot);

		// sanity check: ensure container root matches the leaf hash in the object space tree
		var dimensionTreeRoot = _objectSpaceTreeStore.MerkleTree.GetValue(MerkleCoordinate.LeafAt(i)).ToArray();
		Debug.Assert(
			dimensionTreeRoot.All(x => x == 0) && dimensionRoot is null ||
			ByteArrayEqualityComparer.Instance.Equals(dimensionTreeRoot, dimensionRoot)
		);

		// Listen to underlying collection root changes (and track the handler for unsub later)
		var capturedIndex = i;
		void CollectionRootListener(byte[] oldValue, byte[] newValue) {
			_objectSpaceTreeStore.Update(capturedIndex, newValue);
		}
		dimensionMerkleTree.KeyStore.RootChanged += CollectionRootListener;
		_collectionTreeListeners.Add((dimensionMerkleTree.KeyStore, CollectionRootListener));

	}

	private void UnsubscribeToDimensionMutationEvents() {
		Streams.StreamAdded -= HandleDimensionAdded;
		Streams.StreamRemoved -= HandleDimensionRemoved;
		Streams.StreamInserted -= HandleDimensionInserted;
		Streams.StreamSwapped -= HandleDimensionSwapped;
		
		// Unsub from dimensions trees (allow them to be collected)
		foreach(var listener in _collectionTreeListeners)
			listener.Item1.RootChanged -= listener.Item2;
		_collectionTreeListeners.Clear();

	}

	public void VerifyConsistency() {
		// TODO
		for(var i = 0; i < _objectSpace.Dimensions; i++) {
			var dimension = _objectSpace.GetDimension(i);

			// Check 1
		}
	}


	private void HandleDimensionAdded(long ix, ClusteredStreamDescriptor stream) 
		=> throw new NotSupportedException($"{nameof(ObjectSpaceMerkleTreeIndex)} does not support mutations of dimensions");

	private void HandleDimensionRemoved(long _) 
		=> throw new NotSupportedException($"{nameof(ObjectSpaceMerkleTreeIndex)} does not support mutations of dimensions");

	private void HandleDimensionInserted(long l, ClusteredStreamDescriptor clusteredStreamDescriptor) 
		=> throw new NotSupportedException($"{nameof(ObjectSpaceMerkleTreeIndex)} does not support mutations of dimensions");

	private void HandleDimensionSwapped((long, ClusteredStreamDescriptor) valueTuple, (long, ClusteredStreamDescriptor) valueTuple1) 
		=> throw new NotSupportedException($"{nameof(ObjectSpaceMerkleTreeIndex)} does not support mutations of dimensions");

	private void HandleDimensionsCleared() 
		=> throw new NotSupportedException($"{nameof(ObjectSpaceMerkleTreeIndex)} does not support mutations of dimensions");

}
