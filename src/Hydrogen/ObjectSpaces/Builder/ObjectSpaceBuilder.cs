using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpaceBuilder {

	private static readonly string ErrMsgUseSerializerFactory = $"Unable to use a custom {nameof(SerializerFactory)} since {typeof(IItemSerializer<>).ToStringCS()}'s have already been registered. Ensure all serializer registrations are made to your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUseComparerFactory = $"Unable to use a custom {nameof(ComparerFactory)} since {typeof(IEqualityComparer).ToStringCS()}'s have already been registered. Ensure all comparer registrations are made to your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUsingSerializer = $"Unable to use a custom {typeof(IItemSerializer<>).ToStringCS()} since a custom {nameof(SerializerFactory)} has been registered. Ensure all serializer registrations are made with your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUsingEqualityComparer = $"Unable to use a custom {typeof(IEqualityComparer<>).ToStringCS()} since a custom {nameof(ComparerFactory)} has been registered. Ensure all comparer registrations are made with your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";
	private static readonly string ErrMsgUsingComparer = $"Unable to use a custom {typeof(IComparer<>).ToStringCS()} since a custom {nameof(ComparerFactory)} has been registered. Ensure all comparer registrations are made with your custom factory and not with this {nameof(ObjectSpaceBuilder)}.";

	private string _filepath;
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

	public ObjectSpaceBuilder() {
		_filepath = null;
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
	}

	public ObjectSpaceBuilder UseFile(string filePath) {
		_filepath = filePath;
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

	public ObjectSpaceBuilder Merkleized() {
		_merkleized = true;
		foreach(var dimension in _dimensions) {
			if (!dimension.Indexes.Any(x => x.Type == ObjectSpaceDefinition.IndexType.MerkleTree))
				dimension.Merkleized();
		}
		return this;
	}

	public ObjectSpaceBuilder UseSerializerFactory(SerializerFactory factory) {
		Guard.ArgumentNotNull(factory, nameof(factory));
		Guard.Against(_specifiedCustomSerializer, ErrMsgUseSerializerFactory);
		_usingCustomSerializerFactory = true;
		return this;
	}

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

	public ObjectSpaceBuilder UseComparerFactory(ComparerFactory factory) {
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

	public ObjectSpaceDimensionBuilder<T> AddDimension<T>() {
		var dimensionBuilder = new ObjectSpaceDimensionBuilder<T>(this);
		_dimensions.Add(dimensionBuilder);
		return dimensionBuilder;
	}
	
	public ObjectSpaceDefinition BuildDefinition() {
		var definition = new ObjectSpaceDefinition {
			Merkleized = _merkleized,
			HashFunction = _hashFunction,
			Dimensions = _dimensions.Select(x => x.BuildDefinition()).ToArray()
		};
		definition.Validate().ThrowOnFailure();
		return definition;
	}

	public ObjectSpace Build() {
		var definition = BuildDefinition();
		var fileDescriptor = HydrogenFileDescriptor.From(_filepath, _pagesPath, _pageSize, _maxMemory, _clusterSize, _clusteredStreamsPolicy, _endianness);
		return new ObjectSpace(fileDescriptor, definition, _serializerFactory, _comparerFactory, _accessMode);

	}

}
