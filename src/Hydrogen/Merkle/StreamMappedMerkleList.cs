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
using Hydrogen.Collections;

namespace Hydrogen;

/// <summary>
/// An <see cref="IExtendedList{T}"/> of <see cref="TItem"/> mapped onto a <see cref="Stream"/> which also maintains an <see cref="IMerkleTree"/> of it's items.
/// </summary>
/// <remarks>The stream mapping is achieved via use of an internal <see cref="IStreamMappedList{TItem}"/></remarks>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedMerkleList<TItem> : MerkleListAdapter<TItem, IStreamMappedList<TItem>>, IStreamMappedList<TItem> {
	private const int MerkleTreeStreamIndex = 0;

	public event EventHandlerEx<object> Loading {
		add => Storage.Loading += value;
		remove => Storage.Loading -= value;
	}

	public event EventHandlerEx<object> Loaded {
		add => Storage.Loaded += value;
		remove => Storage.Loaded -= value;
	}


	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null,
	                              ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, long recordKeySize = 0, long reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
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
			endianness
		) {
	}

	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm, IItemHasher<TItem> hasher, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null,
	                              ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, long recordKeySize = 0, long reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
		: this(
			new StreamMappedList<TItem>(
				rootStream,
				clusterSize,
				itemSerializer,
				itemComparer,
				policy,
				recordKeySize,
				reservedRecords,
				endianness
			),
			hasher,
			hashAlgorithm
		) {
		Guard.ArgumentNotNull(hasher, nameof(hasher)); // must be provided with null hash support
		Guard.ArgumentGTE(reservedRecords, 1, nameof(reservedRecords), "Must be greater than 1 to allow storage of merkle-tree");
	}

	public StreamMappedMerkleList(IStreamMappedList<TItem> clusteredList, IItemHasher<TItem> hasher, CHF hashAlgorithm)
		: base(clusteredList, hasher, new ClusteredStorageMerkleTreeStream(clusteredList.Storage, MerkleTreeStreamIndex, hashAlgorithm)) {
	}

	public IClusteredStorage Storage => InternalCollection.Storage;

	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	public bool RequiresLoad => Storage.RequiresLoad;

	public void Load() {
		Storage.Load();
		Guard.Ensure(InternalCollection.Storage.Header.ReservedRecords > 0, "Clustered storage requires at least 1 reserved stream to store merkle-tree");
	}

	public Task LoadAsync() => Storage.LoadAsync();

	public ClusteredStreamScope EnterAddScope(TItem item) {
		InternalMerkleTree.Leafs.Add(ItemHasher.Hash(item));
		return InternalCollection.EnterAddScope(item);
	}

	public ClusteredStreamScope EnterInsertScope(long index, TItem item) {
		InternalMerkleTree.Leafs.Insert(index, ItemHasher.Hash(item));
		return InternalCollection.EnterInsertScope(index, item);
	}

	public ClusteredStreamScope EnterUpdateScope(long index, TItem item) {
		InternalMerkleTree.Leafs.Update(index, ItemHasher.Hash(item));
		return InternalCollection.EnterUpdateScope(index, item);
	}
}
