using System;
using System.Collections.Generic;

namespace Sphere10.Hydrogen.Core.Consensus {

	[Serializable]
	public class NewMinerBlock {

		//version following Major.Minor.SoftFork.00 format
		public uint Version = 0x00010100;
		public byte[] PrevMinerElectionHeader { get; set; }
		public UInt16 PreviousMinerMicroBlockNumber { get; set; }
		public uint UnixTime { get; set; }
		public uint CompactTarget { get; set; }
		public byte[] BlockPolicy { get; set; }
		public byte[] KernelID { get; set; }
		public UInt64 MinerRewardAccount { get; set; }
		public UInt64 DevRewardAccount { get; set; }
		public UInt64 InfrastructureRewardAccount { get; set; }
		public byte[] Signature { get; set; }
		public string MinerTag { get; set; }
		public uint Nonce { get; set; }
	}
}
