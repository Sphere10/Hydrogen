using System;

namespace AbstractProtocol.UDPSimple {
	[Serializable]
	public class Ack {
		public string ServerID { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
