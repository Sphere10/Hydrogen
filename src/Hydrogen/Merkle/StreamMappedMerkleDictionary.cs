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
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Hydrogen.ObjectSpace.Index;

namespace Hydrogen;

/// <summary>
/// A dictionary implementation that is both an <see cref="IStreamMappedDictionary{TKey,TValue}"/> and an <see cref="IMerkleDictionary{TKey,TValue}"/>.
/// </summary>
public class StreamMappedMerkleDictionary<TKey, TValue> : DictionaryDecorator<TKey, TValue, IStreamMappedDictionary<TKey, TValue>>, IStreamMappedDictionary<TKey, TValue>, IMerkleDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }

	private readonly ObjectContainerMerkleTree _merkleTreeIndex;

	public StreamMappedMerkleDictionary(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TKey> keySerializer,
		IItemSerializer<TValue> valueSerializer = null,
		IItemChecksummer<TKey> keyChecksummer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		CHF hashAlgorithm = CHF.SHA2_256,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 3,
		long merkleTreeStreamIndex = 0,
		long freeIndexStoreStreamIndex = 1,
		long keyChecksumIndexStreamIndex = 2,
		Endianness endianness = Endianness.LittleEndian,
		bool readOnly = false,
		bool autoLoad = false,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) : this(
		StreamMappedDictionaryFactory.Create(
			rootStream,
			keySerializer,
			valueSerializer,
			keyChecksummer,
			keyComparer,
			valueComparer,
			clusterSize,
			policy,
			reservedStreams,
			keyChecksumIndexStreamIndex,
			freeIndexStoreStreamIndex,
			endianness,
			readOnly,
			false,
			implementation
		),
		hashAlgorithm,
		autoLoad,
		merkleTreeStreamIndex
	) {
	}


	public StreamMappedMerkleDictionary(
		ObjectContainer objectContainer,
		IItemSerializer<TKey> keySerializer,
		IItemChecksummer<TKey> keyChecksum = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		CHF hashAlgorithm = CHF.SHA2_256,
		bool autoLoad = false,
		long merkleTreeStreamIndex = 0,
		long freeIndexStoreStreamIndex = 1,
		long keyChecksumIndexStreamIndex = 2,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) : this(
		StreamMappedDictionaryFactory.Create(
			objectContainer,
			keySerializer,
			keyComparer,
			valueComparer,
			keyChecksum,
			false,
			freeIndexStoreStreamIndex,
			keyChecksumIndexStreamIndex,
			implementation
		),
		hashAlgorithm,
		autoLoad,
		merkleTreeStreamIndex
	) {
	}

	public StreamMappedMerkleDictionary(
		IStreamMappedDictionary<TKey, TValue> innerDictionary,
		CHF hashAlgorithm,
		bool autoLoad = false,
		long merkleTreeStreamIndex = 0
	) : base(innerDictionary) {
		Guard.ArgumentNotNull(innerDictionary, nameof(innerDictionary));
		_merkleTreeIndex = new ObjectContainerMerkleTree(
			innerDictionary.ObjectContainer,
			DigestItem,
			hashAlgorithm,
			merkleTreeStreamIndex,
			0
		);

		if (autoLoad)
			Load();
	}

	public IMerkleTree MerkleTree => _merkleTreeIndex.MerkleTree;

	public ObjectContainer ObjectContainer => InternalDictionary.ObjectContainer;

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
		_merkleTreeIndex.Dispose();
		InternalDictionary.Dispose();
	}

	private byte[] DigestItem(long index) {
		var chf = MerkleTree.HashAlgorithm;
		var descriptor = ObjectContainer.GetItemDescriptor(index);
		if (descriptor.Traits.HasFlag(ClusteredStreamTraits.Reaped))
			return Hashers.ZeroHash(chf);

		var keyBytes = InternalDictionary.ReadKeyBytes(index);
		var keyDigest = Hashers.HashWithNullSupport(chf, keyBytes);
		var valueBytes = InternalDictionary.ReadValueBytes(index);
		var valueDigest = Hashers.HashWithNullSupport(chf, valueBytes);
		return Hashers.JoinHash(MerkleTree.HashAlgorithm, keyDigest, valueDigest);
	}

}
