// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpaceBuilder {

	private static readonly string ErrMsgUseSerializerFactory = $"Unable to use a custom {nameof(SerializerFactory)} since {typeof(IItemSerializer<>).ToStringCS()}'s have already been registered. Ensure all serializer registrations are made to your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUseComparerFactory = $"Unable to use a custom {nameof(ComparerFactory)} since {typeof(IEqualityComparer).ToStringCS()}'s have already been registered. Ensure all comparer registrations are made to your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUsingSerializer = $"Unable to use a custom {typeof(IItemSerializer<>).ToStringCS()} since a custom {nameof(SerializerFactory)} has been registered. Ensure all serializer registrations are made with your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUsingEqualityComparer = $"Unable to use a custom {typeof(IEqualityComparer<>).ToStringCS()} since a custom {nameof(ComparerFactory)} has been registered. Ensure all comparer registrations are made with your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUsingComparer = $"Unable to use a custom {typeof(IComparer<>).ToStringCS()} since a custom {nameof(ComparerFactory)} has been registered. Ensure all comparer registrations are made with your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";

	private ObjectSpaceType? _type;
	private string _filepath;
	private MemoryStream _memoryStream;
	private string _pagesPath;
	private long _maxMemory;
	private long _pageSize;
	private int _clusterSize;
	private bool _merkleized;
	private CHF _hashFunction;
	private ClusteredStreamsPolicy _clusteredStreamsPolicy;
	private Endianness _endianness;
	private FileAccessMode _accessMode;
	private IList<IObjectSpaceDimensionBuilder> _dimensions;
	private SerializerFactory _serializerFactory;
	private bool _usingCustomSerializerFactory;
	private bool _specifiedCustomSerializer;
	private ComparerFactory _comparerFactory;
	private bool _usingCustomComparerFactory;
	private bool _specifiedCustomComparer;
	private bool _autoSave;

	public ObjectSpaceBuilder() {
		_type = null;
		_filepath = null;
		_memoryStream = null;
		_pagesPath = HydrogenDefaults.TransactionalPageFolder;
		_maxMemory = HydrogenDefaults.MaxMemoryPerCollection;
		_pageSize = HydrogenDefaults.TransactionalPageSize;
		_clusterSize = HydrogenDefaults.ClusterSize;
		_merkleized = false;
		_hashFunction = HydrogenDefaults.HashFunction;
		_clusteredStreamsPolicy = HydrogenDefaults.ContainerPolicy;
		_endianness = HydrogenDefaults.Endianness;
		_accessMode = FileAccessMode.OpenOrCreate;
		_dimensions = new List<IObjectSpaceDimensionBuilder>();

		_serializerFactory = new SerializerFactory(SerializerFactory.Default);
		_usingCustomSerializerFactory = false;
		_specifiedCustomSerializer = false;

		_comparerFactory = new ComparerFactory(ComparerFactory.Default);
		_usingCustomComparerFactory = false;
		_specifiedCustomComparer = false;

		_autoSave = false;
	}

	public ObjectSpaceBuilder UseFile(string filePath) {
		_type = ObjectSpaceType.FileMapped;
		_filepath = filePath;
		return this;
	}

	public ObjectSpaceBuilder UseMemoryStream(MemoryStream stream = null) {
		_type = ObjectSpaceType.MemoryMapped;
		_memoryStream = stream;
		return this;
	}

	public ObjectSpaceBuilder KeepUncommittedPagesAt(string folder) {
		_pagesPath = folder;
		return this;
	}

	public ObjectSpaceBuilder UsingHashFunction(CHF hashFunction) {
		_hashFunction = hashFunction;
		return this;
	}

	public ObjectSpaceBuilder ReadOnly() {
		_accessMode.SetFlags(FileAccessMode.Append, false);
		_accessMode.SetFlags(FileAccessMode.Truncate, false);
		_accessMode.SetFlags(FileAccessMode.Write, false);
		return this;
	}

	public ObjectSpaceBuilder AutoLoad() {
		_accessMode.SetFlags(FileAccessMode.AutoLoad, true);
		return this;
	}

	public ObjectSpaceBuilder WithMaxMemory(long bytes) {
		_maxMemory = bytes;
		return this;
	}

	public ObjectSpaceBuilder WithPageSize(long bytes) {
		_pageSize = bytes;
		return this;
	}

	public ObjectSpaceBuilder WithClusterSize(int bytes) {
		_clusterSize = bytes;
		return this;
	}	

	public ObjectSpaceBuilder WithClusteredStreamsPolicy(ClusteredStreamsPolicy clusteredStreamsPolicy) {
		_clusteredStreamsPolicy = clusteredStreamsPolicy;
		return this;
	}

	public ObjectSpaceBuilder WithEndianness(Endianness endianness) {
		_endianness = endianness;
		return this;
	}

	public ObjectSpaceBuilder AutoSave() {
		_autoSave = true;
		return this;
	}

	public ObjectSpaceBuilder Merkleized() {
		_merkleized = true;
		foreach(var dimension in _dimensions) {
			if (!dimension.Indexes.Any(x => x.Type == ObjectSpaceDefinition.IndexType.MerkleTree))
				dimension.Merkleized();
		}
		return this;
	}

	public ObjectSpaceBuilder UsingSerializerFactory(SerializerFactory factory) {
		Guard.ArgumentNotNull(factory, nameof(factory));
		Guard.Against(_specifiedCustomSerializer, ErrMsgUseSerializerFactory);
		_usingCustomSerializerFactory = true;
		return this;
	}

	public bool ContainsSerializer<TItem>() => ContainsSerializer(typeof(TItem));

	public bool ContainsSerializer(Type type) => _serializerFactory.ContainsSerializer(type);

	public ObjectSpaceBuilder UsingSerializer<TItem, TItemSerializer>() where TItemSerializer : IItemSerializer<TItem>, new() {
		Guard.Against(_usingCustomSerializerFactory, ErrMsgUsingSerializer);
		_specifiedCustomSerializer = true;
		_serializerFactory.Register<TItem, TItemSerializer>();
		return this;
	}

	public ObjectSpaceBuilder UsingSerializer<TItem>(IItemSerializer<TItem> serializer) {
		Guard.Against(_usingCustomSerializerFactory, ErrMsgUsingSerializer);
		_specifiedCustomSerializer = true;
		_serializerFactory.Register(serializer);
		return this;
	}

	public ObjectSpaceBuilder MakeSerializer<TItem>() => MakeSerializer(typeof(TItem));

	public ObjectSpaceBuilder MakeSerializer(Type type) {
		Guard.Ensure(!_serializerFactory.ContainsSerializer(type), $"Serializer for type {type.ToStringCS()} has already been registered.");
		_specifiedCustomSerializer = true;
		_serializerFactory.RegisterAutoBuild(type);
		return this;
	}

	public ObjectSpaceBuilder UsingComparerFactory(ComparerFactory factory) {
		Guard.ArgumentNotNull(factory, nameof(factory));
		Guard.Against(_specifiedCustomComparer, ErrMsgUseComparerFactory);
		_usingCustomComparerFactory = true;
		return this;
	}

	public ObjectSpaceBuilder UsingEqualityComparer<TItem, TItemComparer>() where TItemComparer : IEqualityComparer<TItem>, new() {
		Guard.Against(_usingCustomComparerFactory, ErrMsgUsingEqualityComparer);
		_specifiedCustomComparer = true;
		_comparerFactory.RegisterEqualityComparer<TItem, TItemComparer>();
		return this;
	}

	public ObjectSpaceBuilder UsingEqualityComparer<TItem>(IEqualityComparer<TItem> comparer) {
		Guard.Against(_usingCustomComparerFactory, ErrMsgUsingEqualityComparer);
		_specifiedCustomComparer = true;
		_comparerFactory.RegisterEqualityComparer(comparer);
		return this;
	}

	public ObjectSpaceBuilder UsingEqualityComparer(Type type, object comparer) {
		Guard.Against(_usingCustomComparerFactory, ErrMsgUsingComparer);
		_specifiedCustomComparer = true;
		_comparerFactory.RegisterEqualityComparer(type, comparer);
		return this;
	}

	public ObjectSpaceBuilder UsingComparer<TItem, TComparer>() where TComparer : IComparer<TItem>, new() {
		Guard.Against(_usingCustomComparerFactory, ErrMsgUsingComparer);
		_specifiedCustomComparer = true;
		_comparerFactory.RegisterComparer<TItem, TComparer>();
		return this;
	}

	public ObjectSpaceBuilder UsingComparer<TItem>(IComparer<TItem> comparer) {
		Guard.Against(_usingCustomComparerFactory, ErrMsgUsingComparer);
		_specifiedCustomComparer = true;
		_comparerFactory.RegisterComparer(comparer);
		return this;
	}

	public ObjectSpaceBuilder UsingComparer(Type type, object comparer) {
		Guard.Against(_usingCustomComparerFactory, ErrMsgUsingComparer);
		_specifiedCustomComparer = true;
		_comparerFactory.RegisterComparer(type, comparer);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> AddDimension<T>(bool ignoreAnnotations = false) 
		=> (ObjectSpaceDimensionBuilder<T>)AddDimension(typeof(T), ignoreAnnotations);

	public IObjectSpaceDimensionBuilder AddDimension(Type type, bool ignoreAnnotations = false) {
		var dimensionBuilder = (IObjectSpaceDimensionBuilder)typeof(ObjectSpaceDimensionBuilder<>).MakeGenericType(type).ActivateWithCompatibleArgs(this);
		_dimensions.Add(dimensionBuilder);
		
		if (!ignoreAnnotations) {

			if (type.TryGetCustomAttributeOfType<EqualityComparerAttribute>(false, out var equalityComparerAttribute))
				dimensionBuilder.UsingEqualityComparer(equalityComparerAttribute.EqualityComparerType.ActivateWithCompatibleArgs());

			foreach(var member in SerializerBuilder.GetSerializableMembers(type)) {
				if (member.MemberInfo.TryGetCustomAttributeOfType<IdentityAttribute>(false, out var identityAttribute)) 
					dimensionBuilder.WithIdentifier(member, identityAttribute.IndexName); 

				if (member.MemberInfo.TryGetCustomAttributeOfType<IndexAttribute>(false, out var indexAttribute)) 
					dimensionBuilder.WithIndexOn(member, indexAttribute.IndexName, indexAttribute.NullPolicy); 

				if (member.MemberInfo.TryGetCustomAttributeOfType<UniqueIndexAttribute>(false, out var uniqueIndexAttribute)) 
					dimensionBuilder.WithUniqueIndexOn(member, uniqueIndexAttribute.IndexName, uniqueIndexAttribute.NullPolicy); 
				
			}
		}
		return dimensionBuilder;
	}

	public ObjectSpaceDimensionBuilder<T> Configure<T>() {
		var dim = _dimensions.FirstOrDefault(x => x.ItemType == typeof(T));
		if (dim is null)
			throw new InvalidOperationException($"No dimension for type {typeof(T).ToStringCS()} was found");
		return (ObjectSpaceDimensionBuilder<T>)dim;
	}	
	
	public ObjectSpaceDefinition BuildDefinition() {
		var definition = new ObjectSpaceDefinition(_merkleized, _autoSave) {
			HashFunction = _hashFunction,
			Dimensions = _dimensions.Select(x => x.BuildDefinition()).ToArray()
		};
		definition.Validate().ThrowOnFailure();
		return definition;
	}

	public ObjectSpace Build() {
		var definition = BuildDefinition();
		var fileDescriptor = HydrogenFileDescriptor.From(_filepath, _pagesPath, _pageSize, _maxMemory, _clusterSize, _clusteredStreamsPolicy, _endianness);
		switch(_type) {
			case ObjectSpaceType.FileMapped:
				return new FileObjectSpace(fileDescriptor, definition, _serializerFactory, _comparerFactory, _accessMode);
			case ObjectSpaceType.MemoryMapped:
				return _memoryStream is null ? 
					new MemoryObjectSpace(definition, _serializerFactory, _comparerFactory, _clusterSize, _clusteredStreamsPolicy,  _endianness) : 
					new MemoryObjectSpace(_memoryStream, definition, _serializerFactory, _comparerFactory, _clusterSize, _clusteredStreamsPolicy,  _endianness);
			case null:
				throw new InvalidOperationException("Object Space type has not been specified (file or memory mapped)");
				
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private enum ObjectSpaceType {
		FileMapped,
		MemoryMapped
	}

}
