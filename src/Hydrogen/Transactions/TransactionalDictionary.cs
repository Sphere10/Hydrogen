// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <inheritdoc />
public class TransactionalDictionary<TKey, TValue> : TransactionalDictionaryBase<TKey, TValue> {

	/// <inheritdoc />
	public TransactionalDictionary(string filename, string uncommittedPageFileDir, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null,
	                               IEqualityComparer<TValue> valueComparer = null, int transactionalPageSizeBytes = DefaultTransactionalPageSize, long maxMemory = DefaultMaxMemory, int clusterSize = DefaultClusterSize,
	                               ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = Endianness.LittleEndian, bool readOnly = false)
		: base(
			filename,
			uncommittedPageFileDir,
			b => CreateStreamMappedDictionary(b, keySerializer, valueSerializer, keyChecksum, keyComparer, valueComparer, clusterSize, policy, reservedRecords, endianness),
			transactionalPageSizeBytes,
			maxMemory,
			readOnly
		) {
	}

	protected static IStreamMappedDictionary<TKey, TValue> CreateStreamMappedDictionary(IBuffer buffer, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksum,
	                                                                                    IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, int clusterSize, ClusteredStoragePolicy policy, int reservedRecords,
	                                                                                    Endianness endianness)
		=> new StreamMappedDictionary<TKey, TValue>(
			new ExtendedMemoryStream(
				buffer,
				disposeSource: true
			),
			clusterSize,
			keySerializer,
			valueSerializer,
			keyChecksum,
			keyComparer,
			valueComparer,
			policy,
			reservedRecords,
			endianness
		);
}
