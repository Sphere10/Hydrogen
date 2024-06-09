// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Security.Cryptography;
using Hydrogen.Maths;

namespace Hydrogen;

/// <summary>
/// A non-deterministic cryptographically secure random number generator that uses system libraries under the hood.
/// </summary>
public class SystemCRNG : IRandomNumberGenerator {

	private readonly RNGCryptoServiceProvider _rng;

	public SystemCRNG() {
		_rng = new RNGCryptoServiceProvider();
	}

	public byte[] NextBytes(int count) {
		Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));
		var bytes = new byte[count];
		_rng.GetBytes(bytes);
		return bytes;
	}

}
