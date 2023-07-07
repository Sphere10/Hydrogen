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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hydrogen;

namespace Hydrogen {

	/// <summary>
	/// A set implementation that is both an <see cref="IStreamMappedHashSet{TItem}"/> and an <see cref="IMerkleSet{TItem}"/>.
	/// </summary>
	public class StreamMappedMerkleHashSet<TItem> : SetDecorator<TItem, StreamMappedHashSet<TItem>>, IStreamMappedHashSet<TItem>, IMerkleSet<TItem> {
		public event EventHandlerEx<object> Loading { add => InternalSet.Loading += value; remove => InternalSet.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => InternalSet.Loaded += value; remove => InternalSet.Loaded -= value; }

		public StreamMappedMerkleHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, CHF hashAlgorithm = CHF.SHA2_256, IEqualityComparer<TItem> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
			: this(rootStream, clusterSize, serializer, new ItemDigestor<TItem>(hashAlgorithm, serializer, endianness), hashAlgorithm, comparer, policy, reservedRecords, endianness) {
		}

		public StreamMappedMerkleHashSet(Stream rootStream, int clusterSize, IItemSerializer<TItem> serializer, IItemHasher<TItem> hasher, CHF hashAlgorithm, IEqualityComparer<TItem> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
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

		public bool RequiresLoad => InternalSet.RequiresLoad;

		public IClusteredStorage Storage => InternalSet.Storage;

		public IMerkleTree MerkleTree => ((IMerkleObject)InternalSet.InternalDictionary).MerkleTree;
		
		public void Load() => InternalSet.Load();

		public Task LoadAsync() => InternalSet.LoadAsync();
	}
}
