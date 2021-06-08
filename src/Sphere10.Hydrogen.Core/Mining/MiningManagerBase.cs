using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {

	public abstract class MiningManagerBase : SynchronizedObject, IMiningManager {
		
		public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> SolutionSubmited;
		public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> StatusChanged;
		protected uint _nonceSpace = (uint)Tools.Maths.RNG.Next();

		protected MiningManagerBase(CHF hashAlgorithm, ICompactTargetAlgorithm powAlgorithm, IDAAlgorithm daAlgorithm, Configuration config) {
			HashAlgorithm = hashAlgorithm;
			DAAlgorithm = daAlgorithm;
			PoWAlgorithm = powAlgorithm;
			Config = config;
			MiningTarget = DAAlgorithm.CalculateNextBlockTarget(Enumerable.Empty<DateTime>(), 0, 0);
		}

		public virtual uint MiningTarget { get; private set; }

		public abstract uint BlockHeight { get; }

		protected CHF HashAlgorithm { get; }

		protected IDAAlgorithm DAAlgorithm { get; }

		protected ICompactTargetAlgorithm PoWAlgorithm { get; }

		protected Configuration Config { get; }

		public MiningPuzzle RequestPuzzle(string minerTag) {
			using (EnterReadScope()) {
				var now = DateTime.UtcNow;
				var timeRange = new ValueRange<DateTime>(now, now + Config.RTTInterval);
				var block = new NewMinerBlock {
					Timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
					MinerTag = minerTag,
					Nonce = GeneratePuzzleStartNonce(),
					NoncePlane = GeneratePuzzleStartNonce(),
					NonceSpace = _nonceSpace,
					MerkelRoot = "   Put MerkelRoot Here ".PadLeft(32).ToAsciiByteArray(),
					PreviousBlockHash = "   Put Previous Block Hash here ".PadLeft(32).ToAsciiByteArray(),
					CompactTarget = MiningTarget,
					Config = new Dictionary<string, object> {
						{"maxtime", ((DateTime)timeRange.End) },
						{"hashalgo", HashAlgorithm.ToString() },
						{"powalgo", PoWAlgorithm.GetType().Name },
						{"daaalgo", DAAlgorithm.GetType().Name },
						{"daaalgo.blocktime", (DAAlgorithm as ASERT_RTT).Config.BlockTime},
						{"daaalgo.relaxtime", (DAAlgorithm as ASERT_RTT).Config.RelaxationTime},
					}
				};

				return new MiningPuzzle(block, timeRange, MiningTarget, HashAlgorithm, PoWAlgorithm);
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
				Debug.WriteLine("Testing Result " + result.ToString());
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


		public class Configuration {
			public TimeSpan RTTInterval { get; set; }
		}
	}

}
