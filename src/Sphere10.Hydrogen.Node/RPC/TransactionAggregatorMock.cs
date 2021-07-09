using System;
using System.Diagnostics;
using System.Threading;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Node.RPC;

namespace Sphere10.Hydrogen.Core.Mining {

	////////////////////////////////////////////////////////////////////////////
	//Aggregator MOCK

	//Receive transactions from p2p network
	//aggregate them into a block
	//trigger mining new block every N seconds (according to config, Blocktime/10) with accumulated trx
	//Flush trx once published
	public class TransactionAggregatorMock : IDisposable {
		//protected Timer NotifyTimeTimer;
		protected Timer FakeTRansactionTimer;
		protected UInt64 TransactionSequence = 0;
		protected SynchronizedList<BlockChainTransaction> Pendings = new SynchronizedList<BlockChainTransaction>();
		protected IMiningBlockProducer BlockProducer;
		protected TimeSpan NotifyTimeSpan;
		protected ulong TransactionCountNotifyLastThresold = 0;
		//Note: this mock aggregator does not sent new work when there is no new transactions after a certain amount of time
		//		And it does not stop accepting transactions ever.
		public ulong TransactionCountNotifyThresold { get; set; } = 7;
		public TransactionAggregatorMock(int blockTime, IMiningBlockProducer blockProducer) {
			BlockProducer = blockProducer;
			NotifyTimeSpan = TimeSpan.FromSeconds(blockTime/10);

			if (NotifyTimeSpan.TotalSeconds < 5)
				NotifyTimeSpan = TimeSpan.FromSeconds(5);

			FakeTRansactionTimer = new Timer(this.FakeOnNewTransaction, this, 50, 30000);
			blockProducer.OnBlockAccepted += (trxs) => PublishedTransactions(trxs);
		}

		public void Dispose() {
			FakeTRansactionTimer.Dispose();
		}

		public byte[] ComputeMerkelRoot(SynchronizedList<BlockChainTransaction> list) {
			var merkelMock = "random stuff";
			foreach(var t in list)
				//NOTE: THIS IS A MOCK OF SERIALIZATION. NOT THE REAL DEAL.
				merkelMock += (t.From + t.From);
			return Hashers.Hash(CHF.SHA2_256, StringExtensions.ToAsciiByteArray(merkelMock)); 
		}

		public void PublishedTransactions(SynchronizedList<BlockChainTransaction> publishedTrx) {

			var _oldCnt = Pendings.Count;
			//purge published transaction from pending list
			foreach(var pt in publishedTrx) {
				var idx = Pendings.IndexOf(pt);
				if (idx != -1)
					Pendings.RemoveAt(idx);
			}
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

			var nextTrxTime = Tools.Maths.RNG.Next(100, 500);
			//Debug.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss}: ", DateTime.Now) + $": New transaction {Pendings.Count}. Next in {nextTrxTime} ms");
			if (currentSequence - TransactionCountNotifyLastThresold > TransactionCountNotifyThresold) {
				TransactionCountNotifyLastThresold = currentSequence;
				NotifyNewBlock();
			}

			//randomly spitt the next transaction
			FakeTRansactionTimer.Change((int)nextTrxTime, (int)30000);
		}

		protected void NotifyNewBlock() {
			BlockProducer.NotifyNewBlock();
		}

		protected void FakeOnNewTransaction(Object _this) {
			(_this as TransactionAggregatorMock).OnNewTransaction(new BlockChainTransaction { From = Tools.Text.GenerateRandomString(32), To = Tools.Text.GenerateRandomString(32), Amount = (ulong)Tools.Maths.RNG.Next(), Fees = (ulong)Tools.Maths.RNG.Next() });
		}
	}


}