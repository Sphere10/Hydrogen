using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Core.Consensus.Serializers {
	public class NewMinerBlockSerializer : FixedSizeObjectSerializer<NewMinerBlock> {
		private int _minerTagSize;
		public NewMinerBlockSerializer(int minerTagSize) : base(
			 4 +				//NewMinerBlock.Version
			 4 +				//NewMinerBlock.BlockNumber
			 32 +				//NewMinerBlock.MerkelRoot
			 4 +				//NewMinerBlock.MinerNonce
			 4 +                //NewMinerBlock.VotingBitMask
			 minerTagSize +		//NewMinerBlock.MinerTag
			 8 +				//NewMinerBlock.ExtraNonce
			 32 +				//NewMinerBlock.PreviousBlockHash
			 4 +				//NewMinerBlock.NodeNonce
			 4 +				//NewMinerBlock.Timestamp
			 4					//NewMinerBlock.Nonce
			){ 
			_minerTagSize = minerTagSize; 
		}

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
			return FixedSize;
		}

		public override NewMinerBlock Deserialize(int size, EndianBinaryReader reader) {
			var block = new NewMinerBlock();
			block.Version = reader.ReadUInt32();
			block.BlockNumber = reader.ReadUInt32();
			block.MerkelRoot = reader.ReadBytes(32);
			block.MinerNonce = reader.ReadUInt32();
			block.VotingBitMask = reader.ReadUInt32();
			block.MinerTag = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(_minerTagSize)).TrimEnd(' ');
			block.ExtraNonce= reader.ReadUInt64();
			block.PreviousBlockHash = reader.ReadBytes(32);
			block.NodeNonce = reader.ReadUInt32();
			block.Timestamp = reader.ReadUInt32();
			block.Nonce = reader.ReadUInt32();
			return block;
		}

		private byte[] SanitizeTag(string tag) {
			var newTag = tag;
			if (tag.Length > _minerTagSize)
				newTag = tag.Substring(0, _minerTagSize);
			if (tag.Length < _minerTagSize)
				newTag = tag + System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(Tools.Array.Gen(_minerTagSize - tag.Length, ' ')));
			return System.Text.Encoding.ASCII.GetBytes(newTag);
		}
	}
}
