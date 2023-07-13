// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Hydrogen.CryptoEx.IES;

namespace Hydrogen.CryptoEx.PascalCoin;

public class PascalCoinIesEngine : CustomIesEngine {
	// Structure for Compatibility with PascalCoin Original Implementation.
	private struct SecureHead {
		private byte _key;
		private byte _mac;
		private ushort _orig;
		private ushort _body;
	}


	// SecureHeadSize must be equal to "6" to conform to PascalCoin Original Implementation
	private static readonly int SecureHeadSize;

	static unsafe PascalCoinIesEngine() {
		SecureHeadSize = sizeof(SecureHead);
		Trace.Assert(SecureHeadSize == 6);
	}


	public PascalCoinIesEngine(IBasicAgreement agree, IDerivationFunction kdf, IMac mac) : base(agree, kdf, mac) {
	}

	public PascalCoinIesEngine(IBasicAgreement agree, IDerivationFunction kdf, IMac mac, BufferedBlockCipher cipher)
		: base(agree, kdf, mac, cipher) {
	}

	protected override byte[] DecryptBlock(
		byte[] inEnc,
		int inOff,
		int inLen) {
		// Ensure that the length of the input is greater than the MAC in bytes
		if (inLen < (V.Length + Mac.GetMacSize())) {
			throw new InvalidCipherTextException("Length of input must be greater than the MAC and V combined");
		}

		// note order is important: set up keys, do simple encryption, check mac, do final encryption.
		if (Cipher == null) {
			// Streaming mode.
			throw new ArgumentNullException(string.Format("{0}", "Cipher Cannot be Null in This Mode."));
		}
		// Block cipher mode.

		SetupBlockCipherAndMacKeyBytes(out byte[] k1, out byte[] k2);

		ICipherParameters cp = new KeyParameter(k1);

		// If IV provide use it to initialize the cipher
		if (Iv != null) {
			cp = new ParametersWithIV(cp, Iv);
		}

		Cipher.Init(false, cp);

		// Verify the MAC.
		byte[] t1 = new byte[Mac.GetMacSize()];
		Array.Copy(inEnc, V.Length, t1, 0, t1.Length);

		byte[] t2 = new byte[t1.Length];
		Mac.Init(new KeyParameter(k2));
		Mac.BlockUpdate(inEnc, inOff + V.Length + t2.Length, inLen - V.Length - t2.Length);

		Mac.DoFinal(t2, 0);

		if (!Arrays.ConstantTimeAreEqual(t1, t2)) {
			throw new InvalidCipherTextException("invalid MAC");
		}

		return Cipher.DoFinal(inEnc,
			inOff + V.Length + Mac.GetMacSize(),
			inLen - V.Length - t2.Length);
	}

	protected override unsafe byte[] EncryptBlock(
		byte[] @in,
		int inOff,
		int inLen) {
		if (Cipher == null) {
			// Streaming mode.
			throw new ArgumentNullException(string.Format("{0}", "Cipher Cannot be Null in This Mode."));
		}
		// Block cipher mode.

		SetupBlockCipherAndMacKeyBytes(out byte[] k1, out byte[] k2);


		// If iv provided use it to initialise the cipher
		if (Iv != null) {
			Cipher.Init(true, new ParametersWithIV(new KeyParameter(k1), Iv));
		} else {
			Cipher.Init(true, new KeyParameter(k1));
		}

		byte[] c = Cipher.DoFinal(@in, inOff, inLen);


		// Apply the MAC.
		byte[] T = new byte[Mac.GetMacSize()];

		Mac.Init(new KeyParameter(k2));
		Mac.BlockUpdate(c, 0, c.Length);
		Mac.DoFinal(T, 0);

		int cipherBlockSize = Cipher.GetBlockSize();
		int messageToEncryptSize = inLen - inOff;

		int messageToEncryptPadSize = (messageToEncryptSize % cipherBlockSize) == 0
			? 0
			: cipherBlockSize -
			  (messageToEncryptSize % cipherBlockSize);


		// Output the quadruple (SECURE_HEAD_DETAILS,V,T,C).
		// SECURE_HEAD_DETAILS :=
		// [0] := Convert Byte(Length(V)) to a ByteArray,
		// [1] := Convert Byte(Length(T)) to a ByteArray,
		// [2] and [3] := Convert UInt16(MessageToEncryptSize) to a ByteArray,
		// [4] and [5] := Convert UInt16(MessageToEncryptSize + MessageToEncryptPadSize) to a ByteArray,
		// V := Ephemeral Public Key
		// T := Authentication Message (MAC)
		// C := Encrypted Payload

		byte[] output = new byte[SecureHeadSize + V.Length + T.Length + c.Length];


		fixed (byte* ptrByteOutput = output) {
			ushort* ptrUShortOutput = (ushort*)ptrByteOutput;

			*ptrByteOutput = (byte)(V.Length);
			*(ptrByteOutput + 1) = (byte)(T.Length);
			*(ptrUShortOutput + 1) = (ushort)(messageToEncryptSize);
			*(ptrUShortOutput + 2) = (ushort)(messageToEncryptSize + messageToEncryptPadSize);
		}


		Array.Copy(V, 0, output, SecureHeadSize, V.Length);
		Array.Copy(T, 0, output, SecureHeadSize + V.Length, T.Length);
		Array.Copy(c, 0, output, SecureHeadSize + V.Length + T.Length, c.Length);
		return output;
	}

	public override byte[] ProcessBlock(
		byte[] @in,
		int inOff,
		int inLen) {
		if (ForEncryption) {
			if (KeyPairGenerator != null) {
				EphemeralKeyPair ephKeyPair = KeyPairGenerator.Generate();

				PrivParam = ephKeyPair.GetKeyPair().Private;
				V = ephKeyPair.GetEncodedPublicKey();
			}
		} else {
			if (KeyParser != null) {
				MemoryStream bIn = new MemoryStream(@in, inOff, inLen) { Position = SecureHeadSize };
				try {
					PubParam = KeyParser.ReadKey(bIn);
				} catch (IOException e) {
					throw new InvalidCipherTextException("unable to recover ephemeral public key: " + e.Message, e);
				} catch (ArgumentException e) {
					throw new InvalidCipherTextException("unable to recover ephemeral public key: " + e.Message, e);
				}

				int encLength = (inLen - (int)(bIn.Length - bIn.Position));
				V = Arrays.CopyOfRange(@in, inOff + SecureHeadSize, inOff + encLength);
			}
		}

		// Compute the common value and convert to byte array. 
		Agree.Init(PrivParam);
		BigInteger z = Agree.CalculateAgreement(PubParam);
		byte[] bigZ = BigIntegers.AsUnsignedByteArray(Agree.GetFieldSize(), z);

		try {
			// Initialise the KDF.
			KdfParameters kdfParam = new KdfParameters(bigZ, null);
			Kdf.Init(kdfParam);

			if (ForEncryption) {
				return EncryptBlock(@in, inOff, inLen);
			} else {
				byte[] temp = new byte[inLen - SecureHeadSize];
				Array.Copy(@in, inOff + SecureHeadSize, temp, 0, temp.Length);
				return DecryptBlock(temp, inOff, temp.Length);
			}
		} finally {
			Arrays.Fill(bigZ, 0);
		}
	}
}
