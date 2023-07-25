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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A set implementation of <see cref="IStreamMappedMerkleHashSet{T}"/>
/// </summary>
public class StreamMappedMerkleHashSet<TItem, TInner> : SetDecorator<TItem, TInner>, IStreamMappedMerkleHashSet<TItem> 
	where TInner : IStreamMappedHashSet<TItem> {

	public event EventHandlerEx<object> Loading { add => InternalSet.Loading += value; remove => InternalSet.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalSet.Loaded += value; remove => InternalSet.Loaded -= value; }

	public StreamMappedMerkleHashSet(TInner internalSet, IMerkleTree merkleTreeImpl)
		: base(internalSet) {
		MerkleTree = merkleTreeImpl;
	}

	public bool RequiresLoad => InternalSet.RequiresLoad;

	public IClusteredStorage Storage => InternalSet.Storage;

	public IMerkleTree MerkleTree { get; }

	public void Load() => InternalSet.Load();

	public Task LoadAsync() => InternalSet.LoadAsync();
}

public class StreamMappedMerkleHashSet<TItem> : StreamMappedMerkleHashSet<TItem, IStreamMappedHashSet<TItem>> {

	public StreamMappedMerkleHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, CHF hashAlgorithm = CHF.SHA2_256, IEqualityComparer<TItem> comparer = null,
	                                 ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, int merkleTreeStreamIndex = HydrogenDefaults.ClusteredStorageMerkleTreeStreamIndex,
									 Endianness endianness = Endianness.LittleEndian)
		: this(rootStream, clusterSize, serializer, new ItemDigestor<TItem>(hashAlgorithm, serializer, endianness), hashAlgorithm, comparer, policy, reservedRecords, merkleTreeStreamIndex, endianness) {
	}

	public StreamMappedMerkleHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, IItemHasher<TItem> hasher, CHF hashAlgorithm, IEqualityComparer<TItem> comparer = null,
	                                 ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, int merkleTreeStreamIndex = HydrogenDefaults.ClusteredStorageMerkleTreeStreamIndex,
									 Endianness endianness = Endianness.LittleEndian)
		: this(
			new StreamMappedMerkleDictionary<byte[], TItem>(
				rootStream,
				clusterSize,
				new StaticSizeByteArraySerializer(hasher.DigestLength),
				hashAlgorithm,
				serializer,
				new HashChecksummer(),
				new ByteArrayEqualityComparer(),
				comparer,
				policy,
				reservedRecords,
				merkleTreeStreamIndex,
				endianness
			),
			comparer,
			hasher
		) {
		Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in clustered dictionary implementations.");
	}

	public StreamMappedMerkleHashSet(StreamMappedMerkleDictionary<byte[], TItem> internalDictionary, IEqualityComparer<TItem> comparer, IItemHasher<TItem> hasher)
		: this(new StreamMappedHashSet<TItem>(internalDictionary, comparer, hasher), internalDictionary.MerkleTree) {
	}

	public StreamMappedMerkleHashSet(StreamMappedHashSet<TItem> internalSet, IMerkleTree merkleTree)
		: base(internalSet, merkleTree) {
	}

}
