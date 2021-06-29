using System;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {


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

		public uint CompactTarget { get; }

		public NewMinerBlock Block { get; }

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
