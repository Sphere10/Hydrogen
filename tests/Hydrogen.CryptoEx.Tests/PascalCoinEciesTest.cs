// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using Hydrogen.CryptoEx.IES;
using Hydrogen.CryptoEx.PascalCoin;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

/// <remarks>Test for PascalCoin Ecies - PascalCoin Elliptic Curve Integrated Encryption Scheme</remarks>
/// Test vectors were gotten from the PascalCoin TESTNET Wallet.
[TestFixture]
public class PascalCoinEciesTest {
	private enum KeyType {
		SECP256K1,
		SECP384R1,
		SECP521R1,
		SECT283K1
	}

	private const string VeryShortMessage = "s";
	private const string ShortMessage = "shortmessage";
	private const string LongMessage = "longmessagelongmessagelongmessagelongmessagelongmessagelongmessage";


	private static readonly SecureRandom SecureRandom = new SecureRandom();

	private static X9ECParameters GetCurveFromKeyType(KeyType keyType) {
		return CustomNamedCurves.GetByName(Enum.GetName(typeof(KeyType), keyType));
	}


	private IesParameterSpec GetPascalCoinIesParameterSpec() {
		// Set up IES Parameter Spec For Compatibility With PascalCoin Current Implementation

		// The derivation and encoding vectors are used when initialising the KDF and MAC.
		// They're optional but if used then they need to be known by the other user so that
		// they can decrypt the cipher text and verify the MAC correctly. The security is based
		// on the shared secret coming from the (static-ephemeral) ECDH key agreement.

		byte[] ivBytes = new byte[16]; // using Zero Initialized IV for compatibility

		int macKeySizeInBits = 32 * 8;

		// Since we are using AES256_CBC for compatibility
		int cipherKeySizeInBits = 32 * 8;

		return new IesParameterSpec(null, null, macKeySizeInBits,
			cipherKeySizeInBits, ivBytes, true);
	}


	private PascalCoinIesEngine GetEciesPascalCoinCompatibilityEngine() {
		// Set up IES Cipher Engine For Compatibility With PascalCoin

		ECDHBasicAgreement ecdhBasicAgreementInstance = new ECDHBasicAgreement();

		PascalCoinEciesKdfBytesGenerator kdfInstance = new PascalCoinEciesKdfBytesGenerator
			(DigestUtilities.GetDigest("SHA-512"));

		IMac digestMacInstance = MacUtilities.GetMac("HMAC-MD5");

		// Set Up Block Cipher
		AesEngine aesEngine = new AesEngine(); // AES Engine

		BufferedBlockCipher cipher =
			 new PaddedBufferedBlockCipher(new CbcBlockCipher(aesEngine),
				 new ZeroBytePadding()); // AES-256 CBC ZeroBytePadding

		return new PascalCoinIesEngine(ecdhBasicAgreementInstance, kdfInstance,
			 digestMacInstance, cipher);
	}


	private ECPublicKeyParameters RecreatePublicKeyFromAffineXandAffineYCoord
		(KeyType keyType, byte[] rawAffineX, byte[] rawAffineY) {
		X9ECParameters lCurve = GetCurveFromKeyType(keyType);
		ECDomainParameters domain = new ECDomainParameters(lCurve.Curve, lCurve.G, lCurve.N,
			lCurve.H, lCurve.GetSeed());

		BigInteger bigXCoord = new BigInteger(1, rawAffineX);
		BigInteger bigYCoord = new BigInteger(1, rawAffineY);

		ECPoint point = lCurve.Curve.CreatePoint(bigXCoord, bigYCoord);

		return new ECPublicKeyParameters("ECDSA", point, domain);
	}

	private ECPrivateKeyParameters RecreatePrivateKeyFromByteArray
		(KeyType keyType, byte[] rawPrivateKey) {
		X9ECParameters lCurve = GetCurveFromKeyType(keyType);
		ECDomainParameters domain = new ECDomainParameters(lCurve.Curve, lCurve.G, lCurve.N,
			lCurve.H, lCurve.GetSeed());

		BigInteger privD = new BigInteger(1, rawPrivateKey);

		return new ECPrivateKeyParameters("ECDSA", privD, domain);
	}

	private string DoPascalCoinEciesEncrypt(KeyType keyType,
		string rawAffineXCoord, string rawAffineYCoord, string payloadToEncrypt) {
		// Encryption
		IesCipher cipherEncrypt = new IesCipher(GetEciesPascalCoinCompatibilityEngine());
		cipherEncrypt.Init(true, RecreatePublicKeyFromAffineXandAffineYCoord(keyType,
				Hex.Decode(rawAffineXCoord), Hex.Decode(rawAffineYCoord)),
			GetPascalCoinIesParameterSpec(), SecureRandom);
		return Hex.ToHexString(cipherEncrypt.DoFinal(System.Text.Encoding.ASCII.GetBytes
			(payloadToEncrypt)));
	}


	private string DoPascalCoinEciesDecrypt(KeyType keyType,
		string rawPrivateKey, string payloadToDecrypt) {
		string result;
		try {
			// Decryption
			IesCipher cipherDecrypt = new IesCipher(GetEciesPascalCoinCompatibilityEngine());
			cipherDecrypt.Init(false, RecreatePrivateKeyFromByteArray(keyType,
					Hex.Decode(rawPrivateKey)),
				GetPascalCoinIesParameterSpec(), SecureRandom);

			result = System.Text.Encoding.ASCII.GetString(cipherDecrypt.DoFinal(Hex.Decode
				(payloadToDecrypt)));
		} catch (Exception e) {
			// should only happen if decryption fails
			throw new Exception("Decryption failed", e);
		}

		return result;
	}


	private void DoTestPascalCoinEciesDecrypt(string id,
		KeyType keyType, string rawPrivateKey, string payloadToDecrypt,
		string expectedOutput) {
		string decryptedPayload = DoPascalCoinEciesDecrypt(keyType, rawPrivateKey,
			payloadToDecrypt);

		ClassicAssert.AreEqual(expectedOutput, decryptedPayload, string.Format("Test {0} Failed, Expected {1} but got {2}",
			id + "_Decrypt", expectedOutput,
			decryptedPayload));
	}


	private void DoTestPascalCoinEciesEncryptDecrypt(string id,
		KeyType keyType, string rawPrivateKey, string rawAffineXCoord, string rawAffineYCoord,
		string payloadToEncrypt) {
		string actualOutput = DoPascalCoinEciesDecrypt(keyType, rawPrivateKey,
			DoPascalCoinEciesEncrypt(keyType, rawAffineXCoord, rawAffineYCoord,
				payloadToEncrypt));

		ClassicAssert.AreEqual(payloadToEncrypt, actualOutput, string.Format("Test {0} Failed, Expected {1} but got {2}",
			id + "_EncryptDecrypt",
			payloadToEncrypt,
			actualOutput));
	}

	[Test]
	public void TestPascalCoinEciesDecrypt() {
		DoTestPascalCoinEciesDecrypt("1", KeyType.SECP256K1,
			"5EEBDD98BBD3F96A1A69020F58C147624AF27F9E9831F05EC42A190DD2FB0DF1",
			"21100C001000025ED19E944D69BA45269855B9F73042E0DCCB50C3EE5B103A2FF3762F9B3540B43D39F0DCA42C53C90F57BC15FED1E5E4490B59FD0E0657E7DBDE5C1C57E40411",
			ShortMessage);

		DoTestPascalCoinEciesDecrypt("2", KeyType.SECP256K1,
			"5EEBDD98BBD3F96A1A69020F58C147624AF27F9E9831F05EC42A190DD2FB0DF1",
			"21104200500003CC2EFCBE845C6AF6DD27074DC283E5B118874F0A53BE634C509E6D2D9487E8281824CE68560A3C12D36D502DB5A55905F7DC3E67967E10D590F3EFBDE1F3A"
			+ "D0337ACE569F773C77A18B045B4D4285B5C3B52AAE0BD0C68169C4AB684DA7CF3B73D5C8643EFF99B6F00128B01255B08D83995C1A79C6B2AA0B412343C95CF30B0",
			LongMessage);

		DoTestPascalCoinEciesDecrypt("3", KeyType.SECP384R1,
			"889ED91943C05D599DA1CEF146D68495E650F800B74B6310AD9614DC55E2ABE01604E3398E548E0D4AD82A887070B787",
			"31100C001000021E6A7EBF798DEFF9411940BFFA3B82F8F555165B5BD5CCF8A7CA8E661E2057FB5E721E30D3CC187E97988524370560F8E85CAAF9B640FB277A92780BB2D34DDE49AD19FA53FE9A8CC867731846FE0C4F",
			ShortMessage);

		DoTestPascalCoinEciesDecrypt("4", KeyType.SECP384R1,
			"889ED91943C05D599DA1CEF146D68495E650F800B74B6310AD9614DC55E2ABE01604E3398E548E0D4AD82A887070B787",
			"31104200500003C1CC4FCB1B6B32EACF9EBBFD22D4904055D454263A475261EB4F1BA8008F9E2C6D8B468B7A36BF3DE6D284C07C1CB431887"
			+ "1F197A4CFC055E3312845BCFACB4F185EA02D5D443CE021B76F560D86209D44FF828B2905D4FA30B0873ADA758983F59E25E25598C85C2253B7BAC35B722CAD1F80545FB315C95016FB440559FF626086B7BA09A51481A7B77BF4B129E579",
			LongMessage);

		DoTestPascalCoinEciesDecrypt("5", KeyType.SECP521R1,
			"6316522337DA679C1EF338E54509C19793FC3C02D53F12AF79322086AAA4AA2BAD8108EDA2000763DC99C6DA1909712C2E96A9F2BAB7502BCD2DDD7B39880F0808",
			"43100C00100003013D32C1BACF719D45829502FAD8D7A5FBED41EF6E212E6D1FCC55B70552BF85B6F71A4A045221D36D47C1E538217A80B4918E76C9E84191359419BAE0FBDB3ADB56ABE37C5F02BEA79FDAEF3D5E5312B6A932F57973AF25D58CF42E0C7F2877C711",
			ShortMessage);

		DoTestPascalCoinEciesDecrypt("6", KeyType.SECP521R1,
			"6316522337DA679C1EF338E54509C19793FC3C02D53F12AF79322086AAA4AA2BAD8108EDA2000763DC99C6DA1909712C2E96A9F2BAB7502BCD2DDD7B39880F0808",
			"4310420050000301838307131DF82CF6D23C07AA4A12261784399E2D011C87969E2388659A95A54C11E08C34F49D651D4E5C87106D367C01CB60C7146D20FE60098913074A78519DF182F3BA0A4A097FD11F11D9FC09D1C4586D178082AAC68B2A4BAC012992D3A"
			+ "44ACBCC322E3C3F86BD728A3570B3C24696464088EAE8A82A6EAC5BB444DD28E322351C801D61673B9B8776C190E0FCD468C906448680B3094BD692EC0CA4700361",
			LongMessage);

		DoTestPascalCoinEciesDecrypt("7", KeyType.SECT283K1,
			"0121EFFE82FE34BA6D1B5635903924801CADD54EF2804B56624BB8C0BFBF9F33BB42EADD",
			"25100C00100002079DF758857C4CC50D10A94D25CBD219E752BBD08EB30D61158765200D54CEA6BB5157F95CEB540F0698AF56E7DBD2E612F9C7CBDBC4364C343661F0B4A50309FCA05303",
			ShortMessage);

		DoTestPascalCoinEciesDecrypt("8", KeyType.SECT283K1,
			"0121EFFE82FE34BA6D1B5635903924801CADD54EF2804B56624BB8C0BFBF9F33BB42EADD",
			"25104200500003067C595E47EEC68264B1FD4AC181F5CB2632C88AC5F03ED2F532B5513F0EA7A274E6F53A4F16B6FC9341B79577C11E205F7BF12CEC5F339FA20163E8D6EDB4AD07595AEEDE9EAABDB"
			+ "5468D9CA50F2667313024669580D67C532284A687A2C1172F656B5C144B8E7A4D8206A8D8266164963B74F846A00FDE4268CE7E41C6145ECB38F1DE",
			LongMessage);
	}

	[Test]
	public void TestPascalCoinEciesEncryptDecrypt() {
		DoTestPascalCoinEciesEncryptDecrypt("1", KeyType.SECP256K1,
			"5EEBDD98BBD3F96A1A69020F58C147624AF27F9E9831F05EC42A190DD2FB0DF1",
			"327D9618E226B991E47BA2EF81CEC0AFC0436E3CC22F04454749FCA2AFBB52F7",
			"BE70064DAB4A2A0681889F1EE51B6BB2348A394317EAC2BEA38E6ABC2D78D307",
			VeryShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("2", KeyType.SECP256K1,
			"5EEBDD98BBD3F96A1A69020F58C147624AF27F9E9831F05EC42A190DD2FB0DF1",
			"327D9618E226B991E47BA2EF81CEC0AFC0436E3CC22F04454749FCA2AFBB52F7",
			"BE70064DAB4A2A0681889F1EE51B6BB2348A394317EAC2BEA38E6ABC2D78D307",
			ShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("3", KeyType.SECP256K1,
			"5EEBDD98BBD3F96A1A69020F58C147624AF27F9E9831F05EC42A190DD2FB0DF1",
			"327D9618E226B991E47BA2EF81CEC0AFC0436E3CC22F04454749FCA2AFBB52F7",
			"BE70064DAB4A2A0681889F1EE51B6BB2348A394317EAC2BEA38E6ABC2D78D307",
			LongMessage);

		DoTestPascalCoinEciesEncryptDecrypt("4", KeyType.SECP384R1,
			"889ED91943C05D599DA1CEF146D68495E650F800B74B6310AD9614DC55E2ABE01604E3398E548E0D4AD82A887070B787",
			"F9BB6DD66F26E406AADDC666A65229904A22BA500EACC3D6FA2B7BD4E7D33204BBE87741462258CCD8FB32F43D52ABF0",
			"F7AB9F5D3676FC98946F35269BD73082A57ABF4B66864C703DACB238EA4FBEE2390399C655C6CDAABB26FCD34FD749D7",
			VeryShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("5", KeyType.SECP384R1,
			"889ED91943C05D599DA1CEF146D68495E650F800B74B6310AD9614DC55E2ABE01604E3398E548E0D4AD82A887070B787",
			"F9BB6DD66F26E406AADDC666A65229904A22BA500EACC3D6FA2B7BD4E7D33204BBE87741462258CCD8FB32F43D52ABF0",
			"F7AB9F5D3676FC98946F35269BD73082A57ABF4B66864C703DACB238EA4FBEE2390399C655C6CDAABB26FCD34FD749D7",
			ShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("6", KeyType.SECP384R1,
			"889ED91943C05D599DA1CEF146D68495E650F800B74B6310AD9614DC55E2ABE01604E3398E548E0D4AD82A887070B787",
			"F9BB6DD66F26E406AADDC666A65229904A22BA500EACC3D6FA2B7BD4E7D33204BBE87741462258CCD8FB32F43D52ABF0",
			"F7AB9F5D3676FC98946F35269BD73082A57ABF4B66864C703DACB238EA4FBEE2390399C655C6CDAABB26FCD34FD749D7",
			LongMessage);

		DoTestPascalCoinEciesEncryptDecrypt("7", KeyType.SECP521R1,
			"6316522337DA679C1EF338E54509C19793FC3C02D53F12AF79322086AAA4AA2BAD8108EDA2000763DC99C6DA1909712C2E96A9F2BAB7502BCD2DDD7B39880F0808",
			"014919D3527C3D31FF9EE84D5009E9BA4977B6E6C075EB454B2BA086E75605D88F895247F8E3968F3C26B840D806DB2A6FCFAE96D90A80A955BC277FEA0D69A086BE",
			"01FDA4AFC30977BF0B57CD3202497880D905AF6BF9CFD275EAE5CD6E68E639D4DEBE12C3EA3EA2ED13803D5751FED86C2F35952DBDC935A85C75FEBC371B01698097",
			VeryShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("8", KeyType.SECP521R1,
			"6316522337DA679C1EF338E54509C19793FC3C02D53F12AF79322086AAA4AA2BAD8108EDA2000763DC99C6DA1909712C2E96A9F2BAB7502BCD2DDD7B39880F0808",
			"014919D3527C3D31FF9EE84D5009E9BA4977B6E6C075EB454B2BA086E75605D88F895247F8E3968F3C26B840D806DB2A6FCFAE96D90A80A955BC277FEA0D69A086BE",
			"01FDA4AFC30977BF0B57CD3202497880D905AF6BF9CFD275EAE5CD6E68E639D4DEBE12C3EA3EA2ED13803D5751FED86C2F35952DBDC935A85C75FEBC371B01698097",
			ShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("9", KeyType.SECP521R1,
			"6316522337DA679C1EF338E54509C19793FC3C02D53F12AF79322086AAA4AA2BAD8108EDA2000763DC99C6DA1909712C2E96A9F2BAB7502BCD2DDD7B39880F0808",
			"014919D3527C3D31FF9EE84D5009E9BA4977B6E6C075EB454B2BA086E75605D88F895247F8E3968F3C26B840D806DB2A6FCFAE96D90A80A955BC277FEA0D69A086BE",
			"01FDA4AFC30977BF0B57CD3202497880D905AF6BF9CFD275EAE5CD6E68E639D4DEBE12C3EA3EA2ED13803D5751FED86C2F35952DBDC935A85C75FEBC371B01698097",
			LongMessage);

		DoTestPascalCoinEciesEncryptDecrypt("10", KeyType.SECT283K1,
			"0121EFFE82FE34BA6D1B5635903924801CADD54EF2804B56624BB8C0BFBF9F33BB42EADD",
			"01747F38A49C099DA25231AE47F0820216EDEC6F6DB51A28280ABDCC65B652D76529BB88",
			"05535E7555704E2EAA1C3A29FCF50622D67F65DDB1EA294D92C4BDDEE403DE379E26B280",
			VeryShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("11", KeyType.SECT283K1,
			"0121EFFE82FE34BA6D1B5635903924801CADD54EF2804B56624BB8C0BFBF9F33BB42EADD",
			"01747F38A49C099DA25231AE47F0820216EDEC6F6DB51A28280ABDCC65B652D76529BB88",
			"05535E7555704E2EAA1C3A29FCF50622D67F65DDB1EA294D92C4BDDEE403DE379E26B280",
			ShortMessage);

		DoTestPascalCoinEciesEncryptDecrypt("12", KeyType.SECT283K1,
			"0121EFFE82FE34BA6D1B5635903924801CADD54EF2804B56624BB8C0BFBF9F33BB42EADD",
			"01747F38A49C099DA25231AE47F0820216EDEC6F6DB51A28280ABDCC65B652D76529BB88",
			"05535E7555704E2EAA1C3A29FCF50622D67F65DDB1EA294D92C4BDDEE403DE379E26B280",
			LongMessage);
	}
}
