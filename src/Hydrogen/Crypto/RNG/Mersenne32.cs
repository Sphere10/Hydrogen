// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;

namespace Hydrogen.Maths;

/// <summary>
/// A random number generator based on the Mersenne Twister algorithm.
/// </summary>
public sealed class Mersenne32 : IRandomNumberGenerator {
	private readonly Mersenne32Algorithm _mersenne32;
	private readonly byte[] _data = new byte[4];
	private int _index;
	private readonly object _lock = new object();

	/// <summary>
	/// Initializes a new instance of the <see cref="Mersenne32"/> class with the specified seed.
	/// </summary>
	/// <param name="seed">The seed value for initializing the random number generator.</param>
	public Mersenne32(uint seed) {
		_mersenne32 = new Mersenne32Algorithm(seed);
		GenerateNext4Bytes();
	}

	/// <summary>
	/// Fills the specified span of bytes with random bytes.
	/// </summary>
	/// <param name="result">The span to be filled with random bytes.</param>
	public void NextBytes(Span<byte> result) {
		if (result.Length == 0)
			return;

		lock (_lock) { // Critical to ensure deterministic generation in multi-threaded scenarios
			var count = result.Length;
			var resultIndex = 0;

			while (count > 0) {
				var remainingData = _data.Length - _index;
				var amountToCopy = Math.Min(remainingData, count);
				_data.AsSpan(_index, amountToCopy).CopyTo(result.Slice(resultIndex));
				count -= amountToCopy;
				resultIndex += amountToCopy;
				_index += amountToCopy;

				if (_index >= _data.Length) {
					GenerateNext4Bytes();
					_index = 0;
				}
			}
		}
	}

	/// <summary>
	/// Generates a new 4-byte array from the Mersenne Twister algorithm.
	/// </summary>
	private void GenerateNext4Bytes() {
		var randomUInt = _mersenne32.NextUInt32();
		EndianBitConverter.Little.WriteTo(randomUInt, _data);
	}
}
