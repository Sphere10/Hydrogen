using System;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;

namespace Sphere10.Framework.Tests {

	public class WorkPackage {
		public string coinbase1;
		public string coinbase2;
		public string merkels;
		public uint nTime;
		public uint foundNonce;
	}
	public class StratumWork {
		public string coinbase1;
		public string nonce1;
		public uint nTime;
		public uint foundNonce;
	}

	//Server Apiservice Example
	[RpcAPIService("Mining")]
	public class StratumServerService : JsonRpcServer {
		public StratumServerService(IEndPoint ep) : base(ep) { }

		//Stratum mining.subscribe
		[RpcAPIMethod]
		public void Subscribe([RpcAPIArgument("agent")] string userAgent, [RpcAPIArgument("session")] string sessionID) { }

		//Stratum mining.authorize
		[RpcAPIMethod]
		public void Authorize(string user, string passwd) { }

		//Stratum mining.authorize
		[RpcAPIMethod]
		public void SubmitNonce(uint nonce) { }

		//Stratum mining.notify (send to peer)
		public void Notify(StratumWork work) { }

		//getWork or mining.getwork
		[RpcAPIMethod("GetWork")]
		[RpcAPIMethod]
		public WorkPackage GetWork(uint uxTime) { return null; }
	}

	public class EXAMPLE_ServerSide {
		//Server Side:
		public void StartMiningServer() {
			var server = (StratumServerService)ApiServiceManager.GetService("mining");
			server?.Stop();
			server = new StratumServerService(new TcpEndPointListener(true, 27000, 5));
			server.Start();
			ApiServiceManager.RegisterService(server);
		}
	}
	public class EXAMPLE_Client_Side_GetWork {
		void Test() {
			//Client Side for GetWork:
			var client = new JsonRpcClient(new TcpEndPoint("127.0.0.1", 27000));
			WorkPackage work = client.RemoteCall<WorkPackage>("GetWork", DateTime.Now);
		}
	}

	//Client Side for Stratum:
	[RpcAPIService("Mining")]
	public class StratumClientService : JsonRpcServer {
		public StratumClientService(string server, int port) : base(new TcpEndPoint(server, port)) { }

		public override void Start() {
			EndPoint.Start();

			RemoteCall("mining.subscribe", "rhminer.v3.0", "1");
			bool authorised = RemoteCall<bool>("mining.authorize", "guest", "bob@mining.com");
			if (!authorised)
				base.Stop();
			else {
				serverThread.Start();
			}
		}

		//mining.notify
		[RpcAPIMethod]
		public void Notify(string coinbase1, string nonce1, uint nTime) {
			var work = new StratumWork { coinbase1 = coinbase1, nonce1 = nonce1, nTime = nTime };
			if (Process(work)) {
				bool succeded = RemoteCall<bool>("mining.submitnonce", work.foundNonce);
				if (succeded)
					Console.WriteLine("Nonce found !!!");
			}
		}
		protected bool Process(StratumWork w) { return true; }
	}

	public class EXAMPLE_Client_Side_Stratum {
		//Client Side -> Stratum client:
		public void StartMiningClient() {
			var client = (StratumClientService)ApiServiceManager.GetService("mining");
			client?.Stop();
			client = new StratumClientService("velocitymining.com", 27000);
			client.Start();
			ApiServiceManager.RegisterService(client);
		}
	}


}
