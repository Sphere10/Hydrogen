using Hydrogen;
using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class FilePart {
		public byte[] Data { get; set; }

		internal static FilePart GenRandom() {
			return new FilePart { Data = Tools.Maths.RNG.NextBytes(Tools.Maths.RNG.Next(0, 64)) };
		}
	}

}
