// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Security;
using Hydrogen.CryptoEx.IES;
using Hydrogen.CryptoEx.PascalCoin;

namespace Hydrogen.CryptoEx.EC.IES;

public sealed class ECIES : IIESAlgorithm {

	private static SecureRandom SecureRandom { get; } = new();

	public byte[] Encrypt(ReadOnlySpan<byte> message, IPublicKey publicKey) {
		// Encryption
		var cipherEncrypt = new IesCipher(GetEciesPascalCoinCompatibilityEngine());
		cipherEncrypt.Init(true, ((ECDSA.PublicKey)publicKey).Parameters.Value, GetPascalCoinIesParameterSpec(), SecureRandom);
		return cipherEncrypt.DoFinal(message.ToArray());
	}

	public bool TryDecrypt(ReadOnlySpan<byte> encryptedMessage, out byte[] decryptedMessage, IPrivateKey privateKey) {
		try {
			// Decryption
			var cipherDecrypt = new IesCipher(GetEciesPascalCoinCompatibilityEngine());
			cipherDecrypt.Init(false, ((ECDSA.PrivateKey)privateKey).Parameters.Value, GetPascalCoinIesParameterSpec(), SecureRandom);
			decryptedMessage = cipherDecrypt.DoFinal(encryptedMessage.ToArray());
			return true;
		} catch (Exception) {
			// should only happen if decryption fails
			decryptedMessage = default;
			return false;
		}
	}

	private static PascalCoinIesEngine GetEciesPascalCoinCompatibilityEngine() {
		// Set up IES Cipher Engine For Compatibility With PascalCoin

		var ecdhBasicAgreementInstance = new ECDHBasicAgreement();
		var kdfInstance = new PascalCoinEciesKdfBytesGenerator(DigestUtilities.GetDigest("SHA-512"));
		var digestMacInstance = MacUtilities.GetMac("HMAC-MD5");

		// Set Up Block Cipher
		var aesEngine = new AesEngine(); // AES Engine
		BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(aesEngine), new ZeroBytePadding()); // AES-256 CBC ZeroBytePadding

		return new PascalCoinIesEngine(ecdhBasicAgreementInstance, kdfInstance, digestMacInstance, cipher);
	}

	private static IesParameterSpec GetPascalCoinIesParameterSpec() {
		// Set up IES Parameter Spec For Compatibility With PascalCoin Current Implementation

		// The derivation and encoding vectors are used when initialising the KDF and MAC.
		// They're optional but if used then they need to be known by the other user so that
		// they can decrypt the cipher text and verify the MAC correctly. The security is based
		// on the shared secret coming from the (static-ephemeral) ECDH key agreement.

		var ivBytes = new byte[16]; // using Zero Initialized IV for compatibility

		const int macKeySizeInBits = 32 * 8;

		// Since we are using AES256_CBC for compatibility
		const int cipherKeySizeInBits = 32 * 8;

		return new IesParameterSpec(null, null, macKeySizeInBits, cipherKeySizeInBits, ivBytes, true);
	}


}
