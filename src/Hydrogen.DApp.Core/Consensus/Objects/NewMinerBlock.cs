// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core.Consensus;

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
