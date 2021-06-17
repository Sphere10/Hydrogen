﻿using System;
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

namespace Sphere10.Framework.Communications.RPC {
	//Json server as an ApiService 

	public class JsonRpcServer : JsonRpcClient {
		private static ThreadLocal<ulong> _clientContext = new ThreadLocal<ulong>(() => 0);
		protected bool CancelThread = false;
		protected int ClientCallID = 1;
		protected Thread ServerThread;
		public ILogger Logger { get; set; }

		//KeepAlive will not disconnect client at every call.
		public JsonRpcServer(IEndPoint endpoint, bool keepAlive = false) : base(endpoint) {
			KeepAlive = keepAlive;
			ServerThread = new Thread(() => { this.Run(); });

#if DEBUG
			Logger = new TimestampLogger(new ActionLogger(s => Debug.WriteLine(s)));
#endif
		}

		//For now, ClientContext is the client's UID
		public static ulong ClientContext { get { return _clientContext.Value; } }

		// Convert 'param' into a list of objects. NOTE: will support taking param names into account later. 
		protected List<object> ParseArguments(JsonRequest jreq) {
			List<object> arguments = new List<object>();
			if (jreq.Params is Newtonsoft.Json.Linq.JArray) {
				IEnumerable<object> enumerable = jreq.Params as IEnumerable<object>;
				if (enumerable != null)
					foreach (object a in enumerable) {
						arguments.Add(a);
					}
			} else if (jreq.Params is Newtonsoft.Json.Linq.JObject)
				throw new JsonRpcException(-6, $"RPC does not support full objects in arguments yet");
			else
				// intrinsic value
				arguments.Add(jreq.Params);

			return arguments;
		}

		public virtual void Start() {
			Debug.Assert(EndPoint is TcpEndPointListener);
			EndPoint.Start();
			ServerThread.Start();
		}

		protected JsonResponse ExecuteFunction(JsonRequest jsonReq) {
			Exception callException = null;
			JsonResponse jresult = null;
			try {
				var service = ApiServiceManager.GetServiceFromMethod(jsonReq.Method);
				if (service == null)
					throw new JsonRpcException(-7, $"RPC function '{jsonReq.Method}' not found ");
				List<object> arguments = ParseArguments(jsonReq);

				jresult = service.Call(jsonReq.Method, arguments);
			} catch (Exception e) {
				callException = e;
			}

			if (callException != null) {
				jresult = new JsonResponse();
				jresult.Result = null;
				if (callException is JsonRpcException)
					jresult.Error = callException as JsonRpcException;
				else
					jresult.Error = new JsonRpcException(-8, callException.ToString());
			}
			return jresult;
		}

		public int ClientRun(IEndPoint clientEndPoint) {
			_clientContext.Value = clientEndPoint.GetUID();
			EndpointMessage jsonMessage = null;
			try {
				Logger?.Info($"Server. processing messages from {clientEndPoint.GetDescription()}");
				TcpSecurityPolicies.ValidateConnectionCount(TcpSecurityPolicies.MaxConnecitonPolicy.ConnectionOpen);
				TcpSecurityPolicies.MonitorPotentialAttack(TcpSecurityPolicies.AttackType.ConnectionFlod, clientEndPoint);

				jsonMessage = clientEndPoint.ReadMessage();

				Logger?.Debug("Server received :" + jsonMessage.ToString());

				List<JsonRequest> requests = new List<JsonRequest>();
				List<JsonResponse> jresults = new List<JsonResponse>();
				if (jsonMessage.MessageData[0] == '[' && jsonMessage.MessageData.Last() == ']')
					requests = JsonConvert.DeserializeObject<JsonRequest[]>(jsonMessage.ToString(), JsonSettings).ToList();
				else
					requests.Add(JsonConvert.DeserializeObject<JsonRequest>(jsonMessage.ToString(), JsonSettings));

				//pre-init return value with error msg in case of unexpected exception
				for(int i=0; i < requests.Count; i++)
					jresults.Add(new JsonResponse { Error = new JsonRpcException(-8, "Could not execute") });

				//execute rpc
				for(int i=0; i < requests.Count; i++) {
					jresults[i] = ExecuteFunction(requests[i]);
					jresults[i].Id = Interlocked.Increment(ref ClientCallID);
				}

				//answer back to remote peer
				if (jresults.Count == 1)
					jsonMessage.FromString(JsonConvert.SerializeObject(jresults[0], JsonSettings));
				else
					jsonMessage.FromString(JsonConvert.SerializeObject(jresults, JsonSettings));
				EndPoint.WriteMessage(jsonMessage);

				if (KeepAlive)
					jsonMessage = EndPoint.ReadMessage();
				else {
					jsonMessage.Stream.Stop();
					jsonMessage.Stream = null;
				}

			} catch (SocketException e) {
				//capture communication exceptions here (pipe, tcp,...)
			} catch (TooManyConnectionsException e) {
				//capture too many connections exceptoin
			} catch (IllegalValueException e) {
				//capture badly crafted json text exception
				TcpSecurityPolicies.MonitorPotentialAttack(TcpSecurityPolicies.AttackType.MessageSpoof, clientEndPoint);
			} catch (Exception e) {
				//do someting, maybe !
			}

			//send to peer any call exception error
			try {
				//it must be  socket exception. Then close it. 
				if (jsonMessage != null) {
					jsonMessage.Stream?.Stop();
					jsonMessage.Stream = null;
				}
			} catch (Exception) {
				throw;
  			}

			TcpSecurityPolicies.ValidateConnectionCount(TcpSecurityPolicies.MaxConnecitonPolicy.ConnectionClose);
			return 0;
		}

		public virtual void Run() {
			Thread.CurrentThread.Name = "JsonRpcServer";
			while (!CancelThread) {
				try {
					//deserialize received client text in a Task.
					IEndPoint jsonClient = EndPoint.WaitForMessage(); 
					if (CancelThread == false)
						Task<int>.Factory.StartNew((_jsonClient) => {
							return ClientRun((IEndPoint)_jsonClient);
						}, jsonClient);

				} catch (Exception e) {
					if (CancelThread == false) {
						//handle network lost ...
						Logger?.Error($"Json server exception '{e.ToString()}'");
						Thread.Sleep(20);
					}
				}
			}
		}

		public virtual void Stop() {
			CancelThread = true;
			EndPoint.Stop();
			Thread.Sleep(50);
		}

	}

}
