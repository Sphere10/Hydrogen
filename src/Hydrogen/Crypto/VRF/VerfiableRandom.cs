// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Maths;


namespace Hydrogen;

public class VerfiableRandom : IRandomNumberGenerator {

	private readonly HashRandom _rng;

	public CHF CHF { get; }

	public IVRFAlgorithm VRF { get; }

	public byte[] VRFSeed { get; }
	
	public byte[] VRFProof { get; }

	public byte[] VRFOutput { get; }

	public VerfiableRandom(CryptographicVRF vrf, ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce = 0L) 
		: this(vrf.CHF, vrf, seed, privateKey, nonce) {
	}

	public VerfiableRandom(CHF chf, IVRFAlgorithm vrf, ReadOnlySpan<byte> seed, IPrivateKey privateKey, ulong nonce = 0L) {
		CHF = chf;
		VRF = vrf;
		VRFSeed = seed.ToArray();
		VRFOutput = vrf.Run(seed, privateKey, nonce, out var proof);
		VRFProof = proof;
		_rng = new HashRandom(chf, VRFOutput);
	}
	
	public VerfiableRandom(CryptographicVRF vrf, ReadOnlySpan<byte> seed, IPublicKey publicKey, byte[] unverifiedProof) 
		: this(vrf.CHF, vrf, seed, vrf.CalculateOutput(unverifiedProof),  publicKey, unverifiedProof) {
	}

	public VerfiableRandom(CHF chf, IVRFAlgorithm vrf, ReadOnlySpan<byte> seed, ReadOnlySpan<byte> output, IPublicKey publicKey, byte[] unverifiedProof) {
		vrf.VerifyProofOrThrow(seed, output, unverifiedProof, publicKey);
		CHF = chf;
		VRF = vrf;
		VRFSeed = seed.ToArray();
		VRFProof = unverifiedProof;
		VRFOutput = output.ToArray();
		_rng = new HashRandom(chf, VRFOutput);
	}

	public byte[] NextBytes(int count) {
		return _rng.NextBytes(count);
	}
}
