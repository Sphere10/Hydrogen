using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A dictionary implementation that is both an <see cref="IStreamMappedDictionary{TKey,TValue}"/> and an <see cref="IMerkleDictionary{TKey,TValue}"/>.
	/// </summary>
	public class StreamMappedMerkleDictionary<TKey, TValue> : DictionaryDecorator<TKey, TValue, IStreamMappedDictionary<TKey, TValue>>, IStreamMappedDictionary<TKey, TValue>, IMerkleDictionary<TKey, TValue> {
		
		public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }

		/// <summary>
		/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionary{TKey,TValue}"/> under the hood.
		/// </summary>
		public StreamMappedMerkleDictionary(Stream rootStream, int clusterSize, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
			: this(
				new StreamMappedMerkleList<KeyValuePair<TKey, TValue>>(
					rootStream,
					clusterSize,
					hashAlgorithm,
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
					endianness
				),
				keyChecksum,
				keyComparer,
				valueComparer
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionary<TKey, TValue>)} implementations.");
		}

		/// <summary>
		/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionarySK{TKey,TValue}"/> under the hood.
		/// </summary>
		public StreamMappedMerkleDictionary(Stream rootStream, int clusterSize, IItemSerializer<TKey> staticSizedKeySerializer, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TValue> valueSerializer = null, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
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
					endianness
				),
				staticSizedKeySerializer,
				valueSerializer,
				keyChecksum,
				keyComparer,
				valueComparer
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionary<TKey, TValue>)} implementations.");
		}

		/// <summary>
		/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionary{TKey,TValue}"/> under the hood.
		/// </summary>
		public StreamMappedMerkleDictionary(StreamMappedMerkleList<KeyValuePair<TKey, TValue>> merkleizedKvpStore, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null)
			: this(
				  new StreamMappedDictionary<TKey, TValue>(
					  merkleizedKvpStore,
					  keyChecksum, 
					  keyComparer, 
					  valueComparer
				  ),
				  merkleizedKvpStore.MerkleTree
			) {
		}

		/// <summary>
		/// Constructs using an <see cref="StreamMappedMerkleDictionary{TKey,TValue}"/> using an <see cref="StreamMappedDictionarySK{TKey,TValue}"/> under the hood.
		/// </summary>
		public StreamMappedMerkleDictionary(StreamMappedMerkleList<TValue> merkleizedKvpStore, IItemSerializer<TKey> staticSizedKeySerializer, IItemSerializer<TValue> valueSerializer = null, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null)
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

		protected StreamMappedMerkleDictionary(IStreamMappedDictionary<TKey, TValue> innerDictionary, IMerkleTree dictionaryMerkleTree ) 
			: base(innerDictionary) {
			Guard.ArgumentNotNull(innerDictionary, nameof(innerDictionary));
			Guard.ArgumentNotNull(dictionaryMerkleTree, nameof(dictionaryMerkleTree));
			MerkleTree = dictionaryMerkleTree;
		}

		public IMerkleTree MerkleTree { get; }

		public IClusteredStorage Storage => InternalDictionary.Storage;

		public bool RequiresLoad => InternalDictionary.RequiresLoad;

		public void Load() => InternalDictionary.Load();

		public TKey ReadKey(int index) => InternalDictionary.ReadKey(index);

		public TValue ReadValue(int index) => InternalDictionary.ReadValue(index);

		public bool TryFindKey(TKey key, out int index) => InternalDictionary.TryFindKey(key, out index);

		public bool TryFindValue(TKey key, out int index, out TValue value) => InternalDictionary.TryFindValue(key, out index, out value);

		public void RemoveAt(int index) => InternalDictionary.RemoveAt(index);
	}
}
