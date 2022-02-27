using System;
using System.Collections.Generic;
using Sphere10.Framework.Collections.Stream;

namespace Sphere10.Framework;

public class ClusteredKeyValueStore<TKey, THeader, TRecord> : ClusteredList<KeyValuePair<TKey, byte[]>, THeader, TRecord>, IClusteredKeyValueStore<TKey, THeader, TRecord>
	where THeader : IClusteredStorageHeader
	where TRecord : IClusteredKeyRecord {

	public ClusteredKeyValueStore(IClusteredStorage<THeader, TRecord> storage, IItemSerializer<TKey> keySerializer, IEqualityComparer<TKey> keyComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, Endianness endianness = Endianness.LittleEndian) 
		: base(
			storage, 
			new KeyValuePairSerializer<TKey, byte[]>(
				keySerializer,
				new ByteArraySerializer()
			),
			new KeyValuePairEqualityComparer<TKey, byte[]>(
				keyComparer,
				new ByteArrayEqualityComparer()
			),
			policy,
			endianness
		) {
	}

	public TKey ReadKey(int index) {
        if (Storage.IsNull(index))
            throw new InvalidOperationException($"Stream record {index} is null");
        using var stream = Storage.Open(index, out var record);
        var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), stream);
        return ((KeyValuePairSerializer<TKey, byte[]>)ItemSerializer).DeserializeKey(record.Size, reader);
	}
		

	public byte[] ReadValue(int index) {
        if (Storage.IsNull(index))
			throw new InvalidOperationException($"Stream record {index} is null");
        using var stream = Storage.Open(index, out var record);
        var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), stream);
        return ((KeyValuePairSerializer<TKey, byte[]>)ItemSerializer).DeserializeValue(record.Size, reader);
	}

}
