using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace Hydrogen.Communications.RPC {
	//Json server as an ApiService 

	public class JsonRpcServer {
		protected bool CancelThread = false;
		protected Thread ServerThread;
		protected IEndPoint EndPoint;
		protected JsonRpcConfig Config;
		protected SynchronizedList<JsonRpcClientHandler> ActiveClients = new SynchronizedList<JsonRpcClientHandler>();

		public int ActiveClientsCount { get { return ActiveClients.Count; } private set { } }
		public event EventHandlerEx<JsonRpcClientHandler> OnNewClient;

		public JsonRpcServer(IEndPoint endPoint, JsonRpcConfig config) {
			Config = config;
			EndPoint = endPoint;
			ServerThread = new Thread(() => { this.Run(); });
#if DEBUG
			Config.Logger = new TimestampLogger(new ActionLogger(s => Debug.WriteLine(s)));
#endif
		}

		public void SetLogger(ILogger logger) => Config.Logger = logger;

		public virtual void Start() {
			Debug.Assert(EndPoint is TcpEndPointListener);
			EndPoint.Start();
			ServerThread.Start();
		}

		public virtual void Stop() {
			using (ActiveClients.EnterReadScope()) {
				ActiveClients.ForEach(x => x.Stop());
			}
			CancelThread = true;
			EndPoint.Stop();
			Thread.Sleep(50);
		}

		public virtual void Run() {
			Thread.CurrentThread.Name = "JsonRpcServer";
			while (!CancelThread) {
				try {
					//Wait for connction and deserialize received text in a Task.
					IEndPoint clientEP = EndPoint.WaitForMessage();
					Config.Logger?.Info($"Client {clientEP.GetDescription()} connected");
					if (CancelThread == false) {
						//handle security before queuing the client handler
						TcpSecurityPolicies.ValidateConnectionCount(TcpSecurityPolicies.MaxConnecitonPolicy.ConnectionOpen);
						TcpSecurityPolicies.MonitorPotentialAttack(TcpSecurityPolicies.AttackType.ConnectionFlod, clientEP);

						var newClientHandler = new JsonRpcClientHandler(clientEP, Config);
						newClientHandler.OnStop += (sender, _client) => RemoveClient(_client);
						ActiveClients.Add(newClientHandler);
						newClientHandler.Start();
						OnNewClient?.Invoke(newClientHandler);
					}

				} catch (Exception e) {
					if (CancelThread == false) {
						//handle network lost ...
						Config.Logger?.Error($"Json server exception '{e.ToString()}'");
						Thread.Sleep(20);
					}
				}
			}
		}

		protected void RemoveClient(JsonRpcClientHandler client) {
			var idx = ActiveClients.IndexOf(client);
			Debug.Assert(idx != -1);
			ActiveClients.RemoveAt(idx);
		}
	}

}
