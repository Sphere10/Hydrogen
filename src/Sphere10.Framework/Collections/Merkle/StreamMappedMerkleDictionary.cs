using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A merkleized <see cref="IStreamMappedDictionary"/>.
	/// </summary>
	public class StreamMappedMerkleDictionary<TKey, TValue> : DictionaryDecorator<TKey, TValue, IStreamMappedDictionary<TKey, TValue>>, IMerkleDictionary<TKey, TValue> {

		public StreamMappedMerkleDictionary(Stream rootStream, int clusterSize, CHF hashAlgorithm, IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = Endianness.LittleEndian)
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
					endianness
				),
				keyChecksum,
				keyComparer,
				valueComparer
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in {nameof(StreamMappedDictionary<TKey, TValue>)} implementations.");
		}

		public StreamMappedMerkleDictionary(StreamMappedMerkleList<KeyValuePair<TKey, TValue>> merkleizedKvpStore, IItemChecksum<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null)
			: this(new StreamMappedDictionary<TKey, TValue>(merkleizedKvpStore, keyChecksum, keyComparer, valueComparer)) {
		}

		public StreamMappedMerkleDictionary(IStreamMappedDictionary<TKey, TValue> innerDictionary) 
			: base(innerDictionary) {
			Guard.ArgumentNotNull(innerDictionary, nameof(innerDictionary));
			Guard.ArgumentCast<IMerkleObject>(innerDictionary.InternalList, out var merkleCollection, nameof(innerDictionary.InternalList), "Must use a merkleized list for storing items");
			MerkleTree = merkleCollection.MerkleTree;
		}

		public IMerkleTree MerkleTree { get; }
	}
}
