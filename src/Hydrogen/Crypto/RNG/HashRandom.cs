// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Maths;

/// <summary>
/// A deterministic cryptographically-secure random number generator suitable for blockchain consensus. It works by extracting bytes from an iteratively hashed seed. The cryptographic security
/// of <see cref="HashRandom"/> derives from that of the underlying <see cref="CHF"/>. It will always generate the same sequence of bytes given a seed.
/// </summary>
public sealed class HashRandom : IRandomNumberGenerator {
	public const int MinimumSeedLength = 16;
	private readonly CHF _chf;
	private byte[] _data;
	private int _index;


	public HashRandom(byte[] seed)
		: this(CHF.SHA2_256, seed) {
	}

	public HashRandom(CHF chf, byte[] seed) {
		Guard.ArgumentGT(Hashers.GetDigestSizeBytes(chf), 0, nameof(chf), "Hash function must generate digests larger than 0");
		Guard.ArgumentNotNull(seed, nameof(seed));
		Guard.ArgumentGTE(seed.Length, MinimumSeedLength, nameof(seed), $"Must contain at least {MinimumSeedLength} bytes");
		_chf = chf;
		_index = 0;
		_data = Hashers.Hash(_chf, seed);
	}

	public byte[] NextBytes(int count) {
		var result = new byte[count];
		var resultIndex = 0;
		while (count > 0) {
			var remainingData = _data.Length - _index;
			var amountToCopy = Math.Min(remainingData, count);
			Buffer.BlockCopy(_data, _index, result, resultIndex, amountToCopy);
			count -= amountToCopy;
			resultIndex += amountToCopy;
			_index += amountToCopy;
			if (_index >= _data.Length) {
				_data = Hashers.Hash(_chf, _data);
				_index = 0;
			}
		}
		return result;
	}

}
