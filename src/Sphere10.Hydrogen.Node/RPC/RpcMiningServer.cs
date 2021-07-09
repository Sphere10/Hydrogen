using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Consensus;
using System;
using System.Diagnostics;

namespace Sphere10.Hydrogen.Node.RPC {

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
		public void HandleNewClient(JsonRpcClientHandler client) {
			try {
				client.RemoteCall("miner.notify", BlockProvider.GenerateNewMiningBlock());
			} catch (Exception e) {
				Config.Logger.Error("HandleNewClient exception " + e.ToString());
			}
		}
	}
}
