// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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

/// <summary>
/// Stream-mapped recyclable list that maintains merkle integrity data alongside recyclable index metadata.
/// </summary>
public class StreamMappedMerkleRecyclableList<TItem> : RecyclableListDecorator<TItem, IStreamMappedRecyclableList<TItem>>, IStreamMappedRecyclableList<TItem>, IMerkleList<TItem> {

	/// <summary>
	/// Raised when the underlying stream-mapped list is loading.
	/// </summary>
	public event EventHandlerEx<object> Loading { add => InternalCollection.Loading += value; remove => InternalCollection.Loading -= value; }
	/// <summary>
	/// Raised when the underlying stream-mapped list has finished loading.
	/// </summary>
	public event EventHandlerEx<object> Loaded { add => InternalCollection.Loaded += value; remove => InternalCollection.Loaded -= value; }

	private readonly MerkleTreeIndex _merkleTreeIndex;

	/// <summary>
	/// Initializes a stream-mapped recyclable merkle list over the provided root stream.
	/// </summary>
	/// <param name="rootStream">The root stream backing the list.</param>
	/// <param name="hashAlgorithm">The hash function to use.</param>
	/// <param name="clusterSize">The cluster size for the stream.</param>
	/// <param name="itemSerializer">The item serializer.</param>
	/// <param name="itemComparer">The item comparer.</param>
	/// <param name="itemChecksummer">The item checksummer.</param>
	/// <param name="policy">The clustered streams policy.</param>
	/// <param name="reservedStreams">The number of reserved streams.</param>
	/// <param name="merkleTreeIndexName">The attachment name for the merkle tree index.</param>
	/// <param name="recyclableIndexIndexName">The attachment name for the recyclable index.</param>
	/// <param name="optionalItemChecksumIndexName">Optional attachment name for an item checksum index.</param>
	/// <param name="endianness">The endianness to use.</param>
	/// <param name="autoLoad">Whether to load the list on construction.</param>
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
	
	/// <summary>
	/// Initializes a merkle list wrapper over an existing recyclable list instance.
	/// </summary>
	/// <param name="streamMappedList">The stream-mapped list to wrap.</param>
	/// <param name="merkleTreeIndexName">The attachment name for the merkle tree index.</param>
	/// <param name="autoLoad">Whether to load the list on construction.</param>
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

	/// <summary>
	/// Gets the underlying object stream for the list.
	/// </summary>
	public ObjectStream<TItem> ObjectStream => InternalCollection.ObjectStream;

	/// <summary>
	/// Gets the underlying object stream as the non-generic interface type.
	/// </summary>
	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	/// <summary>
	/// Gets the serializer used for list items.
	/// </summary>
	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;

	/// <summary>
	/// Gets the comparer used for list items.
	/// </summary>
	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	/// <summary>
	/// Gets the merkle tree index that tracks item hashes.
	/// </summary>
	public IMerkleTree MerkleTree => _merkleTreeIndex.MerkleTree;

	/// <summary>
	/// Gets whether the list requires loading before access.
	/// </summary>
	public bool RequiresLoad => InternalCollection.RequiresLoad;

	/// <summary>
	/// Loads the underlying list if required.
	/// </summary>
	public void Load() => InternalCollection.Load();

	/// <summary>
	/// Asynchronously loads the underlying list if required.
	/// </summary>
	public Task LoadAsync() => InternalCollection.LoadAsync();

	/// <summary>
	/// Disposes the underlying list and associated resources.
	/// </summary>
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
