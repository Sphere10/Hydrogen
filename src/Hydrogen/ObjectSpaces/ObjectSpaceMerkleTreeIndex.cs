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
using System.IO;
using System.Linq;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Maintains a <see cref="IMerkleTree"/> of an <see cref="ObjectSpace"/>. It does this by adopting the merkle-roots of the underlying <see cref="ObjectSpace"/>'s dimensions <see cref="IMerkleTree"/> as leaves in it's own tree.
/// When a dimension merkle-tree root is changed, it triggers a leaf-update on this tree.  
/// </summary>
/// <remarks>A dimension of an object space is a <see cref="ObjectStream"/> of a specific type of object.</remarks>
internal class ObjectSpaceMerkleTreeIndex : ClusteredStreamsAttachmentDecorator<MerkleTreeStore> {
	private readonly ObjectSpace _objectSpace;
	private readonly IList<(MerkleTreeStore, EventHandlerEx<byte[], byte[]>)> _collectionTreeListeners;

	public ObjectSpaceMerkleTreeIndex(ObjectSpace objectSpace, long reservedStreamIndex, CHF chf, bool isFirstLoad) 
		: base (new MerkleTreeStore(objectSpace.InternalStreams, reservedStreamIndex, chf, isFirstLoad) ){
		Guard.ArgumentNotNull(objectSpace, nameof(objectSpace));
		_objectSpace = objectSpace;
		_collectionTreeListeners = new List<(MerkleTreeStore, EventHandlerEx<byte[], byte[]>)>();
	}

	public IMerkleTree MerkleTree => Inner.MerkleTree;

	public override void Attach() {
		base.Attach();

		VerifyIntegrity();

		// Ensure when dimension added/removed, that spatial tree listens
		SubscribeToDimensionMutationEvents();
	}

	public override void Detach() {
		// Unsub from dimension changes, since detach may trigger them
		UnsubscribeToDimensionMutationEvents();

		base.Detach();
	}

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
		for(var i = 0; i < _objectSpace.Dimensions.Count; i++)
			SubscribeToDimensionTreeChanges(i);
	}

	private void SubscribeToDimensionTreeChanges(int i) {
		var dimension = _objectSpace.Dimensions[i];
		var dimensionMerkleTree = dimension.ObjectStream.Streams.FindAttachment<MerkleTreeIndex>();

		// Listen to underlying collection root changes (and track the handler for unsub later)
		var capturedIndex = i;
		void CollectionRootListener(byte[] oldValue, byte[] newValue) {
			newValue ??= Hashers.ZeroHash(Inner.MerkleTree.HashAlgorithm); // merkle-root property will return null when changed to zero's
			Inner.Update(capturedIndex, newValue);
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

	public void VerifyIntegrity() {
		// re-compute spatial root from dimensional merkle tree roots
		var dimensionRoots = new List<byte[]>();
		for (var i = 0; i < _objectSpace.Dimensions.Count; i++) {
			// Get the object dimension and it's root
			var dimension = _objectSpace.Dimensions[i];
			if (!dimension.ObjectStream.Streams.TryFindAttachment<MerkleTreeIndex>(out var dimensionTree))
				throw new InvalidDataException($"{nameof(ObjectSpace)} dimension {i} missing a {nameof(MerkleTreeIndex)}");
			var dimensionRoot = dimensionTree.MerkleTree.Root;
			
			// Check that corresponding spatial-tree leaf matches root of dimension tree
			var spatialLeafValue = MerkleTree.GetValue(MerkleCoordinate.LeafAt(i)).ToArray();
			Guard.Ensure(
				spatialLeafValue.All(x => x == 0) && dimensionRoot is null ||
				ByteArrayEqualityComparer.Instance.Equals(spatialLeafValue, dimensionRoot),
				$"{nameof(ObjectSpace)} dimension {i} root does not match corresponding leaf value in spatial-tree"
			);

			// Track this dimension root as a leaf (for later)
			dimensionRoots.Add(dimensionTree.MerkleTree.Root ?? Hashers.ZeroHash(_objectSpace.Definition.HashFunction));
		}

		// check expected matches stored spatial root
		var calculatedRoot = Hydrogen.MerkleTree.ComputeMerkleRoot(dimensionRoots, _objectSpace.Definition.HashFunction);
		if (!ByteArrayEqualityComparer.Instance.Equals(MerkleTree.Root, calculatedRoot))
			throw new InvalidDataException($"{nameof(ObjectSpace)} merkle-tree root did not match roots of dimension trees");

		// NOTE: the checking of spatial-tree root matches the stream mapped property root is done by Inner.Attach() -> VerifyIntegrity()
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
