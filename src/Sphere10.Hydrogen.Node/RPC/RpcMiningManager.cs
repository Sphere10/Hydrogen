﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Node.RPC {

	//anonymous api for getwork/submit 
	[RpcAPIService("Miner")] 
	public class RpcMiningManager : MiningManagerBase, IMiningBlockProducer {
		private ILogger _logger;
		private uint CurrentMinerMicroBlockNumber = 0;
		protected Timer FakeUpdateHeaderTimer;
		protected const uint FakeUpdateHeaderTimerTimeMs = 1*1000+20;
		//TODO: Replace this by actual access to THE BlockChain interface
		protected readonly SynchronizedList<BlockChainLogItem> BlockChainLog = new SynchronizedList<BlockChainLogItem>();
		//TODO: Will be set by Fiber layer later on
		protected SynchronizedList<MiningWork> MiningWorkHistory = new SynchronizedList<MiningWork>();
		protected readonly SynchronizedObject MiningSubmitLock;
		protected RpcMiningServer StratumMiningServer;
		protected TransactionAggregatorMock TransactionAggregator;

		public event EventHandlerEx<SynchronizedList<BlockChainTransaction>> OnBlockAccepted;
		public ILogger Logger { get { return _logger; } 
								set { _logger = value; StratumMiningServer?.SetLogger(value); } }
		public int BlockTime { get; set; }
		public string MinerTag { get; set; }

		public RpcMiningManager(MiningConfig miningConfig, RpcServerConfig rpcConfig, IItemSerializer<NewMinerBlock> blockSerializer, int blockTimeSec, TimeSpan rttInterval)
			: base(miningConfig, blockSerializer, new Configuration { RTTInterval = rttInterval }) {
			BlockTime = blockTimeSec;
			StratumMiningServer = new RpcMiningServer(rpcConfig, this);
			MiningSubmitLock = new SynchronizedObject();
			//register as a RPC service provider
			ApiServiceManager.RegisterService(this);

			FakeUpdateHeaderTimer = new Timer(this.OnFakeUpdateHeaderTimer, this, FakeUpdateHeaderTimerTimeMs, FakeUpdateHeaderTimerTimeMs*30);
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

		public override uint BlockHeight => (uint)BlockChainLog.Count;

		public byte[] GetPrevMinerElectionHeader() => "Put PreviousBlock hash here".PadLeft(32).ToAsciiByteArray();
		public byte[] GetBlockPolicy() => "Put BlockPolicy hash here".PadLeft(32).ToAsciiByteArray();
		public byte[] GetKernelID() => "Put KernelID here".PadLeft(32).ToAsciiByteArray();
		public byte[] GetSignature() => "Put Signature here".PadLeft(32).ToAsciiByteArray();
		/*
		 * New Miner Block
		 * ---------------
		 * ATM we're not fixed on one Mining Block structure so the mining package fields are 
		 * sent, thru json RPC, with a description of it's structure in the field "Config.BlockTemplate".
		 * This struture is(must) represent what the NewMinerBlockSerializer class do.
		 * 
		 * The field convension is as follow:
		 *	- int/uint are all 32bit uint
		 *	- Timestamp, version...  are 32bit uint
		 *	- PrevMinerElectionHeader, BlockPolicy, KernelID, Signature are bytes arrray represented as an hexa 
		 *	  string (without 0x). Size may varry.
		 *	- MinerTag must be smaller than Config.TagSize
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
			var lastPuzzle = MiningWorkHistory.LastOrDefault();
			if (lastPuzzle != null) {
				//assuming RequestPuzzle uses DateTimeOffset.UtcNow.ToUnixTimeSeconds() to set TimeStamp
				while ((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() == lastPuzzle.Puzzle.Block.UnixTime)
					Thread.Sleep(100);
			} 

			var miningWork = new MiningWork();
			miningWork.BlockNumber = BlockHeight;
			miningWork.Puzzle = RequestPuzzle(MinerTag);
			miningWork.Puzzle.Block.PrevMinerElectionHeader = GetPrevMinerElectionHeader();
			miningWork.Puzzle.Block.BlockPolicy = GetBlockPolicy();
			miningWork.Puzzle.Block.KernelID = GetKernelID();
			miningWork.Puzzle.Block.Signature = GetSignature();
			miningWork.Puzzle.Block.PreviousMinerMicroBlockNumber = (UInt16)CurrentMinerMicroBlockNumber;
			miningWork.Puzzle.Block.Nonce = BitConverter.ToUInt32(Hashers.Hash(CHF.SHA2_256, Guid.NewGuid().ToByteArray())[^4..]);
			miningWork.Puzzle.Transactions = transactions;

			var miningBlock = new NewMinerBlockSurogate().FromNonSurogate(miningWork.Puzzle.Block, MiningConfig.TargetAlgorithm);
			miningBlock.WorkID = miningWork.WorkID;
			miningBlock.Config = new Dictionary<string, object> {
							{"maxtime",  Config.RTTInterval.Seconds },
							{"hashalgo", MiningConfig.Hasher.GetDescription() },
							{"tagsize", Constants.MinerTagSize},
							{"paddingsize", Constants.BlockHeaderPaddingSize},
						};

			_logger?.Info($"Miner {MinerTag} Sending work for block #{miningWork.BlockNumber} with target {ByteArrayExtensions.ToHexString(miningBlock.TargetPOW)}");
			MiningWorkHistory.Add(miningWork);

			//Restart fake timer
			FakeUpdateHeaderTimer.Change(FakeUpdateHeaderTimerTimeMs, FakeUpdateHeaderTimerTimeMs*30);
			return miningBlock;
		}

		public void NotifyNewBlock() {
			//NOTE: This is for testing purpose, CurrentMinerMicroBlockNumber should be updated according to incomming micro blocks
			CurrentMinerMicroBlockNumber = (CurrentMinerMicroBlockNumber + 1) % 120;
			StratumMiningServer.NotifyNewBlock();
		}

		public void NotifyNewDiff() {
			using (MiningSubmitLock.EnterWriteScope()) {
				if (MiningWorkHistory.Count > 0) {
					uint currentBlockNumber = CurrentMinerMicroBlockNumber;
					uint newTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					uint newTarget = MiningConfig.DAAlgorithm.CalculateNextBlockTarget(GetPreviousBlockTimeStamps(), MiningWorkHistory.LastOrDefault().Puzzle.CompactTarget, BlockHeight);
					byte[] newTargetPOW = MiningConfig.TargetAlgorithm.ToDigest(MiningTarget);
					//Update work history
					using (MiningWorkHistory.EnterReadScope()) {
						foreach (var work in MiningWorkHistory) {
							work.Puzzle.CompactTarget = newTarget;
							work.Puzzle.Block.UnixTime = newTime;
							work.BlockNumber = currentBlockNumber;
						}
					}

					StratumMiningServer.NotifyNewDiff(new MiningBlockUpdates { TargetPOW = newTargetPOW, TimeStamp = newTime, MicroBlockNumber = currentBlockNumber });
				}
			}
		}
		protected void OnFakeUpdateHeaderTimer(Object _this) {
			(_this as RpcMiningManager).NotifyNewDiff();
		}

		//MinerID, Tag, Nonce, Time
		[RpcAPIMethod("submit")]
		public virtual MiningSolutionResultJsonSurogate SubmitNonce(MiningSolutionJsonSurogate solution) {
			using (MiningSubmitLock.EnterWriteScope()) {
				MiningWork miningWork = null;

				//Filter invalid characters
				var minerTagBytes = StringExtensions.ToHexByteArray(solution.MinerTag);
				foreach (var c in minerTagBytes)
					if (c < 32)
						return new MiningSolutionResultJsonSurogate { SolutionResult = MiningSolutionResult.RejectedInvalid, TimeStamp = solution.Time };

				uint ut = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				//_logger?.Info($"Miner '{solution.MinerTag}' submiting nonce 0x{solution.Nonce:x} at exact timestamp {ut}");

				//dont use Select, this is time-critical.
				using (MiningWorkHistory.EnterReadScope()) {
					foreach (var p in MiningWorkHistory)
						if (p.WorkID == solution.WorkID) {
							p.Puzzle.Block.MinerTag = System.Text.Encoding.ASCII.GetString(StringExtensions.ToHexByteArray(solution.MinerTag));
							p.Puzzle.Block.UnixTime = solution.Time;
							p.Puzzle.Block.Nonce = solution.Nonce;
							miningWork = p;
							break;
						}
				}

				if (miningWork != null)
					return new MiningSolutionResultJsonSurogate { SolutionResult = SubmitSolution(miningWork.Puzzle), BlockNumber = miningWork.BlockNumber, TimeStamp = solution.Time};
				else
					return new MiningSolutionResultJsonSurogate { SolutionResult = MiningSolutionResult.RejectedNotAccepting, BlockNumber = 0, TimeStamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
			}
		}

		protected override List<DateTime> GetPreviousBlockTimeStamps() {
			var blockTimes = BlockChainLog.Select(t => t.Time).ToList();
			blockTimes.Reverse();
			return blockTimes;
		}

		protected override void OnSubmitSolution(MiningPuzzle puzzle, MiningSolutionResult result) {
			if (result == MiningSolutionResult.Accepted) {
				var now = DateTime.UtcNow;
				BlockChainLog.Add(new BlockChainLogItem { Time = now, PrevMinerElectionHeader = " Puy previous header " .PadLeft(32).ToAsciiByteArray(), PreviousMinerMicroBlockNumber = (UInt16)CurrentMinerMicroBlockNumber });

				OnBlockAccepted?.Invoke(puzzle.Transactions);

				//purge work history
				var blocknumber = BlockHeight-1;
				ICollectionExtensions.RemoveWhere(MiningWorkHistory, (mp) => mp.BlockNumber == blocknumber);
			}
		}

		public class MiningWork {
			private static uint _workID = 1;
			public MiningWork() { WorkID = Interlocked.Increment(ref _workID); }
			public uint BlockNumber { get; set; }
			public uint WorkID { get; }
			public MiningPuzzle Puzzle { get; set; }
		}

		public class BlockChainLogItem {
			public DateTime Time;
			public byte[] PrevMinerElectionHeader { get; set; }
			public UInt16 PreviousMinerMicroBlockNumber { get; set; }
		}

	}
}
