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
	private readonly Mersenne32Algorithm _mersenne32;

	public Mersenne32(int seed) {
		_mersenne32 = new Mersenne32Algorithm((uint)seed);
	}


	public byte[] NextBytes(int count) {
		Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));
		var bytes = new byte[count];
		var i = 0;

		while (i < count) {
			// Generate the next random 32-bit unsigned integer
			var randomUInt = _mersenne32.NextUInt32();

			// Convert it to bytes
			var uintBytes = BitConverter.GetBytes(randomUInt);

			// Copy the bytes into the result array, making sure not to exceed the 'count'
			for (var j = 0; j < sizeof(uint) && i < count; j++, i++) {
				bytes[i] = uintBytes[j];
			}
		}

		return bytes;
	}
}
