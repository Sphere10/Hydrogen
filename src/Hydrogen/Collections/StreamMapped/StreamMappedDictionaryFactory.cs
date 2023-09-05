using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public static class StreamMappedDictionaryFactory {

	public static IStreamMappedDictionary<TKey, TValue> Create<TKey, TValue>(
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

		var useCLK = false;
		switch (implementation) {
			case StreamMappedDictionaryImplementation.Auto:
				// if key serializer is static, and less than or equal to 256 bytes, use fixed length key implementation
				if (keySerializer.IsStaticSize && keySerializer.StaticSize <= 256) {
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
				Guard.Ensure(keySerializer.IsStaticSize, $"Argument {nameof(keySerializer)} must be provided when activating implementation {implementation}");
				useCLK = true;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(implementation), implementation, null);
		}
		if (useCLK) {
			return new StreamMappedDictionaryCLK<TKey, TValue>(
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
			return new StreamMappedDictionary<TKey, TValue>(
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

	public static IStreamMappedDictionary<TKey, TValue> Create<TKey, TValue>(
		ObjectContainer objectContainer,
		IItemSerializer<TKey> keySerializer,
		IEqualityComparer<TKey> keyComparer = null,
		IEqualityComparer<TValue> valueComparer = null,
		IItemChecksummer<TKey> keyChecksum = null,
		bool autoLoad = false,
		long freeIndexStoreStreamIndex = 0,
		long keyChecksumIndexStreamIndex = 1,
		StreamMappedDictionaryImplementation implementation = StreamMappedDictionaryImplementation.Auto
	) {
		Guard.ArgumentNotNull(objectContainer, nameof(objectContainer));
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		valueComparer ??= EqualityComparer<TValue>.Default;

		var useCLK = false;
		switch (implementation) {
			case StreamMappedDictionaryImplementation.Auto:
				// if key serializer is static, and less than or equal to 256 bytes, use fixed length key implementation
				if (keySerializer.IsStaticSize && keySerializer.StaticSize <= 256) {
					useCLK = true;
				} else if (keyChecksum is not null) {
					useCLK = false;
				} else {
					throw new InvalidOperationException($"Unable to decide internal implementation. Either enable pass through a constant-length key serializer equal to or under 256b or provide a key checksummer.");
				}

				break;
			case StreamMappedDictionaryImplementation.KeyValueListBased:
				Guard.Ensure(keyChecksum != null, $"Key checksummer must be provided for implementation {implementation}");
				useCLK = false;
				break;
			case StreamMappedDictionaryImplementation.ConstantLengthKeyBased:
				Guard.Ensure(keySerializer.IsStaticSize, $"Key serializer must be constant length size for implementation {implementation}");
				useCLK = true;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(implementation), implementation, null);
		}
		if (useCLK) {
			return new StreamMappedDictionaryCLK<TKey, TValue>(
				objectContainer,
				keySerializer,
				keyComparer,
				valueComparer,
				autoLoad,
				freeIndexStoreStreamIndex,
				keyChecksumIndexStreamIndex
			);
		} else {
			return new StreamMappedDictionary<TKey, TValue>(
				objectContainer,
				keySerializer,
				keyComparer,
				valueComparer,
				keyChecksum,
				autoLoad,
				freeIndexStoreStreamIndex,
				keyChecksumIndexStreamIndex
			);
		}

	}

}
