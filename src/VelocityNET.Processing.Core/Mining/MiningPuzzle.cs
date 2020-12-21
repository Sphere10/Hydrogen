using System;
using Sphere10.Framework;
using VelocityNET.Core.Consensus;
using VelocityNET.Core.Maths;

namespace VelocityNET.Core.Mining {


	public class MiningPuzzle {

		public MiningPuzzle(NewMinerBlock block, ValueRange<DateTime> timeRange, uint target, CHF hashAlgorithm, ITargetAlgorithm powAlgorithm, IObjectSerializer<NewMinerBlock> blockSerializer) {
			PoWAlgorithm = powAlgorithm;
			HashAlgorithm = hashAlgorithm;
			BlockSerializer = blockSerializer;
			Block = block;
			AcceptableTimeStampRange = timeRange;
			CompactTarget = target;
		}

		protected  CHF HashAlgorithm { get; }

		protected ITargetAlgorithm PoWAlgorithm { get; }

		protected IObjectSerializer<NewMinerBlock> BlockSerializer { get; }

		public ValueRange<DateTime> AcceptableTimeStampRange { get; }

		public uint CompactTarget { get; }

		public NewMinerBlock Block { get; }

		public byte[] ComputeWork() 
			=> Hashers.Hash(HashAlgorithm, BlockSerializer.SerializeLE(Block));
		
		public uint ComputeCompactWork() {
			var proofOfWork = ComputeWork();
			var pow = PoWAlgorithm.FromDigest(proofOfWork);
			return pow;
		}

		public bool IsSolved() => ComputeCompactWork() > CompactTarget;
		

	}

}
