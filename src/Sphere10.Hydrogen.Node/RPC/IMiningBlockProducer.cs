using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Node.RPC;

namespace Sphere10.Hydrogen.Node.RPC {

	public interface IMiningBlockProducer {
		public event EventHandlerEx<SynchronizedList<BlockChainTransaction>> OnBlockAccepted;

		NewMinerBlockSurogate GenerateNewMiningBlock();
		void NotifyNewBlock();
	}

}
