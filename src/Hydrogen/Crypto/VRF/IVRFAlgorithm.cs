// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IVRFAlgorithm {

	int OutputLength { get; }

	void Run(ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce, Span<byte> output, out byte[] proof);

	bool TryVerify(ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey);

}

public static class IVRFAlgorithmExtensions {

	public static byte[] Run(this IVRFAlgorithm vrfAlgorithm, ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce, out byte[] proof) {
		var output = new byte[vrfAlgorithm.OutputLength];
		vrfAlgorithm.Run(seed, privateKey, nonce, output, out proof);
		return output;
	}

	public static void VerifyProofOrThrow(this IVRFAlgorithm vrfAlgorithm, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey) {
		if (!vrfAlgorithm.TryVerify(seed, output, proof, publicKey))
			throw new InvalidOperationException($"VRF failed to verify ({vrfAlgorithm})");
	}


}
