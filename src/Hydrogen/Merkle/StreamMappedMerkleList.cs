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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hydrogen.Collections;

namespace Hydrogen;

/// <summary>
/// An <see cref="IExtendedList{T}"/> of <see cref="TItem"/> mapped onto a <see cref="Stream"/> which also maintains an <see cref="IMerkleTree"/> of it's items.
/// </summary>
/// <remarks>The stream mapping is achieved via use of an internal <see cref="IStreamMappedList{TItem}"/></remarks>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedMerkleList<TItem, TInner> : MerkleListAdapter<TItem, TInner>, IStreamMappedList<TItem> where TInner : IStreamMappedList<TItem> {
	public event EventHandlerEx<object> Loading { add => Streams.Loading += value; remove => Streams.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => Streams.Loaded += value; remove => Streams.Loaded -= value; }

	public StreamMappedMerkleList(TInner streamMappedList, IItemHasher<TItem> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex)
		: base(streamMappedList, hasher, new StreamContainerMerkleTreeStream(streamMappedList.Streams, merkleTreeStreamIndex, hashAlgorithm)) {
		Guard.ArgumentNotNull(hasher, nameof(hasher)); 
		try {
			var _ = hasher.Hash(default);
		} catch {
			throw new ArgumentException("Hasher must support null values", nameof(hasher));
		}
	}

	public StreamContainer Streams => InternalCollection.Streams;

	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	public bool RequiresLoad => Streams.RequiresLoad;

	public void Load() {
		Streams.Load();
		Guard.Ensure(InternalCollection.Streams.Header.ReservedStreams > 0, "Clustered streams requires at least 1 reserved stream to store merkle-tree");
	}

	public Task LoadAsync() => Streams.LoadAsync();

	public ClusteredStream EnterAddScope(TItem item) {
		InternalMerkleTree.Leafs.Add(ItemHasher.Hash(item));
		return InternalCollection.EnterAddScope(item);
	}

	public ClusteredStream EnterInsertScope(long index, TItem item) {
		InternalMerkleTree.Leafs.Insert(index, ItemHasher.Hash(item));
		return InternalCollection.EnterInsertScope(index, item);
	}

	public ClusteredStream EnterUpdateScope(long index, TItem item) {
		InternalMerkleTree.Leafs.Update(index, ItemHasher.Hash(item));
		return InternalCollection.EnterUpdateScope(index, item);
	}
}


public class StreamMappedMerkleList<TItem> : StreamMappedMerkleList<TItem, IStreamMappedList<TItem>> {

	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null,
	                              StreamContainerPolicy policy = StreamContainerPolicy.Default, long recordKeySize = 0, long reservedRecords = 1, int merkleTreeStreamIndex = HydrogenDefaults.ClusteredStorageMerkleTreeStreamIndex,
	                              Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(
			rootStream,
			clusterSize,
			hashAlgorithm,
			new ItemDigestor<TItem>(hashAlgorithm, itemSerializer, endianness).WithNullHash(hashAlgorithm),
			itemSerializer,
			itemComparer,
			policy,
			recordKeySize,
			reservedRecords,
			merkleTreeStreamIndex,
			endianness,
			autoLoad
		) {
	}

	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm, IItemHasher<TItem> hasher, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null,
	                              StreamContainerPolicy policy = StreamContainerPolicy.Default, long recordKeySize = 0, long reservedRecords = 1, int merkleTreeStreamIndex = HydrogenDefaults.ClusteredStorageMerkleTreeStreamIndex,
	                              Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(			
			new StreamMappedList<TItem>(
				rootStream,
				clusterSize,
				itemSerializer,
				itemComparer,
				policy,
				recordKeySize,
				reservedRecords,
				endianness,
				autoLoad
			),
			hasher,
			hashAlgorithm,
			merkleTreeStreamIndex) {
		Guard.ArgumentGTE(reservedRecords, 1, nameof(reservedRecords), "Must be greater than 1 to allow streams of merkle-tree");
	}

	public StreamMappedMerkleList(IStreamMappedList<TItem> streamMappedList, IItemHasher<TItem> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex)
		: base(streamMappedList, hasher, hashAlgorithm, merkleTreeStreamIndex) {
	}
	
}
