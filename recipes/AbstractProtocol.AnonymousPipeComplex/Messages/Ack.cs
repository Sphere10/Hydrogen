using System;

namespace AbstractProtocol.AnonymousPipeComplex {
	[Serializable]
	public class Ack {
		public string ServerID { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
