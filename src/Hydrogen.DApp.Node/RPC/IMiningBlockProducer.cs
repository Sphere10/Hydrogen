using System;
using System.Collections.Generic;
using System.Text;
using Hydrogen;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Node.RPC;

namespace Hydrogen.DApp.Node.RPC {

	public interface IMiningBlockProducer {
		public event EventHandlerEx<SynchronizedList<BlockChainTransaction>> OnBlockAccepted;

		public byte[] GetPrevMinerElectionHeader();
		public byte[] GetBlockPolicy();
		public byte[] GetKernelID();
		public byte[] GetSignature();
		NewMinerBlockSurogate GenerateNewMiningBlock();
		public void NotifyNewBlock();

		public void NotifyNewDiff();
	}

}
