using System;

namespace Sphere10.Hydrogen.Core.Mining {

	public class MiningSolution {
		public uint Nonce { get; set; }
		public UInt64 ExtraNonce { get; set; }
		public uint MinerNonce { get; set; }
		public uint Timestamp { get; set; } 
		public string MinerTag { get; set; }
	}
}
