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

namespace Sphere10.Framework.Communications.RPC {
	//Json server as an ApiService 

	public class JsonRpcServer : JsonRpcClient {
		protected bool cancelThread = false;
		protected int clientCallID = 1;
		protected Thread serverThread;
		public JsonRpcServer(IEndPoint endpoint) : base(endpoint) {
			serverThread = new Thread(() => { this.Run(); });
		}

		// Convert 'param' into a list of objects. NOTE: will support taking param names into account later. 
		List<object> ParseArguments(JsonRequest jreq) {
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
			serverThread.Start();
		}

		public int ClientRun(EndpointMessage jsonMessage) {
			Exception callException = null;			
			try {
				Debug.WriteLine("Server. processing message");
				var jsonReq = JsonConvert.DeserializeObject<JsonRequest>(jsonMessage.ToString());
				var service = ApiServiceManager.GetServiceFromMethod(jsonReq.Method);
				if (service == null)
					throw new JsonRpcException(-7, $"RPC function '{jsonReq.Method}' not found ");
				List<object> arguments = ParseArguments(jsonReq);
								
				var jresult = service.Call(jsonReq.Method, arguments);
				jresult.Id = Interlocked.Increment(ref clientCallID);

				//answer back to remote peer
				jsonMessage.FromString(JsonConvert.SerializeObject(jresult));
				EndPoint.WriteMessage(jsonMessage);
				Debug.WriteLine("Server. Destroying connection");
				jsonMessage.streamContext.Stop();
				jsonMessage.streamContext = null;

			} catch (Exception e) {
				callException = e;
			}

			//send to peer any call exception error
			try {
				if (callException != null) {
					var jresult = new JsonResponse();
					jresult.Result = null;
					if (callException is JsonRpcException)
						jresult.Error = callException as JsonRpcException;
					else
						jresult.Error = new JsonRpcException(-8, callException.ToString());

					
					jsonMessage.FromString(JsonConvert.SerializeObject(jresult));
					EndPoint.WriteMessage(jsonMessage);
				}
			} catch (Exception) {
				throw;
  			}

			return 0;
		}

		public virtual void Run() {
			Thread.CurrentThread.Name = "JsonRpcServer";
			while (!cancelThread) {
				try {
					//deserialize received client text in a Task.
					Debug.WriteLine("Server. waiting for message...");
					EndpointMessage jsonMessage = EndPoint.ReadMessage();
					Debug.WriteLine("Server. Got a message");
					if (cancelThread == false)
						Task<int>.Factory.StartNew((_jsonMessage) => {
							return ClientRun((EndpointMessage)_jsonMessage);
						}, jsonMessage);

				} catch (Exception e) {
					//handle network lost ...
					//Debug.WriteLine($"Json server exception '{e.ToString()}'");
					Thread.Sleep(20);
				}
			}
		}

		public virtual void Stop() {
			cancelThread = true;
			EndPoint.Stop();
			Thread.Sleep(50);
		}

	}

}
