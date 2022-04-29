using System;

namespace AbstractProtocol.UDPSimple {
	[Serializable]
	public class Sync {
		public string ClientID { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
