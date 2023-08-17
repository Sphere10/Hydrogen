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

/// <summary>
/// A list whose items are stored on a file and which has ACID transactional commit/rollback capability as well as built-in memory caching for efficiency. There are
/// no restrictions on this list, items may be added, mutated and removed arbitrarily. This class is essentially a light-weight database.
/// </summary>
/// <typeparam name="T">Type of item</typeparam>
public class TransactionalList<T> : StreamMappedList<T>, ITransactionalList<T> {
	
	public event EventHandlerEx<object> Committing { add => _transactionalObject.Committing += value; remove => _transactionalObject.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _transactionalObject.Committed += value; remove => _transactionalObject.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _transactionalObject.RollingBack += value; remove => _transactionalObject.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _transactionalObject.RolledBack += value; remove => _transactionalObject.RolledBack -= value; }

	private readonly ClusteredStorage _storage;
	private readonly ITransactionalObject _transactionalObject;

	public TransactionalList(
		string filename, 
		string uncommittedPageFileDir, 
		IItemSerializer<T> serializer = null,
		IEqualityComparer<T> comparer = null,
		int transactionalPageSize = HydrogenDefaults.TransactionalPageSize, 
		long maxMemory = HydrogenDefaults.MaxMemoryPerCollection,
		int clusterSize = HydrogenDefaults.ClusterSize, 
		ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, 
		int reservedRecords = 0, 
		long recordKeySize = 0,
		Endianness endianness = Endianness.LittleEndian, 
		bool readOnly = false)
		: this(new TransactionalStream(filename, uncommittedPageFileDir, transactionalPageSize, maxMemory, readOnly, readOnly), 
			  serializer, comparer, clusterSize, policy, reservedRecords, recordKeySize, endianness) {
	}

	public TransactionalList(
		TransactionalStream transactionalStream, 
		IItemSerializer<T> serializer = null, 
		IEqualityComparer<T> comparer = null, 
		int clusterSize = HydrogenDefaults.ClusterSize, 
		ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, 
		int reservedRecords = 0,
		long recordKeySize = 0,
		Endianness endianness = HydrogenDefaults.Endianness) 
		: this(new ClusteredStorage(transactionalStream, clusterSize, policy, recordKeySize, reservedRecords, endianness), transactionalStream, serializer, comparer) {
	}

	public TransactionalList(ClusteredStorage storage, ITransactionalObject transactionalObject, IItemSerializer<T> serializer = null, IEqualityComparer<T> comparer = null)
		: base(storage, serializer, comparer) {
		Guard.ArgumentNotNull(storage, nameof(storage));
		Guard.ArgumentNotNull(transactionalObject, nameof(transactionalObject));
		_storage = storage;
		_transactionalObject = transactionalObject;
	}
	
	public virtual void Commit() => _transactionalObject.Commit();

	public virtual Task CommitAsync() => _transactionalObject.CommitAsync();

	public virtual void Rollback() => _transactionalObject.Rollback();

	public virtual Task RollbackAsync() => _transactionalObject.RollbackAsync();

	public virtual void Dispose() {
		using (_storage.EnterAccessScope()) 
			_storage.RootStream.Dispose();
	}

}
