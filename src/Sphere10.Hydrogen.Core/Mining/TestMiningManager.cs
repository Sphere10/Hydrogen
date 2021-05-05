using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Configuration;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {

	/// <summary>
	/// A mining manager suitable for testing DAA, PoW and serialization algorithms. A real mining manager is burdened with the workflow
	/// of awaiting chain updates before adjusting targets.
	/// </summary>
	public class TestMiningManager : MiningManagerBase {
		private readonly IList<DateTime> _blockTimes;

		public Statistics AllStats { get; }

		public Statistics Last5Stats { get; }

		public Statistics Last10Stats { get; }

		public Statistics Last100Stats { get; }

		public override uint BlockHeight => (uint)_blockTimes.Count;

		public TestMiningManager(CHF hashAlgorithm, ICompactTargetAlgorithm targetAlgorithm, IDAAlgorithm daAlgorithm, IItemSerializer<NewMinerBlock> blockSerializer, TimeSpan rttInternal ) 
			: base(hashAlgorithm, targetAlgorithm, daAlgorithm, blockSerializer, new Configuration { RTTInterval = rttInternal }) {
			_blockTimes = new List<DateTime>();
			AllStats = new Statistics();
			Last5Stats = new Statistics();
			Last10Stats = new Statistics();
			Last100Stats = new Statistics();
		}

		protected override IEnumerable<DateTime> GetPreviousBlockTimeStamps()
			=> _blockTimes.Reverse();

		protected override void OnSubmitSolution(MiningPuzzle puzzle, MiningSolutionResult result) {
			if (result == MiningSolutionResult.Accepted) {
				var now = DateTime.UtcNow;
				_blockTimes.Add(now);
				if (_blockTimes.Count > 1) {
					AllStats.AddDatum((_blockTimes[^1] - _blockTimes[^2]).TotalSeconds);

					Last5Stats.Reset();
					for (var i = _blockTimes.Count - 2; i >= Math.Max(_blockTimes.Count - 5, 0); i--) {
						Last5Stats.AddDatum((_blockTimes[i + 1] - _blockTimes[i]).TotalSeconds);
					}

					Last10Stats.Reset();
					for (var i = _blockTimes.Count - 2; i >= Math.Max(_blockTimes.Count - 10, 0); i--) {
						Last10Stats.AddDatum((_blockTimes[i+1] - _blockTimes[i]).TotalSeconds);
					}

					Last100Stats.Reset();
					for (var i = _blockTimes.Count - 2; i >= Math.Max(_blockTimes.Count - 100, 0); i--) {
						Last100Stats.AddDatum((_blockTimes[i+1] - _blockTimes[i]).TotalSeconds);
					}

				}
			}
		}


	}

}
