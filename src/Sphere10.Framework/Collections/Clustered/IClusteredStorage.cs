using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IClusteredStorage<out THeader, TRecord> : IStreamStorage<THeader, TRecord>
		where THeader : IClusteredStorageHeader
		where TRecord : IClusteredRecord {

		ClusteredStorageCachePolicy DefaultStreamPolicy { get; set; }

		bool IntegrityChecks { get; set; }
	}

}
