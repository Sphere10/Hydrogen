// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpace : SyncLoadableBase, ITransactionalObject, IDisposable {

	public event EventHandlerEx<object> Committing { add => _fileStream.Committing += value; remove => _fileStream.Committing -= value; }
	public event EventHandlerEx<object> Committed { add => _fileStream.Committed += value; remove => _fileStream.Committed -= value; }
	public event EventHandlerEx<object> RollingBack { add => _fileStream.RollingBack += value; remove => _fileStream.RollingBack -= value; }
	public event EventHandlerEx<object> RolledBack { add => _fileStream.RolledBack += value; remove => _fileStream.RolledBack -= value; } 

	private readonly TransactionalStream _fileStream;
	private readonly StreamContainer _streamContainer;
	private readonly SerializerFactory _serializerFactory;
	private readonly ComparerFactory _comparerFactory;
	private bool _loaded;

	public ObjectSpace(HydrogenFileDescriptor file, ObjectSpaceDefinition objectSpaceDefinition, SerializerFactory serializerFactory, ComparerFactory comparerFactory, FileAccessMode accessMode = FileAccessMode.Default) {
		Guard.ArgumentNotNull(file, nameof(file));
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));

		File = file;
		Definition = objectSpaceDefinition;
		_serializerFactory = serializerFactory;
		_comparerFactory = comparerFactory;
		_fileStream = new TransactionalStream(
			file,
			accessMode.WithoutAutoLoad()
		);
		_streamContainer = new StreamContainer(
			_fileStream,
			(int)file.ClusterSize,
			file.ContainerPolicy,
			0, // TODO: need to consider schema-level index's like merkle-tree, etc
			Endianness.LittleEndian,
			false
		);
		_loaded = false;
		if (accessMode.HasFlag(FileAccessMode.AutoLoad))
			Load();
	}

	public override bool RequiresLoad => !_loaded || _streamContainer.RequiresLoad;

	public HydrogenFileDescriptor File { get; }

	public ObjectSpaceDefinition Definition { get; }

	public ObjectContainer[] Containers { get; private set; }

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

	public void Commit() => _fileStream.Commit();

	public Task CommitAsync() => _fileStream.CommitAsync();

	public void Rollback() => _fileStream.Rollback();

	public Task RollbackAsync() => _fileStream.RollbackAsync();

	public void Dispose() {
		if (Containers is not null)
			foreach(var container in Containers.Where(x => !x.RequiresLoad)) 
				container.Dispose(); 
		_streamContainer.Dispose();
		_fileStream.Dispose();
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
			var containerStream = _streamContainer.Open(_streamContainer.Header.ReservedStreams + ix, false, true);

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
				IObjectContainerAttachment metaDataObserver = item.Type switch {
					ObjectSpaceDefinition.IndexType.UniqueKey => BuildUniqueKey(container, containerDefinition, item, index),
					ObjectSpaceDefinition.IndexType.Index => BuildIndex(container, containerDefinition, item, index),
					ObjectSpaceDefinition.IndexType.FreeIndexStore => new RecyclableIndexIndex(container, index),
					ObjectSpaceDefinition.IndexType.MerkleTree => throw new NotImplementedException(),
					_ => throw new ArgumentOutOfRangeException()
				};
				container.RegisterAttachment(metaDataObserver);
			}

			// load container
			if (container.RequiresLoad)
				container.Load();

			containers.Add(container);
		}
		Containers = containers.ToArray();
		_loaded = true;

		IObjectContainerAttachment BuildUniqueKey(ObjectContainer container, ObjectSpaceDefinition.ContainerDefinition containerDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
			var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
			var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
			return 
				keySerializer.IsConstantSize ? 
					IndexFactory.CreateUniqueKeyIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) : 
					IndexFactory.CreateUniqueKeyChecksumIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
		}

		IObjectContainerAttachment BuildIndex(ObjectContainer container, ObjectSpaceDefinition.ContainerDefinition containerDefinition, ObjectSpaceDefinition.IndexDefinition indexDefinition, int streamIndex) {
			var keyComparer = _comparerFactory.GetEqualityComparer(indexDefinition.KeyMember.PropertyType);
			var keySerializer = _serializerFactory.GetSerializer(indexDefinition.KeyMember.PropertyType);
			return 
				keySerializer.IsConstantSize ?
					IndexFactory.CreateKeyIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, keyComparer) :
					IndexFactory.CreateKeyChecksumIndexAttachment(container, streamIndex, indexDefinition.KeyMember, keySerializer, null, null, keyComparer);
				
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

}
