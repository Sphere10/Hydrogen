using System;
using System.Collections.Generic;

namespace Sphere10.Hydrogen.Core.Consensus {

	[Serializable]
	public class NewMinerBlock:BlockBase {
		//version following Major.Minor.SoftFork.00 format
		public uint Version = 0x00010100;
		public uint BlockNumber { get; set; }
		//voting on multiple proposal at the same time
		public uint VotingBitMask { get; set; }
		public string MinerTag { get; set; }
		public byte[] MerkelRoot { get; set; }
		public byte[] PreviousBlockHash { get; set; }
		public uint CompactTarget { get; set; }
		public uint Nonce { get; set; }
		public UInt64 ExtraNonce { get; set; }
		public uint   MinerNonce { get; set; }
		public uint   NodeNonce { get; set; }
		public Dictionary<string, object> Config { get; set; } //NOTE: it's in the RPC surogate. Maybe we dont need this config here.
	}
}
