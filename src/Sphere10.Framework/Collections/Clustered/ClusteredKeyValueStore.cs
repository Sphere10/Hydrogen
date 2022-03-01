using System;
using System.Collections.Generic;
using System.IO;
using Sphere10.Framework.Collections.Stream;

namespace Sphere10.Framework;


/// <summary>
/// A specialized list used to contain <see cref="KeyValuePair{TKey, TValue}"/> with value of <see cref="byte[]"/>. 
/// </summary>
/// <remarks>Used to implement <see cref="ClusteredDictionary{TKey,TValue}"/>.</remarks>
/// <typeparam name="TKey"></typeparam>
public class ClusteredKeyValueStore<TKey> : ClusteredList<KeyValuePair<TKey, byte[]>>, IClusteredKeyValueStore<TKey> {

	public ClusteredKeyValueStore(Stream rootStream, int clusterSize, IItemSerializer<TKey> keySerializer, IEqualityComparer<TKey> keyComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, Endianness endianness = Endianness.LittleEndian) 
		: this(new ClusteredStorage(rootStream, clusterSize, endianness, policy), keySerializer, keyComparer) {
		Guard.Argument(policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(policy), $"Checksum tracking must be enabled in clustered dictionary implementations.");
	}

	public ClusteredKeyValueStore(IClusteredStorage storage, IItemSerializer<TKey> keySerializer, IEqualityComparer<TKey> keyComparer = null) 
		: base(
			storage, 
			new KeyValuePairSerializer<TKey, byte[]>(
				keySerializer,
				new ByteArraySerializer()
			),
			new KeyValuePairEqualityComparer<TKey, byte[]>(
				keyComparer,
				new ByteArrayEqualityComparer()
			)
		) {
		Guard.Argument(storage.Policy.HasFlag(ClusteredStoragePolicy.TrackChecksums), nameof(storage), $"Checksum tracking must be enabled in clustered dictionary implementations.");
	}

	public TKey ReadKey(int index) {
        if (Storage.IsNull(index))
            throw new InvalidOperationException($"Stream record {index} is null");
        using var scope = Storage.Open(index);
        var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), scope.Stream);
        return ((KeyValuePairSerializer<TKey, byte[]>)ItemSerializer).DeserializeKey(scope.Record.Size, reader);
	}

	public byte[] ReadValue(int index) {
        if (Storage.IsNull(index))
			throw new InvalidOperationException($"Stream record {index} is null");
        using var scope = Storage.Open(index);
        var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), scope.Stream);
        return ((KeyValuePairSerializer<TKey, byte[]>)ItemSerializer).DeserializeValue(scope.Record.Size, reader);
	}

}
