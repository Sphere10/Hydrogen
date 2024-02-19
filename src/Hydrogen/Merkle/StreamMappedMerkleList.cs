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
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

public class StreamMappedMerkleList<TItem> : ExtendedListDecorator<TItem, IStreamMappedList<TItem>>, IStreamMappedList<TItem>, IMerkleList<TItem> {
	
	public event EventHandlerEx<object> Loading { add => InternalCollection.Loading += value; remove => InternalCollection.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalCollection.Loaded += value; remove => InternalCollection.Loaded -= value; }

	private readonly MerkleTreeIndex _merkleTreeIndex; 

	public StreamMappedMerkleList(
		Stream rootStream,
		CHF hashAlgorithm,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 1,
		long merkleTreeStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) : this(
			CreateList(
				rootStream,
				hashAlgorithm,
				clusterSize,
				itemSerializer,
				itemComparer,
				itemChecksummer,
				policy,
				reservedStreams,
				merkleTreeStreamIndex,
				checksumIndexStreamIndex,
				endianness
			),
			autoLoad
	) {
	}

	internal StreamMappedMerkleList(IStreamMappedList<TItem> streamMappedList, bool autoLoad = false) : base(streamMappedList) {
		Guard.ArgumentNotNull(streamMappedList, nameof(streamMappedList));
		_merkleTreeIndex = streamMappedList.ObjectContainer.Streams.FindAttachment<MerkleTreeIndex>();
		if (autoLoad && RequiresLoad) 
			Load();
	}

	public ObjectContainer<TItem> ObjectContainer => InternalCollection.ObjectContainer;

	ObjectContainer IStreamMappedCollection.ObjectContainer => ObjectContainer;
	
	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;
	
	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	public IMerkleTree MerkleTree => _merkleTreeIndex.MerkleTree;

	public bool RequiresLoad => InternalCollection.RequiresLoad;

	public void Load() => InternalCollection.Load();

	public Task LoadAsync() => InternalCollection.LoadAsync();

	public void Dispose() {
		InternalCollection.Dispose();
	}

	private static IStreamMappedList<TItem> CreateList(
		Stream rootStream,
		CHF hashAlgorithm,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer,
		IEqualityComparer<TItem> itemComparer,
		IItemChecksummer<TItem> itemChecksummer,
		StreamContainerPolicy policy,
		long reservedStreams,
		long merkleTreeStreamIndex,
		long checksumIndexStreamIndex,
		Endianness endianness
	) {
		var streamMappedList =  StreamMappedFactory.CreateList(
			rootStream,
			clusterSize,
			itemSerializer,
			itemComparer,
			itemChecksummer,
			policy,
			reservedStreams,
			checksumIndexStreamIndex,
			endianness,
			false
		);

		var merkleTreeIndex = new MerkleTreeIndex(
			streamMappedList.ObjectContainer,
			merkleTreeStreamIndex,
			x => DigestItem(streamMappedList.ObjectContainer, x, hashAlgorithm),
			hashAlgorithm
		);
		streamMappedList.ObjectContainer.Streams.RegisterAttachment(merkleTreeIndex);

		return streamMappedList;
	}

	private static byte[] DigestItem(ObjectContainer container, long index, CHF chf) {
		var bytes = container.GetItemBytes(index);
		return Hashers.HashWithNullSupport(chf, bytes);
	}

}
