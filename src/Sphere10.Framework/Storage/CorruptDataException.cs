using System;

namespace Sphere10.Framework {

	public class CorruptDataException : InvalidOperationException {

		public CorruptDataException(string msg) 
			: base(msg){ 
		}

		public CorruptDataException(ClusteredStreamStorageHeader containerHeader, string msg )
			: this($"{msg} - [ClusteredStreamContainer]: {containerHeader}") {
		}

		public CorruptDataException(ClusteredStreamStorageHeader containerHeader, int cluster, string? msg = null) 
			: this(containerHeader, $"Corrupt Cluster {cluster} ({msg ?? "Unexpected value in cluster envelope"})") {
		}
	}

}