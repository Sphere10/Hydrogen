using Sphere10.Framework;
using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class RequestFilePart {
		public string Filename { get; set; }

		public long Offset { get; set; }

		public int Length { get; set; }

		internal static RequestFilePart GenRandom() => new() { 
			Filename = $"SomeFile-{ Guid.NewGuid().ToStrictAlphaString() }.dat",
			Offset = Tools.Maths.RNG.Next(0, 65536),
			Length = Tools.Maths.RNG.Next(0, 65536)
		};

	}

}
