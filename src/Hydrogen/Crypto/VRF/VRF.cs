// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;


namespace Hydrogen;

public class VRF {

	public static IVRFAlgorithm CreateCryptographicVRF(DSS signatureScheme, CHF hasher) 
		=> new CryptographicVRF(signatureScheme, hasher);

	public static byte[] Generate(DSS signatureScheme, CHF hasher, ReadOnlySpan<byte> seed, IPrivateKey privateKey, out byte[] proof, ulong nonce = 0UL)
		=> CreateCryptographicVRF(signatureScheme, hasher).Run(seed, privateKey, nonce, out proof);

	public static bool TryVerify(DSS signatureScheme, CHF hasher, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey) 
		=> CreateCryptographicVRF(signatureScheme, hasher).TryVerify(seed, output, proof, publicKey);

	public static void VerifyOrThrow(DSS signatureScheme, CHF hasher, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey)
		=> CreateCryptographicVRF(signatureScheme, hasher).VerifyProofOrThrow(seed, output, proof, publicKey);

}
