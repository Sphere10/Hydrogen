using System;

namespace AbstractProtocol.AnonymousPipeComplex {
	[Serializable]
	public class Sync {
		public string ClientID { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
