// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Hydrogen.CryptoEx.EC;
using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework.Legacy;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ECDSATests {
	private const string InvalidDerSignature = "Invalid DER Signature";
	private const string InvalidRAndSSignature = "Invalid R and S Signature";

	private static string RandomString(int length) {
		var rng = new Random(31337);
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[rng.Next(s.Length)]).ToArray());
	}

	private static bool IsLowS(BigInteger order, BigInteger s) {
		return s.CompareTo(order.ShiftRight(1)) <= 0;
	}

	private static BigInteger[] DerSig_To_R_And_S(byte[] derSig) {
		if (derSig == null) {
			throw new ArgumentNullException(nameof(derSig));
		}

		try {
			var decoder = new Asn1InputStream(derSig);
			var derSequence = decoder.ReadObject() as DerSequence;
			if (derSequence is not { Count: 2 }) {
				throw new FormatException(InvalidDerSignature);
			}

			var r = (derSequence[0] as DerInteger)?.Value;
			var s = (derSequence[1] as DerInteger)?.Value;
			return new[] { r, s };
		} catch (Exception ex) {
			throw new FormatException(InvalidDerSignature, ex);
		}
	}

	private static byte[] R_And_S_To_DerSig(BigInteger[] rs) {
		if (rs == null)
			throw new ArgumentNullException(nameof(rs));
		if (rs.Length != 2) {
			throw new ArgumentException(InvalidRAndSSignature, nameof(rs));
		}

		using (var outputStream = new MemoryStream()) {
			using (var sequenceGenerator = new DerSequenceGenerator(outputStream)) {
				sequenceGenerator.AddObject(new DerInteger(rs[0]));
				sequenceGenerator.AddObject(new DerInteger(rs[1]));
			}
			return outputStream.ToArray();
		}
	}

	public static byte[] CanonicalizeSig(BigInteger order, byte[] sig) {
		var rs = DerSig_To_R_And_S(sig);
		if (!IsLowS(order, rs[1])) {
			rs[1] = order.Subtract(rs[1]);
			return R_And_S_To_DerSig(rs);
		}
		return sig;
	}

	[Test, Repeat(64)]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	[TestCase(ECDSAKeyType.SECT283K1)]
	public void SignVerify_Basic(ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
		var privateKey = ecdsa.GeneratePrivateKey(secret);
		var publicKey = ecdsa.DerivePublicKey(privateKey);
		var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
		var sig = ecdsa.Sign(privateKey, message);
		ClassicAssert.IsTrue(ecdsa.Verify(sig, message, publicKey));
	}

	[Test]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	[TestCase(ECDSAKeyType.SECT283K1)]
	public void IsPublicKey(ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
		var privateKey = ecdsa.GeneratePrivateKey(secret);
		var publicKey = ecdsa.DerivePublicKey(privateKey);
		ClassicAssert.IsTrue(ecdsa.IsPublicKey(privateKey, publicKey.RawBytes));
	}

	[Test]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	[TestCase(ECDSAKeyType.SECT283K1)]
	public void VerifyThatTryParsePrivateKeyPassForGoodKeys(ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		var privateKeyBytes = ecdsa.GeneratePrivateKey().RawBytes;
		ClassicAssert.IsTrue(ecdsa.TryParsePrivateKey(privateKeyBytes, out var privateKey));
		ClassicAssert.AreEqual(privateKeyBytes, privateKey.RawBytes);
	}

	[Test]
	[TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP256K1)]
	[TestCase(new byte[] { }, ECDSAKeyType.SECP384R1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP384R1)]
	[TestCase(new byte[] { }, ECDSAKeyType.SECP521R1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP521R1)]
	[TestCase(new byte[] { }, ECDSAKeyType.SECT283K1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECT283K1)]
	public void VerifyThatTryParsePrivateKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		ClassicAssert.IsFalse(ecdsa.TryParsePrivateKey(badRawKey, out _));
	}

	[Test]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	[TestCase(ECDSAKeyType.SECT283K1)]
	public void VerifyThatTryParsePrivateKeyFailsForValuesNotInBetweenZeroToCurveOrderMinusOne(ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		var negativeOne = BigInteger.One.Negate();
		ClassicAssert.IsFalse(ecdsa.TryParsePrivateKey(negativeOne.ToByteArray(), out _));
		var order = keyType.GetAttribute<KeyTypeOrderAttribute>().Value;
		ClassicAssert.IsFalse(ecdsa.TryParsePrivateKey(BigIntegerUtils.BigIntegerToBytes(order.Add(BigInteger.One), ecdsa.KeySize), out _));
	}

	[Test]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	[TestCase(ECDSAKeyType.SECT283K1)]
	public void VerifyThatTryParsePublicKeyPassForGoodKeys(ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		var privateKey = ecdsa.GeneratePrivateKey();
		var publicKeyBytes = ecdsa.DerivePublicKey(privateKey).RawBytes;
		ClassicAssert.IsTrue(ecdsa.TryParsePublicKey(publicKeyBytes, out var publicKey));
		ClassicAssert.AreEqual(publicKeyBytes, publicKey.RawBytes);
	}

	[Test]
	[TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP256K1)]
	[TestCase(new byte[] { }, ECDSAKeyType.SECP384R1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP384R1)]
	[TestCase(new byte[] { }, ECDSAKeyType.SECP521R1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP521R1)]
	[TestCase(new byte[] { }, ECDSAKeyType.SECT283K1)]
	[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECT283K1)]
	public void VerifyThatTryParsePublicKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		ClassicAssert.IsFalse(ecdsa.TryParsePublicKey(badRawKey, out _));
	}

	//SECT283K1 excluded as it is a binary curve
	[Test]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	public void VerifyThatTryParsePublicKeyFailsForValuesNotInBetweenZeroToPrimeFieldMinusOne(ECDSAKeyType keyType) {
		var ecdsa = new ECDSA(keyType);
		var negativeOne = BigInteger.One.Negate();
		ClassicAssert.IsFalse(ecdsa.TryParsePublicKey(negativeOne.ToByteArray(), out _));
		var primeField = keyType.GetAttribute<KeyTypePrimeFieldAttribute>().Value;
		ClassicAssert.IsFalse(ecdsa.TryParsePublicKey(BigIntegerUtils.BigIntegerToBytes(primeField, ecdsa.CompressedPublicKeySize), out _));
	}

	[Test, Repeat(64)]
	[TestCase(ECDSAKeyType.SECP256K1)]
	[TestCase(ECDSAKeyType.SECP384R1)]
	[TestCase(ECDSAKeyType.SECP521R1)]
	[TestCase(ECDSAKeyType.SECT283K1)]
	public void TestSignatureMalleability_Low_S(ECDSAKeyType keyType) {
		var ecdsaNoMalleability = new ECDSA(keyType);
		var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
		var privateKey = ecdsaNoMalleability.GeneratePrivateKey(secret);
		var publicKey = ecdsaNoMalleability.DerivePublicKey(privateKey);
		var messageDigest = Hashers.Hash(CHF.SHA2_256,
			Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));

		var order = privateKey.Parameters.Value.Parameters.Curve.Order;

		var ecdsaAllowMalleability = SignerUtilities.GetSigner("NONEwithECDSA");

		var secureRandom = new SecureRandom();
		byte[] ecdsaAllowMalleabilitySig;
		BigInteger[] sig;
		// generate a "High S" signature
		do {
			var parametersWithRandom = new ParametersWithRandom(privateKey.Parameters.Value, secureRandom);
			ecdsaAllowMalleability.Init(true, parametersWithRandom);
			ecdsaAllowMalleability.BlockUpdate(messageDigest, 0, messageDigest.Length);

			ecdsaAllowMalleabilitySig = ecdsaAllowMalleability.GenerateSignature();
			sig = DerSig_To_R_And_S(ecdsaAllowMalleabilitySig);
		} while (IsLowS(order, sig[1]));

		var canonicalSig = CanonicalizeSig(order, ecdsaAllowMalleabilitySig);

		// normal ECDSA should be able to verify both the OriginalSig and CanonicalSig
		ecdsaAllowMalleability.Init(false, publicKey.Parameters.Value);
		ecdsaAllowMalleability.BlockUpdate(messageDigest, 0, messageDigest.Length);
		ClassicAssert.IsTrue(ecdsaAllowMalleability.VerifySignature(ecdsaAllowMalleabilitySig));

		ecdsaAllowMalleability.Init(false, publicKey.Parameters.Value);
		ecdsaAllowMalleability.BlockUpdate(messageDigest, 0, messageDigest.Length);
		ClassicAssert.IsTrue(ecdsaAllowMalleability.VerifySignature(canonicalSig));

		// our LowS ECDSA should be able to verify only the CanonicalSig
		ClassicAssert.IsFalse(ecdsaNoMalleability.VerifyDigest(ecdsaAllowMalleabilitySig, messageDigest, publicKey));
		ClassicAssert.IsTrue(ecdsaNoMalleability.VerifyDigest(canonicalSig, messageDigest, publicKey));
	}

	[Test]
	[TestCase("0x0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
	[TestCase("0x30220220000000000000000000000000000000000000000000000000000000000000000000")]
	[TestCase("0x3024021077777777777777777777777777777777020a7777777777777777777777777777777701")]
	[TestCase("0x302403107777777777777777777777777777777702107777777777777777777777777777777701")]
	[TestCase("0x302402107777777777777777777777777777777703107777777777777777777777777777777701")]
	[TestCase("0x3014020002107777777777777777777777777777777701")]
	[TestCase("0x3014021077777777777777777777777777777777020001")]
	[TestCase("302402107777777777777777777777777777777702108777777777777777777777777777777701")]
	public void TestSignatureMalleability_Invalid_Strict_DER(string badDerSig) {
		var invalidBip66Der = badDerSig.ToHexByteArray();
		ClassicAssert.IsFalse(CustomDsaEncoding.IsValidSignatureEncoding(invalidBip66Der));
	}

	[Test]
	[TestCase("302502107777777777777777777777777777777702110087777777777777777777777777777777")]
	public void TestSignatureMalleability_Valid_Strict_DER(string goodDerSig) {
		var validBip66Der = goodDerSig.ToHexByteArray();
		ClassicAssert.IsTrue(CustomDsaEncoding.IsValidSignatureEncoding(validBip66Der));
	}
}
