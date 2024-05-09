// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen.CryptoEx.EC;
using NUnit.Framework;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Hydrogen.CryptoEx.EC.Schnorr;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
public class MuSigTest {
	private static Schnorr.PublicKey[] GetPublicKeys(MuSig muSig, Schnorr.PrivateKey[] privateKeys) {
		var publicKeys = new Schnorr.PublicKey[privateKeys.Length];
		var i = 0;
		foreach (var key in privateKeys) {
			var publicKey = muSig.Schnorr.DerivePublicKey(key);
			publicKeys[i++] = publicKey;
		}
		return publicKeys;
	}

	private static Schnorr.PrivateKey[] GetPrivateKeys(MuSig muSig, byte[][] keys) {
		var privateKeys = new Schnorr.PrivateKey[keys.Length];
		var i = 0;
		foreach (var key in keys) {
			ClassicAssert.IsTrue(muSig.Schnorr.TryParsePrivateKey(key, out var privateKey));
			privateKeys[i++] = privateKey;
		}
		return privateKeys;
	}

	private static byte[][] GetSessionIds(string[] sessionIds) {
		return sessionIds.Select(x => x.ToHexByteArray()).ToArray();
	}

	private static byte[] GetSessionCacheAsBytes(MuSigSessionCache sessionCache) {
		return Arrays.ConcatenateAll(BigIntegerUtils.BigIntegerToBytes(sessionCache.NonceCoefficient,
				32),
			BigIntegerUtils.BigIntegerToBytes(sessionCache.Challenge,
				32),
			sessionCache.FinalNonceParity ? new byte[] { 1 } : new byte[] { 0 });
	}

	[Test]
	public void TestFullMuSigWithTestVectors() {
		const string messageDigestVector = "746869735F636F756C645F62655F7468655F686173685F6F665F615F6D736721";
		const string publicKeyHashVector = "6f3282475dda826e361a617d0c1da9db9e345fc642c4775c4e56a154c10a51ed";
		const string combinedPublicKeyVector = "49c0791208d995cd9507ed594c6caa8845db411e81218c50101bac249b1771b3";
		const string combinedNonceVector = "02861f7cdfc60874b69a3743aeb83566a2b8ceacd39b9c31f23127066b61e80aef02828902ed4a86c266cb46c23b923b3577ea4fcbcb99579302ed801765d205c361";
		const string sessionCacheVector = "8c6ec9c83028ba09a10cc74e0af9890281b73b0f05440d0f261aab051fb31abaa5121da2d218956e8e69df8f5bfabf94c51bad7283cd09a07db4fb3d205ccf1b00";
		const string combinedSigsVector = "ed7d22176b48817351b197be4ff6df813c938dfc3cd5c9823640c2303e22e80fa00235ceebac132159e4e15ef107dbb027ca7a4cd557be2266ba26b8bfeced58";

		var privateKeysVector = new[] {
			"3FC866534575FA473CA1FFAAFA6A64001B5B319A6928A138A82146C367BF699C",
			"14DAD4678588D866F50B084204A69C7ECAF0E3F34B12638DD25545ED153A8128",
			"16E31D4126EE7C217EF3A45D5C99DCCE2A5F0477E694AF948A771D6735903B0F"
		};

		var sessionIdsVector = new[] {
			"7CB6E93BCF96AEE2BB31AB80AC880E108438FCECCD2E6132B49A2CF103991ED0",
			"236851BDBB4E62E06D08DC228D4E83A0A0971816EED785F994D19B165952F38D",
			"BC1DCCF8BB20655E39ABB6279152D46AB999F4C36982DB3296986DC0368319EB",
		};

		var noncesVector = new[] {
			"037800f8e53168d271ede5e04fb727f81548995524c92f657d4c36db2b4fb79fd403bbb489b7fe065e715afbc2e997a4c6d1a22f4e44222e3e36b6aedfded6958dc3",
			"03450fd04a75d11b640535780a1f1bc6f80c0fb67a9a4a8c9f7943da7f8ec21c1b03c8bca6416aab169b73f76a17cf2b740dd57c7e77d3b5dff9b2b3fab15246b715",
			"039ef367f51368c3a96fb8d53112e126efd37eca59f2a8cd4d7c8168a4647ee50f0308b633b6f68fd75ba4d10e731f588cc993752065c0cead6f2daa92a4674b5bad"
		};

		var sigsVector = new[] {
			"34ce323cd18ce1193b45f8e1e8ff4d23241053211a3bd26d240f1fbd036f5d07",
			"f985a08c5da23b409a7a8ad2501598c20e71f9566df0168082136b0c9afc0f45",
			"71ae6305bc7cf6c784245daab7f2f5c9aff70abbfc7475708069fa7bf1b7c24d"
		};

		var messageDigest = messageDigestVector.ToHexByteArray();

		var muSig = new MuSig(new Schnorr(ECDSAKeyType.SECP256K1));

		var numberOfSigners = privateKeysVector.Length;

		var privateKeys = GetPrivateKeys(muSig, privateKeysVector.Select(x => x.ToHexByteArray()).ToArray());

		// 1. derive the public keys.
		var publicKeys = GetPublicKeys(muSig, privateKeys).Select(x => x.RawBytes).ToArray();

		// 2. compute the public keys hash.
		var publicKeyHash = muSig.ComputeEll(publicKeys);

		ClassicAssert.AreEqual(publicKeyHashVector, publicKeyHash.ToHexString(true));

		// 3. get second public key
		var secondPublicKey = muSig.GetSecondPublicKey(publicKeys);

		// 4. create private signing sessions
		var sessionIds = GetSessionIds(sessionIdsVector);
		var signerSessions = new SignerMuSigSession[numberOfSigners];
		for (var i = 0; i < numberOfSigners; i++) {
			signerSessions[i] = muSig.InitializeSignerSession(sessionIds[i],
				Schnorr.BytesToBigIntPositive(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				publicKeyHash,
				secondPublicKey);
		}

		var keyCoefficients = signerSessions.Select(k => k.KeyCoefficient).ToArray();

		// 5. combine the public keys.
		var publicKeyAggregationData = muSig.AggregatePublicKeys(keyCoefficients, publicKeys);
		var combinedPublicKey = muSig.Schnorr.BytesOfXCoord(publicKeyAggregationData.CombinedPoint);

		ClassicAssert.AreEqual(combinedPublicKeyVector, combinedPublicKey.ToHexString(true));

		var publicKeyParity = publicKeyAggregationData.PublicKeyParity;

		/* Communication round 1: A production system would exchange public nonces
		* here before moving on. */

		// 6. combine nonce
		var publicNonces = signerSessions.Select(x => x.PublicNonce).ToArray();

		ClassicAssert.AreEqual(noncesVector.Select(x => x.ToHexByteArray()).ToArray(), publicNonces);

		/* Create aggregate nonce */
		var combinedNonce = muSig.AggregatePublicNonces(publicNonces, combinedPublicKey, messageDigest);

		ClassicAssert.AreEqual(combinedNonceVector, combinedNonce.AggregatedNonce.ToHexString(true));
		// 7. compute challenge
		var challenge = muSig.ComputeChallenge(combinedNonce.FinalNonce, combinedPublicKey, messageDigest);

		// 8. initialize musig session cache. same for all signers
		var sessionCache = muSig.InitializeSessionCache(combinedNonce, challenge, publicKeyParity);

		ClassicAssert.AreEqual(sessionCacheVector, GetSessionCacheAsBytes(sessionCache).ToHexString(true));

		// 9. generate partial signatures
		var partialSignatures = new BigInteger[numberOfSigners];
		for (var i = 0; i < signerSessions.Length; i++) {
			partialSignatures[i] = muSig.PartialSign(signerSessions[i], sessionCache);
		}

		ClassicAssert.AreEqual(sigsVector.Select(x => x.ToHexByteArray()),
			partialSignatures.Select(x => BigIntegerUtils.BigIntegerToBytes(x,
					32))
				.ToArray());

		/* Communication round 2: A production system would exchange
		* partial signatures here before moving on. */

		// 10. verify individual partial signatures
		for (var i = 0; i < numberOfSigners; i++) {
			ClassicAssert.IsTrue(muSig.PartialSigVerify(signerSessions[i], sessionCache, publicKeys[i], partialSignatures[i]));
		}

		// 11. combine partial signatures
		var combinedSignature = muSig.AggregatePartialSignatures(sessionCache.FinalNonce, partialSignatures);
		ClassicAssert.AreEqual(combinedSigsVector, combinedSignature.ToHexString(true));

		// 12. validate combined signature
		ClassicAssert.IsTrue(muSig.Schnorr.VerifyDigest(combinedSignature, messageDigest, combinedPublicKey));
	}

	[Test]
	[TestCase(ECDSAKeyType.SECP256K1), Repeat(64)]
	public void TestFullMuSigWithRandomSignAndVerify(ECDSAKeyType keyType) {
		// messageDigest must be 32 bytes in length
		var messageDigest = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);

		var muSig = new MuSig(new Schnorr(keyType));

		var numberOfSigners = new Random().Next(2, 16);
		var keys = new List<byte[]>();
		for (var i = 0; i < numberOfSigners; i++) {
			keys.Add(Tools.Crypto.GenerateCryptographicallyRandomBytes(muSig.KeySize));
		}
		var privateKeys = GetPrivateKeys(muSig, keys.ToArray());

		// 1. derive the public keys.
		var publicKeys = GetPublicKeys(muSig, privateKeys).Select(x => x.RawBytes).ToArray();

		// 2. compute the public keys hash.
		var allPublicKeysHash = muSig.ComputeEll(publicKeys);

		// 3. get second public key
		var secondPublicKey = muSig.GetSecondPublicKey(publicKeys);

		// 4. create private signing sessions
		var signerSessions = new SignerMuSigSession[numberOfSigners];
		for (var i = 0; i < numberOfSigners; i++) {
			signerSessions[i] = muSig.InitializeSignerSession(Tools.Crypto.GenerateCryptographicallyRandomBytes(32),
				Schnorr.BytesToBigIntPositive(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				allPublicKeysHash,
				secondPublicKey);
		}

		var keyCoefficients = signerSessions.Select(k => k.KeyCoefficient).ToArray();

		// 5. combine the public keys.
		var publicKeyAggregationData = muSig.AggregatePublicKeys(keyCoefficients, publicKeys);
		var combinedPublicKey = muSig.Schnorr.BytesOfXCoord(publicKeyAggregationData.CombinedPoint);
		var publicKeyParity = publicKeyAggregationData.PublicKeyParity;

		/* Communication round 1: A production system would exchange public nonces
		* here before moving on. */

		// 6. combine nonce
		var publicNonces = signerSessions.Select(x => x.PublicNonce).ToArray();

		/* Create aggregate nonce */
		var combinedNonce = muSig.AggregatePublicNonces(publicNonces, combinedPublicKey, messageDigest);

		// 7. compute challenge
		var challenge = muSig.ComputeChallenge(combinedNonce.FinalNonce, combinedPublicKey, messageDigest);

		// 8. initialize musig session cache. same for all signers
		var sessionCache = muSig.InitializeSessionCache(combinedNonce, challenge, publicKeyParity);

		// 9. generate partial signatures
		var partialSignatures = new BigInteger[numberOfSigners];
		for (var i = 0; i < signerSessions.Length; i++) {
			partialSignatures[i] = muSig.PartialSign(signerSessions[i], sessionCache);
		}

		/* Communication round 2: A production system would exchange
		* partial signatures here before moving on. */

		// 10. verify individual partial signatures
		for (var i = 0; i < numberOfSigners; i++) {
			ClassicAssert.IsTrue(muSig.PartialSigVerify(signerSessions[i], sessionCache, publicKeys[i], partialSignatures[i]));
		}

		// 11. combine partial signatures
		var combinedSignature = muSig.AggregatePartialSignatures(sessionCache.FinalNonce, partialSignatures);

		// 11. validate combined signature
		ClassicAssert.IsTrue(muSig.Schnorr.VerifyDigest(combinedSignature, messageDigest, combinedPublicKey));
	}

}
