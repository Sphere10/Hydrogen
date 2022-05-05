using Newtonsoft.Json;

namespace Hydrogen.Communications.RPC {
 
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRequest {
        public JsonRequest() {}

        [JsonProperty("jsonrpc")]
        public string JsonRpc
        {
            get { return "2.0"; }
        }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object Params { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }
    }
}


