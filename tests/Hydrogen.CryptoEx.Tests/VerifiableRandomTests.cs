// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Maths;
using NUnit.Framework;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
public class VerifiableRandomTests {

	[Test]
	public void Integration(
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
		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);


		// Generate 1MB of random bytes using private key
		var sourceGenerator = new VerfiableRandom(chf, dss, seed, privateKey, nonce);
		var sourceBytes = sourceGenerator.NextBytes(1048576);

		// Re-generate 1mb of random bytes using public key
		var verifierGenerator = new VerfiableRandom(chf, dss, seed, publicKey, sourceGenerator.VRFProof);
		var destBytes = verifierGenerator.NextBytes(1048576);

		// Ensure bytes generated same
		Assert.That(destBytes, Is.EqualTo(sourceBytes).Using(ByteArrayEqualityComparer.Instance));

		// Ensure statistically random
		var globalStats = new Statistics();
		var stats = new Statistics[256];
		for(var i = 0; i < 256; i++) {
			globalStats.AddDatum(sourceBytes[i]);
			stats[i] = new Statistics();
			for (var j = 0; j < 256; j++) { 
				stats[i].AddDatum(i == j ? 1 : 0);
			}
		}

		// NOTE: error margin of 20 is used here but this is due to loss of precision in global stats, there's lots of doubles being aggregated
		// the below stats are more accurate
		Assert.That((decimal)globalStats.Mean, Is.EqualTo(128M).Using(new ErrorBandEqualityComparer(20M)), $"Expected mean 128 (+/- 20) but was {globalStats.Mean}"); 

		// Ensure every byte occured with equal probability
		var byteStatComparer = new ErrorBandEqualityComparer(0.000001M);
		for(var i = 0; i < 256; i++) 
			Assert.That((decimal)stats[i].Mean, Is.EqualTo(1/256M).Using(byteStatComparer), $"Byte {i} expected mean 1/256 (+/- 0.000001) but was {stats[i].Mean}");
		
	}


		[Test]
	public void IntegrationFails(
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
		var seed = rng.NextBytes(seedLen);

		var privateKey = Signers.GeneratePrivateKey(dss);
		var publicKey = Signers.DerivePublicKey(dss, privateKey, nonce);


		var sourceGenerator = new VerfiableRandom(chf, dss, seed, privateKey, nonce);
		
		// vary a seed byte randomly (try every index)
		var badSeed = Tools.Array.Clone(sourceGenerator.VRFSeed);
		for (var i = 0; i < seed.Length; i++) {
			while ((badSeed[i] = rng.NextByte()) == sourceGenerator.VRFSeed[i]);
			Assert.That(() => new VerfiableRandom(chf, dss, badSeed, publicKey, sourceGenerator.VRFProof), Throws.InvalidOperationException);
		}

		// vary proof byte randomly (try every index)
		var badProof = Tools.Array.Clone(sourceGenerator.VRFProof);
		for (var i = 0; i < sourceGenerator.VRFProof.Length; i++) {
			while ((badProof[i] = rng.NextByte()) == sourceGenerator.VRFProof[i]);
			Assert.That(() => new VerfiableRandom(chf, dss, sourceGenerator.VRFSeed, publicKey, badProof), Throws.InvalidOperationException);
		}
		
	}
}
