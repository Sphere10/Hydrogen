// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalDictionary<TKey, TValue> : StreamMappedDictionary<TKey, TValue>, ITransactionalDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Committing { add => _kvpStore.Committing += value; remove => _kvpStore.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _kvpStore.Committed += value; remove => _kvpStore.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _kvpStore.RollingBack += value; remove => _kvpStore.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _kvpStore.RolledBack += value; remove => _kvpStore.RolledBack -= value; }

	private readonly ITransactionalList<KeyValuePair<TKey, TValue>> _kvpStore;

	public TransactionalDictionary(string filename, string uncommittedPageFileDir, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null,
								   IEqualityComparer<TValue> valueComparer = null, int transactionalPageSize = HydrogenDefaults.TransactionalPageSize, long maxMemory = HydrogenDefaults.MaxMemoryPerCollection, int clusterSize = HydrogenDefaults.ClusterSize,
								   ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness, bool readOnly = false)
		: this(
			new TransactionalStream(filename, uncommittedPageFileDir, transactionalPageSize, maxMemory, readOnly),
			keySerializer,
			valueSerializer,
			keyChecksum,
			keyComparer,
			valueComparer, 
			clusterSize, 
			policy, 
			reservedRecords, 
			endianness
		) {
	}

	public TransactionalDictionary(TransactionalStream transactionalStream, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IItemChecksummer<TKey> keyChecksum = null, IEqualityComparer<TKey> keyComparer = null,
	                               IEqualityComparer<TValue> valueComparer = null, int clusterSize = HydrogenDefaults.ClusterSize,
	                               ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, int reservedRecords = 0, Endianness endianness = HydrogenDefaults.Endianness)
		: this(
			new TransactionalList<KeyValuePair<TKey, TValue>>(
				transactionalStream,
				new KeyValuePairSerializer<TKey, TValue>(keySerializer, valueSerializer),
				new KeyValuePairEqualityComparer<TKey, TValue>(keyComparer, valueComparer),
				clusterSize,
				policy, 
				reservedRecords, 
				0,   // Key's are stored with values, no need to reserve space for them in record header
				endianness
			),
			keySerializer,
			keyChecksum,
			keyComparer,
			valueComparer,
			endianness
		) {
	}

	public TransactionalDictionary(ITransactionalList<KeyValuePair<TKey, TValue>> kvpStore, IItemSerializer<TKey> keySerializer, IItemChecksummer<TKey> keyChecksummer = null, IEqualityComparer<TKey> keyComparer = null,
	                               IEqualityComparer<TValue> valueComparer = null, Endianness endianness = Endianness.LittleEndian)
		: base(kvpStore, keySerializer, keyChecksummer, keyComparer, valueComparer, endianness) {
		Guard.ArgumentNotNull(kvpStore, nameof(kvpStore));
		Guard.ArgumentNotNull(keySerializer, nameof(keySerializer));
		
		_kvpStore = kvpStore;
	}

	public void Commit() => _kvpStore.Commit();

	public Task CommitAsync() => _kvpStore.CommitAsync();

	public void Rollback() => _kvpStore.Rollback();

	public Task RollbackAsync() => _kvpStore.RollbackAsync();

	public void Dispose() => _kvpStore.Dispose();
}
