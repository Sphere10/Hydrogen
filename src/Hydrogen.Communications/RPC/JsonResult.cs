using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Hydrogen.Communications.RPC {

	[JsonObject(MemberSerialization.OptIn)]

	public class JsonResponse {
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jsonrpc")]
		public string JsonRpc { get; set; } = "2.0";

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
		public object Result { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
		public JsonRpcException Error { get; set; }

		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }
	}
}
