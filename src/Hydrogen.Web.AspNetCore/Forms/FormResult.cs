using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hydrogen.Web.AspNetCore {
	public class FormResult {

		[JsonProperty("result")]
		public bool Result { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public FormResultType ResultType { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }
	}
}
