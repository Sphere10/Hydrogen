using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core.Consensus.Serializers {
	public class NewMinerBlockSerializer : FixedSizeObjectSerializer<NewMinerBlock> {
		
		public NewMinerBlockSerializer() : base(4 + 64 + 4) {
		}

		public override int Serialize(NewMinerBlock @object, EndianBinaryWriter writer) {
			writer.Write(@object.Timestamp);
			writer.Write(SanitizeTag(@object.MinerTag));
			writer.Write(@object.Nonce);
			return FixedSize;
		}

		public override NewMinerBlock Deserialize(int size, EndianBinaryReader reader) {
			var block = new NewMinerBlock();
			block.Timestamp = reader.ReadUInt32();
			block.MinerTag = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(64)).TrimEnd(' ');
			block.Nonce = reader.ReadUInt32();
			return block;
		}

		private string SanitizeTag(string tag) {
			if (tag.Length > 64)
				return tag.Substring(0, 64);
			if (tag.Length < 64)
				return tag + Tools.Array.Gen(64 - tag.Length, ' ');
			return tag;
		}
	}
}
