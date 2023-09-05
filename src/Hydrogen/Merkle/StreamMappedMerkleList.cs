// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
//
// NOTE: This file is part of the reference implementation for Dynamic Merkle-Trees. Read the paper at:
// Web: https://sphere10.com/tech/dynamic-merkle-trees
// e-print: https://vixra.org/abs/2305.0087

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

public class StreamMappedMerkleList<TItem> : ExtendedListDecorator<TItem, IStreamMappedList<TItem>>, IStreamMappedList<TItem>, IMerkleList<TItem> {
	
	public event EventHandlerEx<object> Loading { add => InternalCollection.Loading += value; remove => InternalCollection.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalCollection.Loaded += value; remove => InternalCollection.Loaded -= value; }

	private readonly ObjectContainerMerkleTree _merkleTreeIndex; 

	public StreamMappedMerkleList(
		Stream rootStream,
		int clusterSize,
		CHF hashAlgorithm,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 2,
		long merkleTreeStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false
	) : this(
		new StreamContainer(
			rootStream,
			clusterSize,
			policy,
			reservedStreams,
			endianness,
			false
		),
		hashAlgorithm,
		itemSerializer,
		itemComparer,
		itemChecksummer,
		merkleTreeStreamIndex,
		checksumIndexStreamIndex,
		autoLoad
	) {
		ObjectContainer.OwnsStreamContainer = true;
	}

	public StreamMappedMerkleList(
		StreamContainer streamContainer,
		CHF hashAlgorithm,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		long merkleTreeStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		bool autoLoad = false
	) : this(
			new StreamMappedList<TItem>(
				streamContainer,
				itemSerializer,
				itemComparer,
				itemChecksummer,
				checksumIndexStreamIndex,
				false
			),
			hashAlgorithm,
			merkleTreeStreamIndex,
			autoLoad
	) {
	}

	protected StreamMappedMerkleList(
		IStreamMappedList<TItem> streamMappedList,
		CHF hashAlgorithm,
		long merkleTreeStreamIndex = 0,
		bool autoLoad = false
	) : base(streamMappedList) {
		_merkleTreeIndex = new ObjectContainerMerkleTree(
			ObjectContainer,
			DigestItem,
			hashAlgorithm,
			merkleTreeStreamIndex,
			0
		);

		if (autoLoad && RequiresLoad) 
			Load();
	}

	public ObjectContainer<TItem> ObjectContainer => InternalCollection.ObjectContainer;
	
	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;
	
	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	public IMerkleTree MerkleTree => _merkleTreeIndex.MerkleTree;

	public bool RequiresLoad => InternalCollection.RequiresLoad;

	public void Load() => InternalCollection.Load();

	public Task LoadAsync() => InternalCollection.LoadAsync();

	public void Dispose() {
		_merkleTreeIndex.Dispose();
		InternalCollection.Dispose();
	}

	private byte[] DigestItem(long index) {
		var bytes = InternalCollection.ObjectContainer.GetItemBytes(index);
		return Hashers.HashWithNullSupport(MerkleTree.HashAlgorithm, bytes);
	}

}
