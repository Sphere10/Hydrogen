using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Node.RPC {

	[Serializable]
	public class NewMinerBlockSurogate {
		//version following Major.Minor.SoftFork.00 format		
		public uint Version {get; set; }
		public uint BlockNumber { get; set; }
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] MerkelRoot { get; set; }
		//Search dimension 1: unique to every miner client connected to wallet. Miners are encourage to not modify to help avoid collision amongst all miner clients.
		[JsonConverter(typeof(HexadecimalValueConverterReader))] 
		public uint MinerNonce { get; set; }
		//voting on multiple proposal at the same time
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint VotingBitMask { get; set; }
		public string MinerTag { get; set; }
		//Search dimension 3: [placeholder for miner] Unique to every workpackage. Prevent search nonce overrun.
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public ulong ExtraNonce { get; set; }
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] PreviousBlockHash { get; set; }
		//Search dimension 2: unique to wallet. Miners are encourage to not modify to help avoid collision amongst all miner clients. (for Fiber layer's nonce space management, intialized by node with Tag+IP)
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint	NodeNonce { get; set; }
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint Nonce { get; set; }
		public uint Timestamp { get; set; }
		//Search nonce : [placeholder for miner] Unique to every run
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] TargetPOW { get; set; }
		public uint CompactTarget{ get; set; }
		//Extra data for per node conifg (algo tweak for erc20 like tokens, RTT-DA params, version migration parameters, ...)
		public Dictionary<string, object> Config { get; set; }
		
		public NewMinerBlockSurogate FromNonSurogate(NewMinerBlock block, ICompactTargetAlgorithm targetComputer) {
			//var blockSurogate = new NewMinerBlockSurogate();
			this.Version = block.Version;
			this.BlockNumber = block.BlockNumber;
			this.Timestamp = block.Timestamp;
			this.VotingBitMask = block.VotingBitMask;
			this.MinerTag = block.MinerTag;
			this.MerkelRoot = block.MerkelRoot;
			this.PreviousBlockHash = block.PreviousBlockHash;
			this.CompactTarget = block.CompactTarget;
			this.TargetPOW = targetComputer.ToDigest(block.CompactTarget);
			this.MinerNonce = block.MinerNonce;
			this.NodeNonce = block.NodeNonce;
			if (block.Config is not null)
				foreach(var kp in block.Config)
					this.Config[kp.Key] = kp.Value;
			return this;
		}

		public NewMinerBlock ToNonSurrogate(ICompactTargetAlgorithm targetComputer)
		{
			var block = new NewMinerBlock();
			block.BlockNumber = BlockNumber;
			block.Timestamp = Timestamp;
			block.VotingBitMask = VotingBitMask;
			block.MinerTag = MinerTag;
			block.MerkelRoot = MerkelRoot;
			block.PreviousBlockHash = PreviousBlockHash;
			block.CompactTarget = CompactTarget;
			block.CompactTarget = targetComputer.FromDigest(TargetPOW);
			block.MinerNonce = MinerNonce;
			block.NodeNonce = NodeNonce;
			foreach (var kp in Config)
				block.Config[kp.Key] = kp.Value;
			return block;
		}
	}
}
