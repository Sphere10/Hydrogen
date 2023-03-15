using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Maths;

public sealed class Mersenne32 : IRandomNumberGenerator {
	private readonly Mersenne32Algorithm _Mersenne32; 

	public Mersenne32(int seed) {
		_Mersenne32 = new Mersenne32Algorithm((uint)seed);
	}


	public byte[] NextBytes(int count) {
		// implement based on generating int's to fill an array
		// must be wary of endianness compatibility
		throw new NotImplementedException();
	}
}