// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Hydrogen.ObjectSpaces.ObjectSpaceDefinition;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpace : SyncLoadableBase, ISynchronizedObject, ITransactionalObject, IDisposable {

	public event EventHandlerEx<object> Committing { add => _fileStream.Committing += value; remove => _fileStream.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _fileStream.Committed += value; remove => _fileStream.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _fileStream.RollingBack += value; remove => _fileStream.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _fileStream.RolledBack += value; remove => _fileStream.RolledBack -= value; }

	private readonly SynchronizedObject _lock;
	private readonly TransactionalStream _fileStream;
	private readonly StreamContainer _streamContainer;
	private readonly SerializerFactory _serializerFactory;
	private readonly ComparerFactory _comparerFactory;
	private readonly IDictionary<Type, IStreamMappedCollection> _collections;
	private readonly InstanceTracker _instanceTracker;
	private bool _loaded;

	public ObjectSpace(HydrogenFileDescriptor file, ObjectSpaceDefinition objectSpaceDefinition, SerializerFactory serializerFactory, ComparerFactory comparerFactory, FileAccessMode accessMode = FileAccessMode.Default) {
		Guard.ArgumentNotNull(file, nameof(file));
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));

		File = file;
		Definition = objectSpaceDefinition;
		_serializerFactory = serializerFactory;
		_comparerFactory = comparerFactory;
		_lock = new SynchronizedObject();
		_fileStream = new TransactionalStream(
			file,
			accessMode.WithoutAutoLoad()
		);
		_fileStream.Committing += _ => OnCommitting();
		_fileStream.Committed += _ => OnCommitted();
		_fileStream.RollingBack += _ => OnRollingBack();
		_fileStream.RolledBack += _ => OnRolledBack();
		_streamContainer = new StreamContainer(
			_fileStream,
			(int)file.ClusterSize,
			file.ContainerPolicy,
			0, // TODO: need to consider schema-level index's like merkle-tree, etc
			Endianness.LittleEndian,
			false
		);

		_loaded = false;
		_collections = new Dictionary<Type, IStreamMappedCollection>();
		_instanceTracker = new InstanceTracker();
		if (accessMode.HasFlag(FileAccessMode.AutoLoad))
			Load();
	}

	public override bool RequiresLoad => !_loaded || _streamContainer.RequiresLoad;

	public HydrogenFileDescriptor File { get; }

	public ObjectSpaceDefinition Definition { get; }

	public ISynchronizedObject ParentSyncObject { get => _lock.ParentSyncObject; set => _lock.ParentSyncObject = value; }

	public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

	public IDisposable EnterReadScope() => _lock.EnterReadScope();

	public IDisposable EnterWriteScope() => _lock.EnterWriteScope();

	public IEnumerable<TItem> GetAll<TItem>() {
		throw new NotImplementedException();
	}

	public TItem Get<TItem>(long index) {
		if (!TryGet<TItem>(index, out var item))
			throw new InvalidOperationException($"No {typeof(TItem).ToStringCS()} at index {index} found");
		return item;
	}

	public bool TryGet<TItem>(long index, out TItem item) {
		// First try return an already fetched instance
		if (_instanceTracker.TryGet(index, out item))
			return true;	
		
		// Get underlying stream mapped collection
		var objectList = GetList<TItem>();
		
		// Range check
		if (0 > index || index >= objectList.Count)
			return false;

		// Reap check
		if (objectList.ObjectContainer.IsReaped(index))
			return false;

		// Deserialize from stream
		item = objectList.ObjectContainer.LoadItem(index);

		// Track item instance
		_instanceTracker.Track(item, index);

		return true;
	}

	public long Count<TItem>() {
		// Get underlying stream mapped collection
		var objectList = GetList<TItem>();
		return objectList.Count;
	}

	public long Save<TItem>(TItem item) {
		// Get underlying stream mapped collection
		var objectList = GetList<TItem>();

		// Get the item index (if applicable)
		var existingItem = _instanceTracker.TryGetIndexOf(item, out var index);
		
		if (existingItem) {
			// Update existing
			objectList.Update(index, item);
		} else {
			// Add if new
			objectList.Add(item, out index);
			_instanceTracker.Track(item, index);
		}

		return index;
	}

	public void Delete<TItem>(TItem item) {
		// Get tracked index of item instance
		if (!_instanceTracker.TryGetIndexOf(item, out var index)) 
			throw new InvalidOperationException($"Instance of {item.GetType().ToStringCS()} was not tracked");
		
		// Stop tracking instance
		_instanceTracker.Untrack(item);

		// Remove it from the list 
		var objectList = GetList<TItem>();
		objectList.Recycle(index);
	}

	public void Commit() => _fileStream.Commit();

	public Task CommitAsync() => _fileStream.CommitAsync();

	public void Rollback() => _fileStream.Rollback();

	public Task RollbackAsync() => _fileStream.RollbackAsync();

	public void Dispose() {
		//foreach (var container in _collections.Values.Cast<ObjectContainer>().Where(x => !x.RequiresLoad))
		//	container.Dispose();
		foreach (var disposable in _collections.Values.Cast<IDisposable>())
			disposable.Dispose();

		_streamContainer.Dispose();
		_fileStream.Dispose();
	}

	protected override void LoadInternal() {
		Guard.Against(_loaded, "ObjectSpace already loaded.");
		Definition.Validate().ThrowOnFailure();

		// Load up stream container
		if (_streamContainer.RequiresLoad)
			_streamContainer.Load();

		// Ensure container streams exist
		var containerStreams = _streamContainer.Header.StreamCount - _streamContainer.Header.ReservedStreams;
		if (containerStreams == 0) {
			// TODO: create the consensus-space merkle tree here
			for (var i = 0; i < Definition.Containers.Length; i++) {
				using var _ = _streamContainer.Add();
			}
		}
		containerStreams = _streamContainer.Header.StreamCount - _streamContainer.Header.ReservedStreams;
		Guard.Ensure(containerStreams == Definition.Containers.Length, $"Unexpected stream count {Definition?.Containers.Length}, expected {containerStreams}");


		// Here parse the internal structure and create the containers.
		for (var ix = 0; ix < Definition.Containers.Length; ix++) {
			var containerDefinition = Definition.Containers[ix];
			var collection = BuildObjectList(containerDefinition, ix);
			_collections.Add(containerDefinition.ObjectType, collection);
		}
		_loaded = true;

	}

	protected void Unload() {

		// close all object containers when rolling back
		foreach (var disposable in _collections.Values.Cast<IDisposable>())
			disposable.Dispose();
		_collections.Clear();
		_loaded = false;
	}

	protected virtual IStreamMappedCollection BuildObjectList(ContainerDefinition containerDefinition, int containerIndex) {
		// Get the stream within the object space which will comprise the object container
		var containerStream = _streamContainer.Open(_streamContainer.Header.ReservedStreams + containerIndex, false, true);

		// Create a StreamContainer mapped to this stream, so it can contain items
		var containerItemsStreamContainer = new StreamContainer(
			containerStream,
			SanitizeContainerClusterSize(containerDefinition.AverageObjectSizeBytes),
			StreamContainerPolicy.FastAllocate,
			containerDefinition.Indexes.Length,
			Endianness.LittleEndian,
			false
		) {
			OwnsStream = true
		};

		// construct the object container
		var container =
			typeof(ObjectContainer<>)
				.MakeGenericType(containerDefinition.ObjectType)
				.ActivateWithCompatibleArgs(
					containerItemsStreamContainer,
					CreateItemSerializer(containerDefinition.ObjectType),
					false
				) as ObjectContainer;
		container.OwnsStreamContainer = true;

		// construct indexes
		foreach (var (item, index) in containerDefinition.Indexes.WithIndex()) {
			IStreamContainerAttachment metaDataObserver = item.Type switch {
				IndexType.Identifier => BuildIdentifier(container, containerDefinition, item, index),
				IndexType.UniqueKey => BuildUniqueKey(container, containerDefinition, item, index),
				IndexType.Index => BuildIndex(container, containerDefinition, item, index),
				IndexType.FreeIndexStore => new RecyclableIndexIndex(container, index),
				IndexType.MerkleTree => throw new NotImplementedException(),
				_ => throw new ArgumentOutOfRangeException()
			};
			container.Streams.RegisterAttachment(metaDataObserver);
		}
	
		// Get a comparer
		var comparer = _comparerFactory.GetEqualityComparer(containerDefinition.ObjectType);

		// construct the the object collection
		var list = (IStreamMappedCollection)typeof(StreamMappedRecyclableList<>)
			.MakeGenericType(containerDefinition.ObjectType)
			.ActivateWithCompatibleArgs(
				container,
				comparer,
				false
			);
		Tools.Reflection.SetPropertyValue(list, nameof(StreamMappedRecyclableList<int>.OwnsContainer), true);

		// load list if applicable
		if (list is ILoadable { RequiresLoad: true } loadable)
			loadable.Load();

		return list;
	}

	protected virtual IStreamContainerAttachment BuildIdentifier(ObjectContainer container, ObjectSpaceDefinition.ContainerDefinition containerDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
		var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateUniqueKeyIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
				IndexFactory.CreateUniqueKeyChecksumIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
	}

	protected virtual IStreamContainerAttachment BuildUniqueKey(ObjectContainer container, ObjectSpaceDefinition.ContainerDefinition containerDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
		var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateUniqueKeyIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
				IndexFactory.CreateUniqueKeyChecksumIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
	}

	protected virtual IStreamContainerAttachment BuildIndex(ObjectContainer container, ObjectSpaceDefinition.ContainerDefinition containerDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
		var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateKeyIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
				IndexFactory.CreateKeyChecksumIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);

	}
	
	protected virtual IItemSerializer CreateItemSerializer(Type objectType) {
		return _serializerFactory.GetSerializer(objectType);
	}

	protected virtual int SanitizeContainerClusterSize(int clusterSizeB)
		=> Tools.Values.ClipValue(clusterSizeB, 256, 8192);


	protected virtual void OnCommitting() {
		// TODO: potentially move this up to Consensus Space

		// need to ensure all merkle-trees are committed 
		foreach(var collection in _collections.Values) {
			if (collection.ObjectContainer.Streams.TryFindAttachment<MerkleTreeIndex>(out var merkleTreeIndex)) {
				// fetching the root ensures the stream-mapped merkle-tree is fully calculated
				var root = merkleTreeIndex.MerkleTree.Root;
			}
		}

		// TODO: ensure the global merkle-tree is updated
		// var truthSingularity = containerTree.MerkleTree.Root;
	}

	protected virtual void OnCommitted() {
	}

	protected virtual void OnRollingBack() {
		Unload();
	}

	protected virtual void OnRolledBack() {
		// reload after rollback
		_instanceTracker.Clear();
		_streamContainer.Initialize();
		Load();
	}

	private StreamMappedRecyclableList<TItem> GetList<TItem>() {
		var itemType = typeof(TItem);

		if (!_collections.TryGetValue(itemType, out var dimension))
			throw new InvalidOperationException($"A container for type '{itemType.ToStringCS()}' was not registered");

		return (StreamMappedRecyclableList<TItem>)dimension;
	}

}
