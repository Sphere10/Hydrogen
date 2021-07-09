using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Node.RPC {

	//anonymous api for getwork/submit 
	[RpcAPIService("Miner")] 
	public class RpcMiningManager : MiningManagerBase, IMiningBlockProducer {
		private readonly List<DateTime> _blockTimes;
		private ILogger _Logger;
		//TODO: Will be set by Fiber layer later on
		protected uint NodeNonce = (uint)Environment.TickCount;
		protected Random MinerNonceGen = new Random(Environment.TickCount);
		protected SynchronizedList<MiningPuzzle> MiningPuzzleHistory = new SynchronizedList<MiningPuzzle>();
		private readonly SynchronizedObject MiningSubmitLock;
		protected RpcMiningServer StratumMiningServer;
		protected TransactionAggregatorMock TransactionAggregator;

		public event EventHandlerEx<SynchronizedList<BlockChainTransaction>> OnBlockAccepted;
		public ILogger Logger { get { return _Logger; } 
								set { _Logger = value; StratumMiningServer?.SetLogger(value); } }
		public int BlockTime { get; set; }
		public string MinerTag { get; set; }

		public RpcMiningManager(MiningConfig miningConfig, RpcServerConfig rpcConfig, IItemSerializer<NewMinerBlock> blockSerializer, int blockTimeSec, TimeSpan rttInterval)
			: base(miningConfig, blockSerializer, new Configuration { RTTInterval = rttInterval }) {
			_blockTimes = new List<DateTime>();
			BlockTime = blockTimeSec;
			StratumMiningServer = new RpcMiningServer(rpcConfig, this);
			MiningSubmitLock = new SynchronizedObject();
			//register as a RPC service provider
			ApiServiceManager.RegisterService(this);
		}

		public void StartMiningServer(string minerTag) {
			MinerTag = minerTag;
			StratumMiningServer.Start();
			TransactionAggregator?.Dispose();
			TransactionAggregator = new TransactionAggregatorMock(BlockTime, this);
		}

		public void StopMiningServer() {
			TransactionAggregator?.Dispose();
			TransactionAggregator = null;
			StratumMiningServer.Stop();
		}

		public override void Dispose() {
			TransactionAggregator?.Dispose();
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

		/*
		 * New Miner Block
		 * ---------------
		 * ATM we're not fixed on one Mining Block structure so the mining package fields are 
		 * sent, thru json RPC, with a description of it's structure in the field "Config.BlockTemplate".
		 * This struture is(must) represent what the NewMinerBlockSerializer class do.
		 * 
		 * The field convension is as follow:
		 *	- int/uint are all 32bit uint
		 *	- Timestamp, Nonce, MinerNonce, NodeNonce, version...  are 32bit uint
		 *	- ExtraNonce is always 64bit uint
		 *	- The 2 hash field (MerkelRoot, PreviousBlockHash) are bytes arrray represented as an hexa 
		 *	  string (without 0x). Size may varry.
		 *	- MinerTag must be smaller than Config.TagSize
		 *	
		 * Nonce search space:
		 *  To minimize nonce dupliates and double works accross the entire mining network, the nonce search 
		 *  space is devided into 4 dimensions : Node | Miner | CPU/GPU | Nonce
		 *  - NodeNonce :  By convention, this is a the Node GUID. It's unique amongst all nodes in the network. 
		 *				   It cannot be modified by the miner client.
		 *	- MinerNonce : By convention, this is a the miner client LUID. It can be the client's IP, as long as 
		 *				   it's unique amongst all miners. It cannot be modified by the miner client.
		 *	- ExtraNonce : This 64bit search dimention is indented to prevent the Nonce search sub-space to be 
		 *				   enterely scanned within the block time limit. It must be unique amongst all 
		 *				   computing unit on the system. It should be modified by the miner. 
		 *	- Nonce :      The actual nonce sub-space to search. It must be modified by the miner.
		 *	
		 *	To maximize randomness, and minimize dead-pockets(pockets of search space that are skipped due 
		 *	to "bad" entropy) it is strongly recomended to sparsly distribute the 4 search nonces, 
		 *	inside the mining block.
		 *	
		 *	Until we settle on a NewMinerBlock structure, we can add/remove/reorder any field without having 
		 *	to recompile RHminer. The only requirement is that you provide TagSize and BlockTemplate 
		 *	in the Config object
		 *	
		 */
		public NewMinerBlockSurogate GenerateNewMiningBlock() {
			//wait for 1 block to fill
			var transactions = TransactionAggregator.TakeSnapshot();
			if (transactions == null)
				throw new Exception("Not enough transactions yet");

			//wait AT LEAST 1 seconds between blocks (note: will happen at genessis time)
			var last = MiningPuzzleHistory.LastOrDefault();
			if (last != null)
				//assuming RequestPuzzle uses DateTimeOffset.UtcNow.ToUnixTimeSeconds() to set TimeStamp
				while((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() == last.Block.Timestamp)
					Thread.Sleep(100);

			var basePuzzle = RequestPuzzle(MinerTag);
			basePuzzle.Block.MerkelRoot = TransactionAggregator.ComputeMerkelRoot(transactions);
			basePuzzle.Block.BlockNumber = BlockHeight;
			basePuzzle.Transactions = transactions;
			var miningBlock = new NewMinerBlockSurogate().FromNonSurogate(basePuzzle.Block, MiningConfig.TargetAlgorithm);
			miningBlock.Config = new Dictionary<string, object> {
							{"maxtime",  Config.RTTInterval.Seconds },
							{"hashalgo", MiningConfig.Hasher.GetDescription() },
							{"tagsize", MiningConfig.MinerTagSize },
							//Mining block template ordered as NewMinerBlockSerializer. External mining client MUST recompose block from json data acording to the order in this field list.
							{"blocktemplate", "Version,BlockNumber,MerkelRoot,MinerNonce,VotingBitMask,MinerTag,ExtraNonce,PreviousBlockHash,NodeNonce,Timestamp,Nonce" }
						};

			Logger?.Info($"Miner {MinerTag} Sending work for block #{miningBlock.BlockNumber} with target {ByteArrayExtensions.ToHexString(miningBlock.TargetPOW)}");
			MiningPuzzleHistory.Add(basePuzzle);
			return miningBlock;
		}

		public void NotifyNewBlock() {
			StratumMiningServer.NotifyNewBlock();
		}
/*
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

			MiningPuzzleHistory.Add(puzzle);
			return miningBlock;
		}
*/
		[RpcAPIMethod("submit")]
		public virtual MiningSolutionResultJsonSurogate SubmitNonce(MiningSolutionJsonSurogate solution) {
			using (MiningSubmitLock.EnterWriteScope()) {
				MiningPuzzle puzzle = null;

				//Filter invalid characters
				var minerTagBytes = StringExtensions.ToHexByteArray(solution.MinerTag);
				foreach (var c in minerTagBytes)
					if (c < 32)
						return new MiningSolutionResultJsonSurogate { SolutionResult = MiningSolutionResult.RejectedInvalid, TimeStamp = solution.Time };

				var logicalSolution = solution.ToNonSurrogate();

				uint ut = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				//Logger?.Info($"Miner '{logicalSolution.MinerTag}' submiting nonce 0x{logicalSolution.Nonce:x}-0x{logicalSolution.ExtraNonce:x} at exact timestamp {ut}");

				//dont use Select, this is time-critical.
				using (MiningPuzzleHistory.EnterReadScope()) {
					foreach (var p in MiningPuzzleHistory)
						if (p.Block.MinerNonce == logicalSolution.MinerNonce) {
							p.Block.MinerTag = logicalSolution.MinerTag;
							//p.Block.Timestamp = logicalSolution.Timestamp;
							p.Block.Timestamp = solution.Time;
							p.Block.ExtraNonce = logicalSolution.ExtraNonce;
							p.Block.Nonce = logicalSolution.Nonce;
							puzzle = p;
							break;
						}
				}

				if (puzzle != null)
					return new MiningSolutionResultJsonSurogate { SolutionResult = SubmitSolution(puzzle), BlockNumber = puzzle.Block.BlockNumber, TimeStamp = puzzle.Block.Timestamp };
				else
					return new MiningSolutionResultJsonSurogate { SolutionResult = MiningSolutionResult.RejectedNotAccepting, BlockNumber = 0, TimeStamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
			}
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
				OnBlockAccepted?.Invoke(puzzle.Transactions);

				//purge work history
				var blocknumber = puzzle.Block.BlockNumber;
				ICollectionExtensions.RemoveWhere(MiningPuzzleHistory, (mp) => mp.Block.BlockNumber == blocknumber);
			}
		}

	}
}
