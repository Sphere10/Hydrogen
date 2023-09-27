// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hydrogen.Communications.RPC;

//Decode and execute received call request
public class JsonRpcClientHandler : JsonRpcClient {
	private static ThreadLocal<ulong> _clientContext = new ThreadLocal<ulong>(() => 0);
	protected bool CancelThread = false;
	protected Task<int> Task { get; set; }

	//Context Accessor for RPC functions. For now, ClientContext is the client's UID
	public static ulong ClientContext {
		get { return _clientContext.Value; }
	}

	public event EventHandler<JsonRpcClientHandler>? OnStop;
	public event EventHandler<JsonRpcClientHandler>? OnStart;

	public JsonRpcClientHandler(IEndPoint endPoint, JsonRpcConfig config) : base(endPoint, config) {
	}

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

	protected virtual JsonResponse ExecuteCall(JsonRequest jsonReq) {
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

		//Hardcode the ID from the caller.
		if (jsonReq.Id is UInt64 ||
		    jsonReq.Id is Int64 ||
		    jsonReq.Id is UInt32 ||
		    jsonReq.Id is Int32)
			jresult.Id = Convert.ToInt32(jsonReq.Id);
		else
			jresult.Id = -1;

		return jresult;
	}

	protected List<JsonRequest> ProcessBatchCalls(string message) {
		return JsonConvert.DeserializeObject<JsonRequest[]>(message, JsonSettings).ToList();
	}

	protected JsonRequest ProcessManySingleCalls(string message) {
		var jobj = JObject.Parse(message);
		//process function call request (Pulse or Persitant mode)
		if (jobj.ContainsKey("method") && jobj.ContainsKey("params")) {
			return JsonConvert.DeserializeObject<JsonRequest>(message, JsonSettings);
		}
		//process results of function called made outside this loop (for Persistant connection mode)
		else if (jobj.ContainsKey("result") && jobj.ContainsKey("id")) {
			//TODO: put that in an interface or a function
			if (Config.ConnectionMode == JsonRpcConfig.ConnectionModeEnum.Persistant) {
				var id = (int)jobj.Value<uint>("id");

				Tuple<AutoResetEvent, string> waitingCaller;
				var found = persistantCallResults.TryGetValue(id, out waitingCaller);
				if (found == true) {
					//Debug.WriteLine($"Client {EndPoint.GetDescription()} recording and seding result for callid {id}");
					var waitEvent = waitingCaller.Item1;
					persistantCallResults[id] = new Tuple<AutoResetEvent, string>(waitEvent, message);
					waitEvent.Set();
				} else
					Config.Logger.Error($"{EndPoint.GetDescription()} Received answer for call id {id} but no pending caller exists. Result will be discarded.");

				//No execution needed ...					
			} else
				//this is an error in Pulse mode, ClientHandler is expecting a function call
				throw new JsonRpcException(-12, "Invalid data received. Received result instead of call request");
		} else
			throw new JsonRpcException(-13, "Invalid data received. Must be call result or call request");

		return null;
	}

	public virtual int ProcessCalls() {
		_clientContext.Value = EndPoint.GetUID();
		EndpointMessage jsonMessage = null;
		while (!CancelThread) {
			try {
				jsonMessage = EndPoint.ReadMessage();
				var message = jsonMessage.ToSafeString();
				Config.Logger?.Debug($"{EndPoint.GetDescription()} received '{message}'");

				if (string.IsNullOrEmpty(message)) {
					//trap disgracefully disconnected connections and end this thread accordignly
					if (!EndPoint.IsOpened())
						break;

					//ignore empty strings
					continue;
				}

				List<JsonRequest> requests = new List<JsonRequest>();
				List<JsonResponse> jresults = new List<JsonResponse>();
				var isBatchMode = message[0] == '[' && message.Last() == ']';
				//handle Bach mode. Pulse mode only
				if (isBatchMode)
					requests = ProcessBatchCalls(message);
				else {
					//external clients cann often call manu submit at once, separated buy endl by convention.
					var messages = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var line in messages) {
						var request = ProcessManySingleCalls(line);
						if (request != null)
							requests.Add(request);
					}
				}

				//No execution needed. Was just answering pending results
				if (requests.Count == 0)
					continue;

				//pre-init return value with error msg in case of unexpected exception
				for (int i = 0; i < requests.Count; i++)
					jresults.Add(new JsonResponse { Error = new JsonRpcException(-10, "Could not execute") });

				//execute rpc
				for (int i = 0; i < requests.Count; i++) {
					//set id as -1 in case ExecuteCall doesnt set it properly
					jresults[i] = ExecuteCall(requests[i]);
				}

				//answer back to remote peer
				if (!isBatchMode)
					foreach (var result in jresults) {
						//handle IgnoreEmptyReturnValue for RPC without return type
						if (Config.IgnoreEmptyReturnValue == true && result.Result == null && result.Error == null)
							jsonMessage.FromString("");
						else
							jsonMessage.FromString(JsonConvert.SerializeObject(result, JsonSettings) + "\n");

						EndPoint.WriteMessage(jsonMessage);
					}
				else {
					jsonMessage.FromString(JsonConvert.SerializeObject(jresults, JsonSettings) + "\n");
					EndPoint.WriteMessage(jsonMessage);
				}

			} catch (SocketException e) {
				//capture communication exceptions here (pipe, tcp,...)
				Config.Logger?.Error($"{EndPoint.GetDescription()} JsonRpcClientHandler Exception :" + e.ToString());
				//end loop on socket exceptions
				Stop();
			} catch (System.IO.IOException e) {
				Config.Logger?.Error($"{EndPoint.GetDescription()} Client disconnected");
				//end loop on socket/IO exceptions
				Stop();
			} catch (TooManyConnectionsException e) {
				//capture too many connections exception
				Config.Logger?.Error($"{EndPoint.GetDescription()} JsonRpcClientHandler Exception :" + e.ToString());
			} catch (IllegalValueException e) {
				//capture badly crafted json text exception
				TcpSecurityPolicies.MonitorPotentialAttack(TcpSecurityPolicies.AttackType.MessageSpoof, EndPoint);
				Config.Logger?.Error($"{EndPoint.GetDescription()} JsonRpcClientHandler Exception :" + e.ToString());
			} catch (Exception e) {
				//do someting, maybe !
				Config.Logger?.Error($"{EndPoint.GetDescription()} JsonRpcClientHandler Exception :" + e.ToString());
			}

			//close EP for single RPC calls
			if (Config.ConnectionMode == JsonRpcConfig.ConnectionModeEnum.Pulsed) {
				try {
					jsonMessage.Stream.Stop();
					jsonMessage.Stream = null;
				} catch (Exception) {
				}

				//break the loop in pulse mode
				break;
			}
		}

		TcpSecurityPolicies.ValidateConnectionCount(TcpSecurityPolicies.MaxConnecitonPolicy.ConnectionClose);
		return 1;
	}
	public virtual void Start() {
		EndPoint.Start();
		Task = Task<int>.Factory.StartNew(() => { return ProcessCalls(); });

		OnStart?.Invoke(this, this);
	}

	public virtual void Stop() {
		OnStop?.Invoke(this, this);
		CancelThread = true;
		EndPoint.Stop();
	}
}
