using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredList<TItem> : IClusteredList<TItem, ClusteredStorageHeader, ClusteredRecord> {
	}

	public interface IClusteredList<TItem, out THeader, TRecord> : IStreamPersistedList<TItem, THeader, TRecord>
		where THeader : IClusteredStorageHeader 
		where TRecord : IClusteredRecord {

		new IClusteredStorage<THeader, TRecord> Storage { get; }
	}

}
