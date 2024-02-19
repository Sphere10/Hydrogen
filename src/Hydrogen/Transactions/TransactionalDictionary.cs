// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;

namespace Hydrogen;

public class TransactionalDictionary<TKey, TValue> : DictionaryDecorator<TKey, TValue, IStreamMappedDictionary<TKey, TValue>>, ITransactionalDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }
	public event EventHandlerEx<object> Committing { add => _transactionalObject.Committing += value; remove => _transactionalObject.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _transactionalObject.Committed += value; remove => _transactionalObject.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _transactionalObject.RollingBack += value; remove => _transactionalObject.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _transactionalObject.RolledBack += value; remove => _transactionalObject.RolledBack -= value; }

	private readonly ITransactionalObject _transactionalObject;

	public TransactionalDictionary(
		HydrogenFileDescriptor fileDescriptor,
		IItemSerializer<TKey> keySerializer = null,
		IItemSerializer<TValue> valueSerializer = null,
		IItemChecksummer<TKey> keyChecksum = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		int reservedStreamCount = 2,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1,
		Endianness endianness = HydrogenDefaults.Endianness,
		FileAccessMode accessMode = FileAccessMode.Default,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) : this(
		new TransactionalStream(fileDescriptor, accessMode),
		keySerializer,
		valueSerializer,
		keyChecksum,
		keyComparer,
		valueComparer, 
		fileDescriptor.ClusterSize, 
		fileDescriptor.ContainerPolicy,
		reservedStreamCount,
		freeIndexStoreStreamIndex,
		keyChecksumIndexStreamIndex,
		endianness,
		accessMode.IsReadOnly(),
		accessMode.HasFlag(FileAccessMode.AutoLoad),
		implementation
	) {
		InternalDictionary.ObjectContainer.Streams.OwnsStream = true;
	}

	public TransactionalDictionary(
		TransactionalStream transactionalStream, 
		IItemSerializer<TKey> keySerializer, 
		IItemSerializer<TValue> valueSerializer, 
		IItemChecksummer<TKey> keyChecksum = null, 
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null, 
		int clusterSize = HydrogenDefaults.ClusterSize,
		StreamContainerPolicy policy = StreamContainerPolicy.Default, 
		int reservedStreamCount = 2,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1,
		Endianness endianness = HydrogenDefaults.Endianness,
		bool readOnly = false,
		bool autoLoad = false,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) : this(
		StreamMappedFactory.CreateDictionary(
			transactionalStream, 
			keySerializer, 
			valueSerializer, 
			keyChecksum,
			keyComparer, 
			valueComparer,
			clusterSize, 
			policy,
			reservedStreamCount,
			freeIndexStoreStreamIndex,
			keyChecksumIndexStreamIndex,
			endianness, 
			readOnly,
			false,
			implementation
		),
		transactionalStream,
		autoLoad
	) {
		OwnsDictionary = true;
	}

	public TransactionalDictionary(
		IStreamMappedDictionary<TKey, TValue> internalDictionary,
		ITransactionalObject transactionalObject,
		bool autoLoad = false
	) : base(internalDictionary) {
		Guard.ArgumentNotNull(transactionalObject, nameof(transactionalObject));
		_transactionalObject = transactionalObject;
		_transactionalObject.RolledBack += _ => {
			internalDictionary.ObjectContainer.Streams.Initialize();
		};

		if (autoLoad && RequiresLoad)
			Load();
	}

	public bool OwnsDictionary { get; set; }

	public ObjectContainer ObjectContainer => InternalDictionary.ObjectContainer;

	public bool RequiresLoad => InternalDictionary.RequiresLoad;

	public void Commit() => _transactionalObject.Commit();

	public Task CommitAsync() => _transactionalObject.CommitAsync();

	public void Rollback() => _transactionalObject.Rollback();

	public Task RollbackAsync() => _transactionalObject.RollbackAsync();

	public void Dispose() {
		if (OwnsDictionary)
			InternalDictionary.Dispose();
	}

	public void Load() => InternalDictionary.Load();

	public Task LoadAsync() => InternalDictionary.LoadAsync();

	public TKey ReadKey(long index) => InternalDictionary.ReadKey(index);

	public byte[] ReadKeyBytes(long index) => InternalDictionary.ReadKeyBytes(index);

	public TValue ReadValue(long index) => InternalDictionary.ReadValue(index);

	public byte[] ReadValueBytes(long index) => InternalDictionary.ReadValueBytes(index);

	public bool TryFindKey(TKey key, out long index) => InternalDictionary.TryFindKey(key, out index);

	public bool TryFindValue(TKey key, out long index, out TValue value) => InternalDictionary.TryFindValue(key, out index, out value);

	public void RemoveAt(long index) => InternalDictionary.RemoveAt(index);

}
