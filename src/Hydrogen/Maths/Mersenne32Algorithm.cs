// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Maths;

public sealed class Mersenne32Algorithm {
	// Define MT19937 constants (32-bit RNG)
	private const int N = 624;
	private const int M = 397;
	private const int R = 31;
	private const int F = 1812433253;
	private const int U = 11;
	private const int S = 7;
	private const int T = 15;
	private const int L = 18;
	private const uint A = 0x9908B0DF;
	private const uint B = 0x9D2C5680;
	private const uint C = 0xEFC60000;
	private const uint MaskLower = (uint)(((ulong)1 << R) - 1);
	private const uint MaskUpper = (uint)((ulong)1 << R);

	private ushort _index;
	private readonly uint[] _mt = new uint[N];


	public Mersenne32Algorithm(uint aSeed) {
		Initialize(aSeed);
	}

	public void Initialize(uint aSeed) {
		_mt[0] = aSeed;
		for (var i = 1; i < N; i++) {
			_mt[i] = (uint)(F * (_mt[i - 1] ^ _mt[i - 1] >> 30) + i);
		}
		_index = N;
	}

	public uint NextInt32() {
		return NextUInt32();
	}

	public uint NextUInt32() {
		int i = _index;

		if (_index >= N) {
			Twist();
			i = _index;
		}

		var result = _mt[i];
		_index = (ushort)(i + 1);
		result ^= _mt[i] >> U;
		result ^= result << S & B;
		result ^= result << T & C;
		result ^= result >> L;
		return result;
	}

	public float NextFloat() {
		return (float)(NextUInt32() * 4.6566128730773926E-010);
	}

	public float NextUFloat() {
		return (float)(NextUInt32() * 2.32830643653869628906E-10);
	}

	private void Twist() {
		for (var i = 0; i <= N - M - 1; i++) {
			_mt[i] = _mt[i + M] ^ (_mt[i] & MaskUpper | _mt[i + 1] & MaskLower) >> 1 ^
			         (uint)-(_mt[i + 1] & 1) & A;
		}
		for (var i = N - M; i <= N - 2; i++) {
			_mt[i] = _mt[i + (M - N)] ^ (_mt[i] & MaskUpper | _mt[i + 1] & MaskLower) >> 1 ^
			         (uint)-(_mt[i + 1] & 1) & A;
			_mt[N - 1] = _mt[M - 1] ^ (_mt[N - 1] & MaskUpper | _mt[0] & MaskLower) >> 1 ^
			             (uint)-(_mt[0] & 1) & A;
		}
		_index = 0;
	}

}
