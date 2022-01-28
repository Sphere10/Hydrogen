using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	/// <summary>
	/// A list whose items are mapped onto a stream in a clustered manner.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="THeader"></typeparam>
	/// <typeparam name="TRecord"></typeparam>
	public class ClusteredList<TItem, THeader, TRecord> : StreamPersistedList<TItem, THeader, TRecord>, IClusteredList<TItem, THeader, TRecord>
		where THeader : IClusteredStorageHeader
		where TRecord : IClusteredRecord { 
		public ClusteredList(IStreamStorage<THeader, TRecord> storage, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null, ClusteredStorageCachePolicy recordsCachePolicy = ClusteredStorageCachePolicy.Remember, Endianness endianness = Endianness.LittleEndian)
			: base(storage, itemSerializer, itemComparer, endianness) {
		}
	}

	/// <summary>
	/// A list whose items are mapped onto a stream in a clustered manner.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ClusteredList<TItem> : ClusteredList<TItem, ClusteredStorageHeader,  ClusteredRecord> {
		public ClusteredList(Stream stream, int clusterSize, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null, ClusteredStorageCachePolicy recordsCachePolicy = ClusteredStorageCachePolicy.Remember, Endianness endianness = Endianness.LittleEndian)
			: base(new ClusteredStorage(stream, clusterSize, endianness, recordsCachePolicy), itemSerializer, itemComparer, recordsCachePolicy, endianness) {
		}
	}
}
