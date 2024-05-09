// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using NUnit.Framework;
//using System;
//using System.Linq;
//using System.Text;

//namespace Crypto.UnitTests {
//	[TestFixture]
//	public class CryptoTest {
//		private static ECPrivateKey GetNewPrivateKey(ushort ecOpensslNid) =>
//			new ECPrivateKey().GenerateRandomPrivateKey(ecOpensslNid);

//		private static ECDSA_SIG DoEcdsaSign(ECPrivateKey privateKey, byte[] digest) =>
//			ECCrypto.ECDSASign(privateKey.PrivateKey, digest);

//		private static byte[] DoSha256Digest(byte[] message) => ECCrypto.DoSha256(message);

//		private static bool AreEcdsaSigEqual(ECDSA_SIG a, ECDSA_SIG b) =>
//			a.R.SequenceEqual(b.R) && a.S.SequenceEqual(b.S);

//		private static bool AreEcPrivateKeyInfoEqual(ECPrivateKeyInfo a, ECPrivateKeyInfo b) =>
//			a.EC_OpenSSL_NID == b.EC_OpenSSL_NID && a.RAW_PrivKey.SequenceEqual(b.RAW_PrivKey);

//		private static bool AreEcPrivateKeyEqual(ECPrivateKey a, ECPrivateKey b) =>
//			AreEcPrivateKeyInfoEqual(a.PrivateKey, b.PrivateKey) && a.PublicKey.IsEqualTo(b.PublicKey);

//		[Test]
//		[TestCase(Constants.CT_NID_secp256k1)]
//		[TestCase(Constants.CT_NID_secp384r1)]
//		[TestCase(Constants.CT_NID_secp521r1)]
//		[TestCase(Constants.CT_NID_sect283k1)]
//		public void TestEcdsaSignAndVerify(ushort ecOpensslNid) {
//			var digest = DoSha256Digest(Encoding.UTF8.GetBytes("PascalCoin"));
//			var privateKey = GetNewPrivateKey(ecOpensslNid);
//			var sig = DoEcdsaSign(privateKey, digest);
//			ClassicAssert.IsTrue(ECCrypto.ECDSAVerify(privateKey.PublicKey, digest, sig));
//		}

//		[Test]
//		[TestCase(Constants.CT_NID_secp256k1)]
//		[TestCase(Constants.CT_NID_secp384r1)]
//		[TestCase(Constants.CT_NID_secp521r1)]
//		[TestCase(Constants.CT_NID_sect283k1)]
//		public void TestEncodeAndDecodeSig(ushort ecOpensslNid) {
//			var origSig = DoEcdsaSign(GetNewPrivateKey(ecOpensslNid),
//				DoSha256Digest(Encoding.UTF8.GetBytes("PascalCoin")));
//			var encodeSig = ECCrypto.EncodeSignature(origSig);
//			_ = ECCrypto.DecodeSignature(encodeSig, out var newSig);
//			ClassicAssert.IsTrue(AreEcdsaSigEqual(origSig, newSig));
//		}

//		[Test]
//		[TestCase(Constants.CT_NID_secp256k1)]
//		[TestCase(Constants.CT_NID_secp384r1)]
//		[TestCase(Constants.CT_NID_secp521r1)]
//		[TestCase(Constants.CT_NID_sect283k1)]
//		public void TestSerializeAndDeserializeEcdsaPublicKey(ushort ecOpensslNid) {
//			var privateKey = GetNewPrivateKey(ecOpensslNid);
//			var originalPubKey = privateKey.PublicKey;
//			var serialized = originalPubKey.ToSerialized();
//			ECDSA_Public reconstructedPubKey = default;
//			reconstructedPubKey.FromSerialized(serialized);
//			ClassicAssert.IsTrue(originalPubKey.IsEqualTo(reconstructedPubKey));
//		}

//		[Test]
//		[TestCase(Constants.CT_NID_secp256k1)]
//		[TestCase(Constants.CT_NID_secp384r1)]
//		[TestCase(Constants.CT_NID_secp521r1)]
//		[TestCase(Constants.CT_NID_sect283k1)]
//		public void TestToAndFromRawEcPrivateKey(ushort ecOpensslNid) {
//			var originalPrivateKey = GetNewPrivateKey(ecOpensslNid);
//			var raw = originalPrivateKey.ExportToRaw();
//			var reconstructedPrivateKey = ECPrivateKey.ImportFromRaw(raw);
//			ClassicAssert.IsTrue(AreEcPrivateKeyEqual(originalPrivateKey, reconstructedPrivateKey));
//		}

//		[Test]
//		[TestCase(new byte[] { 26, 45, 88, 98 }, new byte[] { 87, 55, 09, 82, 17 })]
//		[TestCase(new byte[] { 87, 55, 09, 82, 17 }, new byte[] { 26, 45, 88, 98 })]
//		public void TestPascalCoinAesEncryptAndDecrypt(byte[] message, byte[] password) {
//			var decryptedMessage = Array.Empty<byte>();
//			var isDecrypted =
//				ECCrypto.DoPascalCoinAESDecrypt(ECCrypto.DoPascalCoinAESEncrypt(message, password),
//					password, ref decryptedMessage);
//			ClassicAssert.IsTrue(isDecrypted);
//			ClassicAssert.IsTrue(message.SequenceEqual(decryptedMessage));
//		}

//		[Test]
//		[TestCase(Constants.CT_NID_secp256k1, new byte[] { 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_secp256k1, new byte[] { 87, 55, 09, 82, 17 })]
//		[TestCase(Constants.CT_NID_secp256k1, new byte[] { 87, 55, 09, 82, 17, 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_secp384r1, new byte[] { 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_secp384r1, new byte[] { 87, 55, 09, 82, 17 })]
//		[TestCase(Constants.CT_NID_secp384r1, new byte[] { 87, 55, 09, 82, 17, 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_secp521r1, new byte[] { 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_secp521r1, new byte[] { 87, 55, 09, 82, 17 })]
//		[TestCase(Constants.CT_NID_secp521r1, new byte[] { 87, 55, 09, 82, 17, 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_sect283k1, new byte[] { 26, 45, 88, 98 })]
//		[TestCase(Constants.CT_NID_sect283k1, new byte[] { 87, 55, 09, 82, 17 })]
//		[TestCase(Constants.CT_NID_sect283k1, new byte[] { 87, 55, 09, 82, 17, 26, 45, 88, 98 })]
//		public void TestPascalCoinEciesEncryptAndDecrypt(ushort ecOpensslNid, byte[] message) {
//			var encryptedMessage = Array.Empty<byte>();
//			var decryptedMessage = Array.Empty<byte>();
//			var privateKey = GetNewPrivateKey(ecOpensslNid);
//			var publicKey = privateKey.PublicKey;
//			var isEncrypted = ECCrypto.DoPascalCoinECIESEncrypt(publicKey, message, ref encryptedMessage);
//			ClassicAssert.IsTrue(isEncrypted);
//			var isDecrypted = ECCrypto.DoPascalCoinECIESDecrypt(privateKey.EC_OpenSSL_NID,
//				privateKey.PrivateKey.RAW_PrivKey, encryptedMessage, ref decryptedMessage);
//			ClassicAssert.IsTrue(isDecrypted);
//			ClassicAssert.IsTrue(message.SequenceEqual(decryptedMessage));
//		}
//	}
//}


