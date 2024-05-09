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
using Org.BouncyCastle.Utilities;
using Hydrogen.CryptoEx.EC.Schnorr;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
public class MuSigBuilderTest {
	private static readonly MuSig MuSig = new(new Schnorr(ECDSAKeyType.SECP256K1));

	private static readonly byte[] MessageDigest =
		"746869735F636F756C645F62655F7468655F686173685F6F665F615F6D736721".ToHexByteArray();

	private static readonly byte[] AggregatedSig =
		"ed7d22176b48817351b197be4ff6df813c938dfc3cd5c9823640c2303e22e80fa00235ceebac132159e4e15ef107dbb027ca7a4cd557be2266ba26b8bfeced58"
			.ToHexByteArray();

	private static readonly Schnorr.PrivateKey AlicePrivateKey = GetPrivateKey(MuSig,
		"3FC866534575FA473CA1FFAAFA6A64001B5B319A6928A138A82146C367BF699C".ToHexByteArray());

	private static readonly Schnorr.PrivateKey BobPrivateKey = GetPrivateKey(MuSig,
		"14DAD4678588D866F50B084204A69C7ECAF0E3F34B12638DD25545ED153A8128".ToHexByteArray());

	private static readonly Schnorr.PrivateKey CharliePrivateKey = GetPrivateKey(MuSig,
		"16E31D4126EE7C217EF3A45D5C99DCCE2A5F0477E694AF948A771D6735903B0F".ToHexByteArray());

	private static readonly byte[] AliceSessionId =
		"7CB6E93BCF96AEE2BB31AB80AC880E108438FCECCD2E6132B49A2CF103991ED0".ToHexByteArray();

	private static readonly byte[] BobSessionId =
		"236851BDBB4E62E06D08DC228D4E83A0A0971816EED785F994D19B165952F38D".ToHexByteArray();

	private static readonly byte[] CharlieSessionId =
		"BC1DCCF8BB20655E39ABB6279152D46AB999F4C36982DB3296986DC0368319EB".ToHexByteArray();

	private static readonly byte[] AlicePublicKey = GetPublicKey(MuSig, AlicePrivateKey).RawBytes;
	private static readonly byte[] BobPublicKey = GetPublicKey(MuSig, BobPrivateKey).RawBytes;
	private static readonly byte[] CharliePublicKey = GetPublicKey(MuSig, CharliePrivateKey).RawBytes;

	private static readonly Random Random = new();

	private static void SlightlyTamperWithPartialSig(byte[] partialSig) {
		var copy = Arrays.CopyOf(partialSig, partialSig.Length);
		do {
			var indexToModify = Random.Next(0, partialSig.Length);
			var newValue = (byte)Random.Next(0, 255);
			partialSig[indexToModify] = newValue;
		} while (copy.SequenceEqual(partialSig));
	}

	private static Schnorr.PublicKey GetPublicKey(MuSig muSig, Schnorr.PrivateKey privateKey) {
		return muSig.Schnorr.DerivePublicKey(privateKey);
	}

	private static Schnorr.PrivateKey GetPrivateKey(MuSig muSig, byte[] privateKeyBytes) {
		_ = muSig.Schnorr.TryParsePrivateKey(privateKeyBytes, out var privateKey);
		return privateKey;
	}

	[Test]
	// test that method sorts public Keys in lexicographic order.
	public void TestSortPublicKeysInLexicographicOrder() {
		var input = new List<byte[]> {
			new byte[] { 86 },
			new byte[] { 0 },
			new byte[] { 255 },
			new byte[] { 86 },
			new byte[] { 55 },
		}.ToArray();

		var expected = new List<byte[]> {
			new byte[] { 0 },
			new byte[] { 55 },
			new byte[] { 86 },
			new byte[] { 86 },
			new byte[] { 255 },
		};
		MuSigBuilder.SortPublicKeysInLexicographicOrder(input);
		ClassicAssert.AreEqual(expected, input);
	}

	[Test]
	// this test performs a muSig between 3 parties using known test vectors.
	public void TestFullMuSigBuilderWithKnownTestVectors() {
		var aliceMuSigBuilder = new MuSigBuilder(AlicePrivateKey, MessageDigest, AliceSessionId);
		var bobMuSigBuilder = new MuSigBuilder(BobPrivateKey, MessageDigest, BobSessionId);
		var charlieMuSigBuilder = new MuSigBuilder(CharliePrivateKey, MessageDigest, CharlieSessionId);

		// it is important to maintain same order when passing the public keys between builders as
		// the output of the KeyAgg algorithm depends on the order of the input public keys.
		aliceMuSigBuilder.AddPublicKey(AlicePublicKey);
		aliceMuSigBuilder.AddPublicKey(BobPublicKey);
		aliceMuSigBuilder.AddPublicKey(CharliePublicKey);

		bobMuSigBuilder.AddPublicKey(AlicePublicKey);
		bobMuSigBuilder.AddPublicKey(BobPublicKey);
		bobMuSigBuilder.AddPublicKey(CharliePublicKey);

		charlieMuSigBuilder.AddPublicKey(AlicePublicKey);
		charlieMuSigBuilder.AddPublicKey(BobPublicKey);
		charlieMuSigBuilder.AddPublicKey(CharliePublicKey);


		// add public nonce
		// order between builders doesn't matter in the case of nonce
		aliceMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PublicNonce);
		aliceMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PublicNonce);
		aliceMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PublicNonce);

		bobMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PublicNonce);
		bobMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PublicNonce);
		bobMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PublicNonce);

		charlieMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PublicNonce);
		charlieMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PublicNonce);
		charlieMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PublicNonce);


		// add partial signature
		// order between builders doesn't matter in the case of partial signature
		aliceMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PartialSignature);
		aliceMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PartialSignature);
		aliceMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PartialSignature);

		bobMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PartialSignature);
		bobMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PartialSignature);
		bobMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PartialSignature);

		charlieMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PartialSignature);
		charlieMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PartialSignature);
		charlieMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PartialSignature);

		var aliceAggregatedSignature = aliceMuSigBuilder.BuildAggregatedSignature();
		var bobAggregatedSignature = bobMuSigBuilder.BuildAggregatedSignature();
		var charlieAggregatedSignature = charlieMuSigBuilder.BuildAggregatedSignature();

		// all participants validate each other partial signatures.
		Assert.DoesNotThrow(aliceMuSigBuilder.VerifyPartialSignatures);
		Assert.DoesNotThrow(bobMuSigBuilder.VerifyPartialSignatures);
		Assert.DoesNotThrow(charlieMuSigBuilder.VerifyPartialSignatures);

		ClassicAssert.AreEqual(aliceAggregatedSignature.AggregatedSignature, bobAggregatedSignature.AggregatedSignature);
		ClassicAssert.AreEqual(bobAggregatedSignature.AggregatedSignature, charlieAggregatedSignature.AggregatedSignature);
		ClassicAssert.AreEqual(aliceAggregatedSignature.AggregatedSignature, AggregatedSig);
		// since all aggregated signatures are same from above check, we can just verify one.
		ClassicAssert.IsTrue(MuSig.Schnorr.VerifyDigest(charlieAggregatedSignature.AggregatedSignature,
			MessageDigest,
			charlieAggregatedSignature.AggregatedPublicKey));
	}

	[Test]
	[Repeat(64)]
	// this test performs a muSig between 3 parties using known test vectors where one participant is dishonest.
	public void TestFullMuSigBuilderWithKnownTestVectorsWhereAParticipantIsDishonest() {
		var aliceMuSigBuilder = new MuSigBuilder(AlicePrivateKey, MessageDigest, AliceSessionId);
		var bobMuSigBuilder = new MuSigBuilder(BobPrivateKey, MessageDigest, BobSessionId);
		var charlieMuSigBuilder = new MuSigBuilder(CharliePrivateKey, MessageDigest, CharlieSessionId);

		// it is important to maintain same order when passing the public keys between builders as
		// the output of the KeyAgg algorithm depends on the order of the input public keys.
		aliceMuSigBuilder.AddPublicKey(AlicePublicKey);
		aliceMuSigBuilder.AddPublicKey(BobPublicKey);
		aliceMuSigBuilder.AddPublicKey(CharliePublicKey);

		bobMuSigBuilder.AddPublicKey(AlicePublicKey);
		bobMuSigBuilder.AddPublicKey(BobPublicKey);
		bobMuSigBuilder.AddPublicKey(CharliePublicKey);

		charlieMuSigBuilder.AddPublicKey(AlicePublicKey);
		charlieMuSigBuilder.AddPublicKey(BobPublicKey);
		charlieMuSigBuilder.AddPublicKey(CharliePublicKey);

		// add public nonce
		// order between builders doesn't matter in the case of nonce
		aliceMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PublicNonce);
		aliceMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PublicNonce);
		aliceMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PublicNonce);

		bobMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PublicNonce);
		bobMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PublicNonce);
		bobMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PublicNonce);

		charlieMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PublicNonce);
		charlieMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PublicNonce);
		charlieMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PublicNonce);

		// add partial signature
		// order between builders doesn't matter in the case of partial signature
		// let's make "bob" dishonest by tampering with his signature
		// tamper with bob's partial signature
		SlightlyTamperWithPartialSig(bobMuSigBuilder.PartialSignature);
		aliceMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PartialSignature);
		aliceMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PartialSignature);
		aliceMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PartialSignature);

		bobMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PartialSignature);
		bobMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PartialSignature);
		bobMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PartialSignature);

		charlieMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PublicKey, aliceMuSigBuilder.PartialSignature);
		charlieMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PublicKey, bobMuSigBuilder.PartialSignature);
		charlieMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PublicKey, charlieMuSigBuilder.PartialSignature);

		// all participants validate each other partial signatures.
		var errorMessage =
			$"partial signature verification of participant (publicKey: {bobMuSigBuilder.PublicKey.ToHexString()}) failed";
		var result = Assert.Throws<InvalidOperationException>(() => aliceMuSigBuilder.VerifyPartialSignatures());
		ClassicAssert.AreEqual(result?.Message, errorMessage);
		result = Assert.Throws<InvalidOperationException>(() => bobMuSigBuilder.VerifyPartialSignatures());
		ClassicAssert.AreEqual(result?.Message, errorMessage);
		result = Assert.Throws<InvalidOperationException>(() => charlieMuSigBuilder.VerifyPartialSignatures());
		ClassicAssert.AreEqual(result?.Message, errorMessage);

		var aliceAggregatedSignature = aliceMuSigBuilder.BuildAggregatedSignature();
		var bobAggregatedSignature = bobMuSigBuilder.BuildAggregatedSignature();
		var charlieAggregatedSignature = charlieMuSigBuilder.BuildAggregatedSignature();

		ClassicAssert.IsFalse(MuSig.Schnorr.VerifyDigest(aliceAggregatedSignature.AggregatedSignature,
			MessageDigest,
			aliceAggregatedSignature.AggregatedPublicKey));

		ClassicAssert.IsFalse(MuSig.Schnorr.VerifyDigest(bobAggregatedSignature.AggregatedSignature,
			MessageDigest,
			bobAggregatedSignature.AggregatedPublicKey));

		ClassicAssert.IsFalse(MuSig.Schnorr.VerifyDigest(charlieAggregatedSignature.AggregatedSignature,
			MessageDigest,
			charlieAggregatedSignature.AggregatedPublicKey));
	}

	// this test performs a muSig between random number of parties using random values.
	[Test]
	[TestCase(ECDSAKeyType.SECP256K1), Repeat(64)]
	public void TestFullMuSigBuilderWithRandomInputs(ECDSAKeyType keyType) {
		var muSig = new MuSig(new Schnorr(keyType));
		// messageDigest must be 32 bytes in length
		var messageDigest = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);

		var numberOfSigners = Random.Next(2, 16);
		var publicKeys = new List<byte[]>();
		var muSigBuilders = new List<MuSigBuilder>();

		for (var i = 0; i < numberOfSigners; i++) {
			var privateKey = GetPrivateKey(muSig, Tools.Crypto.GenerateCryptographicallyRandomBytes(muSig.KeySize));
			publicKeys.Add(GetPublicKey(muSig, privateKey).RawBytes);
			muSigBuilders.Add(new MuSigBuilder(privateKey, messageDigest));
		}

		for (var i = 0; i < numberOfSigners; i++) {
			for (var j = 0; j < numberOfSigners; j++) {
				muSigBuilders[i].AddPublicKey(publicKeys[j]);
			}
		}

		for (var i = 0; i < numberOfSigners; i++) {
			for (var j = 0; j < numberOfSigners; j++) {
				muSigBuilders[i].AddPublicNonce(muSigBuilders[j].PublicKey, muSigBuilders[j].PublicNonce);
			}
		}

		for (var i = 0; i < numberOfSigners; i++) {
			for (var j = 0; j < numberOfSigners; j++) {
				muSigBuilders[i].AddPartialSignature(muSigBuilders[j].PublicKey, muSigBuilders[j].PartialSignature);
			}
		}

		for (var i = 0; i < numberOfSigners; i++) {
			// all participants validate each other partial signatures.
			Assert.DoesNotThrow(muSigBuilders[i].VerifyPartialSignatures);
		}

		var aggregatedSignatures = new List<MuSigData>();
		for (var i = 0; i < numberOfSigners; i++) {
			aggregatedSignatures.Add(muSigBuilders[i].BuildAggregatedSignature());
		}

		ClassicAssert.IsTrue(aggregatedSignatures.All(bytes => bytes.AggregatedSignature.SequenceEqual(aggregatedSignatures
			.First()
			.AggregatedSignature)));
		// since all aggregated signatures are same from above check, we can just verify one.
		ClassicAssert.IsTrue(muSig.Schnorr.VerifyDigest(aggregatedSignatures.Last().AggregatedSignature,
			messageDigest,
			aggregatedSignatures.Last().AggregatedPublicKey));
	}

	// this test performs a muSig between random number of parties using random values where one random participant is dishonest
	[Test]
	[TestCase(ECDSAKeyType.SECP256K1), Repeat(64)]
	public void TestFullMuSigBuilderWithRandomInputsWhereAParticipantIsDishonest(ECDSAKeyType keyType) {
		var muSig = new MuSig(new Schnorr(keyType));
		// messageDigest must be 32 bytes in length
		var messageDigest = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);

		var numberOfSigners = Random.Next(2, 16);
		var publicKeys = new List<byte[]>();
		var muSigBuilders = new List<MuSigBuilder>();

		for (var i = 0; i < numberOfSigners; i++) {
			var privateKey = GetPrivateKey(muSig, Tools.Crypto.GenerateCryptographicallyRandomBytes(muSig.KeySize));
			publicKeys.Add(GetPublicKey(muSig, privateKey).RawBytes);
			muSigBuilders.Add(new MuSigBuilder(privateKey, messageDigest));
		}

		for (var i = 0; i < numberOfSigners; i++) {
			for (var j = 0; j < numberOfSigners; j++) {
				muSigBuilders[i].AddPublicKey(publicKeys[j]);
			}
		}

		for (var i = 0; i < numberOfSigners; i++) {
			for (var j = 0; j < numberOfSigners; j++) {
				muSigBuilders[i].AddPublicNonce(muSigBuilders[j].PublicKey, muSigBuilders[j].PublicNonce);
			}
		}

		// let's make a random participant (codenamed "anna") dishonest by tampering with her signature
		var randomParticipantIndex = Random.Next(0, numberOfSigners);
		var anna = muSigBuilders[randomParticipantIndex];
		// tamper with anna's partial signature
		SlightlyTamperWithPartialSig(anna.PartialSignature);

		for (var i = 0; i < numberOfSigners; i++) {
			for (var j = 0; j < numberOfSigners; j++) {
				muSigBuilders[i].AddPartialSignature(muSigBuilders[j].PublicKey, muSigBuilders[j].PartialSignature);
			}
		}

		var errorMessage =
			$"partial signature verification of participant (publicKey: {anna.PublicKey.ToHexString()}) failed";
		for (var i = 0; i < numberOfSigners; i++) {
			// all participants validate each other partial signatures.
			var result = Assert.Throws<InvalidOperationException>(() => muSigBuilders[i].VerifyPartialSignatures());
			ClassicAssert.AreEqual(result?.Message, errorMessage);
		}

		var aggregatedSignatures = new List<MuSigData>();
		for (var i = 0; i < numberOfSigners; i++) {
			aggregatedSignatures.Add(muSigBuilders[i].BuildAggregatedSignature());
		}

		for (var i = 0; i < numberOfSigners; i++) {
			ClassicAssert.IsFalse(MuSig.Schnorr.VerifyDigest(aggregatedSignatures[i].AggregatedSignature,
				MessageDigest,
				aggregatedSignatures[i].AggregatedPublicKey));
		}
	}
}
