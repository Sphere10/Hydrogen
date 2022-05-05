using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class Ping {
		internal static Ping GenRandom() => new() {  };
	}

}
