using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		protected uint NodeNonce = (uint)Environment.TickCount;
		protected Random MinerNonceGen = new Random(Environment.TickCount);
		protected SynchronizedList<MiningPuzzle> minersWorkHistory = new SynchronizedList<MiningPuzzle>();
		public ILogger Logger { get; set; }

		public RpcMiningManager(MiningConfig miningConfig, IItemSerializer<NewMinerBlock> blockSerializer, TimeSpan rttInternal)
			: base(miningConfig, blockSerializer, new Configuration { RTTInterval = rttInternal }) {
			_blockTimes = new List<DateTime>();

			//register as a RPC service provider
			ApiServiceManager.RegisterService(this);
		}
		public override void Dispose() {
			ApiServiceManager.UnregisterService(this);
		}

		public override uint BlockHeight => (uint)_blockTimes.Count;

		public override MiningPuzzle RequestPuzzle(string minerTag) {
			var puzzle = base.RequestPuzzle(minerTag);

			//Set proper Node and Miner Nonces. TODO: 
			puzzle.Block.MinerNonce = (uint)MinerNonceGen.Next();
			puzzle.Block.NodeNonce = NodeNonce;
			return puzzle;
		}

		[RpcAPIMethod("getwork")]
		public NewMinerBlockSurogate RequestWork(string minerTag) {
			var puzzle = RequestPuzzle(minerTag);

			//Setup proper workpackage
			var miningBlock = new NewMinerBlockSurogate().FromNonSurogate(puzzle.Block, MiningConfig.TargetAlgorithm);
			Logger?.Info($"Miner {minerTag} getting work for block #{miningBlock.BlockNumber} with target {ByteArrayExtensions.ToHexString(miningBlock.TargetPOW)}");

			miningBlock.Config = new Dictionary<string, object> {
				{"maxtime",  Config.RTTInterval.Seconds },
				{"hashalgo", MiningConfig.Hasher.GetDescription() },
				{"tagsize", MiningConfig.MinerTagSize },
				//Mining block template ordered as NewMinerBlockSerializer. Client MUST recompose block from json data acording to the order in this field list.
				{"blocktemplate", "Version,BlockNumber,MerkelRoot,MinerNonce,VotingBitMask,MinerTag,ExtraNonce,PreviousBlockHash,NodeNonce,Timestamp,Nonce" }
			};

			minersWorkHistory.Add(puzzle);
			return miningBlock;
		}

		//Use camel case as in arguments name so Json data can be consistant with workpackage field name that are also in camel case (aka to not confuse miners).
		[RpcAPIMethod("submit")]
		public virtual MiningSolutionResult SubmitNonce(MiningSolutionJsonSurogate solution) {
			MiningPuzzle puzzle = null;

			//Filter invalid characters
			var minerTagBytes = StringExtensions.ToHexByteArray(solution.MinerTag);
			foreach (var c in minerTagBytes)
				if (c < 32)
					return MiningSolutionResult.RejectedInvalid;

			var logicalSolution = solution.ToNonSurrogate();

			uint ut = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			Logger?.Info($"Miner {logicalSolution.MinerTag} submiting nonce 0x{logicalSolution.Nonce:x}-0x{logicalSolution.ExtraNonce:x} at exact timestamp {ut}");

			//dont use Select, this is time-critical.
			using (minersWorkHistory.EnterReadScope()) {
				foreach (var p in minersWorkHistory)
					if (p.Block.MinerNonce == logicalSolution.MinerNonce) {
						p.Block.MinerTag = logicalSolution.MinerTag;
						p.Block.Timestamp = logicalSolution.Timestamp;
						p.Block.ExtraNonce = logicalSolution.ExtraNonce;
						p.Block.Timestamp= logicalSolution.Timestamp;
						p.Block.Nonce = logicalSolution.Nonce;
						puzzle = p;
						break;
					}
			}

			return puzzle == null ? MiningSolutionResult.RejectedStale : SubmitSolution(puzzle);
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


		public class VNetMiningSolution : MiningSolution {
			uint MinerNonce;
			UInt64 ExtraNonce;
		}
	}
}
