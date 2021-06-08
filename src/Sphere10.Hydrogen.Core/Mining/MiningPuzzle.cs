using System;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {


	public class MiningPuzzle {

		public MiningPuzzle(NewMinerBlock block, ValueRange<DateTime> timeRange, uint target, CHF hashAlgorithm, ICompactTargetAlgorithm powAlgorithm) {
			PoWAlgorithm = powAlgorithm;
			HashAlgorithm = hashAlgorithm;
			Block = block;
			AcceptableTimeStampRange = timeRange;
			CompactTarget = target;
		}

		protected  CHF HashAlgorithm { get; }

		protected ICompactTargetAlgorithm PoWAlgorithm { get; }

		public ValueRange<DateTime> AcceptableTimeStampRange { get; }

		public uint CompactTarget { get; }

		public NewMinerBlock Block { get; }

		public byte[] ComputeWork() 
			=> Hashers.Hash(HashAlgorithm, Block.GetWorkHeader());
		
		public uint ComputeCompactWork() {
			var proofOfWork = ComputeWork();
			var pow = PoWAlgorithm.FromDigest(proofOfWork);
			return pow;
		}

		public bool IsSolved() => ComputeCompactWork() > CompactTarget;
		

	}

}
