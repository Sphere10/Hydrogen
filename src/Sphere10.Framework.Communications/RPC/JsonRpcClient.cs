using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sphere10.Framework.Communications.RPC {
	//Simple implementation of remote call to Json server

	public class JsonRpcClient : ApiRemoteService {
		protected static uint callID = 1;
		protected bool KeepAlive = false;
		protected JsonSerializerSettings jsonSettings = new JsonSerializerSettings {/*Formatting = Formatting.Indented,*/ Converters = { { new ByteArrayHexConverter() } } };

		public JsonRpcClient(IEndPoint endPoint, bool keepAlive = false) : base(endPoint) {
			KeepAlive = keepAlive;
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
			}
			else if (typeof(T) == typeof(Void))
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

		protected string CallInternal(string methodName, params object[] arguments) {
			if (!KeepAlive)
				//Reconnect at every call, as per standard.
				EndPoint.Stop();

			var id = Interlocked.Increment(ref callID);
			var messageJson = new JsonRequest { Method = methodName, Params = arguments, Id = id };
			var messageStr = JsonConvert.SerializeObject(messageJson, jsonSettings);

			EndPoint.WriteMessage(new EndpointMessage(messageStr)); 
			var resultStr = EndPoint.ReadMessage().ToString();
#if DEBUG
			//Debug.WriteLine("Client received :" + resultStr.ToString());
#endif
			return resultStr;
		}

		//Call RPC function with no return value
		public override void RemoteCall(string methodName, params object[] arguments) {
			var resultStr = CallInternal(methodName, arguments);
			JsonResponse result = JsonConvert.DeserializeObject<JsonResponse>(resultStr, jsonSettings);
			if (result.Error != null)
				throw result.Error;
		}

		//Call RPC function with a return value.
		//NOTE: Special case for arrays of 32 bits integer (int[] and uint[]). Because of a limitation in Newtonsoft.Json, you MUST give a template type of Int64[] and Uint64[] respectively.
		public override TReturnType RemoteCall<TReturnType>(string methodName, params object[] arguments) {
			var resultStr = CallInternal(methodName, arguments);
			JsonResponse result = JsonConvert.DeserializeObject<JsonResponse>(resultStr, jsonSettings);
			if (result.Error != null)
				throw new Exception(result.Error.ToString());

			//user called RemoteCall with expecting return value but RPC method does not return a value. WARNING: The method as been called remotly, this is a client side exception
			if (result.Result == null && typeof(TReturnType) != typeof(Void))
				throw new ArgumentException($"RPC method {methodName} does not return a value. Use non-templated RemoteCall to call this method");

			//Control different bitsize casts
			return (TReturnType)ProperCast<TReturnType>(result.Result);
		}

		//Call RPC functions in batch. Return array of return-values OR exception object for every array item.
		public override object[] RemoteCall(ApiBatchCallDescriptor batchCalls) {
			if (!KeepAlive)
				//Reconnect at every call, as per standard.
				EndPoint.Stop();

			var messagesJson = new List<JsonRequest>();
			foreach(var func in batchCalls.functionCalls) { 
				var id = Interlocked.Increment(ref callID);
				messagesJson.Add(new JsonRequest { Method = func.Item2, Params = func.Item3, Id = id });
			}
			var messageStr = JsonConvert.SerializeObject(messagesJson, jsonSettings);
			EndPoint.WriteMessage(new EndpointMessage(messageStr)); 
			var resultStr = EndPoint.ReadMessage().ToString();
#if DEBUG
			//Debug.WriteLine("Client received :" + resultStr.ToString());
#endif

			JsonResponse[] result = JsonConvert.DeserializeObject<JsonResponse[]>(resultStr, jsonSettings);
			var retVal = new List<object>();
			for(int i=0; i < result.Count(); i++) { 
				if(result[i].Result != null) {
					var res = result[i].Result;
					if (batchCalls.functionCalls[i].Item1 == typeof(int))
						retVal.Add((int)ProperCast<int>(res));
					else if (batchCalls.functionCalls[i].Item1 == typeof(uint))
						retVal.Add((uint)ProperCast<uint>(res));
					else if (batchCalls.functionCalls[i].Item1 == typeof(System.UInt16))
						retVal.Add((System.UInt16)ProperCast<System.UInt16>(res));
					else if (batchCalls.functionCalls[i].Item1 == typeof(System.Int16))
						retVal.Add((System.Int16)ProperCast<System.Int16>(res));
					else if (batchCalls.functionCalls[i].Item1 == typeof(float))
						retVal.Add((float)ProperCast<float>(res));
					else if (res is JArray && batchCalls.functionCalls[i].Item1.IsArray)
						retVal.Add(((JArray)res).ToObject(batchCalls.functionCalls[i].Item1));
					else if (res is JObject) 
						retVal.Add(((JObject)res).ToObject(batchCalls.functionCalls[i].Item1));
					else if (batchCalls.functionCalls[i].Item1 == typeof(Void))
						retVal.Add(null);
					else
						retVal.Add(res);					
				}
				else
					retVal.Add(result[i].Error);
			}

			return retVal.ToArray();
		}
	}
}
