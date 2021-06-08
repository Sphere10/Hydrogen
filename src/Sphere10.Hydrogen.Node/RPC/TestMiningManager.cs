using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Configuration;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {
	//make getwork api anonymous
	[RpcAPIService("")]
	public class TestMiningManager : MiningManagerBase {
		private readonly List<DateTime> _blockTimes;

		public Statistics AllStats { get; }

		public Statistics Last5Stats { get; }

		public Statistics Last10Stats { get; }

		public Statistics Last100Stats { get; }

		public override uint BlockHeight => (uint)_blockTimes.Count;

		public TestMiningManager(CHF hashAlgorithm, ICompactTargetAlgorithm targetAlgorithm, IDAAlgorithm daAlgorithm, TimeSpan rttInternal)
			: base(hashAlgorithm, targetAlgorithm, daAlgorithm, new Configuration { RTTInterval = rttInternal }) {
			_blockTimes = new List<DateTime>();
			AllStats = new Statistics();
			Last5Stats = new Statistics();
			Last10Stats = new Statistics();
			Last100Stats = new Statistics();

			//register as a RPC service provider
			ApiServiceManager.RegisterService(this);
		}

		protected override List<DateTime> GetPreviousBlockTimeStamps() {
			var newList = new List<DateTime>(_blockTimes);
			newList.Reverse();
			return newList;
		}

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
						Last10Stats.AddDatum((_blockTimes[i + 1] - _blockTimes[i]).TotalSeconds);
					}

					Last100Stats.Reset();
					for (var i = _blockTimes.Count - 2; i >= Math.Max(_blockTimes.Count - 100, 0); i--) {
						Last100Stats.AddDatum((_blockTimes[i + 1] - _blockTimes[i]).TotalSeconds);
					}

				}
			}
		}

		//------------- RPC mining stuff ---------------------------
		[RpcAPIMethod("getwork")]
		public NewMinerBlock RequestWork(string minerTag) {
			var puzzle = base.RequestPuzzle(minerTag);
			//TODO: keep a backlog of sent workpackage so the submit can test on more than one solution (automatic in Stratum mode)
			return puzzle.Block;
		}

		[RpcAPIMethod("submit")]
		public virtual MiningSolutionResult SubmitNonce(string minerTag, uint time, uint nonceSpace, uint noncePlane, uint nonce) {
			MiningSolutionResult result = MiningSolutionResult.RejectedNotAccepting;
			var puzzle = base.RequestPuzzle(minerTag);
			puzzle.Block.MinerTag = minerTag;
			puzzle.Block.Timestamp = time;
			puzzle.Block.NonceSpace = nonceSpace;
			puzzle.Block.NoncePlane = noncePlane;
			puzzle.Block.Nonce = nonce;
			result = SubmitSolution(puzzle);
			return result;
		}
	}
}
