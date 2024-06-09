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

/// <summary>
/// A list whose items are stored on a file and which has ACID transactional commit/rollback capability as well as built-in memory caching for efficiency. There are
/// no restrictions on this list, items may be added, mutated and removed arbitrarily. This class is essentially a light-weight database.
/// </summary>
/// <typeparam name="T">Type of item</typeparam>
public class TransactionalList<T> : ExtendedListDecorator<T, IStreamMappedList<T>>, ITransactionalList<T> {
	public event EventHandlerEx<object> Loading { add => InternalCollection.Loading += value; remove => InternalCollection.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InternalCollection.Loaded += value; remove => InternalCollection.Loaded -= value; }
	public event EventHandlerEx Committing { add => _transactionalObject.Committing += value; remove => _transactionalObject.Committing -= value; }
	public event EventHandlerEx Committed { add => _transactionalObject.Committed += value; remove => _transactionalObject.Committed -= value; }
	public event EventHandlerEx RollingBack { add => _transactionalObject.RollingBack += value; remove => _transactionalObject.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => _transactionalObject.RolledBack += value; remove => _transactionalObject.RolledBack -= value; }

	private readonly ITransactionalObject _transactionalObject;

	public TransactionalList(
		HydrogenFileDescriptor fileDescriptor, 
		IItemSerializer<T> serializer = null,
		IEqualityComparer<T> comparer = null,
		IItemChecksummer<T> itemChecksummer = null,
		long reservedStreams = 0,
		string optionalItemChecksumIndexName = null,
		FileAccessMode accessMode = FileAccessMode.Default
	) : this( 
			new TransactionalStream(
				fileDescriptor, 
				accessMode.WithoutAutoLoad()
			),
			serializer, 
			comparer, 
			itemChecksummer,
			fileDescriptor.ClusterSize, 
			fileDescriptor.ContainerPolicy, 
			reservedStreams,
			optionalItemChecksumIndexName,
			fileDescriptor.Endianness,
			accessMode.HasFlag(FileAccessMode.AutoLoad)
		) {
		InternalCollection.ObjectStream.Streams.OwnsStream = true;
	}

	public TransactionalList(
		TransactionalStream transactionalStream, 
		IItemSerializer<T> serializer = null, 
		IEqualityComparer<T> comparer = null, 
		IItemChecksummer<T> itemChecksummer = null,
		int clusterSize = HydrogenDefaults.ClusterSize, 
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default, 
		long reservedStreams = 0,
		string optionalItemChecksumIndexName = null,
		Endianness endianness = HydrogenDefaults.Endianness,
		bool autoLoad = false
	) : this(
			new ClusteredStreams(
			  transactionalStream, 
			  clusterSize, 
			  policy, 
			  reservedStreams, 
			  endianness,
			  false
			),
			transactionalStream, 
			serializer, 
			comparer,
			itemChecksummer,
			optionalItemChecksumIndexName, 
			autoLoad
		) {
		InternalCollection.ObjectStream.OwnsStreams = true;
	}

	public TransactionalList(
		ClusteredStreams streams, 
		ITransactionalObject transactionalObject, 
		IItemSerializer<T> serializer = null, 
		IEqualityComparer<T> comparer = null,
		IItemChecksummer<T> itemChecksummer = null,
		string optionalItemChecksumIndexName = null,
		bool autoLoad = false
	) : this(
			StreamMappedFactory.CreateList(
				streams,
				serializer,
				comparer,
				itemChecksummer,
				optionalItemChecksumIndexName,
				false
			),
			transactionalObject,
			autoLoad
		) {
		Guard.ArgumentNotNull(transactionalObject, nameof(transactionalObject));
		_transactionalObject = transactionalObject;
		OwnsList = true;
	}

	public TransactionalList(IStreamMappedList<T> streamMappedList, ITransactionalObject transactionalObject, bool autoLoad = false)
		: base(streamMappedList) {
		Guard.ArgumentNotNull(transactionalObject, nameof(transactionalObject));
		_transactionalObject = transactionalObject;
		_transactionalObject.RolledBack += streamMappedList.ObjectStream.Streams.Initialize;
		
		if (autoLoad && RequiresLoad)
			Load();
	}

	public bool OwnsList { get; set; }

	public bool RequiresLoad => InternalCollection.RequiresLoad;

	public ObjectStream<T> ObjectStream => InternalCollection.ObjectStream;

	ObjectStream IStreamMappedCollection.ObjectStream => ObjectStream;

	public IItemSerializer<T> ItemSerializer => InternalCollection.ItemSerializer;

	public IEqualityComparer<T> ItemComparer => InternalCollection.ItemComparer;

	public void Load() => InternalCollection.Load();

	public Task LoadAsync() => InternalCollection.LoadAsync();

	public virtual void Commit() => _transactionalObject.Commit();

	public virtual Task CommitAsync() => _transactionalObject.CommitAsync();

	public virtual void Rollback() => _transactionalObject.Rollback();

	public virtual Task RollbackAsync() => _transactionalObject.RollbackAsync();

	public virtual void Dispose() {
		if (OwnsList)
			InternalCollection.Dispose();
	}
}
