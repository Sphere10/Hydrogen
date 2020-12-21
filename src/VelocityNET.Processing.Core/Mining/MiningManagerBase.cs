using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;
using VelocityNET.Core.Configuration;
using VelocityNET.Core.Consensus;
using VelocityNET.Core.Maths;

namespace VelocityNET.Core.Mining {

	
	public abstract class MiningManagerBase : ReadWriteSafeObject, IMiningManager {
		
		public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> SolutionSubmited;

		protected MiningManagerBase(CHF hashAlgorithm, ITargetAlgorithm powAlgorithm, IDAAlgorithm daAlgorithm, IObjectSerializer<NewMinerBlock> blockSerializer, IConfiguration configuration) {
			HashAlgorithm = hashAlgorithm;
			DAAlgorithm = daAlgorithm;
			PoWAlgorithm = powAlgorithm;
			BlockSerializer = blockSerializer;
			Configuration = configuration;
			MiningTarget = DAAlgorithm.CalculateNextBlockTarget(Enumerable.Empty<DateTime>(), 0, 0);
		}

		public virtual MiningManagerStatus Status => MiningManagerStatus.Available;

		public virtual uint MiningTarget { get; private set; }

		public abstract uint BlockHeight { get; }

		protected CHF HashAlgorithm { get; }

		protected IDAAlgorithm DAAlgorithm { get; }

		protected ITargetAlgorithm PoWAlgorithm { get; }

		protected IConfiguration Configuration { get; }

		protected IObjectSerializer<NewMinerBlock> BlockSerializer { get; }

		public MiningPuzzle RequestPuzzle(string minerTag) {
			using (EnterReadScope()) {
				Guard.Ensure(Status == MiningManagerStatus.Available, "Mining not available");
				var block = new NewMinerBlock {
					MinerTag = minerTag,
					Nonce = GeneratePuzzleStartNonce(),
					Timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
				};
				var now = DateTime.UtcNow;
				return new MiningPuzzle(block, new ValueRange<DateTime>(now, now + Configuration.RTTInterval), MiningTarget, HashAlgorithm, PoWAlgorithm, BlockSerializer);
			}
		}

		public MiningSolutionResult SubmitSolution(MiningPuzzle puzzle) {
			using (EnterWriteScope()) {
				var result = MiningSolutionResult.RejectedInvalid;
				var now = DateTime.UtcNow;
				if (puzzle.IsSolved()) {
					if (puzzle.AcceptableTimeStampRange.Start <= now && now <= puzzle.AcceptableTimeStampRange.End) {
						result = MiningSolutionResult.Accepted;
						MiningTarget = DAAlgorithm.CalculateNextBlockTarget(GetPreviousBlockTimeStamps(), MiningTarget, BlockHeight);
					} else {
						result = MiningSolutionResult.RejectedStale;
					}
				}
				NotifyOnSubmitSolution(puzzle, result);
				return result;
			}
		}

		protected virtual uint GeneratePuzzleStartNonce()
			=> (uint)Tools.Maths.RNG.Next();

		protected abstract IEnumerable<DateTime> GetPreviousBlockTimeStamps();

		protected virtual void OnSubmitSolution(MiningPuzzle puzzle, MiningSolutionResult result) {
		}

		private void NotifyOnSubmitSolution(MiningPuzzle puzzle, MiningSolutionResult result) {
			OnSubmitSolution(puzzle, result);
			SolutionSubmited?.Invoke(this, puzzle, result);
		}
	}

}
