﻿using System;
using System.Diagnostics;
using Hydrogen;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Core.Mining {


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
			=> _miningConfig.Hasher.Hash(BlockSerializer.SerializeLE(Block));
		
		public uint ComputeCompactWork() {
			var proofOfWork = ComputeWork();
			var pow = _miningConfig.TargetAlgorithm.FromDigest(proofOfWork);
			return pow;
		}

		public bool IsSolved() => ComputeCompactWork() > CompactTarget;

	}
}
