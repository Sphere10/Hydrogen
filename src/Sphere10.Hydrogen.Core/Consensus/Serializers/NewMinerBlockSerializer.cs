using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core.Consensus.Serializers {
	public class NewMinerBlockSerializer : FixedSizeObjectSerializer<NewMinerBlock> {
		
		public NewMinerBlockSerializer() : base(
			 4 +			//NewMinerBlock.Version
			 4 +			//NewMinerBlock.BlockNumber
			 32 +			//NewMinerBlock.MerkelRoot
			 4 +			//NewMinerBlock.MinerNonce
			 4 +			//NewMinerBlock.VotingBitMask
			 MinerTagSize + //NewMinerBlock.MinerTag
			 8 +			//NewMinerBlock.ExtraNonce
			 32 +			//NewMinerBlock.PreviousBlockHash
			 4 +			//NewMinerBlock.NodeNonce
			 4 +			//NewMinerBlock.Timestamp
			 4				//NewMinerBlock.Nonce
			)
		{}

		public override int Serialize(NewMinerBlock @object, EndianBinaryWriter writer) {
			writer.Write(@object.Version);
			writer.Write(@object.BlockNumber);
			writer.Write(@object.MerkelRoot);
			writer.Write(@object.MinerNonce);
			writer.Write(@object.VotingBitMask);
			writer.Write(SanitizeTag(@object.MinerTag));
			writer.Write(@object.ExtraNonce);
			writer.Write(@object.PreviousBlockHash);
			writer.Write(@object.NodeNonce);
			writer.Write(@object.Timestamp);
			writer.Write(@object.Nonce);
			writer.Write(@object.Nonce);
			return FixedSize;
		}

		public override NewMinerBlock Deserialize(int size, EndianBinaryReader reader) {
			var block = new NewMinerBlock();
			block.Version = reader.ReadUInt32();
			block.BlockNumber = reader.ReadUInt32();
			block.MerkelRoot = reader.ReadBytes(32);
			block.MinerNonce = reader.ReadUInt32();
			block.VotingBitMask = reader.ReadUInt32();
			block.MinerTag = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(64)).TrimEnd(' ');
			block.ExtraNonce= reader.ReadUInt64();
			block.PreviousBlockHash = reader.ReadBytes(32);
			block.NodeNonce = reader.ReadUInt32();
			block.Timestamp = reader.ReadUInt32();
			block.Nonce = reader.ReadUInt32();
			return block;
		}

		private const int MinerTagSize = 64;
		private string SanitizeTag(string tag) {
			if (tag.Length > 64)
				return tag.Substring(0, 64);
			if (tag.Length < 64)
				return tag + System.Text.Encoding.Default.GetString(System.Text.Encoding.Default.GetBytes(Tools.Array.Gen(64 - tag.Length, ' ')));
			return tag;
		}
	}
}
