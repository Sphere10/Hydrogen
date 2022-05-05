using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class Pong {
		internal static Pong GenRandom() => new() { };
	}

}
