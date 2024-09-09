// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Maths;
using System;
using System.Security.Cryptography;

namespace Hydrogen;

/// <summary>
/// A non-deterministic cryptographically secure random number generator that uses system libraries under the hood.
/// </summary>
public class SystemCRNG : IRandomNumberGenerator {

	private readonly RNGCryptoServiceProvider _rng;

	/// <summary>
	/// Initializes a new instance of the <see cref="SystemCRNG"/> class.
	/// </summary>
	public SystemCRNG() {
		_rng = new RNGCryptoServiceProvider();
	}

	/// <summary>
	/// Fills the specified span of bytes with cryptographically secure random bytes.
	/// </summary>
	/// <param name="result">The span to be filled with random bytes.</param>
	public void NextBytes(Span<byte> result) {
		if (result.Length == 0)
			return;

		// Fill the byte array with random bytes
		_rng.GetBytes(result);
	}
}
