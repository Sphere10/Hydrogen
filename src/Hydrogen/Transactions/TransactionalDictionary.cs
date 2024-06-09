// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

public class TransactionalDictionary<TKey, TValue> : DictionaryDecorator<TKey, TValue, IStreamMappedDictionary<TKey, TValue>>, ITransactionalDictionary<TKey, TValue> {

	public event EventHandlerEx<object> Loading { add => InternalDictionary.Loading += value; remove => InternalDictionary.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalDictionary.Loaded += value; remove => InternalDictionary.Loaded -= value; }
	public event EventHandlerEx Committing { add => _transactionalObject.Committing += value; remove => _transactionalObject.Committing -= value; }
	public event EventHandlerEx Committed { add => _transactionalObject.Committed += value; remove => _transactionalObject.Committed -= value; }
	public event EventHandlerEx RollingBack { add => _transactionalObject.RollingBack += value; remove => _transactionalObject.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => _transactionalObject.RolledBack += value; remove => _transactionalObject.RolledBack -= value; }

	private readonly ITransactionalObject _transactionalObject;

	public TransactionalDictionary(
		HydrogenFileDescriptor fileDescriptor,
		IItemSerializer<TKey> keySerializer = null,
		IItemSerializer<TValue> valueSerializer = null,
		IItemChecksummer<TKey> keyChecksum = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		int reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyIndexName = null,
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
		recyclableIndexIndexName,
		keyIndexName,
		fileDescriptor.Endianness,
		accessMode.IsReadOnly(),
		accessMode.HasFlag(FileAccessMode.AutoLoad),
		implementation
	) {
		InternalDictionary.ObjectStream.Streams.OwnsStream = true;
	}

	public TransactionalDictionary(
		TransactionalStream transactionalStream, 
		IItemSerializer<TKey> keySerializer, 
		IItemSerializer<TValue> valueSerializer, 
		IItemChecksummer<TKey> keyChecksum = null, 
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null, 
		int clusterSize = HydrogenDefaults.ClusterSize,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default, 
		int reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyIndexName = null,
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
			recyclableIndexIndexName,
			keyIndexName,
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
		_transactionalObject.RolledBack += internalDictionary.ObjectStream.Streams.Initialize;

		if (autoLoad && RequiresLoad)
			Load();
	}

	public bool OwnsDictionary { get; set; }

	public ObjectStream ObjectStream => InternalDictionary.ObjectStream;

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
