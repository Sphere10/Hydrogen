using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Hydrogen.ObjectSpaces;
using static Hydrogen.AMS;
using Tools;

namespace Hydrogen;

public static class StreamMappedFactory {

	#region List

	public static IStreamMappedList<TItem> CreateList<TItem>(
		Stream rootStream,
		int clusterSize,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 0,
		long checksumIndexStreamIndex = 0,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) {
		var streamContainer = new StreamContainer(
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
			checksumIndexStreamIndex,
			autoLoad
		);
		list.ObjectContainer.OwnsStreamContainer = true;
		return list;
	}

	public static IStreamMappedList<TItem> CreateList<TItem>(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		long checksumIndexStreamIndex = 0,
		bool autoLoad = false
	) {
		var container = CreateListContainer(
			streamContainer,
			itemSerializer,
			itemChecksummer,
			checksumIndexStreamIndex,
			out var checksumIndex
		);

		var list = new StreamMappedList<TItem>(
			container,
			checksumIndex,
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
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 1,
		long freeIndexStoreStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		Endianness endianness = Endianness.LittleEndian, 
		bool autoLoad = false
	) {
		var list = CreateRecyclableList(
		new StreamContainer(
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
			freeIndexStoreStreamIndex,
			checksumIndexStreamIndex,
			autoLoad
		);
		list.ObjectContainer.OwnsStreamContainer = true;
		return list;
	}

	public static IStreamMappedRecyclableList<TItem> CreateRecyclableList<TItem>(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer = null,
		IEqualityComparer<TItem> itemComparer = null,
		IItemChecksummer<TItem> itemChecksummer = null,
		long freeIndexStoreStreamIndex = 0,
		long checksumIndexStreamIndex = 1,
		bool autoLoad = false
	) {
		var list = new StreamMappedRecyclableList<TItem>(
			BuildRecyclableListContainer(
				streamContainer,
				itemSerializer,
				itemChecksummer,
				freeIndexStoreStreamIndex,
				checksumIndexStreamIndex,
				out var freeIndexStore,
				out var checksumIndex
			),
			freeIndexStore,
			checksumIndex,
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
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		long reservedStreams = 2,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1,
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
		valueSerializer = valueSerializer.AsReferenceSerializer();

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
				freeIndexStoreStreamIndex,
				keyChecksumIndexStreamIndex
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
				freeIndexStoreStreamIndex,
				keyChecksumIndexStreamIndex
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
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false,
		long reservedStreamCount = 2,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1
	) {
		var dict = CreateDictionaryKvp (
			new StreamContainer(
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
			freeIndexStoreStreamIndex,
			keyChecksumIndexStreamIndex
		);
		dict.ObjectContainer.OwnsStreamContainer = true;
		return dict;
	}

	public static StreamMappedDictionary<TKey, TValue> CreateDictionaryKvp<TKey, TValue>(
		StreamContainer streamContainer,
		IItemSerializer<TKey> keySerializer = null,
		IItemSerializer<TValue> valueSerializer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		IItemChecksummer<TKey> keyChecksummer = null,
		bool autoLoad = false,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1
	) {

		var container = CreateKvpObjectContainer(
				streamContainer,
				keySerializer ?? ItemSerializer<TKey>.Default,
				valueSerializer ?? ItemSerializer<TValue>.Default,
				keyChecksummer ?? new ItemDigestor<TKey>(keySerializer, streamContainer.Endianness),
				keyComparer ?? EqualityComparer<TKey>.Default,
				freeIndexStoreStreamIndex,
				keyChecksumIndexStreamIndex,
				out var freeIndexStore,
				out var keyChecksumIndex
			);

		var dict = new StreamMappedDictionary<TKey, TValue>(
			container,
			freeIndexStore,
			keyChecksumIndex,
			keyComparer,
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
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false,
		long reservedStreamCount = 2,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1
	) {
		var container = new StreamContainer(
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
			freeIndexStoreStreamIndex,
			keyChecksumIndexStreamIndex
		);

		dict.ObjectContainer.OwnsStreamContainer = true;
		
		return dict;
	}

	public static StreamMappedDictionaryCLK<TKey, TValue> CreateDictionaryClk<TKey, TValue>(
		StreamContainer streamContainer,
		IItemSerializer<TKey> constantLengthKeySerializer,
		IItemSerializer<TValue> valueSerializer = null,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		bool autoLoad = false,
		long freeIndexStoreStreamIndex = 0,
		long keyStoreStreamIndex = 1
	) {
		var container = CreateClkContainer(
			streamContainer,
			constantLengthKeySerializer,
			valueSerializer ?? ItemSerializer<TValue>.Default,
			keyComparer ?? EqualityComparer<TKey>.Default,
			freeIndexStoreStreamIndex,
			keyStoreStreamIndex,
			out var keyStore,
			out var freeIndexStore
		);
		
		var dict = new StreamMappedDictionaryCLK<TKey, TValue>(
			container,
			freeIndexStore,
			keyStore,
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
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
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
		StreamContainerPolicy policy = StreamContainerPolicy.Default,
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

	internal static ObjectContainer<KeyValuePair<TKey, TValue>> CreateKvpObjectContainer<TKey, TValue>(
		StreamContainer streamContainer,
		IItemSerializer<TKey> keySerializer,
		IItemSerializer<TValue> valueSerializer,
		IItemChecksummer<TKey> keyChecksummer,
		IEqualityComparer<TKey> keyComparer,
		long freeIndexStoreStreamIndex,
		long keyChecksumIndexStreamIndex,
		out RecyclableIndexIndex recyclableIndexIndex,
		out KeyChecksumIndex<KeyValuePair<TKey, TValue>, TKey> keyChecksumKeyIndex
	) {
		Guard.ArgumentNotNull(streamContainer, nameof(streamContainer));
		Guard.ArgumentNotNull(keySerializer, nameof(keySerializer));
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.ArgumentNotNull(keyChecksummer, nameof(keyChecksummer));

		// Create object container
		var container = new ObjectContainer<KeyValuePair<TKey, TValue>>(
			streamContainer,
			new KeyValuePairSerializer<TKey, TValue>(
				keySerializer ?? ItemSerializer<TKey>.Default,
				valueSerializer ?? ItemSerializer<TValue>.Default
			),
			streamContainer.Policy.HasFlag(StreamContainerPolicy.FastAllocate)
		);

		// Create free-index store
		recyclableIndexIndex = new RecyclableIndexIndex(
			container,
			freeIndexStoreStreamIndex
		);
		container.RegisterAttachment(recyclableIndexIndex);

		// Create key checksum index (for fast key lookups)
		keyChecksumKeyIndex = container.AddChecksumKeyIndex(kvp => kvp.Key, keyChecksummer, keySerializer, ReadKey, keyComparer, keyChecksumIndexStreamIndex);

		return container;

		TKey ReadKey(long index) {
			using (container.EnterAccessScope()) {
				var traits = container.GetItemDescriptor(index).Traits;
				if (traits.HasFlag(ClusteredStreamTraits.Reaped))
					throw new InvalidOperationException($"Object {index} has been reaped");

				using var stream = container.StreamContainer.OpenRead(container.StreamContainer.Header.ReservedStreams + index);
				var reader = new EndianBinaryReader(EndianBitConverter.For(container.StreamContainer.Endianness), stream);
				return ((KeyValuePairSerializer<TKey, TValue>)container.ItemSerializer).DeserializeKey(reader);
			}
		}

	}

	internal static ObjectContainer<TValue> CreateClkContainer<TKey, TValue>(
		StreamContainer streamContainer,
		IItemSerializer<TKey> constantLengthKeySerializer,
		IItemSerializer<TValue> valueSerializer,
		IEqualityComparer<TKey> keyComparer,
		long freeIndexStoreStreamIndex,
		long keyStoreStreamIndex,
		out UniqueKeyStore<TKey> keyStore,
		out RecyclableIndexIndex recyclableIndexIndex
	) {
		Guard.ArgumentNotNull(streamContainer, nameof(streamContainer));
		Guard.ArgumentNotNull(constantLengthKeySerializer, nameof(constantLengthKeySerializer));
		Guard.Argument(constantLengthKeySerializer.IsConstantSize, nameof(constantLengthKeySerializer), "Keys must be statically sized");
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));

		var container = new ObjectContainer<TValue>(
			streamContainer, 
			valueSerializer,
			streamContainer.Policy.HasFlag(StreamContainerPolicy.FastAllocate)
		);

		recyclableIndexIndex = new RecyclableIndexIndex(
			container,
			freeIndexStoreStreamIndex
		);
		container.RegisterAttachment(recyclableIndexIndex);

		keyStore = new UniqueKeyStore<TKey>(
			container,
			keyStoreStreamIndex,
			keyComparer,
			constantLengthKeySerializer
		);
		container.RegisterAttachment(keyStore);
		
		
		return container;
	}

	private static ObjectContainer<TItem> CreateListContainer<TItem>(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer,
		IItemChecksummer<TItem> itemChecksummer,
		long checksumIndexStreamIndex,
		out KeyIndex<TItem, int> checksumKeyIndex
	) {
		var container = new ObjectContainer<TItem>(
			streamContainer, 
			itemSerializer, 
			streamContainer.Policy.HasFlag(StreamContainerPolicy.FastAllocate)
		);

		if (itemChecksummer is not null) {
			checksumKeyIndex = new KeyIndex<TItem, int>(
				container,
				checksumIndexStreamIndex,
				itemChecksummer.CalculateChecksum,
				EqualityComparer<int>.Default,
				PrimitiveSerializer<int>.Instance
			);
			container.RegisterAttachment( checksumKeyIndex);
		} else {
			checksumKeyIndex = null;
		}

		return container;
	}

	private static ObjectContainer<TItem> BuildRecyclableListContainer<TItem>(
		StreamContainer streamContainer,
		IItemSerializer<TItem> itemSerializer,
		IItemChecksummer<TItem> itemChecksummer,
		long freeIndexStoreStreamIndex,
		long checksumIndexStreamIndex,
		out RecyclableIndexIndex recyclableIndexIndex,
		out KeyIndex<TItem, int> checksumKeyIndex
	) {
		var container = new ObjectContainer<TItem>(
			streamContainer, 
			itemSerializer, 
			streamContainer.Policy.HasFlag(StreamContainerPolicy.FastAllocate)
		);

		// Create free-index store
		recyclableIndexIndex = new RecyclableIndexIndex(
			container,
			freeIndexStoreStreamIndex
		);
		container.RegisterAttachment(recyclableIndexIndex);

		// Create item checksum index (if applicable)
		if (itemChecksummer is not null) {
			checksumKeyIndex = new KeyIndex<TItem, int>(
				container,
				checksumIndexStreamIndex,
				itemChecksummer.CalculateChecksum,
				EqualityComparer<int>.Default,
				PrimitiveSerializer<int>.Instance
			);
			container.RegisterAttachment( checksumKeyIndex);
		} else {
			checksumKeyIndex = null;
		}

		return container;
	}

	#endregion
}
