// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.DApp.Core.Consensus;

namespace Hydrogen.DApp.Core.Mining;

public class MiningPuzzle {

	public MiningPuzzle(NewMinerBlock block, ValueRange<DateTime> timeRange, uint target, MiningConfig miningConfig, IItemSerializer<NewMinerBlock> blockSerializer) {
		//PoWAlgorithm = powAlgorithm;
		_miningConfig = miningConfig;
		BlockSerializer = blockSerializer;
		Block = block;
		AcceptableTimeStampRange = timeRange;
		CompactTarget = target;
	}

	protected MiningConfig _miningConfig { get; }

	protected IItemSerializer<NewMinerBlock> BlockSerializer { get; }

	public ValueRange<DateTime> AcceptableTimeStampRange { get; }

	public uint CompactTarget { get; set; }

	public NewMinerBlock Block { get; }

	public SynchronizedList<BlockChainTransaction> Transactions;

	public byte[] ComputeWork()
		=> _miningConfig.Hasher.Hash(BlockSerializer.SerializeBytesLE(Block));

	public uint ComputeCompactWork() {
		var proofOfWork = ComputeWork();
		var pow = _miningConfig.TargetAlgorithm.FromDigest(proofOfWork);
		return pow;
	}

	public bool IsSolved() => ComputeCompactWork() > CompactTarget;

}
