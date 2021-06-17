using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Node.RPC {

	//anonymous api for getwork/submit 
	[RpcAPIService("")]
	public class RpcMiningManager : MiningManagerBase {
		private readonly List<DateTime> _blockTimes;
		//TODO: Will be set by Fiber layer later on
		protected uint	 NodeNonce = (uint)Environment.TickCount;  
		protected Random MinerNonceGen = new Random(Environment.TickCount);
		protected SynchronizedList<MiningPuzzle> minersWorkHistory = new SynchronizedList<MiningPuzzle>();

		public RpcMiningManager(CHF hashAlgorithm, ICompactTargetAlgorithm targetAlgorithm, IDAAlgorithm daAlgorithm, IItemSerializer<NewMinerBlock> blockSerializer, Configuration config)
			: base(hashAlgorithm, targetAlgorithm, daAlgorithm, blockSerializer, config) {
			_blockTimes = new List<DateTime>();

			//register as a RPC service provider
			ApiServiceManager.RegisterService(this);
		}

		public override uint BlockHeight => (uint)_blockTimes.Count;

		[RpcAPIMethod("getwork")]
		public NewMinerBlockSurogate RequestWork(string minerTag) {
			var puzzle = base.RequestPuzzle(minerTag);

			//Setup proper workpackage
			var miningBlock = new NewMinerBlockSurogate().FromNonSurogate(puzzle.Block);
			miningBlock.MinerNonce = (uint)MinerNonceGen.Next();
			miningBlock.NodeNonce = NodeNonce;
			miningBlock.Config = new Dictionary<string, object> {
				{"maxtime", (DateTime)(DateTime.UtcNow + Config.RTTInterval) },
				{"hashalgo", HashAlgorithm.ToString() },
				{"powalgo", PoWAlgorithm.GetType().Name },
				{"daaalgo", DAAlgorithm.GetType().Name },
				{"daaalgo.blocktime", (DAAlgorithm as ASERT_RTT).Config.BlockTime},
				{"daaalgo.relaxtime", (DAAlgorithm as ASERT_RTT).Config.RelaxationTime} };

			minersWorkHistory.Add(puzzle);
			return miningBlock;
		}

		[RpcAPIMethod("submit")]
		public virtual MiningSolutionResult SubmitNonce(uint minerNonce, string minerTag, uint time, UInt64 extraNonce, uint nonce) {
			MiningPuzzle puzzle = null;

			//dont use Select, this is time-critical.
			using (minersWorkHistory.EnterReadScope()) {
				foreach (var p in minersWorkHistory)
					if (p.Block.MinerNonce == minerNonce) {
						p.Block.MinerTag = minerTag;
						p.Block.Timestamp = time;
						p.Block.ExtraNonce = extraNonce;
						p.Block.Nonce = nonce;
						puzzle = p;
						break;
					}
			}
			return puzzle == null ? MiningSolutionResult.RejectedStale :  SubmitSolution(puzzle);
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

				//purge work history
				minersWorkHistory.Clear();				
			}
		}

	}
}
