// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core.Consensus.Serializers;

public class NewMinerBlockSerializer : ConstantSizeItemSerializerBase<NewMinerBlock> {
	public NewMinerBlockSerializer() : base(
		4 + //Version
		32 + //PrevMinerElectionHeader
		2 + //PreviousMinerMicroBlockNumber
		4 + //CompactTarget
		Constants.BlockHeaderPaddingSize + //PADDING
		32 + //BlockPolicy
		32 + //KernelID
		8 + //MinerRewardAccount
		8 + //DevRewardAccount
		8 + //InfrastructureRewardAccount
		32 + //Signature
		Constants.MinerTagSize + //MinerTag
		4 + //UnixTime
		4 //Nonce
		, false
	) {
	}

	public override void Serialize(NewMinerBlock item, EndianBinaryWriter writer, SerializationContext context) {
		writer.Write(item.Version);
		writer.Write(item.PrevMinerElectionHeader);
		writer.Write(item.PreviousMinerMicroBlockNumber);
		writer.Write(item.CompactTarget);
		var PADDING = new byte[Constants.BlockHeaderPaddingSize];
		writer.Write(@PADDING);
		writer.Write(item.BlockPolicy);
		writer.Write(item.KernelID);
		writer.Write(item.MinerRewardAccount);
		writer.Write(item.DevRewardAccount);
		writer.Write(item.InfrastructureRewardAccount);
		writer.Write(item.Signature);
		writer.Write(SanitizeTag(item.MinerTag));
		writer.Write(item.UnixTime);
		writer.Write(item.Nonce);
	}


	public override NewMinerBlock Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var block = new NewMinerBlock();
		block.Version = reader.ReadUInt32();
		block.PrevMinerElectionHeader = reader.ReadBytes(32);
		block.PreviousMinerMicroBlockNumber = reader.ReadUInt16();
		block.CompactTarget = reader.ReadUInt32();
		//NOTE: we serialize the extranonce here for a better entropy when mining
		var PADDING = reader.ReadBytes(Constants.BlockHeaderPaddingSize);
		block.BlockPolicy = reader.ReadBytes(32);
		block.KernelID = reader.ReadBytes(32);
		block.MinerRewardAccount = reader.ReadUInt64();
		block.DevRewardAccount = reader.ReadUInt64();
		block.InfrastructureRewardAccount = reader.ReadUInt64();
		block.Signature = reader.ReadBytes(32);
		block.MinerTag = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(Constants.MinerTagSize)).TrimEnd(' ');
		block.UnixTime = reader.ReadUInt32();
		block.Nonce = reader.ReadUInt32();
		return block;
	}

	private byte[] SanitizeTag(string tag) {
		var newTag = tag;
		if (tag.Length > Constants.MinerTagSize)
			newTag = tag.Substring(0, Constants.MinerTagSize);
		if (tag.Length < Constants.MinerTagSize)
			newTag = tag + System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(Tools.Array.Gen(Constants.MinerTagSize - tag.Length, ' ')));
		return System.Text.Encoding.ASCII.GetBytes(newTag);
	}
}
