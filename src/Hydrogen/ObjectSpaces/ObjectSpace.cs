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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpace : SyncLoadableBase, ITransactionalObject, ICriticalObject, IDisposable {

	public event EventHandlerEx Committing { add => _fileStream.Committing += value; remove => _fileStream.Committing -= value; }
	public event EventHandlerEx Committed { add => _fileStream.Committed += value; remove => _fileStream.Committed -= value; }
	public event EventHandlerEx RollingBack { add => _fileStream.RollingBack += value; remove => _fileStream.RollingBack -= value; }
	public event EventHandlerEx RolledBack { add => _fileStream.RolledBack += value; remove => _fileStream.RolledBack -= value; }

	private TransactionalStream _fileStream;
	private ClusteredStreams _streams;
	private readonly SerializerFactory _serializerFactory;
	private readonly ComparerFactory _comparerFactory;
	private readonly IndexedValueDictionary<Type, IStreamMappedCollection> _dimensions;
	private readonly InstanceTracker _instanceTracker;
	private bool _loaded;

	public ObjectSpace(HydrogenFileDescriptor file, ObjectSpaceDefinition objectSpaceDefinition, SerializerFactory serializerFactory, ComparerFactory comparerFactory, FileAccessMode accessMode = FileAccessMode.Default) {
		Guard.ArgumentNotNull(file, nameof(file));
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));

		File = file;
		AccessMode = accessMode;
		Definition = objectSpaceDefinition;
		_serializerFactory = serializerFactory;
		_comparerFactory = comparerFactory;

		// Create the file stream
		_fileStream = new TransactionalStream(File, AccessMode.WithoutAutoLoad());
		SubscribeToFileStreamEvents();
		
		// Create clustered streams from file stream
		var objectSpaceMetaDataStreamCount = Definition.Merkleized ? 1 : 0;
		_streams = new ClusteredStreams(
			_fileStream,
			(int)File.ClusterSize,
			File.ContainerPolicy,
			objectSpaceMetaDataStreamCount,
			Endianness.LittleEndian, // TODO: use File.Endianness when added
			false
		);
		_streams.OwnsStream = true; // disposes _fileStream
		_loaded = false;
		_dimensions = new IndexedValueDictionary<Type, IStreamMappedCollection>(TypeEquivalenceComparer.Instance, ReferenceEqualityComparer.Instance);
		_instanceTracker = new InstanceTracker();
		if (AccessMode.HasFlag(FileAccessMode.AutoLoad))
			Load();
	}

	public override bool RequiresLoad => !_loaded || _streams.RequiresLoad;

	public FileAccessMode AccessMode { get; }
	
	public HydrogenFileDescriptor File { get; }
	
	public ObjectSpaceDefinition Definition { get; }

	public SerializerFactory Serializers => _serializerFactory;

	public ComparerFactory Comparers => _comparerFactory;
	
	public ICriticalObject ParentCriticalObject { get => _streams.ParentCriticalObject; set => _streams.ParentCriticalObject = value; }
	
	public object Lock => _streams.Lock;
	
	public bool IsLocked => _streams.IsLocked;

	public int Dimensions => checked((int)_dimensions.Count);

	internal ClusteredStreams InternalStreams => _streams;

	public IDisposable EnterAccessScope() => _streams.EnterAccessScope();

	public IEnumerable<TItem> GetAll<TItem>() {
		throw new NotImplementedException();
	}

	public TItem Get<TItem>(long index) {
		if (!TryGet<TItem>(index, out var item))
			throw new InvalidOperationException($"No {typeof(TItem).ToStringCS()} at index {index} found");
		return item;
	}

	public bool TryGet<TItem>(long index, out TItem item) {
		using (EnterAccessScope()) {

			// First try return an already fetched instance
			if (_instanceTracker.TryGet(index, out item))
				return true;	
			
			// Get underlying stream mapped collection
			var objectList = GetDimension<TItem>();
			
			// Range check
			if (0 > index || index >= objectList.Count)
				return false;

			// Reap check
			if (objectList.ObjectStream.IsReaped(index))
				return false;

			// Deserialize from stream
			item = objectList.ObjectStream.LoadItem(index);

			// Track item instance
			_instanceTracker.Track(item, index);

			return true;
		}
	}

	public long Count<TItem>() {
		using (EnterAccessScope()) {
			// Get underlying stream mapped collection
			var objectList = GetDimension<TItem>();
			return objectList.Count;
		}
	}

	public long Save<TItem>(TItem item) {
		using (EnterAccessScope()) {
			// Get underlying stream mapped collection
			var objectList = GetDimension<TItem>();

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
	}

	public void Delete<TItem>(TItem item) {
		using (EnterAccessScope()) {
			// Get tracked index of item instance
			if (!_instanceTracker.TryGetIndexOf(item, out var index)) 
				throw new InvalidOperationException($"Instance of {item.GetType().ToStringCS()} was not tracked");
			
			// Stop tracking instance
			_instanceTracker.Untrack(item);

			// Remove it from the list 
			var objectList = GetDimension<TItem>();
			objectList.Recycle(index);
		}
	}

	public void Commit()  {
		using (EnterAccessScope()) {
			// flush all cached changes
			_fileStream.Commit();
		}
	}

	public Task CommitAsync() => throw new NotSupportedException();
	
	public void Rollback() {
		using (EnterAccessScope()) {
			_fileStream.Rollback();
		}
	}

	public Task RollbackAsync() => throw new NotSupportedException();

	public void Dispose() {
		UnsubscribeToFileStreamEvents();
		Unload();
		_streams.Dispose();
	}

	internal IStreamMappedCollection GetDimension(int number) => _dimensions[number];

	protected override void LoadInternal() {
		const int ObjectSpaceMerkleTreeReservedStreamIndex = 0;
		Guard.Against(_loaded, "ObjectSpace already loaded.");

		Definition.Validate().ThrowOnFailure();

		// Load ObjectStream streams
		if (_streams.RequiresLoad)
			_streams.Load();

	
		// Ensure all dimension streams are created on the first load
		var isFirstTimeLoad = (_streams.Header.StreamCount - _streams.Header.ReservedStreams) == 0;
		if (isFirstTimeLoad) {
			// Create dimension streams
			for (var i = 0; i < Definition.Dimensions.Length; i++) {
				_streams.AddBytes(ReadOnlySpan<byte>.Empty);
			}
		}

		// Check streams and dimensions match
		var dimensionStreams = _streams.Header.StreamCount - _streams.Header.ReservedStreams;
		Guard.Ensure(dimensionStreams == Definition.Dimensions.Length, $"Unexpected stream count {Definition?.Dimensions.Length}, expected {dimensionStreams}");

		// Add dimensions to object space
		for (var ix = 0; ix < Definition.Dimensions.Length; ix++) {
			var containerDefinition = Definition.Dimensions[ix];
			var dimension = BuildDimension(containerDefinition, ix);
			_dimensions.Add(containerDefinition.ObjectType, dimension);
		}

		// Attach object space merkle-tree (if applicable)
		if (Definition.Merkleized) {
			if (isFirstTimeLoad) {
				// On first load, we need to pre-fill the spatial-tree with an empty tree that denotes all null leafs for each dimension.
				// This is to ensure when spatial tree is loaded that it's buffer matches what is expected. This is ugly
				// but safe since the merkle-tree and it's mapped stream are kept 1-1 consistent at all times.
				_streams.UpdateBytes(
					ObjectSpaceMerkleTreeReservedStreamIndex, 
					MerkleTreeStore.GenerateBytes(
						Definition.HashFunction, 
						Tools.Collection.RepeatValue(Hashers.ZeroHash(Definition.HashFunction), dimensionStreams)
					)
				);
			}

			var spaceTree = new ObjectSpaceMerkleTreeIndex(this, ObjectSpaceMerkleTreeReservedStreamIndex, Definition.HashFunction, isFirstTimeLoad);
			_streams.RegisterAttachment(spaceTree);

			spaceTree.VerifyConsistency();
		}

		_loaded = true;
	}

	protected void Unload() {
		// close all object containers when rolling back
		foreach (var disposable in _dimensions.Values.Cast<IDisposable>())
			disposable.Dispose();
		_dimensions.Clear();
		_instanceTracker.Clear();
		
		// unsubscribe to RollingBack event prevent re-entrant unloads (disposal of _streams will result in internal rollback event)
		_streams.UnloadAttachments();


		// Mark as loaded
		_loaded = false;
	}

	protected virtual IStreamMappedCollection BuildDimension(ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, int spatialStreamIndex) {
		// Get the stream within the object space which will comprise the object dimension
		var dimensionStream = _streams.Open(_streams.Header.ReservedStreams + spatialStreamIndex, false, true); // note: locking scope is kept here?

		// Create a ClusteredStreams that is mapped to the dimension stream. The object stream
		// will write itself over this clustered stream.
		var clusteredDimensionStream = new ClusteredStreams(
			dimensionStream,
			SanitizeContainerClusterSize(dimensionDefinition.AverageObjectSizeBytes),
			ClusteredStreamsPolicy.FastAllocate,
			dimensionDefinition.Indexes.Length,
			Endianness.LittleEndian,
			false
		) {
			OwnsStream = true
		};

		// Construct the object stream dimension, mapped to the stream
		var dimension =
			typeof(ObjectStream<>)
				.MakeGenericType(dimensionDefinition.ObjectType)
				.ActivateWithCompatibleArgs(
					clusteredDimensionStream,
					CreateItemSerializer(dimensionDefinition.ObjectType),
					false
				) as ObjectStream;
		dimension.OwnsStreamContainer = true;

		// construct indexes of the object stream
		foreach (var (item, index) in dimensionDefinition.Indexes.WithIndex()) {
			IClusteredStreamsAttachment metaDataObserver = item.Type switch {
				ObjectSpaceDefinition.IndexType.Identifier => BuildIdentifier(dimension, dimensionDefinition, item, index),
				ObjectSpaceDefinition.IndexType.UniqueKey => BuildUniqueKey(dimension, dimensionDefinition, item, index),
				ObjectSpaceDefinition.IndexType.Index => BuildIndex(dimension, dimensionDefinition, item, index),
				ObjectSpaceDefinition.IndexType.FreeIndexStore => BuildFreeIndexStore(dimension, dimensionDefinition, item, index),
				ObjectSpaceDefinition.IndexType.MerkleTree => BuildMerkleTreeIndex(dimension, dimensionDefinition, item, index),
				_ => throw new ArgumentOutOfRangeException()
			};
			dimension.Streams.RegisterAttachment(metaDataObserver);
		}
	
		// Get a comparer
		var comparer = _comparerFactory.GetEqualityComparer(dimensionDefinition.ObjectType);

		// construct the the collection
		var list = (IStreamMappedCollection)typeof(StreamMappedRecyclableList<>)
			.MakeGenericType(dimensionDefinition.ObjectType)
			.ActivateWithCompatibleArgs(
				dimension,
				comparer,
				false
			);
		Tools.Reflection.SetPropertyValue(list, nameof(StreamMappedRecyclableList<int>.OwnsContainer), true);

		// load list if applicable
		if (list is ILoadable { RequiresLoad: true } loadable)
			loadable.Load();

		return list;
	}

	protected virtual IClusteredStreamsAttachment BuildIdentifier(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
		var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateUniqueKeyIndexAttachment(dimension, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
				IndexFactory.CreateUniqueKeyChecksumIndexAttachment(dimension, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
	}

	protected virtual IClusteredStreamsAttachment BuildUniqueKey(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
		var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateUniqueKeyIndexAttachment(dimension, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
				IndexFactory.CreateUniqueKeyChecksumIndexAttachment(dimension, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
	}

	protected virtual IClusteredStreamsAttachment BuildIndex(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
		var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateKeyIndexAttachment(dimension, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
				IndexFactory.CreateKeyChecksumIndexAttachment(dimension, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
	}

	protected virtual IClusteredStreamsAttachment BuildFreeIndexStore(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		return new RecyclableIndexIndex(dimension, streamIndex);
	}

	protected virtual IClusteredStreamsAttachment BuildMerkleTreeIndex(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
		return new MerkleTreeIndex(dimension, streamIndex, new ObjectStreamItemHasher(dimension, Definition.HashFunction), Definition.HashFunction);
	}
	
	protected virtual IItemSerializer CreateItemSerializer(Type objectType) {
		return _serializerFactory.GetSerializer(objectType);
	}

	protected virtual int SanitizeContainerClusterSize(int clusterSizeB)
		=> Tools.Values.ClipValue(clusterSizeB, 256, 8192);

	protected virtual void OnCommitting() {
		// TODO: potentially move this up to Consensus Space
		// need to ensure all merkle-trees are committed 
		foreach(var collection in _dimensions.Values) {
			if (collection.ObjectStream.Streams.TryFindAttachment<MerkleTreeIndex>(out var merkleTreeIndex)) {
				// fetching the root ensures the stream-mapped merkle-tree is fully calculated
				merkleTreeIndex.KeyStore.Flush();
			}
		}

		// ensure the global merkle-tree is updated
		if (_streams.TryFindAttachment<ObjectSpaceMerkleTreeIndex>(out var objectSpaceMerkleTreeIndex)) {
			objectSpaceMerkleTreeIndex.MerkleTreeStore.Flush();
		}
	}

	protected virtual void OnCommitted() {
	}

	protected virtual void OnRollingBack() {
		Unload();
	}

	protected virtual void OnRolledBack() {
		// reload after rollback
		_streams.Initialize();
		Load();
	}

	private StreamMappedRecyclableList<TItem> GetDimension<TItem>() {
		var itemType = typeof(TItem);

		if (!_dimensions.TryGetValue(itemType, out var dimension))
			throw new InvalidOperationException($"A dimension for type '{itemType.ToStringCS()}' was not registered");

		return (StreamMappedRecyclableList<TItem>)dimension;
	}

	private void SubscribeToFileStreamEvents() {
		_fileStream.Committing += OnCommitting;
		_fileStream.Committed += OnCommitted;
		_fileStream.RollingBack += OnRollingBack;
		_fileStream.RolledBack += OnRolledBack;
	}

	private void UnsubscribeToFileStreamEvents() {
		if (_fileStream is null)
			return;

		_fileStream.Committing -= OnCommitting;
		_fileStream.Committed -= OnCommitted;
		_fileStream.RollingBack -= OnRollingBack;
		_fileStream.RolledBack -= OnRolledBack;
	}
}
