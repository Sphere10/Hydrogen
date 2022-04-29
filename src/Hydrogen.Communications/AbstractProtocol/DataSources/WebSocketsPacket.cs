using System.Text;
using Newtonsoft.Json;

namespace Hydrogen.Communications {
	public class WebSocketsPacket {
		public string Message { get; set; }
		public string[] Tokens { get; private set; } = new string[0];
		public string JsonData { get; set; }

		public WebSocketsPacket() {
		}

		public WebSocketsPacket(byte[] bytes) {

			var text = Encoding.ASCII.GetString(bytes);
			WebSocketsPacket packet = JsonConvert.DeserializeObject<WebSocketsPacket>(text);

			Message = packet.Message;
			Tokens = Message.Split(" ");
			JsonData = packet.JsonData;
		}

		public WebSocketsPacket(string message, string jsonData) {
			Message = message;
			JsonData = jsonData;
		}

		public byte[] ToBytes() {
			var jsonData = JsonConvert.SerializeObject(this);
			return Encoding.ASCII.GetBytes(jsonData);
		}
	}
}