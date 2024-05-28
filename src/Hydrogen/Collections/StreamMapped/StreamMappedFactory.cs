using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen;

public static class StreamMappedFactory {

	#region List

	public static IStreamMappedList<TItem> CreateList<TItem>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		long reservedStreams = 0,
		string optionalItemChecksumIndexName = null,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) {
		var streamContainer = new ClusteredStreams(
				rootStream, 
				clusterSize,
				policy,
				reservedStreams,
				endianness, 
				false
			);

		var list = CreateList(
			streamContainer,
			itemSerializer, 
			itemComparer, 
			itemChecksummer,
			optionalItemChecksumIndexName,
			autoLoad
		);
		list.ObjectStream.OwnsStreams = true;
		return list;
	}

	public static IStreamMappedList<TItem> CreateList<TItem>(
		ClusteredStreams streams,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		string optionalItemChecksumIndexName = null,
		bool autoLoad = false
	) {
		var container = CreateListContainer(
			streams,
			itemSerializer,
			itemChecksummer,
			optionalItemChecksumIndexName
		);

		var list = new StreamMappedList<TItem>(
			container,
			optionalItemChecksumIndexName,
			itemComparer,
			autoLoad
		) {
			OwnsContainer = true
		};
		
		return list;
	}

	#endregion

	#region RecyclableList

	public static IStreamMappedRecyclableList<TItem> CreateRecyclableList<TItem>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		long reservedStreams = 1,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string optionalItemChecksumIndexName = null,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) {
		var list = CreateRecyclableList(
		new ClusteredStreams(
				rootStream, 
				clusterSize,
				policy,
				reservedStreams,
				endianness, 
				false
			),
			itemSerializer, 
			itemComparer, 
			itemChecksummer,
			recyclableIndexIndexName,
			optionalItemChecksumIndexName,
			autoLoad
		);
		list.ObjectStream.OwnsStreams = true;
		return list;
	}

	public static IStreamMappedRecyclableList<TItem> CreateRecyclableList<TItem>(
		ClusteredStreams streams,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string optionalItemChecksumIndexName = null,
		bool autoLoad = false
	) {
		var list = new StreamMappedRecyclableList<TItem>(
			BuildRecyclableListContainer(
				streams,
				itemSerializer,
				itemChecksummer,
				recyclableIndexIndexName,
				optionalItemChecksumIndexName
			),
			recyclableIndexIndexName,
			optionalItemChecksumIndexName,
			itemComparer,
			autoLoad
		);
		list.OwnsContainer = true;
		return list;
	}

	#endregion

	#region Dictionary

	public static IStreamMappedDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(
		Stream stream,
		IItemSerializer<TKey> keySerializer,
		IItemSerializer<TValue> valueSerializer,
		IItemChecksummer<TKey> keyChecksum,
		IEqualityComparer<TKey> keyComparer,
		IEqualityComparer<TValue> valueComparer,
		int clusterSize = HydrogenDefaults.ClusterSize,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		long reservedStreams = 2,
		string recylableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyIndexName = null,
		Endianness endianness = HydrogenDefaults.Endianness,
		bool readOnly = false,
		bool autoLoad = false,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		keySerializer ??= ItemSerializer<TKey>.Default;
		valueSerializer ??= ItemSerializer<TValue>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		valueComparer ??= EqualityComparer<TValue>.Default;

		// ensure it can serialize null's
		if (!typeof(TValue).IsValueType)
			Guard.Ensure(valueSerializer.SupportsNull, $"Value serializer {valueSerializer.GetType().ToStringCS()} does not support null values");

		var useCLK = false;
		switch (implementation) {
			case StreamMappedDictionaryImplementation.Auto:
				// if key serializer is static, and less than or equal to 256 bytes, use fixed length key implementation
				if (keySerializer.IsConstantSize && keySerializer.ConstantSize <= 256) {
					useCLK = true;
				} else if (keyChecksum is not null) {
					useCLK = false;
				} else {
					throw new InvalidOperationException($"Unable to decide internal implementation. Either enable pass through a constant-length key serializer equal to or under 256b or provide a key checksummer.");
				}

				break;
			case StreamMappedDictionaryImplementation.KeyValueListBased:
				Guard.Ensure(keyChecksum != null, $"Argument {nameof(keyChecksum)} must be provided when activating implementation {implementation}");
				useCLK = false;
				break;
			case StreamMappedDictionaryImplementation.ConstantLengthKeyBased:
				Guard.Ensure(keySerializer.IsConstantSize, $"Argument {nameof(keySerializer)} must be provided when activating implementation {implementation}");
				useCLK = true;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(implementation), implementation, null);
		}
		if (useCLK) {
			return CreateDictionaryClk(
				stream,
				clusterSize,
				keySerializer,
				valueSerializer,
				keyComparer,
				valueComparer,
				policy,
				endianness,
				autoLoad,
				reservedStreams,
				recylableIndexIndexName,
				keyIndexName ?? HydrogenDefaults.DefaultKeyStoreAttachmentName
			);
		} else {
			return CreateDictionaryKvp(
				stream,
				clusterSize,
				keySerializer,
				valueSerializer,
				keyComparer,
				valueComparer,
				keyChecksum,
				policy,
				endianness,
				autoLoad,
				reservedStreams,
				recylableIndexIndexName,
				keyIndexName ?? HydrogenDefaults.DefaultKeyChecksumIndexName
			);
		}

	}


	#endregion

	#region KVP-Dictionary

	public static StreamMappedDictionary<TKey, TValue> CreateDictionaryKvp<TKey, TValue>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TKey> keySerializer = null,
		IItemSerializer<TValue> valueSerializer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		IItemChecksummer<TKey> keyChecksummer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false,
		long reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyChecksumIndexName = HydrogenDefaults.DefaultKeyChecksumIndexName
	) {
		var dict = CreateDictionaryKvp (
			new ClusteredStreams(
				rootStream,
				clusterSize,
				policy,
				reservedStreamCount,
				endianness,
				false
			),
			keySerializer,
			valueSerializer,
			keyComparer,
			valueComparer,
			keyChecksummer,
			autoLoad,
			recyclableIndexIndexName,
			keyChecksumIndexName
		);
		dict.ObjectStream.OwnsStreams = true;
		return dict;
	}

	public static StreamMappedDictionary<TKey, TValue> CreateDictionaryKvp<TKey, TValue>(
		ClusteredStreams streams,
		IItemSerializer<TKey> keySerializer = null,
		IItemSerializer<TValue> valueSerializer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		IItemChecksummer<TKey> keyChecksummer = null,
		bool autoLoad = false,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyChecksumIndexName = HydrogenDefaults.DefaultKeyChecksumIndexName
	) {

		var container = CreateKvpObjectContainer(
				streams,
				keySerializer ?? ItemSerializer<TKey>.Default,
				valueSerializer ?? ItemSerializer<TValue>.Default,
				keyChecksummer ?? new ItemDigestor<TKey>(keySerializer, streams.Endianness),
				keyComparer ?? EqualityComparer<TKey>.Default,
				recyclableIndexIndexName,
				keyChecksumIndexName
			);

		var dict = new StreamMappedDictionary<TKey, TValue>(
			container,
			recyclableIndexIndexName,
			keyChecksumIndexName,
			valueComparer,
			autoLoad
		);
		dict.OwnsContainer = true;
		return dict;
	}



	#endregion

	#region CLK-Dictionary

	public static StreamMappedDictionaryCLK<TKey, TValue> CreateDictionaryClk<TKey, TValue>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TKey> constantLengthKeySerializer,
		IItemSerializer<TValue> valueSerializer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false,
		long reservedStreamCount = 2,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyStoreName = HydrogenDefaults.DefaultKeyStoreAttachmentName
	) {
		var container = new ClusteredStreams(
			rootStream,
			clusterSize,
			policy,
			reservedStreamCount,
			endianness,
			false
		);

		var dict = CreateDictionaryClk(
			container,
			constantLengthKeySerializer,
			valueSerializer,
			keyComparer,
			valueComparer,
			autoLoad,
			recyclableIndexIndexName,
			keyStoreName
		);

		dict.ObjectStream.OwnsStreams = true;
		
		return dict;
	}

	public static StreamMappedDictionaryCLK<TKey, TValue> CreateDictionaryClk<TKey, TValue>(
		ClusteredStreams streams,
		IItemSerializer<TKey> constantLengthKeySerializer,
		IItemSerializer<TValue> valueSerializer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		bool autoLoad = false,
		string recyclableIndexIndexName = HydrogenDefaults.DefaultReyclableIndexIndexName,
		string keyStoreName = HydrogenDefaults.DefaultKeyStoreAttachmentName
	) {
		var container = CreateClkContainer(
			streams,
			constantLengthKeySerializer,
			valueSerializer ?? ItemSerializer<TValue>.Default,
			keyComparer ?? EqualityComparer<TKey>.Default,
			recyclableIndexIndexName,
			keyStoreName
		);
		
		var dict = new StreamMappedDictionaryCLK<TKey, TValue>(
			container,
			recyclableIndexIndexName,
			keyStoreName,
			valueComparer,
			autoLoad
		);
		dict.OwnsContainer = true;
		return dict;
	}

	#endregion

	#region StreamMappedHashSet
	public static StreamMappedHashSet<TItem> CreateHashSet<TItem>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		CHF chf,
		IEqualityComparer<TItem> comparer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		Endianness endianness = Endianness.LittleEndian
	) => CreateHashSet(
		rootStream,
		clusterSize,
		serializer,
		new ItemDigestor<TItem>(chf, serializer),
		comparer,
		policy,
		endianness
	);

	public static StreamMappedHashSet<TItem> CreateHashSet<TItem>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> serializer,
		IItemHasher<TItem> hasher,
		IEqualityComparer<TItem> comparer = null,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		Endianness endianness = Endianness.LittleEndian
	) => new (
		CreateDictionaryClk(
			rootStream,
			clusterSize,
			new ConstantSizeByteArraySerializer(hasher.DigestLength).AsNullableConstantSize(),
			serializer,
			new ByteArrayEqualityComparer(),
			comparer,
			policy,
			endianness
		),
		comparer,
		hasher
	);

	#endregion

	#region Object Container

	internal static ObjectStream<KeyValuePair<TKey, TValue>> CreateKvpObjectContainer<TKey, TValue>(
		ClusteredStreams streams,
		IItemSerializer<TKey> keySerializer,
		IItemSerializer<TValue> valueSerializer,
		IItemChecksummer<TKey> keyChecksummer,
		IEqualityComparer<TKey> keyComparer,
		string recyclableIndexIndexName,
		string keyChecksumIndexName
	) {
		Guard.ArgumentNotNull(streams, nameof(streams));
		Guard.ArgumentNotNull(keySerializer, nameof(keySerializer));
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.ArgumentNotNull(keyChecksummer, nameof(keyChecksummer));

		// Create object objectStream
		var container = new ObjectStream<KeyValuePair<TKey, TValue>>(
			streams,
			new KeyValuePairSerializer<TKey, TValue>(
				keySerializer ?? ItemSerializer<TKey>.Default,
				valueSerializer ?? ItemSerializer<TValue>.Default
			),
			streams.Policy.HasFlag(ClusteredStreamsPolicy.FastAllocate)
		);

		// Create free-index store
		var recyclableIndexIndex = new RecyclableIndexIndex(
			container,
			recyclableIndexIndexName
		);
		container.Streams.RegisterAttachment(recyclableIndexIndex);

		// Create key checksum index (for fast key lookups)
		var keyChecksumKeyIndex = IndexFactory.CreateProjectionChecksumIndex(
			container, 
			keyChecksumIndexName, 
			kvp => kvp.Key, 
			keySerializer, 
			keyChecksummer, 
			ReadKey, 
			keyComparer
		);
		container.Streams.RegisterAttachment(keyChecksumKeyIndex);

		return container;

		TKey ReadKey(long index) {
			using (container.EnterAccessScope()) {
				var traits = container.GetItemDescriptor(index).Traits;
				if (traits.HasFlag(ClusteredStreamTraits.Reaped))
					throw new InvalidOperationException($"Object {index} has been reaped");

				using var stream = container.Streams.OpenRead(container.Streams.Header.ReservedStreams + index);
				var reader = new EndianBinaryReader(EndianBitConverter.For(container.Streams.Endianness), stream);
				return ((KeyValuePairSerializer<TKey, TValue>)container.ItemSerializer).DeserializeKey(reader);
			}
		}

	}

	internal static ObjectStream<TValue> CreateClkContainer<TKey, TValue>(
		ClusteredStreams streams,
		IItemSerializer<TKey> constantLengthKeySerializer,
		IItemSerializer<TValue> valueSerializer,
		IEqualityComparer<TKey> keyComparer,
		string recyclableIndexIndexName,
		string keyStoreName
	) {
		Guard.ArgumentNotNull(streams, nameof(streams));
		Guard.ArgumentNotNull(constantLengthKeySerializer, nameof(constantLengthKeySerializer));
		Guard.Argument(constantLengthKeySerializer.IsConstantSize, nameof(constantLengthKeySerializer), "Keys must be statically sized");
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));

		var container = new ObjectStream<TValue>(
			streams, 
			valueSerializer,
			streams.Policy.HasFlag(ClusteredStreamsPolicy.FastAllocate)
		);

		var recyclableIndexIndex = new RecyclableIndexIndex(
			container,
			recyclableIndexIndexName
		);
		container.Streams.RegisterAttachment(recyclableIndexIndex);

		var keyStore = new UniqueKeyStorageAttachment<TKey>(
			container.Streams,
			keyStoreName,
			constantLengthKeySerializer,
			keyComparer
		);
		container.Streams.RegisterAttachment(keyStore);
		
		
		return container;
	}

	private static ObjectStream<TItem> CreateListContainer<TItem>(
		ClusteredStreams streams,
		IItemSerializer<TItem> itemSerializer,
		IItemChecksummer<TItem> itemChecksummer,
		string optionalItemChecksumIndexName
	) {
		if (!string.IsNullOrEmpty(optionalItemChecksumIndexName)) 
			Guard.Argument(itemChecksummer is not null, nameof(itemChecksummer), $"Must be provided when specifying a checksum index"); 

		var container = new ObjectStream<TItem>(
			streams, 
			itemSerializer, 
			streams.Policy.HasFlag(ClusteredStreamsPolicy.FastAllocate)
		);

		if (!string.IsNullOrEmpty(optionalItemChecksumIndexName)) {
			Guard.Ensure(itemChecksummer is not null);
			var itemChecksumIndex = new ProjectionIndex<TItem, int>(
				container,
				optionalItemChecksumIndexName,
				itemChecksummer.CalculateChecksum,
				PrimitiveSerializer<int>.Instance,
				EqualityComparer<int>.Default
			);
			container.Streams.RegisterAttachment(itemChecksumIndex);
		} 

		return container;
	}

	private static ObjectStream<TItem> BuildRecyclableListContainer<TItem>(
		ClusteredStreams streams,
		IItemSerializer<TItem> itemSerializer,
		IItemChecksummer<TItem> itemChecksummer,
		string recyclableIndexName,
		string optionalItemChecksumIndexName
	) {
		if (!string.IsNullOrEmpty(optionalItemChecksumIndexName)) 
			Guard.Argument(itemChecksummer is not null, nameof(itemChecksummer), $"Must be provided when specifying a checksum index"); 

		var container = new ObjectStream<TItem>(
			streams, 
			itemSerializer, 
			streams.Policy.HasFlag(ClusteredStreamsPolicy.FastAllocate)
		);

		// Create free-index store
		var recyclableIndexIndex = new RecyclableIndexIndex(
			container,
			recyclableIndexName
		);
		container.Streams.RegisterAttachment(recyclableIndexIndex);

		// Create item checksum index (if applicable)
		if (!string.IsNullOrEmpty(optionalItemChecksumIndexName)) {
			Guard.Ensure(itemChecksummer is not null); 
			var checksumKeyIndex = new ProjectionIndex<TItem, int>(
				container,
				optionalItemChecksumIndexName,
				itemChecksummer.CalculateChecksum,
				PrimitiveSerializer<int>.Instance,
				EqualityComparer<int>.Default
			);
			container.Streams.RegisterAttachment(checksumKeyIndex);
		}

		return container;
	}

	#endregion
}
