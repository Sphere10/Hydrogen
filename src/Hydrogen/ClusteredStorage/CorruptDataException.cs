using System;

namespace Hydrogen {

	public class CorruptDataException : InvalidOperationException {

		public CorruptDataException(string msg) 
			: base(msg){ 
		}

		public CorruptDataException(ClusteredStorageHeader containerHeader, string msg )
			: this($"{msg} - [ClusteredStorage]: {containerHeader}") {
		}

		public CorruptDataException(ClusteredStorageHeader containerHeader, int cluster, string? msg = null) 
			: this(containerHeader, $"Corrupt Cluster {cluster} ({msg ?? "Unexpected value in cluster envelope"})") {
		}
	}

}