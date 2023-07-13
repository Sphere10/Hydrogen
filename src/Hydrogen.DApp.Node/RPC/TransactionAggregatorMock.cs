// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Node.RPC;

namespace Hydrogen.DApp.Core.Mining;

////////////////////////////////////////////////////////////////////////////
//Aggregator MOCK


//Receive transactions from p2p network
//aggregate them into a block
//trigger mining new block every N seconds (according to config, Blocktime/10) with accumulated trx
//Flush trx once published
public class TransactionAggregatorMock : IDisposable {
	//protected Timer NotifyTimeTimer;
	protected Timer FakeTRansactionTimer;
	protected Timer DelayNotify;
	protected UInt64 TransactionSequence = 0;
	protected SynchronizedList<BlockChainTransaction> Pendings = new SynchronizedList<BlockChainTransaction>();
	protected IMiningBlockProducer BlockProducer;
	protected ulong TransactionCountNotifyLastThresold = 0;
	protected int BlockTime { get; set; } = 10;
	public ulong TransactionCountNotifyThresold { get; set; } = 7;
	public TransactionAggregatorMock(int blockTimeSec, IMiningBlockProducer blockProducer) {
		BlockProducer = blockProducer;
		BlockTime = blockTimeSec;

		FakeTRansactionTimer = new Timer(this.FakeOnNewTransaction, this, 1000, 30000);
		blockProducer.OnBlockAccepted += (trxs) => PublishedTransactions(trxs);
	}

	public void Dispose() {
		FakeTRansactionTimer.Dispose();
	}

	public void PublishedTransactions(SynchronizedList<BlockChainTransaction> publishedTrx) {
		//OnResendBlock(this);
	}

	public SynchronizedList<BlockChainTransaction> TakeSnapshot() {
		if (Pendings.Count == 0)
			return null;

		var snapshot = new SynchronizedList<BlockChainTransaction>();
		snapshot.AddRangeSequentially(Pendings);
		return snapshot;
	}

	protected void OnNewTransaction(BlockChainTransaction trx) {
		var currentSequence = Interlocked.Increment(ref TransactionSequence);
		trx.Sequence = currentSequence;
		trx.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		Pendings.Add(trx);

		NotifyNewBlock();

		//randomly spitt the next transaction
		FakeTRansactionTimer.Change((int)BlockTime * 1000, (int)30000);
	}

	protected void NotifyNewBlock() {
		BlockProducer.NotifyNewBlock();
	}

	protected void OnResendBlock(Object _this) {
		if (DelayNotify != null) {
			BlockProducer.NotifyNewBlock();
			DelayNotify?.Dispose();
			DelayNotify = null;
		}
	}
	protected void FakeOnNewTransaction(Object _this) {
		(_this as TransactionAggregatorMock).OnNewTransaction(new BlockChainTransaction
			{ From = Tools.Text.GenerateRandomString(32), To = Tools.Text.GenerateRandomString(32), Amount = (ulong)Tools.Maths.RNG.Next(), Fees = (ulong)Tools.Maths.RNG.Next() });
	}
}
