using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Hydrogen;

namespace Hydrogen {

	/// <summary>
	/// A set implementation that is both an <see cref="IStreamMappedHashSet{TItem}"/> and an <see cref="IMerkleSet{TItem}"/>.
	/// </summary>
	public class StreamMappedMerkleHashSet<TItem> : SetDecorator<TItem, StreamMappedHashSet<TItem>>, IStreamMappedHashSet<TItem>, IMerkleSet<TItem> {

		public StreamMappedMerkleHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, CHF hashAlgorithm = CHF.SHA2_256, IEqualityComparer<TItem> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
			: this(rootStream, clusterSize, serializer, new ItemHasher<TItem>(hashAlgorithm, serializer, endianness), hashAlgorithm, comparer, policy, reservedRecords, endianness) {
		}

		public StreamMappedMerkleHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, IItemHasher<TItem> hasher, CHF hashAlgorithm, IEqualityComparer<TItem> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
			: this(
				new StreamMappedMerkleDictionary<byte[], TItem>(
					rootStream,
					clusterSize,
					new StaticSizeByteArraySerializer(hasher.DigestLength),
					hashAlgorithm,
					serializer,
					new HashChecksum(),
					new ByteArrayEqualityComparer(),
					comparer,
					policy,
					reservedRecords,
					endianness
				),
				comparer,
				hasher
			) {
			Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in clustered dictionary implementations.");
		}

		public StreamMappedMerkleHashSet(StreamMappedMerkleDictionary<byte[], TItem> internalDictionary, IEqualityComparer<TItem> comparer, IItemHasher<TItem> hasher)
			: this(new StreamMappedHashSet<TItem>(internalDictionary, comparer, hasher)) {
		}

		public StreamMappedMerkleHashSet(StreamMappedHashSet<TItem> internalSet)
			: base(internalSet) {
			Guard.ArgumentCast<IMerkleObject>(internalSet.InternalDictionary, out _, nameof(internalSet.InternalDictionary), "Internal dictionary is not merkleized");
		}

		public IClusteredStorage Storage => InternalSet.Storage;

		public IMerkleTree MerkleTree => ((IMerkleObject)InternalSet.InternalDictionary).MerkleTree;
	}
}
