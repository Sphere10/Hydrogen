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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A dictionary implementation that is both an <see cref="IStreamMappedDictionary{TKey,TValue}"/> and an <see cref="IMerkleDictionary{TKey,TValue}"/>.
/// </summary>
public class StreamMappedMerkleDictionary<TKey, TValue, TInner> : DictionaryDecorator<TKey, TValue, TInner>, IStreamMappedDictionary<TKey, TValue>, IMerkleDictionary<TKey, TValue> 
	where TInner : IStreamMappedDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }

	public StreamMappedMerkleDictionary(TInner innerDictionary, IMerkleTree merkleTree)
		: base(innerDictionary) {
		Guard.ArgumentNotNull(innerDictionary, nameof(innerDictionary));
		Guard.ArgumentNotNull(merkleTree, nameof(merkleTree));
		MerkleTree = merkleTree;
	}

	public IMerkleTree MerkleTree { get; }

	public ClusteredStorage Storage => InternalDictionary.Storage;

	public bool RequiresLoad => InternalDictionary.RequiresLoad;

	public void Load() => InternalDictionary.Load();

	public Task LoadAsync() => Task.Run(Load);

	public TKey ReadKey(int index) => InternalDictionary.ReadKey(index);

	public TValue ReadValue(int index) => InternalDictionary.ReadValue(index);

	public bool TryFindKey(TKey key, out int index) => InternalDictionary.TryFindKey(key, out index);

	public bool TryFindValue(TKey key, out int index, out TValue value) => InternalDictionary.TryFindValue(key, out index, out value);

	public void RemoveAt(int index) => InternalDictionary.RemoveAt(index);
}

/// <inheritdoc />
public class StreamMappedMerkleDictionary<TKey, TValue> : StreamMappedMerkleDictionary<TKey, TValue, IStreamMappedDictionary<TKey, TValue>> {

	/// <summary>
	/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionary{TKey,TValue}"/> under the hood.
	/// </summary>
	public StreamMappedMerkleDictionary(Stream rootStream, int clusterSize, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, IItemChecksummer<TKey> keyChecksummer = null,
	                                    IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1,
	                                    int merkleTreeStreamIndex = HydrogenDefaults.ClusteredStorageMerkleTreeStreamIndex, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(
			new StreamMappedMerkleList<KeyValuePair<TKey, TValue>>(
				rootStream,
				clusterSize,
				hashAlgorithm,
				new ProjectionHasher<KeyValuePair<TKey, TValue>, TValue>(kvp => kvp.Value, new ItemDigestor<TValue>(hashAlgorithm, valueSerializer, endianness).WithNullHash(hashAlgorithm)),
				new KeyValuePairSerializer<TKey, TValue>(
					keySerializer,
					valueSerializer
				),
				new KeyValuePairEqualityComparer<TKey, TValue>(
					keyComparer,
					valueComparer
				),
				policy,
				0,
				reservedRecords,
				merkleTreeStreamIndex,
				endianness,
				autoLoad
			),
			keyChecksummer,
			keyComparer,
			valueComparer
		) {
		Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionary<TKey, TValue>)} implementations.");
	}

	/// <summary>
	/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionarySK{TKey,TValue}"/> under the hood.
	/// </summary>
	public StreamMappedMerkleDictionary(Stream rootStream, int clusterSize, IItemSerializer<TKey> staticSizedKeySerializer, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TValue> valueSerializer = null,
	                                    IItemChecksummer<TKey> keyChecksummer = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null,
	                                    ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, long reservedRecords = 1, int merkleTreeStreamIndex = HydrogenDefaults.ClusteredStorageMerkleTreeStreamIndex, 
										Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(
			new StreamMappedMerkleList<TValue>(
				rootStream,
				clusterSize,
				hashAlgorithm,
				valueSerializer,
				valueComparer,
				policy | ClusteredStoragePolicy.TrackKey,
				staticSizedKeySerializer.StaticSize,
				reservedRecords,
				merkleTreeStreamIndex,
				endianness,
				autoLoad
			),
			staticSizedKeySerializer,
			valueSerializer,
			keyChecksummer,
			keyComparer,
			valueComparer
		) {
		Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionary<TKey, TValue>)} implementations.");
	}

	/// <summary>
	/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionary{TKey,TValue}"/> under the hood.
	/// </summary>
	public StreamMappedMerkleDictionary(StreamMappedMerkleList<KeyValuePair<TKey, TValue>> merkleizedKvpStore, IItemChecksummer<TKey> keyChecksummer = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null,
	                                     Endianness endianness = Endianness.LittleEndian, bool autoLoad = false)
		: this(
			new StreamMappedDictionary<TKey, TValue>(
				merkleizedKvpStore,
				((KeyValuePairSerializer<TKey, TValue>)merkleizedKvpStore.ItemSerializer).KeySerializer,
				keyChecksummer,
				keyComparer,
				valueComparer,
				endianness,
				autoLoad
			),
			merkleizedKvpStore.MerkleTree
		) {
	}

	/// <summary>
	/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionarySK{TKey,TValue}"/> under the hood.
	/// </summary>
	public StreamMappedMerkleDictionary(StreamMappedMerkleList<TValue> merkleizedKvpStore, IItemSerializer<TKey> staticSizedKeySerializer, IItemSerializer<TValue> valueSerializer = null, IItemChecksummer<TKey> keyChecksum = null,
	                                    IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null)
		: this(
			new StreamMappedDictionarySK<TKey, TValue>(
				merkleizedKvpStore,
				staticSizedKeySerializer,
				valueSerializer,
				keyChecksum,
				keyComparer,
				valueComparer
			),
			merkleizedKvpStore.MerkleTree
		) {
	}

	protected StreamMappedMerkleDictionary(IStreamMappedDictionary<TKey, TValue> innerDictionary, IMerkleTree dictionaryMerkleTree)
		: base(innerDictionary, dictionaryMerkleTree) {
	}

}
