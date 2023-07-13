// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hydrogen.Communications;
using Hydrogen.DApp.Core;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Node.RPC;

//TODO: make surogate byte[] values end by 'Hash' so rhminer can identify then as byte array automaticaly
[Serializable]
public class NewMinerBlockSurogate {
	//version following Major.Minor.SoftFork.00 format		
	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint Version { get; set; }

	//voting on multiple proposal at the same time
	[JsonConverter(typeof(ByteArrayHexConverter))]
	public byte[] PrevMinerElectionHeader { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint PreviousMinerMicroBlockNumber { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint CompactTarget { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint PADDING { get; set; }

	[JsonConverter(typeof(ByteArrayHexConverter))]
	public byte[] TargetPOW { get; set; }

	[JsonConverter(typeof(ByteArrayHexConverter))]
	public byte[] BlockPolicy { get; set; }

	[JsonConverter(typeof(ByteArrayHexConverter))]
	public byte[] KernelID { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public UInt64 MinerRewardAccount { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public UInt64 DevRewardAccount { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public UInt64 InfrastructureRewardAccount { get; set; }

	[JsonConverter(typeof(ByteArrayHexConverter))]
	public byte[] Signature { get; set; }

	public string MinerTag { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint TimeStamp { get; set; }

	//Search nonce : [placeholder for external miner] Unique to every run
	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint Nonce { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint WorkID { get; set; }

	//Extra data for per node conifg (algo tweak for erc20 like tokens, RTT-DA params, version migration parameters, ...)
	public Dictionary<string, object> Config { get; set; }

	public NewMinerBlockSurogate FromNonSurogate(NewMinerBlock block, ICompactTargetAlgorithm targetComputer) {
		//var blockSurogate = new NewMinerBlockSurogate();
		this.Version = block.Version;
		this.PrevMinerElectionHeader = block.PrevMinerElectionHeader;
		this.PreviousMinerMicroBlockNumber = block.PreviousMinerMicroBlockNumber;
		this.CompactTarget = block.CompactTarget;
		this.TargetPOW = targetComputer.ToDigest(block.CompactTarget);
		this.PADDING = Constants.BlockHeaderPaddingSize;
		this.BlockPolicy = block.BlockPolicy;
		this.KernelID = block.KernelID;
		this.MinerRewardAccount = block.MinerRewardAccount;
		this.DevRewardAccount = block.DevRewardAccount;
		this.InfrastructureRewardAccount = block.InfrastructureRewardAccount;
		this.Signature = block.Signature;
		this.MinerTag = block.MinerTag;
		this.TimeStamp = block.UnixTime;
		this.Nonce = block.Nonce;
		return this;
	}

	public NewMinerBlock ToNonSurrogate(ICompactTargetAlgorithm targetComputer) {
		var block = new NewMinerBlock();
		block.Version = Version;
		block.PrevMinerElectionHeader = PrevMinerElectionHeader;
		block.PreviousMinerMicroBlockNumber = (UInt16)PreviousMinerMicroBlockNumber;
		block.CompactTarget = CompactTarget;
		block.CompactTarget = targetComputer.FromDigest(TargetPOW);
		block.BlockPolicy = BlockPolicy;
		block.KernelID = KernelID;
		block.MinerRewardAccount = MinerRewardAccount;
		block.DevRewardAccount = DevRewardAccount;
		block.InfrastructureRewardAccount = InfrastructureRewardAccount;
		block.Signature = Signature;
		block.MinerTag = MinerTag;
		block.UnixTime = TimeStamp;
		block.Nonce = Nonce;

		return block;
	}
}
