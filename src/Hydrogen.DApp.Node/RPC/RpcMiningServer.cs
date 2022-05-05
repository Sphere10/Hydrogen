using Hydrogen;
using Hydrogen.Communications.RPC;
using Hydrogen.DApp.Core.Consensus;
using System;
using System.Diagnostics;

namespace Hydrogen.DApp.Node.RPC {

	//Simple Stratum like server
	public class RpcMiningServer: JsonRpcServer {
		protected IMiningBlockProducer BlockProvider;
		public RpcMiningServer(RpcServerConfig config, IMiningBlockProducer blockProvider) : base(new TcpEndPointListener(config.IsLocal, config.Port, config.MaxListeners), new JsonRpcConfig() { ConnectionMode = JsonRpcConfig.ConnectionModeEnum.Persistant, IgnoreEmptyReturnValue = true }) {
			OnNewClient += HandleNewClient;
			BlockProvider = blockProvider;
		}

		//Send new block to ALL connected peers
		public void NotifyNewBlock() {
			foreach (var client in ActiveClients) {
				try {
					client.RemoteCall("miner.notify", BlockProvider.GenerateNewMiningBlock());
				} catch (Exception e) {
					Config.Logger.Error("NotifyNewBlock exception " + e.ToString());
				}
			}
		}
		public void NotifyNewDiff(MiningBlockUpdates updates) {
			foreach (var client in ActiveClients) {
				try {
					client.RemoteCall("miner.update_difficulty", updates);
				} catch (Exception e) {
					Config.Logger.Error("NotifyNewBlock exception " + e.ToString());
				}
			}
		}
		public void HandleNewClient(JsonRpcClientHandler client) {
			try {
				client.RemoteCall("miner.notify", BlockProvider.GenerateNewMiningBlock());
			} catch (Exception e) {
				Config.Logger.Error("HandleNewClient exception " + e.ToString());
			}
		}
	}
}
