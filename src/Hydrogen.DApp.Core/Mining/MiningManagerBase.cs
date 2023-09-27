// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen.DApp.Core.Consensus;

namespace Hydrogen.DApp.Core.Mining;

public abstract class MiningManagerBase : SynchronizedObject, IMiningManager, IDisposable {

	public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> SolutionSubmited;
	public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> StatusChanged;

	protected MiningManagerBase(MiningConfig miningConfig, IItemSerializer<NewMinerBlock> blockSerializer, Configuration config) {
		MiningConfig = miningConfig;
		BlockSerializer = blockSerializer;
		Config = config;
		MiningTarget = MiningConfig.DAAlgorithm.CalculateNextBlockTarget(Enumerable.Empty<DateTime>(), 0, 0);
	}

	public virtual uint MiningTarget { get; private set; }

	public abstract uint BlockHeight { get; }

	protected MiningConfig MiningConfig { get; }

	protected Configuration Config { get; }

	protected IItemSerializer<NewMinerBlock> BlockSerializer { get; }

	public virtual MiningPuzzle RequestPuzzle(string minerTag) {
		using (EnterReadScope()) {
			var now = DateTime.UtcNow;
			var timeRange = new ValueRange<DateTime>(now, now + Config.RTTInterval);
			var block = new NewMinerBlock {
				Version = 0x00010100,
				BlockPolicy = new byte[32],
				KernelID = new byte[32],
				Signature = new byte[32],
				UnixTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
				MinerTag = minerTag,
				Nonce = (uint)Tools.Maths.RNG.Next(),
				PrevMinerElectionHeader = "   Put MerkelRoot Here ".PadLeft(32).ToAsciiByteArray(),
				PreviousMinerMicroBlockNumber = (UInt16)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
				CompactTarget = MiningTarget,
			};

			return new MiningPuzzle(block, timeRange, MiningTarget, MiningConfig, BlockSerializer);
		}
	}

	public virtual MiningSolutionResult SubmitSolution(MiningPuzzle puzzle) {
		using (EnterWriteScope()) {
			var result = MiningSolutionResult.RejectedInvalid;
			var now = DateTime.UtcNow;

			if (puzzle.IsSolved()) {
				if (puzzle.AcceptableTimeStampRange.Start <= now && now <= puzzle.AcceptableTimeStampRange.End) {
					result = MiningSolutionResult.Accepted;
					MiningTarget = MiningConfig.DAAlgorithm.CalculateNextBlockTarget(GetPreviousBlockTimeStamps(), MiningTarget, BlockHeight);
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

	public virtual void Dispose() {
	}


	public class Configuration {
		public TimeSpan RTTInterval { get; set; }
	}
}
