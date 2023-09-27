// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IOTSAlgorithm {
	OTSConfig Config { get; }

	void SerializeParameters(Span<byte> buffer);

	void ComputeKeyHash(byte[,] key, Span<byte> result);

	byte[,] SignDigest(byte[,] privateKey, ReadOnlySpan<byte> digest);

	bool VerifyDigest(byte[,] signature, byte[,] publicKey, ReadOnlySpan<byte> digest);

	OTSKeyPair GenerateKeys(ReadOnlySpan<byte> seed);
}


public static class IOTSAlgorithmExtensions {
	public static byte[] ComputeKeyHash(this IOTSAlgorithm algo, byte[,] key) {
		var result = new byte[algo.Config.DigestSize];
		algo.ComputeKeyHash(key, result);
		return result;
	}
}
