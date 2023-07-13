// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hydrogen.Communications.RPC;

//Makes remote calls on a server
public class JsonRpcClient : ApiRemoteService {
	protected static int CallID = 1;
	protected SynchronizedDictionary<int, Tuple<AutoResetEvent, string>> persistantCallResults = new SynchronizedDictionary<int, Tuple<AutoResetEvent, string>>();
	protected JsonRpcConfig Config;

	//NOTE: to have all byte[] properties serialized using hexa strings, you must also add this attribute to them : JsonConverter(typeof(ByteArrayHexConverter))] on byte array property 
	public static JsonSerializerSettings JsonSettings {
		get => new JsonSerializerSettings {
			/*Formatting = Formatting.Indented,*/ Converters = { { new ByteArrayHexConverter() } }
		};
		private set { }
	}

	public JsonRpcClient(IEndPoint endPoint, JsonRpcConfig config) : base(endPoint) {
		Config = config;
	}

	//Call RPC functions in batch. Return array of return-values OR exception object for every array item.
	public override object[] RemoteCall(ApiBatchCallDescriptor batchCalls) {
		if (Config.ConnectionMode == JsonRpcConfig.ConnectionModeEnum.Pulsed)
			//Reconnect at every call, as per standard.
			EndPoint.Stop();

		var messagesJson = new List<JsonRequest>();
		foreach (var func in batchCalls.FunctionCalls) {
			var id = Interlocked.Increment(ref CallID);
			messagesJson.Add(new JsonRequest { Method = func.Item2, Params = func.Item3, Id = id });
		}
		var messageStr = JsonConvert.SerializeObject(messagesJson, JsonSettings) + "\n";
		EndPoint.WriteMessage(new EndpointMessage(messageStr));

		var resultStr = EndPoint.ReadMessage().ToSafeString();
		if (String.IsNullOrEmpty(resultStr))
			throw new JsonRpcException(-11, $"RPC function cannot be decoded");

#if DEBUG
		Config.Logger?.Debug("Client received batch :" + resultStr.ToString());
#endif

		JsonResponse[] result = JsonConvert.DeserializeObject<JsonResponse[]>(resultStr, JsonSettings);
		var retVal = new List<object>();
		for (int i = 0; i < result.Count(); i++) {
			if (result[i].Result != null) {
				var res = result[i].Result;
				if (batchCalls.FunctionCalls[i].Item1 == typeof(int))
					retVal.Add((int)ProperCast<int>(res));
				else if (batchCalls.FunctionCalls[i].Item1 == typeof(uint))
					retVal.Add((uint)ProperCast<uint>(res));
				else if (batchCalls.FunctionCalls[i].Item1 == typeof(System.UInt16))
					retVal.Add((System.UInt16)ProperCast<System.UInt16>(res));
				else if (batchCalls.FunctionCalls[i].Item1 == typeof(System.Int16))
					retVal.Add((System.Int16)ProperCast<System.Int16>(res));
				else if (batchCalls.FunctionCalls[i].Item1 == typeof(float))
					retVal.Add((float)ProperCast<float>(res));
				else if (res is JArray && batchCalls.FunctionCalls[i].Item1.IsArray)
					retVal.Add(((JArray)res).ToObject(batchCalls.FunctionCalls[i].Item1));
				else if (res is JObject)
					retVal.Add(((JObject)res).ToObject(batchCalls.FunctionCalls[i].Item1));
				else if (batchCalls.FunctionCalls[i].Item1 == typeof(Void))
					retVal.Add(null);
				else
					retVal.Add(res);
			} else
				retVal.Add(result[i].Error);
		}

		return retVal.ToArray();
	}

	protected object ProperCastInt<T>(object value) {
		if (typeof(T).IsEnum)
			return (T)Enum.ToObject(typeof(T), value);
		if (typeof(T) == typeof(int))
			return Convert.ToInt32(value);
		if (typeof(T) == typeof(uint))
			return Convert.ToUInt32(value);
		if (typeof(T) == typeof(System.UInt16))
			return Convert.ToUInt16(value);
		if (typeof(T) == typeof(System.Int16))
			return Convert.ToInt16(value);
		return value;
	}

	protected object ProperCast<T>(object value) {
		if (value is JArray) {
			if (value.GetType().IsAssignableFrom(typeof(T)))
				return value;
			if (typeof(T).IsArray)
				return ((JArray)value).ToObject(typeof(T));
		} else if (value is JObject) {
			return ((JObject)value).ToObject(typeof(T));
		} else if (typeof(T) == typeof(Void))
			return value;
		else if (value is String && typeof(T) == typeof(byte[])) {
			//array of bytes are xfer as strings. The ByteArrayHexConverter cannot get them on it's own because their hidden thru the string token. You must use [JsonConverter(typeof(ByteArrayHexConverter))] on byte array property 
			return ByteArrayHexConverter.ToByteArray(value as String);
		} else {
			if (value.GetType() == typeof(System.Int64))
				return ProperCastInt<T>(value);
			if (value.GetType() == typeof(System.UInt64))
				return ProperCastInt<T>(value);

			if (value.GetType() == typeof(System.Double) && typeof(T) == typeof(System.Single))
				return Convert.ToSingle(value);

			if (value.GetType() == typeof(System.Double) && typeof(T) == typeof(System.Decimal))
				return Convert.ToDecimal(value);
		}

		return value;
	}

	protected string CallInternal(string methodName, bool noReturnType, params object[] arguments) {
		if (Config.ConnectionMode == JsonRpcConfig.ConnectionModeEnum.Pulsed)
			//Reconnect at every call, as per standard.
			EndPoint.Stop();

		var id = Interlocked.Increment(ref CallID);
		var messageJson = new JsonRequest { Method = methodName, Params = arguments, Id = id };
		var messageStr = JsonConvert.SerializeObject(messageJson, JsonSettings) + "\n";
		var resultStr = "";

		if (!(Config.IgnoreEmptyReturnValue == false || noReturnType == false)) {
			//ignore result
			EndPoint.WriteMessage(new EndpointMessage(messageStr));
		} else {
			if (Config.ConnectionMode == JsonRpcConfig.ConnectionModeEnum.Pulsed) {
				//Reading the result in Pulse mode 
				EndPoint.WriteMessage(new EndpointMessage(messageStr));
				resultStr = EndPoint.ReadMessage().ToSafeString();
			} else {
				//reading the result in Persistant mode : wait for the ClientHandler loop to reveice the answer.
				Debug.Assert(persistantCallResults.ContainsKey(id) == false);
				var waitForAnswer = new Tuple<AutoResetEvent, string>(new AutoResetEvent(false), "");
				var res = persistantCallResults[id] = waitForAnswer;

				EndPoint.WriteMessage(new EndpointMessage(messageStr));

				waitForAnswer.Item1.WaitOne(Config.MaxTimeWaitingForResult);
				Debug.Assert(persistantCallResults.ContainsKey(id) == true);
				persistantCallResults.TryGetValue(id, out waitForAnswer);
				resultStr = waitForAnswer.Item2;
				persistantCallResults.Remove(id);
			}

			if (String.IsNullOrEmpty(resultStr))
				throw new JsonRpcException(-11, $"RPC function cannot be decoded");
		}

		return resultStr;
	}

	//Call RPC function with no return value
	public override void RemoteCall(string methodName, params object[] arguments) {
		var resultStr = CallInternal(methodName, true, arguments);

		//RPC without return types are allowed to have null string as a return value
		if (resultStr.Length != 0) {
			JsonResponse result = JsonConvert.DeserializeObject<JsonResponse>(resultStr, JsonSettings);
			if (result.Error != null)
				throw result.Error;
		}
	}

	//Call RPC function with a return value.
	//NOTE: Special case for arrays of 32 bits integer (int[] and uint[]). Because of a limitation in Newtonsoft.Json, you MUST give a template type of Int64[] and Uint64[] respectively.
	public override TReturnType RemoteCall<TReturnType>(string methodName, params object[] arguments) {
		var resultStr = CallInternal(methodName, false, arguments);
		JsonResponse result = JsonConvert.DeserializeObject<JsonResponse>(resultStr, JsonSettings);
		if (result.Error != null)
			throw new Exception(result.Error.ToString());

		//user called RemoteCall with expecting return value but RPC method does not return a value. WARNING: The method as been called remotly, this is a client side exception
		if (result.Result == null && typeof(TReturnType) != typeof(Void))
			throw new ArgumentException($"RPC method {methodName} does not return a value. Use non-templated RemoteCall to call this method");

		//Control different bitsize casts
		return (TReturnType)ProperCast<TReturnType>(result.Result);
	}


}
