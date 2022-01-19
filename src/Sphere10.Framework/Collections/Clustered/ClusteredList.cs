using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {
	/// <summary>
	/// A list whose items are mapped onto a stream in a clustered manner.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ClusteredList<TItem> : StreamPersistedList<TItem, ClusteredStorageHeader,  ClusteredRecord> {
		public ClusteredList(Stream stream, int clusterSize, IItemSerializer<TItem> itemSerializer, IEqualityComparer<TItem> itemComparer = null, Endianness endianness = Endianness.LittleEndian)
			: base(new ClusteredStorage(stream, clusterSize, endianness), itemSerializer, itemComparer) {
		}
	}
}
