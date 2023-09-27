// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Diagnostics;

namespace Hydrogen.Communications.RPC;
//Json server as an ApiService 


public class JsonRpcServer {
	protected bool CancelThread = false;
	protected Thread ServerThread;
	protected IEndPoint EndPoint;
	protected JsonRpcConfig Config;
	protected SynchronizedList<JsonRpcClientHandler> ActiveClients = new SynchronizedList<JsonRpcClientHandler>();

	public int ActiveClientsCount {
		get { return ActiveClients.Count; }
		private set { }
	}

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
