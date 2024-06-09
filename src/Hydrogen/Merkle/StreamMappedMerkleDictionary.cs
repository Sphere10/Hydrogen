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
/// A dictionary implementation that is both an <see cref="IStreamMappedDictionary{TKey,TValue}"/> and an <see cref="IMerkleDictionary{TKey,TValue}"/>.
/// </summary>
public class StreamMappedMerkleDictionary<TKey, TValue> : DictionaryDecorator<TKey, TValue, IStreamMappedDictionary<TKey, TValue>>, IStreamMappedDictionary<TKey, TValue>, IMerkleDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }

	private readonly MerkleTreeIndex _merkleTreeIndex;

	public StreamMappedMerkleDictionary(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TKey> keySerializer,
		IItemSerializer<TValue> valueSerializer = null,
		IItemChecksummer<TKey> keyChecksummer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		CHF hashAlgorithm = CHF.SHA2_256,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		string merkleTreeIndexName = HydrogenDefaults.DefaultMerkleTreeIndexName,
		string reyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyChecksumIndexName = HydrogenDefaults.DefaultKeyChecksumIndexName,
		Endianness endianness = Endianness.LittleEndian,
		bool readOnly = false,
		bool autoLoad = false,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) : this(
		CreateDictionary(
			rootStream,
			hashAlgorithm,
			keySerializer,
			valueSerializer,
			keyChecksummer,
			keyComparer,
			valueComparer,
			clusterSize,
			policy,
			keyChecksumIndexName,
			reyclableIndexIndexName,
			merkleTreeIndexName,
			endianness,
			readOnly,
			implementation
		),
		merkleTreeIndexName,
		autoLoad
	) {
	}

	internal StreamMappedMerkleDictionary(IStreamMappedDictionary<TKey, TValue> innerDictionary, string merkleTreeIndexName, bool autoLoad = false) 
		: base(innerDictionary) {
		Guard.ArgumentNotNull(innerDictionary, nameof(innerDictionary));
		_merkleTreeIndex = (MerkleTreeIndex)innerDictionary.ObjectStream.Streams.Attachments[merkleTreeIndexName];

		if (autoLoad && RequiresLoad)
			Load();
	}

	public IMerkleTree MerkleTree => _merkleTreeIndex.MerkleTree;

	public ObjectStream ObjectStream => InternalDictionary.ObjectStream;

	public bool RequiresLoad => InternalDictionary.RequiresLoad;

	public void Load() => InternalDictionary.Load();

	public Task LoadAsync() => Task.Run(Load);

	public TKey ReadKey(long index) => InternalDictionary.ReadKey(index);

	public byte[] ReadKeyBytes(long index) => InternalDictionary.ReadKeyBytes(index);

	public TValue ReadValue(long index) => InternalDictionary.ReadValue(index);

	public byte[] ReadValueBytes(long index) => InternalDictionary.ReadValueBytes(index);

	public bool TryFindKey(TKey key, out long index) => InternalDictionary.TryFindKey(key, out index);

	public bool TryFindValue(TKey key, out long index, out TValue value) => InternalDictionary.TryFindValue(key, out index, out value);

	public void RemoveAt(long index) => InternalDictionary.RemoveAt(index);

	public void Dispose() {
		InternalDictionary.Dispose();
	}

	private static IStreamMappedDictionary<TKey, TValue> CreateDictionary(
		Stream stream,
		CHF hashAlgorithm,
		IItemSerializer<TKey> keySerializer,
		IItemSerializer<TValue> valueSerializer,
		IItemChecksummer<TKey> keyChecksum,
		IEqualityComparer<TKey> keyComparer,
		IEqualityComparer<TValue> valueComparer,
		int clusterSize,
		ClusteredStreamsPolicy policy,
		string recyclableIndexIndexName,
		string checksumIndexName,
		string merkleTreeIndexName,
		Endianness endianness,
		bool readOnly,
		StreamMappedDictionaryImplementation implementation
	) {
		var smDict = StreamMappedFactory.CreateDictionary(
			stream,
			keySerializer,
			valueSerializer,
			keyChecksum,
			keyComparer,
			valueComparer,
			clusterSize,
			policy,
			3L,
			recyclableIndexIndexName,
			checksumIndexName,
			endianness,
			readOnly,
			false,
			implementation
		);

		var merkleTreeIndex = new MerkleTreeIndex(
			smDict.ObjectStream,
			merkleTreeIndexName,
			new StreamMappedDictionaryKeyValueHasher(smDict, hashAlgorithm),
			hashAlgorithm
		);
		smDict.ObjectStream.Streams.RegisterAttachment(merkleTreeIndex);

		return smDict;
	}

}