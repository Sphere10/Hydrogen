using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.Consensus;

namespace Sphere10.Hydrogen.Node.RPC {

	//PASCAL: getwork = block, time, part1, part3, target, targetPOW
	//PASCAL: work    = part1 + payload + extraNonce32HEX + part3 + time32 + nonce32  (size 236)
	//PASCAL: submit  = payloadStr + extraNonce32HEX, time32, nonce32

	//VNET: getwork = block, time, MerkelRoot, PrevHash, CompactTarget, Configs[]
	//VNET: work    = MerkelRoot + payload + nonceSpace64 + PrevHash + time32 + nonce32  (size 364)
	//VNET: submit  = payloadStr, time32, nonceSpace64, nonce32

	[Serializable]
	public class NewMinerBlockSurogate {
		//version following Major.Minor.SoftFork.00 format		
		public uint Version {get; set; }
		public uint BlockNumber { get; set; }
		public uint Timestamp { get; set; }
		//voting on multiple proposal at the same time
		public uint VotingBitMask { get; set; }		
		//PASCAL payload -> 6+26=32 bytes
		//VNET   payload -> 32 bytes
		public string MinerTag { get; set; }
		//PASCAL part1		-> 90 bytes for secp256K1
		//VNET   merkelroot -> 32 bytes
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] MerkelRoot { get; set; }
		//PASCAL part3    -> 68 bytes
		//VNET   prevhash -> 32 bytes
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] PreviousBlockHash { get; set; }
		//PASCAL Target			-> 8 bytes
		//VNET:  CompactTarget	-> 8 bytes				
		public uint CompactTarget { get; set; }			//TODO: RnD if we can have Full Target in block, or a 64 bit version of CompactTarget. The idea is to lower the amount of void work due to CompactTarget imprecision.
		public uint	MinerNonce { get; set; }			//Search dimension 1: unique to miner client. Miners are encourage to not modify to help avoid collision amongst all miner clients.
		public uint	NodeNonce { get; set; }				//Search dimension 2: unique to pool/wallet. Miners are encourage to not modify to help avoid collision amongst all miner clients. (for Fiber layer's nonce space management, intialized by node with Tag+IP)

		//Extra data for per node conifg (algo tweak for erc20 like tokens, RTT-DA params, version migration parameters, ...)
		public Dictionary<string, object> Config { get; set; }

        public NewMinerBlockSurogate FromNonSurogate(NewMinerBlock block) {
			//var blockSurogate = new NewMinerBlockSurogate();
			this.Version = block.Version;
			this.BlockNumber = block.BlockNumber;
			this.Timestamp = block.Timestamp;
			this.VotingBitMask = block.VotingBitMask;
			this.MinerTag = block.MinerTag;
			this.MerkelRoot = block.MerkelRoot;
			this.PreviousBlockHash = block.PreviousBlockHash;
			this.CompactTarget = block.CompactTarget;
			this.MinerNonce = block.MinerNonce;
			this.NodeNonce = block.NodeNonce;
			foreach (var kp in block.Config)
				this.Config[kp.Key] = kp.Value;
			return this;
		}

		public NewMinerBlock ToNonSurrogate()
		{
			var block = new NewMinerBlock();
			block.BlockNumber = BlockNumber;
			block.Timestamp = Timestamp;
			block.VotingBitMask = VotingBitMask;
			block.MinerTag = MinerTag;
			block.MerkelRoot = MerkelRoot;
			block.PreviousBlockHash = PreviousBlockHash;
			block.CompactTarget = CompactTarget;
			block.MinerNonce = MinerNonce;
			block.NodeNonce = NodeNonce;
			foreach (var kp in Config)
				block.Config[kp.Key] = kp.Value;
			return block;
		}
	}
}
