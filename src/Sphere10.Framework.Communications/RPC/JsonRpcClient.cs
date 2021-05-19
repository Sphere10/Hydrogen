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

namespace Sphere10.Framework.Communications.RPC {
	//Simple implementation of remote call to Json server

	public class JsonRpcClient : ApiRemoteService {
		protected static uint callID = 1;
		public JsonRpcClient(IEndPoint endPoint) : base(endPoint) { }
		
		protected object ProperCastInt<T>(object value) {
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
			if (value.GetType() == typeof(System.Int64))
				return ProperCastInt<T>(value);
			if (value.GetType() == typeof(System.UInt64))
				return ProperCastInt<T>(value);

			if (value.GetType() == typeof(System.Double) && typeof(T) == typeof(System.Single))
				return Convert.ToSingle(value);

			if (value.GetType() == typeof(System.Double) && typeof(T) == typeof(System.Decimal))
				return Convert.ToDecimal(value);

			//Handle array here

			return value;
		}

		protected string CallInternal(string methodName, params object[] arguments) {
			//Reconnect at every call, as per standard.
			EndPoint.Stop();

			var id = Interlocked.Increment(ref callID);
			var messageJson = new JsonRequest { Method = methodName, Params = arguments, Id = id };
			string messageStr = JsonConvert.SerializeObject(messageJson);

			Debug.WriteLine("Client. Sending message");
			EndPoint.WriteMessage(new EndpointMessage(messageStr));
			Debug.WriteLine("Client. Receiving message...");
			return EndPoint.ReadMessage().ToString();
		}

		//Call RPC function with no return value
		public override void RemoteCall(string methodName, params object[] arguments) {
			string resultStr = CallInternal(methodName, arguments);
			JsonResponse result = JsonConvert.DeserializeObject<JsonResponse>(resultStr);
			if (result.Error != null)
				throw result.Error;
		}
		//Call RPC function with a return value
		public override TReturnType RemoteCall<TReturnType>(string methodName, params object[] arguments) {
			string resultStr = CallInternal(methodName, arguments);
			JsonResponse result = JsonConvert.DeserializeObject<JsonResponse>(resultStr);
			if (result.Error != null)
				throw new Exception(result.Error.ToString());

			//user called RemoteCall with expecting return value but RPC method does not return a value. WARNING: The method as been called remotly, this is a client side exception
			if (result.Result == null)
				throw new ArgumentException($"RPC method {methodName} does not return a value. Use non-templated RemoteCall to call this method");

			//Control different bitsize casts
			return (TReturnType)ProperCast<TReturnType>(result.Result);
		}
	}
}
