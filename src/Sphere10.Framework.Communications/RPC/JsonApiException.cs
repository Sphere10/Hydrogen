using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Sphere10.Framework.Communications.RPC {

	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class JsonRpcException : System.Exception {
		[JsonProperty]
		public int code { get; set; }

		[JsonProperty("Message")]
		public string text { get; set; }

		public override string ToString() {
			return $"RPC error {code}: {text}";
		}

		public JsonRpcException(int _code, string _text):base(_text) {
			code = _code;
			text = _text;
		}
	}
}

