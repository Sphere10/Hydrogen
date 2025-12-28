// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Core implementation of an object space that orchestrates clustered streams, dimensions, serializers, and indexes for persisted objects.
/// </summary>
public class ObjectSpace : SyncLoadableBase, ICriticalObject, IDisposable {

	protected readonly ClusteredStreams _streams;
	protected readonly DictionaryList<Type, Dimension> _dimensions;
	private readonly InstanceTracker _instanceTracker;
	private bool _loaded;
	protected readonly bool AutoSave;

	protected ObjectSpace(ClusteredStreams streams, ObjectSpaceDefinition objectSpaceDefinition, SerializerFactory serializerFactory, ComparerFactory comparerFactory, FileAccessMode accessMode = FileAccessMode.Default) {
		Guard.ArgumentNotNull(streams, nameof(streams));
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));
		Guard.ArgumentNotNull(serializerFactory, nameof(serializerFactory));
		Guard.ArgumentNotNull(comparerFactory, nameof(comparerFactory));
		_streams = streams;
		Definition = objectSpaceDefinition;
		Serializers = serializerFactory;
		Comparers = comparerFactory;
		_loaded = false;
		_dimensions = new DictionaryList<Type, Dimension>(TypeEquivalenceComparer.Instance, ReferenceEqualityComparer.Instance);
		_instanceTracker = new InstanceTracker();
		AutoSave = objectSpaceDefinition.Traits.HasFlag(ObjectSpaceTraits.AutoSave);
		Disposables = Disposables.None;
		FlushOnDispose = true;
	}

	public override bool RequiresLoad => !_loaded || _streams.RequiresLoad;
	
	public ObjectSpaceDefinition Definition { get; }

	public SerializerFactory Serializers { get; }

	public ComparerFactory Comparers { get; }

	public ICriticalObject ParentCriticalObject { get => _streams.ParentCriticalObject; set => _streams.ParentCriticalObject = value; }
	
	public object Lock => _streams.Lock;
	
	public bool IsLocked => _streams.IsLocked;

	internal ClusteredStreams Streams => _streams;

	public IReadOnlyList<Dimension> Dimensions => _dimensions;

	public Disposables Disposables { get; }

	protected bool FlushOnDispose { get; set; }

	public IDisposable EnterAccessScope() => _streams.EnterAccessScope();

	public IEnumerable<TItem> GetAll<TItem>() {
		throw new NotImplementedException();
	}

	public TItem Get<TItem>(long index) {
		if (!TryGet<TItem>(index, out var item))
			throw new InvalidOperationException($"No {typeof(TItem).ToStringCS()} at index {index} found");
		return item;
	}

	public TItem Get<TItem, TMember>(Expression<Func<TItem, TMember>> memberExpression, TMember memberValue) {
		if (!TryGet(memberExpression, memberValue, out var item))
			throw new InvalidOperationException($"No {typeof(TItem).ToStringCS()} item found with member '{memberExpression.ToMember().Name}' matching '{memberValue}'");
		return item;
	}

	public bool TryGet<TItem>(long index, out TItem item) {
		using (EnterAccessScope()) {

			// First try return an already fetched instance
			if (_instanceTracker.TryGet(index, out item))
				return true;	
			
			// Get underlying stream mapped collection
			var dimension = GetDimension<TItem>();
			
			// Range check
			if (0 > index || index >= dimension.Container.Count)
				return false;

			// Reap check
			if (dimension.Container.ObjectStream.IsReaped(index))
				return false;

			// Deserialize from stream
			item = dimension.Container.ObjectStream.LoadItem(index);

			// Track item instance
			_instanceTracker.Track(item, index);
			dimension.Definition.ChangeTracker.SetChanged(item, false);

			return true;
		}
	}

	public bool TryGet<TItem, TMember>(Expression<Func<TItem, TMember>> memberExpression, TMember memberValue, out TItem item) {
		using (EnterAccessScope()) {

			// Get underlying stream mapped collection
			var dimension = GetDimension<TItem>();

			// Get index for member
			var index = dimension.Container.ObjectStream.GetUniqueIndexFor(memberExpression);

			// Find the item in the index
			if (!index.Values.TryGetValue(memberValue, out var itemIndex)) {
				item = default;
				return false;
			}

			// Get the item referenced by the index
			// NOTE: the item could be cached and contain an unsaved update to the member
			if (!TryGet(itemIndex, out item))
				throw new InvalidOperationException($"Index for {memberExpression.ToMember()} referenced a non-existent item at {itemIndex}");

			return true;
		}
	}

	public TItem New<TItem>() where TItem : new() {
		var instance = new TItem();
		AcceptNew(instance);
		return instance;
	}

	public int AcceptNew<TItem>(TItem item)
		=> AcceptNewInternal(typeof(TItem), item);

	public int AcceptNew(object item)
		=> AcceptNewInternal(item.GetType(), item);

	public long Count<TItem>() => CountInternal(typeof(TItem));

	public long Save<TItem>(TItem item) 
		=> SaveInternal(typeof(TItem), item);

	public long Save(object item) 
		=> SaveInternal(item.GetType(), item);

	public void Delete<TItem>(TItem item) 
		=> DeleteInternal(typeof(TItem), item);

	public void Delete(object item) 
		=> DeleteInternal(item.GetType(), item);

	/// <summary>
	/// Clears all data in the object space. This is a destructive operation and cannot be undone. Must pass <b>"I CONSENT TO CLEAR ALL DATA"</b> for execution.
	/// </summary>
	/// <param name="consentGuard">Must be: "I CONSENT TO CLEAR ALL DATA"</param>
	public void Clear(string consentGuard) {
		Guard.ArgumentNotNull(consentGuard, nameof(consentGuard));
		Guard.Argument(consentGuard == "I CONSENT TO CLEAR ALL DATA", nameof(consentGuard), "Consent guard not provided");
		using (EnterAccessScope()) {
			foreach(var dimension in Dimensions) {
				dimension.Container.Clear();
			}
		}
		_instanceTracker.Clear();
	}

	public virtual void Flush() {
		// save any modified objects (persistence ignorance)
		if (AutoSave)
			SaveModifiedObjects();

		// ensure all dirty merkle-trees are fully calculated
		foreach(var dimension in _dimensions.Values) 
		foreach(var merkleTreeIndex in dimension.Container.ObjectStream.Streams.Attachments.Values.Where(x => x is MerkleTreeIndex).Cast<MerkleTreeIndex>())
			merkleTreeIndex.Flush();

		// ensure any spatial merkle-trees are fully calculated
		foreach (var spatialTreeIndex in _streams.Attachments.Values.Where(x => x is ObjectSpaceMerkleTreeIndex).Cast<ObjectSpaceMerkleTreeIndex>())
			spatialTreeIndex.Flush();

		// flush the underlying stream that maps entire object-space
		Streams.RootStream.Flush();

		// Reset instance tracker
		_instanceTracker.Clear();
	}

	public virtual void Dispose() {
		if (FlushOnDispose)
			Flush();
		Unload();
		_streams.Dispose();
		Disposables.Dispose();
	}

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
			var dimensionDefinition = Definition.Dimensions[ix];
			var dimension = BuildDimension(dimensionDefinition, ix);
			_dimensions.Add(dimensionDefinition.ObjectType, dimension);
		}

		// Attach object space merkle-tree (if applicable)
		if (Definition.Traits.HasFlag(ObjectSpaceTraits.Merkleized)) {
			if (isFirstTimeLoad) {
				// On first load, we need to pre-fill the spatial-tree with an empty tree that denotes all null leafs for each dimension.
				// This is to ensure when spatial tree is loaded that it's buffer matches what is expected. This is ugly
				// but safe since the merkle-tree and it's mapped stream are kept 1-1 consistent at all times.
				_streams.UpdateBytes(
					ObjectSpaceMerkleTreeReservedStreamIndex, 
					MerkleTreeStorageAttachment.GenerateBytes(
						Definition.HashFunction, 
						Tools.Collection.RepeatValue(Hashers.ZeroHash(Definition.HashFunction), dimensionStreams)
					)
				);
			}

			var spaceTree = new ObjectSpaceMerkleTreeIndex(
				this,
				_streams,
				HydrogenDefaults.DefaultSpatialMerkleTreeIndexName,
				HydrogenDefaults.DefaultMerkleTreeIndexName,
				Definition.HashFunction, 
				isFirstTimeLoad
			);
			_streams.RegisterAttachment(spaceTree);

		}

		_loaded = true;
	}
	
	protected void SaveModifiedObjects() {
		Guard.Ensure(AutoSave, "AutoSave is not enabled");
		using (EnterAccessScope()) {
			var changedObjects = 
				_instanceTracker
					.GetInstances()
					.Select(x => (Type: x.GetType(), Instance: x))
					.Where(x => _dimensions[x.Type].Definition.ChangeTracker.HasChanged(x.Instance))
					.Select(x => x.Instance)
					.ToArray();
			foreach(var changedObject in changedObjects) {
				// check if still changed (prior connected object may have saved it recursively)
				if (_dimensions[changedObject.GetType()].Definition.ChangeTracker.HasChanged(changedObject))
					Save(changedObject);
			}
		}
	}

	protected void Unload() {
		// close all object containers when rolling back
		foreach (var disposable in _dimensions.Values.Cast<IDisposable>())
			disposable.Dispose();
		_dimensions.Clear();
		_instanceTracker.Clear();
		
		// unsubscribe to RollingBack event prevent re-entrant unloads (disposal of Streams will result in internal rollback event)
		_streams.UnloadAttachments();


		// Mark as loaded
		_loaded = false;
	}

	protected int AcceptNewInternal(Type itemType, object item) {
		var dimension = GetDimension(itemType);
		if (AutoSave)
			dimension.Definition.ChangeTracker.SetChanged(item, true);
		return _instanceTracker.TrackNew(item);
	}

	protected long CountInternal(Type itemType) {
		using (EnterAccessScope()) {
			// Get underlying stream mapped collection
			var dimension = GetDimension(itemType);
			return dimension.Container.Count;
		}
	}

	protected long SaveInternal(Type itemType, object item) {
		using (EnterAccessScope()) {
			// Get the item index (if applicable)
			if (!_instanceTracker.TryGetIndexOf(item, out var index)) 
				index = AcceptNewInternal(itemType, item);

			// Get underlying stream mapped collection
			var dimension = GetDimension(itemType);

			if (index >= 0) {
				// Update existing
				dimension.Container.Update(index, item);
			} else {
				// Add if new
				dimension.Container.Add(item, out index);
				_instanceTracker.Track(item, index); // updates index from negative value to actual index
			}

			// Mark as unchanged
			dimension.Definition.ChangeTracker.SetChanged(item, false);

			return index;
		}
	}

	protected void DeleteInternal(Type itemType, object item) {
		using (EnterAccessScope()) {
			// Get tracked index of item instance
			if (!_instanceTracker.TryGetIndexOf(item, out var index)) 
				throw new InvalidOperationException($"Instance of {item.GetType().ToStringCS()} was not tracked");
			
			// Stop tracking instance
			_instanceTracker.Untrack(item);

			// Remove it from the dimension 
			var dimension = GetDimension(itemType);
			if (index >= 0)
				dimension.Container.Recycle(index);
		}
	}

	protected virtual Dimension BuildDimension(ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, int spatialStreamIndex) {

		// Get the stream designated for this dimension from the object space
		var dimensionStream = _streams.Open(_streams.Header.ReservedStreams + spatialStreamIndex, false, true); // note: locking scope is kept here?

		// Create a ClusteredStreamCollection which maps over the dimension's stream. This will be used by the ObjectStream to store the objects.
		var clusteredDimensionStream = new ClusteredStreams(
			dimensionStream,
			SanitizeContainerClusterSize(dimensionDefinition.AverageObjectSizeBytes),
			ClusteredStreamsPolicy.FastAllocate,
			dimensionDefinition.Indexes.Length,
			_streams.Endianness,
			false
		) {
			OwnsStream = true
		};

		// Construct the object stream collection which uses the clustered stream collection which maps over the dimension's stream
		var objectStream =
			typeof(ObjectStream<>)
				.MakeGenericType(dimensionDefinition.ObjectType)
				.ActivateWithCompatibleArgs(
					clusteredDimensionStream,
					CreateItemSerializer(dimensionDefinition.ObjectType),
					false
				) as ObjectStream;
		objectStream.OwnsStreams = true;

		// construct the required indexes for the dimension
		foreach (var index in dimensionDefinition.Indexes) {
			IClusteredStreamsAttachment metaDataObserver = index.Type switch {
				ObjectSpaceDefinition.IndexType.Identifier => BuildIdentifier(objectStream, dimensionDefinition, index),
				ObjectSpaceDefinition.IndexType.UniqueKey => BuildUniqueKey(objectStream, dimensionDefinition, index),
				ObjectSpaceDefinition.IndexType.Index => BuildIndex(objectStream, dimensionDefinition, index),
				ObjectSpaceDefinition.IndexType.RecyclableIndexStore => BuildRecyclableIndexStore(objectStream, dimensionDefinition, index),
				ObjectSpaceDefinition.IndexType.MerkleTree => BuildMerkleTreeIndex(objectStream, dimensionDefinition, index),
				_ => throw new ArgumentOutOfRangeException()
			};
			objectStream.Streams.RegisterAttachment(metaDataObserver);
		}
	
		// Construct a suitable a comparer
		var comparer = Comparers.GetEqualityComparer(dimensionDefinition.ObjectType);

		// Construct the object collection which uses the underlying object stream
		var list = (IStreamMappedCollection)typeof(StreamMappedRecyclableList<>)
			.MakeGenericType(dimensionDefinition.ObjectType)
			.ActivateWithCompatibleArgs(
				objectStream,
				dimensionDefinition.Indexes.First(x => x.Type == ObjectSpaceDefinition.IndexType.RecyclableIndexStore).Name,
				comparer,
				false
			);
		Tools.Reflection.SetPropertyValue(list, nameof(StreamMappedRecyclableList<int>.OwnsContainer), true);

		// load list if applicable
		if (list is ILoadable { RequiresLoad: true } loadable)
			loadable.Load();


		// construct a typed dimension object for client
		var dimension = (Dimension)Activator.CreateInstance(
			typeof(Dimension<>).MakeGenericType(dimensionDefinition.ObjectType),
			new object[] { dimensionDefinition, list }
		);

		return dimension;
	}

	protected virtual IClusteredStreamsAttachment BuildIdentifier(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition) {
		// NOTE: same as BuildUniqueKey, but may have differentiating functionality in future
		var keyComparer = Comparers.GetEqualityComparer(indexDefinition.Member.PropertyType);
		var keySerializer = Serializers.GetSerializer(indexDefinition.Member.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateUniqueMemberIndex(dimension, indexDefinition.Name, indexDefinition.Member, keySerializer, keyComparer) :
				IndexFactory.CreateUniqueMemberChecksumIndex(dimension, indexDefinition.Name, indexDefinition.Member, keySerializer, null, null, keyComparer, indexDefinition.NullPolicy);
	}

	protected virtual IClusteredStreamsAttachment BuildUniqueKey(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition) {
		var keyComparer = Comparers.GetEqualityComparer(indexDefinition.Member.PropertyType);
		var keySerializer = Serializers.GetSerializer(indexDefinition.Member.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateUniqueMemberIndex(dimension, indexDefinition.Name, indexDefinition.Member, keySerializer, keyComparer) :
				IndexFactory.CreateUniqueMemberChecksumIndex(dimension, indexDefinition.Name, indexDefinition.Member, keySerializer, null, null, keyComparer, indexDefinition.NullPolicy);
	}

	protected virtual IClusteredStreamsAttachment BuildIndex(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition) {
		var keyComparer = Comparers.GetEqualityComparer(indexDefinition.Member.PropertyType);
		var keySerializer = Serializers.GetSerializer(indexDefinition.Member.PropertyType);
		return
			keySerializer.IsConstantSize ?
				IndexFactory.CreateMemberIndex(dimension, indexDefinition.Name, indexDefinition.Member, keySerializer, keyComparer) :
				IndexFactory.CreateMemberChecksumIndex(dimension, indexDefinition.Name, indexDefinition.Member, keySerializer, null, null, keyComparer, indexDefinition.NullPolicy);
	}

	protected virtual IClusteredStreamsAttachment BuildRecyclableIndexStore(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition) {
		return new RecyclableIndexIndex(dimension, indexDefinition.Name);
	}

	protected virtual IClusteredStreamsAttachment BuildMerkleTreeIndex(ObjectStream dimension, ObjectSpaceDefinition.DimensionDefinition dimensionDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition) {
		return new MerkleTreeIndex(dimension, indexDefinition.Name, new ObjectStreamItemHasher(dimension, Definition.HashFunction), Definition.HashFunction);
	}
	
	protected virtual IItemSerializer CreateItemSerializer(Type objectType) {
		return Serializers.GetSerializer(objectType);
	}

	protected virtual int SanitizeContainerClusterSize(int? clusterSizeB)
		=> Tools.Values.ClipValue(
			clusterSizeB.GetValueOrDefault(), 
			HydrogenDefaults.SmallestRecommendedClusterSize, 
			HydrogenDefaults.LargestRecommendedClusterSize
		);

	private Dimension<TItem> GetDimension<TItem>() 
		=> (Dimension<TItem>)GetDimension(typeof(TItem));

	private Dimension GetDimension(Type itemType) {
		if (!_dimensions.TryGetValue(itemType, out var dimension))
			throw new InvalidOperationException($"A dimension for type '{itemType.ToStringCS()}' was not registered");

		return dimension;
	}

	#region Aux Types
	public record Dimension(ObjectSpaceDefinition.DimensionDefinition Definition, IStreamMappedRecylableList Container) : IDisposable {
		public void Dispose() => (Container as IDisposable)?.Dispose();
	};
	public record Dimension<T>(ObjectSpaceDefinition.DimensionDefinition Definition, StreamMappedRecyclableList<T> Container) : Dimension(Definition, Container) {
		public new StreamMappedRecyclableList<T> Container => (StreamMappedRecyclableList<T>)base.Container;
	}

	#endregion

}
