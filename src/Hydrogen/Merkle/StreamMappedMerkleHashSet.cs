﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A set implementation of <see cref="IStreamMappedHashSet{TItem}" and <see cref="IMerkleSet{TItem}"/>. />
/// </summary>
public class StreamMappedMerkleHashSet<TItem, TInner> : SetDecorator<TItem, TInner>, IStreamMappedHashSet<TItem>, IMerkleSet<TItem>
	where TInner : IStreamMappedHashSet<TItem> {

	public event EventHandlerEx<object> Loading { add => InternalSet.Loading += value; remove => InternalSet.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalSet.Loaded += value; remove => InternalSet.Loaded -= value; }

	public StreamMappedMerkleHashSet(TInner internalSet, IMerkleTree merkleTreeImpl)
		: base(internalSet) {
		MerkleTree = merkleTreeImpl;
	}

	public bool RequiresLoad => InternalSet.RequiresLoad;

	public ObjectContainer ObjectContainer => InternalSet.ObjectContainer;

	public IMerkleTree MerkleTree { get; }

	public void Load() => InternalSet.Load();

	public Task LoadAsync() => InternalSet.LoadAsync();

	public void Dispose() => InternalSet.Dispose();
}

/// <inheritdoc />
public class StreamMappedMerkleHashSet<TItem> : StreamMappedMerkleHashSet<TItem, IStreamMappedHashSet<TItem>> {

	public StreamMappedMerkleHashSet(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		CHF hashAlgorithm = CHF.SHA2_256,
		IEqualityComparer<TItem> comparer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long merkleTreeStreamIndex = 0,
		long freeIndexStoreStreamIndex = 1,
		long keyChecksumIndexStreamIndex = 2,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false
	) : this(
		rootStream,
		clusterSize,
		serializer,
		new ItemDigestor<TItem>(hashAlgorithm, serializer, endianness),
		hashAlgorithm,
		comparer,
		policy,
		merkleTreeStreamIndex,
		freeIndexStoreStreamIndex,
		keyChecksumIndexStreamIndex,
		endianness,
		autoLoad
	) {
	}

	public StreamMappedMerkleHashSet(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		IItemHasher<TItem> hasher,
		CHF hashAlgorithm,
		IEqualityComparer<TItem> comparer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long merkleTreeStreamIndex = 0,
		long freeIndexStoreStreamIndex = 1,
		long keyChecksumIndexStreamIndex = 2,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false
	) : this(
			new StreamMappedMerkleDictionary<byte[], TItem>(
				rootStream,
				clusterSize,
				new ConstantSizeByteArraySerializer(hasher.DigestLength),
				serializer,
				new HashChecksummer(),
				ByteArrayEqualityComparer.Instance,
				comparer,
				hashAlgorithm,
				policy,
				merkleTreeStreamIndex,
				freeIndexStoreStreamIndex,
				keyChecksumIndexStreamIndex,
				endianness,
				autoLoad
			),
			comparer,
			hasher
		) {
	}

	public StreamMappedMerkleHashSet(
		StreamMappedMerkleDictionary<byte[], TItem> internalDictionary,
		IEqualityComparer<TItem> comparer,
		IItemHasher<TItem> hasher
	) : this(new StreamMappedHashSet<TItem>(internalDictionary, comparer, hasher), internalDictionary.MerkleTree) {
	}

	public StreamMappedMerkleHashSet(
		IStreamMappedHashSet<TItem> internalSet,
		IMerkleTree merkleTree
	) : base(internalSet, merkleTree) {
	}

}
