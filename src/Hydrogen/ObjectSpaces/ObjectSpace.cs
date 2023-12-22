using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpace : SyncLoadableBase, IDisposable {
	private readonly StreamContainer _streamContainer;
	private readonly SerializerFactory _serializerFactory;
	private readonly ComparerFactory _comparerFactory;
	private bool _loaded;

	public ObjectSpace(ObjectSpaceFile file, ObjectSpaceDefinition objectSpaceDefinition, SerializerFactory serializerFactory, ComparerFactory comparerFactory, bool readOnly = false, bool autoLoad = false) {
		Guard.ArgumentNotNull(file, nameof(file));
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));

		File = file;
		Definition = objectSpaceDefinition;
		_serializerFactory = serializerFactory;
		_comparerFactory = comparerFactory;
		_streamContainer = new StreamContainer(
			new TransactionalStream(
				file.FilePath,
				file.PageFileDir,
				file.PageSize,
				file.MaxMemory,
				readOnly,
				false
			),
			(int)file.ClusterSize,
			file.ContainerPolicy,
			0, // TODO: need to consider schema-level index's like merkle-tree, etc
			Endianness.LittleEndian,
			false
		) {
			OwnsStream = true
		};
		_loaded = false;
		if (autoLoad)
			Load();
	}

	public override bool RequiresLoad => !_loaded || _streamContainer.RequiresLoad;

	public ObjectSpaceFile File { get; }

	public ObjectSpaceDefinition Definition { get; }

	public ObjectContainer[] Containers { get; }

	public IStreamMappedRecyclableList<TItem> GetContainer<TItem>() {
		CheckLoaded();
		throw new NotImplementedException();
	}

	public IStreamMappedDictionary<TKey, TValue> GetContainer<TKey, TValue>() {
		CheckLoaded();
		throw new NotImplementedException();
	}

	public IReadOnlyDictionary<TMember, IFuture<TItem>> GetUniqueLookup<TItem, TMember>(Expression<Func<TItem, TMember>> memberExpression) {
		CheckLoaded();
		throw new NotImplementedException();
	}

	public ILookup<TMember, IFuture<TItem>> GetLookup<TItem, TMember>(Expression<Func<TItem, TMember>> memberExpression) {
		CheckLoaded();
		throw new NotImplementedException();
	}

	protected override void LoadInternal() {
		Guard.Against(_loaded, "ObjectSpace already loaded.");
		var validationResult = Validate(Definition);
		if (validationResult.IsFailure) {
			throw new InvalidOperationException(validationResult.ErrorMessages.ToParagraphCase());
		}

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
		var containers = new List<ObjectContainer>();
		foreach (var (containerDefinition, ix) in Definition.Containers.WithIndex()) {
			// Get the stream within the object space which will comprise the object container
			var containerStream = _streamContainer.Open(_streamContainer.Header.ReservedStreams + ix, false, false); // nb: no lock is acquired

			// Create a StreamContainer mapped to this stream, so it can contain items
			var containerItemsStreamContainer = new StreamContainer(
				containerStream,
				SanitizeContainerClusterSize(containerDefinition.AverageObjectSizeBytes),
				StreamContainerPolicy.FastAllocate,
				containerDefinition.Indexes.Length,
				Endianness.LittleEndian,
				false
			);

			// construct the object container
			var container = new ObjectContainer(
				containerDefinition.ObjectType,
				containerItemsStreamContainer,
				CreateItemSerializer(containerDefinition.ObjectType),
				false
			);

			// construct meta-data provider
			foreach (var (item, index) in containerDefinition.Indexes.WithIndex()) {
				IObjectContainerAttachment metaDataObserver = item.Type switch {
					ObjectSpaceDefinition.IndexType.UniqueKey => BuildUniqueKey(container, containerDefinition, item, index),
					ObjectSpaceDefinition.IndexType.Index => throw new NotImplementedException(),
					ObjectSpaceDefinition.IndexType.FreeIndexStore => new RecyclableIndexIndex(container, index),
					ObjectSpaceDefinition.IndexType.MerkleTree => throw new NotImplementedException(),
					_ => throw new ArgumentOutOfRangeException()
				};
				container.RegisterAttachment(metaDataObserver);
			}

			// register the object container into the root 

		}

		_loaded = true;

		IObjectContainerAttachment BuildUniqueKey(ObjectContainer container, ObjectSpaceDefinition.ContainerDefinition containerDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
			var dataProviderType = typeof(UniqueKeyIndex<,>).MakeGenericType(containerDefinition.ObjectType, indexDefinition.KeyMember.PropertyType);
			//var projectionType = typeof(Func<,>).MakeGenericType(containerDefinition.ObjectType, indexDefinition.KeyMember.PropertyType);
			var projection = Tools.Lambda.ConvertFunc(indexDefinition.KeyMember.GetValue, containerDefinition.ObjectType, indexDefinition.KeyMember.PropertyType);
			var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
			var serializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
		
			// public ObjectContainerUniqueKey(ObjectContainer container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer) {
			var uniqueKeyInstance = dataProviderType.ActivateWithCompatibleArgs(container, (long)streamIndex, projection, keyComparer, serializer);
			return (IObjectContainerAttachment)uniqueKeyInstance;
		}

	}

	protected Result Validate(ObjectSpaceDefinition definition) {
		var result = Result.Success;

		// Verify all containers
		if (definition.Containers != null) {
			// At least 1 container
			if (definition.Containers.Length == 0) {
				result.AddError("At least 1 container must be defined.");
			}

			// Verify container
			foreach (var (container, ix) in definition.Containers.WithIndex()) {
				if (container.Indexes != null && container.Indexes.Length > 0) {
					// Ensure has 1 free-index store
					var freeIndexStores = container.Indexes.Count(x => x.Type == ObjectSpaceDefinition.IndexType.FreeIndexStore);
					if (freeIndexStores != 1)
						result.AddError($"Container {ix} ({container.ObjectType.ToStringCS()}) had {freeIndexStores} FreeIndexStore's defined, required {1}.");

					// TODO: Ensure has 0 or 1 merkle-trees

				} else {
					result.AddError("Container requires index definition");
				}
			}


			// All containers must have a unique type
			definition
				.Containers
				.GroupBy(x => x.ObjectType)
				.Where(x => x.Count() > 1)
				.ForEach(x => result.AddError($"Container type '{x.Key}' is defined more than once."));

			// Each container index correctly maps to an object property

		} else {
			result.AddError("No containers defined.");
		}

		return result;
	}

	protected IItemSerializer CreateItemSerializer(Type objectType) {
		return _serializerFactory.GetSerializer(objectType);
	}

	protected int SanitizeContainerClusterSize(int clusterSizeB)
		=> Tools.Values.ClipValue(clusterSizeB, 256, 8192);


	public void Dispose() {
		_streamContainer?.Dispose();
	}

}
