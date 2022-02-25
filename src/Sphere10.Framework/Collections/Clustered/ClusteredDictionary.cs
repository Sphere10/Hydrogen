using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sphere10.Framework.Collections.Stream;

namespace Sphere10.Framework {

	/// <summary>
	/// A dictionary whose keys and values are mapped over a stream. When deleting a key, it's listing record is marked as unused and re-used later for efficiency.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <typeparam name="THeader"></typeparam>
	/// <typeparam name="TRecord"></typeparam>
	/// <remarks>This is useful when the underlying KVP store isn't efficient at deletion. When deleting an item, it's record is marked as available and re-used later.</remarks>
	public class ClusteredDictionary<TKey, TValue, THeader, TRecord> : StreamPersistedDictionary<TKey, TValue, THeader, TRecord>
		where THeader : IClusteredStorageHeader
		where TRecord : IClusteredKeyRecord {

		public ClusteredDictionary(IClusteredKeyValueStore<TKey, THeader, TRecord> kvpStore, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer = null, Endianness endianess = Endianness.LittleEndian)
			: base(kvpStore, valueSerializer, keyComparer, endianess) {
		}

		public new IClusteredStorage<THeader, TRecord> Storage => (IClusteredStorage<THeader,TRecord>)base.Storage;
	}

	public class ClusteredDictionary<TKey, TValue> : ClusteredDictionary<TKey, TValue, ClusteredStorageHeader, ClusteredKeyRecord> {

		public ClusteredDictionary(Stream stream, int clusterSize, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, Endianness endianess = Endianness.LittleEndian)
			: base(
				new ClusteredKeyValueStore<TKey, ClusteredStorageHeader, ClusteredKeyRecord>(
					new ClusteredStorage<ClusteredKeyRecord>(
						stream, 
						clusterSize,
						new ClusteredKeyRecordSerializer(),
						endianess
					),
					keySerializer,
					keyComparer,
					policy,
					endianess
				),
				valueSerializer, 
				keyComparer, 
				endianess
			) {
		}
	}
}
