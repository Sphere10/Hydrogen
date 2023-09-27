// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hydrogen.Communications.RPC;
using Hydrogen.DApp.Core;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Mining;

namespace Hydrogen.DApp.Node.RPC;

//anonymous api for getwork/submit 
[RpcAPIService("Miner")]
public class RpcMiningManager : MiningManagerBase, IMiningBlockProducer {
	private ILogger _logger;
	private uint CurrentMinerMicroBlockNumber = 0;
	protected Timer UpdateHeaderTimer;
	protected Timer ResendHeaderTimer;

	protected const uint FakeUpdateHeaderTimerTimeMs = 1 * 1000;

	//TODO: Replace this by actual access to THE BlockChain interface
	protected readonly SynchronizedList<BlockChainLogItem> BlockChainLog = new SynchronizedList<BlockChainLogItem>();

	//TODO: Will be set by Fiber layer later on
	protected SynchronizedList<MiningWork> MiningWorkHistory = new SynchronizedList<MiningWork>();
	private readonly object MiningSubmitLock;
	protected RpcMiningServer StratumMiningServer;
	protected TransactionAggregatorMock TransactionAggregator;

	public event EventHandlerEx<SynchronizedList<BlockChainTransaction>> OnBlockAccepted;

	public ILogger Logger {
		get { return _logger; }
		set {
			_logger = value;
			StratumMiningServer?.SetLogger(value);
		}
	}

	public int BlockTime { get; set; }
	public string MinerTag { get; set; }

	private readonly IList<DateTime> _blockTimes;
	public Statistics AllStats { get; }

	public Statistics Last5Stats { get; }

	public Statistics Last10Stats { get; }

	public Statistics Last100Stats { get; }


	public RpcMiningManager(MiningConfig miningConfig, RpcServerConfig rpcConfig, IItemSerializer<NewMinerBlock> blockSerializer, int blockTimeSec, TimeSpan rttInterval)
		: base(miningConfig, blockSerializer, new Configuration { RTTInterval = rttInterval }) {
		BlockTime = blockTimeSec;
		StratumMiningServer = new RpcMiningServer(rpcConfig, this);
		//register as a RPC service provider
		ApiServiceManager.RegisterService(this);

		MiningSubmitLock = new object();
		_blockTimes = new List<DateTime>();
		AllStats = new Statistics();
		Last5Stats = new Statistics();
		Last10Stats = new Statistics();
		Last100Stats = new Statistics();
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
			{ "maxtime", Config.RTTInterval.Seconds },
			{ "hashalgo", MiningConfig.Hasher.GetDescription() },
			{ "tagsize", Constants.MinerTagSize },
			{ "paddingsize", Constants.BlockHeaderPaddingSize },
		};

		_logger?.Info($"Miner {MinerTag} Sending work for block #{miningWork.BlockNumber} with target {ByteArrayExtensions.ToHexString(miningBlock.TargetPOW)}");
		MiningWorkHistory.Add(miningWork);

		return miningBlock;
	}

	public void NotifyNewBlock() {
		//NOTE: This is for testing purpose, CurrentMinerMicroBlockNumber should be updated according to incomming micro blocks
		CurrentMinerMicroBlockNumber = (CurrentMinerMicroBlockNumber + 1) % 120;
		StratumMiningServer.NotifyNewBlock();

		if (UpdateHeaderTimer == null)
			UpdateHeaderTimer = new Timer(this.OnFakeUpdateHeaderTimer, this, FakeUpdateHeaderTimerTimeMs, FakeUpdateHeaderTimerTimeMs);
		else
			//Restart fake timer
			UpdateHeaderTimer.Change(FakeUpdateHeaderTimerTimeMs, FakeUpdateHeaderTimerTimeMs);
	}

	public void NotifyNewDiff() {
		lock (MiningSubmitLock) {
			if (MiningWorkHistory.Count > 0) {
				/*
				uint currentBlockNumber = CurrentMinerMicroBlockNumber;
				uint newTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				uint newTarget = MiningConfig.DAAlgorithm.CalculateNextBlockTarget(GetPreviousBlockTimeStamps(), MiningWorkHistory.LastOrDefault().Puzzle.CompactTarget, BlockHeight);
				byte[] newTargetPOW = MiningConfig.TargetAlgorithm.ToDigest(MiningTarget);
				//Update work history
				using (MiningWorkHistory.EnterWriteScope()) {
					foreach (var work in MiningWorkHistory) {
						work.Puzzle.CompactTarget = newTarget;
						work.Puzzle.Block.UnixTime = newTime;
						work.BlockNumber = currentBlockNumber;
					}
				}

				StratumMiningServer.NotifyNewDiff(new MiningBlockUpdates { TargetPOW = newTargetPOW, TimeStamp = newTime, MicroBlockNumber = currentBlockNumber });
				*/
				StratumMiningServer.NotifyNewBlock();
			} else {
				UpdateHeaderTimer.Dispose();
				UpdateHeaderTimer = null;
			}
		}
	}
	protected void OnFakeUpdateHeaderTimer(Object _this) {
		(_this as RpcMiningManager).NotifyNewDiff();
	}

	//MinerID, Tag, Nonce, Time
	[RpcAPIMethod("submit")]
	public virtual MiningSolutionResultJsonSurogate SubmitNonce(MiningSolutionJsonSurogate solution) {
		MiningSolutionResultJsonSurogate result;
		lock (MiningSubmitLock) {
			MiningWork miningWork = null;

			//Filter invalid characters
			var minerTagBytes = StringExtensions.ToHexByteArray(solution.MinerTag);
			foreach (var c in minerTagBytes)
				if (c < 32)
					return new MiningSolutionResultJsonSurogate { SolutionResult = MiningSolutionResult.RejectedNotAccepting, TimeStamp = solution.Time };

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

			if (miningWork != null) {
				//Logger.Debug($"New solution for id {solution.WorkID}: time {miningWork.Puzzle.Block.UnixTime} min {DateTimeExtensions.ToUnixTime(miningWork.Puzzle.AcceptableTimeStampRange.Start)} max {DateTimeExtensions.ToUnixTime(miningWork.Puzzle.AcceptableTimeStampRange.End)}");
				result = new MiningSolutionResultJsonSurogate { SolutionResult = SubmitSolution(miningWork.Puzzle), BlockNumber = miningWork.BlockNumber, TimeStamp = solution.Time };
			} else
				result = new MiningSolutionResultJsonSurogate { SolutionResult = MiningSolutionResult.RejectedNotAccepting, BlockNumber = 0, TimeStamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
		}

		if (result.SolutionResult == MiningSolutionResult.Accepted)
			NotifyNewBlock();

		return result;
	}

	protected override List<DateTime> GetPreviousBlockTimeStamps() {
		var blockTimes = BlockChainLog.Select(t => t.Time).ToList();
		blockTimes.Reverse();
		return blockTimes;
	}

	protected override void OnSubmitSolution(MiningPuzzle puzzle, MiningSolutionResult result) {
		if (result == MiningSolutionResult.Accepted) {
			var now = DateTime.UtcNow;
			BlockChainLog.Add(new BlockChainLogItem { Time = now, PrevMinerElectionHeader = " Puy previous header ".PadLeft(32).ToAsciiByteArray(), PreviousMinerMicroBlockNumber = (UInt16)CurrentMinerMicroBlockNumber });

			OnBlockAccepted?.Invoke(puzzle.Transactions);

			//purge work history
			var blocknumber = BlockHeight - 1;
			ICollectionExtensions.RemoveWhere(MiningWorkHistory, (mp) => mp.BlockNumber == blocknumber);

			// Track Stats
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


	public class MiningWork {
		private static uint _workID = 1;
		public MiningWork() {
			WorkID = Interlocked.Increment(ref _workID);
		}
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
