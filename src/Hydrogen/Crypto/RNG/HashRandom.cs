// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Threading;

namespace Hydrogen.Maths;

/// <summary>
/// A deterministic cryptographically secure random number generator that generates a sequence of random bytes based on a given seed.
/// The generator works by hashing the seed and then iteratively hashing the resulting bytes, combined with an index, to produce new random bytes.
/// This process ensures that the same seed always produces the same sequence of bytes.
/// The generator uses a counter to add entropy between hash invocations to ensure the output is not predictable within the same seed cycle.
/// </summary>
/// <remarks>
/// This class is designed to be thread-safe and ensures deterministic behavior across multiple threads.
/// </remarks>
public sealed class HashRandom : IRandomNumberGenerator {
	public const int MinimumSeedLength = 16;
	private readonly CHF _chf;
	private readonly object _lock;
	private readonly byte[] _data;
	private int _index;
	private long _counter;

	public HashRandom(byte[] seed)
		: this(CHF.SHA2_256, seed) {
	}

	public HashRandom(CHF chf, byte[] seed) {
		Guard.ArgumentGT(Hashers.GetDigestSizeBytes(chf), 0, nameof(chf), "Hash function must generate digests larger than 0");
		Guard.ArgumentNotNull(seed, nameof(seed));
		Guard.ArgumentGTE(seed.Length, MinimumSeedLength, nameof(seed), $"Must contain at least {MinimumSeedLength} bytes");
		_chf = chf;
		_index = 0;
		_counter = 0;
		_lock = new object();
		_data = Hashers.Hash(_chf, Tools.Array.Concat<byte>(seed, EndianBitConverter.Little.GetBytes(_counter)));
		Seed = seed.ToArray();
	}

	public byte[] Seed { get; }

	public void NextBytes(Span<byte> result) {
		if (result.Length == 0)
			return;

		lock (_lock) { // Critical to ensure deterministic generation in multi-threaded scenarios
			var count = result.Length;
			var resultIndex = 0;
			Span<byte> counterBytes = stackalloc byte[sizeof(ulong)];
			while (count > 0) {
				var remainingData = _data.Length - _index;
				var amountToCopy = Math.Min(remainingData, count);
				_data.AsSpan(_index, amountToCopy).CopyTo(result.Slice(resultIndex));
				count -= amountToCopy;
				resultIndex += amountToCopy;
				_index += amountToCopy;
				if (_index >= _data.Length) {
					Interlocked.Increment(ref _counter);
					EndianBitConverter.Little.WriteTo(_counter, counterBytes);
					using (Hashers.BorrowHasher(_chf, out var hasher)) {
						hasher.Transform(_data);
						hasher.Transform(counterBytes);
						hasher.GetResult(_data);
					}
					_index = 0;
				}
			}
		}
	}

}
