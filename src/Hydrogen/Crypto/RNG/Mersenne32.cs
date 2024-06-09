// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

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
