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

	public static CryptographicVRF CreateCryptographicVRF(CHF chf, DSS dss) 
		=> new CryptographicVRF(dss, chf);

	public static byte[] Generate(CHF chf, DSS dss, ReadOnlySpan<byte> seed, IPrivateKey privateKey, out byte[] proof, ulong nonce = 0UL)
		=> CreateCryptographicVRF(chf, dss).Run(seed, privateKey, nonce, out proof);

	public static bool TryVerify(CHF chf, DSS dss, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey) 
		=> CreateCryptographicVRF(chf, dss).TryVerify(seed, output, proof, publicKey);

	public static void VerifyOrThrow(CHF chf, DSS dss, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, ReadOnlySpan<byte> proof, IPublicKey publicKey)
		=> CreateCryptographicVRF(chf, dss).VerifyProofOrThrow(seed, output, proof, publicKey);

}
