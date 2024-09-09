// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
public class VRFTests {

	private IVRFAlgorithm BuildVRF(DSS dss, CHF chf)
		=> VRF.CreateCryptographicVRF(chf, dss);

	[Test]
	public void ValidProofAndOutputsPass(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL)]
		ulong nonce,

		[Values(32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		var output = vrf.Run(seed, privateKey, nonce, out var proof);

		var isValid = vrf.TryVerify(seed, output, proof, publicKey);
		Assert.That(isValid, Is.True);
	}


	#region Bad Proofs

	[Test]
	public void BadProofFails(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL, ulong.MaxValue - 1)]
		ulong nonce,

		[Values(0, 1, 10, 11, 32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		var output = vrf.Run(seed, privateKey, nonce, out var proof);

		for (var i = 0; i < proof.Length; i++) {
			// Randomize the i'th byte in the proof
			var badProof = Tools.Array.Clone(proof);
			while ((badProof[i] = rng.NextByte()) == proof[i]) ;

			// Should fail
			var isValid = vrf.TryVerify(seed, output, badProof, publicKey);
			Assert.That(isValid, Is.False);
		}
	}

	[Test]
	public void EmptyProofsThrow(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL, ulong.MaxValue)]
		ulong nonce,

		[Values(0, 1, 10, 11, 32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		var output = vrf.Run(seed, privateKey, nonce, out _);

		// Should throw
		Assert.That(() => vrf.TryVerify(seed, output, Array.Empty<byte>(), publicKey), Throws.ArgumentException);
	}

	[Test]
	public void NullProofThrows(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL, ulong.MaxValue)]
		ulong nonce,

		[Values(0, 1, 10, 11, 32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		var output = vrf.Run(seed, privateKey, nonce, out _);

		Assert.That(() => vrf.TryVerify(seed, output, null, publicKey), Throws.ArgumentException);

	}


	#endregion

	#region Bad Outputs 

	[Test]
	public void AlteredOutputFails(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL, ulong.MaxValue)]
		ulong nonce,

		[Values(0, 1, 10, 11, 32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		var output = vrf.Run(seed, privateKey, nonce, out var proof);

		for (var i = 0; i < output.Length; i++) {
			// Randomize the i'th byte in the proof
			var badOutput = Tools.Array.Clone(output);
			while ((badOutput[i] = rng.NextByte()) == output[i]) ;

			// Should fail
			var isValid = vrf.TryVerify(seed, badOutput, proof, publicKey);
			Assert.That(isValid, Is.False);
		}
	}


	[Test]
	public void EmptyOutputThrows(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL, ulong.MaxValue)]
		ulong nonce,

		[Values(0, 1, 10, 11, 32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		vrf.Run(seed, privateKey, nonce, out var proof);

		// Should throw
		Assert.That(() => vrf.TryVerify(seed, Array.Empty<byte>(), proof, publicKey), Throws.ArgumentException);
	}

	[Test]
	public void NullOutputThrows(
		[Values(DSS.ECDSA_SECP256k1, DSS.ECDSA_SECP384R1, DSS.ECDSA_SECP521R1, DSS.ECDSA_SECT283K1, DSS.PQC_WAMS, DSS.PQC_WAMSSharp)]
		DSS dss,

		[Values(CHF.SHA2_256, CHF.SHA2_512, CHF.SHA3_256, CHF.Blake2b_256, CHF.Blake2b_128)]
		CHF chf,

		[Values(0UL, 1UL, 111UL, ulong.MaxValue)]
		ulong nonce,

		[Values(0, 1, 10, 11, 32, 256, 1024)]
		int seedLen
	) {
		var rng = new Random(31337);
		var vrf = BuildVRF(dss, chf);

		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);

		vrf.Run(seed, privateKey, nonce, out var proof);

		// Should throw
		Assert.That(() => vrf.TryVerify(seed, null, proof, publicKey), Throws.ArgumentException);
	}

	#endregion
}
