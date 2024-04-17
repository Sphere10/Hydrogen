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

public class StreamMappedMerkleRecyclableList<TItem> : RecyclableListDecorator<TItem, IStreamMappedRecyclableList<TItem>>, IStreamMappedRecyclableList<TItem>, IMerkleList<TItem> {

	public event EventHandlerEx<object> Loading { add => InternalCollection.Loading += value; remove => InternalCollection.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalCollection.Loaded += value; remove => InternalCollection.Loaded -= value; }

	private readonly MerkleTreeIndex _merkleTreeIndex;

	public StreamMappedMerkleRecyclableList(
		Stream rootStream,
		CHF hashAlgorithm,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		long reservedStreams = 2,
		string merkleTreeIndexName = HydrogenDefaults.DefaultMerkleTreeIndexName,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string optionalItemChecksumIndexName = null,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false
	) : this(
		CreateRecyclableList(
			rootStream,
			hashAlgorithm,
			clusterSize,
			itemSerializer,
			itemComparer,
			itemChecksummer,
			policy,
			reservedStreams,
			recyclableIndexIndexName,
			optionalItemChecksumIndexName,
			merkleTreeIndexName,
			endianness
		),
		merkleTreeIndexName,
		autoLoad
	) {
	}
	
	internal StreamMappedMerkleRecyclableList(
		IStreamMappedRecyclableList<TItem> streamMappedList, 
		string merkleTreeIndexName,
		bool autoLoad = false
	) 
		: base(streamMappedList) {
		Guard.ArgumentNotNull(streamMappedList, nameof(streamMappedList));
		_merkleTreeIndex = (MerkleTreeIndex)streamMappedList.ObjectStream.Streams.Attachments[merkleTreeIndexName];

		if (autoLoad && RequiresLoad)
			Load();
	}

	public ObjectStream<TItem> ObjectStream => InternalCollection.ObjectStream;

	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	public IMerkleTree MerkleTree => _merkleTreeIndex.MerkleTree;

	public bool RequiresLoad => InternalCollection.RequiresLoad;

	public void Load() => InternalCollection.Load();

	public Task LoadAsync() => InternalCollection.LoadAsync();

	public void Dispose() {
		InternalCollection.Dispose();
	}

	private static IStreamMappedRecyclableList<TItem> CreateRecyclableList(
		Stream rootStream,
		CHF hashAlgorithm,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer,
		IEqualityComparer<TItem> itemComparer,
		IItemChecksummer<TItem> itemChecksummer,
		ClusteredStreamsPolicy policy,
		long reservedStreams,
		string recyclableIndexIndexName,
		string optionalItemChecksumIndexName,
		string merkleTreeIndexName,
		Endianness endianness
	) {
		var streamMappedList = StreamMappedFactory.CreateRecyclableList(
			rootStream,
			clusterSize,
			itemSerializer,
			itemComparer,
			itemChecksummer,
			policy,
			reservedStreams,
			recyclableIndexIndexName,
			optionalItemChecksumIndexName,
			endianness,
			false
		);

		var merkleTreeIndex = new MerkleTreeIndex(
			streamMappedList.ObjectStream,
			merkleTreeIndexName,
			new ObjectStreamItemHasher(streamMappedList.ObjectStream, hashAlgorithm),
			hashAlgorithm
		);
		streamMappedList.ObjectStream.Streams.RegisterAttachment(merkleTreeIndex);

		return streamMappedList;
	}

}
