using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;
using Newtonsoft.Json;

namespace Sphere10.Hydrogen.Core.Consensus {

	//PASCAL: getwork = block, time, part1, part3, target, targetPOW
	//PASCAL: work    = part1 + payload + extraNonce32HEX + part3 + time32 + nonce32  (size 236)
	//PASCAL: submit  = payloadStr + extraNonce32HEX, time32, nonce32

	//VNET: getwork = block, time, MerkelRoot, PrevHash, CompactTarget, Configs[]
	//VNET: work    = MerkelRoot + payload + nonceSpace64 + PrevHash + time32 + nonce32  (size 364)
	//VNET: submit  = payloadStr, time32, nonceSpace64, nonce32



	[Serializable]
	public class NewMinerBlock {
		//version following Major.Minor.SoftFork.00 format		
		public readonly uint Version = 0x00010100;
		public uint BlockNumber { get; set; }
		public uint Timestamp { get; set; }
		//voting on multiple proposal at the same time
		public uint ProposalBitMask { get; set; }
		
		[field: NonSerializedAttribute]
		public uint VotingBitMask { get; set; }
		//PASCAL payload -> 6+26=32 bytes
		//VNET   payload -> 32 bytes
		public string MinerTag {
			get => minerTAG;
			set => minerTAG = SanitizeTag(value);
		}
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
		public uint CompactTarget { get; set; }
		//PASCAL nonce -> 4 bytes
		//VNET   nonce -> 4 bytes
		public uint Nonce { get; set; }                 //Search dimension 0: unique to thread

		//PASCAL extraNonce32HEX -> 8 bytes
		//VNET   NoncePlane+NonceSpace
		[field: NonSerializedAttribute]
		public uint NoncePlane { get; set; }          //Search dimension 1: unique to computing unit (cpu/gpu)
		[field: NonSerializedAttribute]
		public uint NonceSpace { get; set; }			//Search dimension 2: unique to pool/wallet. Miners are encourage to not modify (Fiber layer's nonce space management)

		//Extra data for per node conifg (algo tweak for erc20 like tokens, RTT-DA params, version migration parameters, ...)
		public Dictionary<string, object> Config { get; set; }

		//compute work header
		public byte[] GetWorkHeader() {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			writer.Write(@Version);				//part1.1 4		(4)
			writer.Write(@BlockNumber);         //part1.1 4		(8) 
			writer.Write(@MerkelRoot);          //part1.2 32	(40)
			writer.Write(@ProposalBitMask);     //part1.2 4 	(44)
			writer.Write(@VotingBitMask);       //part1.2 4 	(48)
												//dummy   42	(/90)

			writer.Write(@MinerTag);			//Payload.1		32
			writer.Write(@NoncePlane);          //Payload.2		4
			writer.Write(@NonceSpace);          //Payload.3		4	(/34)	

			writer.Write(@PreviousBlockHash);	//part3.1		32	(/68)
			writer.Write(@Timestamp);			
			writer.Write(@Nonce);
			return stream.GetBuffer();
		}

		//For debuging purpose
		public void Load(byte[] buffer) {
			using var stream = new MemoryStream(buffer);
			using var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
			uint v = reader.ReadUInt32();
			BlockNumber = reader.ReadUInt32();
			MerkelRoot = reader.ReadBytes(32);
			ProposalBitMask = reader.ReadUInt32();
			VotingBitMask = reader.ReadUInt32();
			MinerTag = reader.ReadString();
			NoncePlane = reader.ReadUInt32();
			NonceSpace = reader.ReadUInt32();
			PreviousBlockHash = reader.ReadBytes(32);
			Timestamp = reader.ReadUInt32();
			Nonce = reader.ReadUInt32();
		}

		[NonSerializedAttribute]
		private string minerTAG;
		private const int MinerTagSize = 32;
		private string SanitizeTag(string tag) {
			if (tag.Length > MinerTagSize)
				return tag.Substring(0, MinerTagSize);
			if (tag.Length < MinerTagSize)
				return tag + System.Text.Encoding.Default.GetString(System.Text.Encoding.Default.GetBytes(Tools.Array.Gen(MinerTagSize - tag.Length, ' ')));
			return tag;
		}

	}
}
