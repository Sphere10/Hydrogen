using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Collections.Stream {
	public interface IClusteredKeyValueStore<TKey, out THeader, TRecord> : IStreamKeyValueStore<TKey, THeader, TRecord>
		where THeader : IClusteredStorageHeader
		where TRecord : IClusteredKeyRecord {
	}

}
