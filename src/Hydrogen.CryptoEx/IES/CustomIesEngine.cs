// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx.IES;

/**
    * support class for constructing integrated encryption ciphers
    * for doing basic message exchanges on top of key agreement ciphers
    */
public class CustomIesEngine {
	private IesParameters _param;
	protected readonly IBasicAgreement Agree;
	protected readonly IDerivationFunction Kdf;
	protected readonly IMac Mac;
	protected readonly BufferedBlockCipher Cipher;

	protected bool ForEncryption;
	protected ICipherParameters PrivParam, PubParam;

	protected byte[] V;
	protected EphemeralKeyPairGenerator KeyPairGenerator;
	protected IKeyParser KeyParser;
	protected byte[] Iv;

	private void ExtractParams(ICipherParameters @params) {
		if (@params is ParametersWithIV) {
			Iv = ((ParametersWithIV)@params).GetIV();
			_param = (IesParameters)((ParametersWithIV)@params).Parameters;
		} else {
			Iv = null;
			_param = (IesParameters)@params;
		}
	}


	private void SimilarMacCompute(byte[] p2, byte[] l2, byte[] t) {
		if (p2 != null) {
			Mac.BlockUpdate(p2, 0, p2.Length);
		}

		if (V.Length != 0) {
			Mac.BlockUpdate(l2, 0, l2.Length);
		}

		Mac.DoFinal(t, 0);
	}


	// as described in Shroup's paper and P1363a
	private byte[] GetLengthTag(byte[] p2) {
		byte[] l2 = new byte[8];
		if (p2 != null) {
			Pack.UInt64_To_BE((ulong)(p2.Length * 8), l2, 0);
		}

		return l2;
	}

	protected void SetupBlockCipherAndMacKeyBytes(out byte[] k1,
	                                              out byte[] k2) {
		k1 = new byte[((IesWithCipherParameters)_param).CipherKeySize / 8];
		k2 = new byte[_param.MacKeySize / 8];
		byte[] k = new byte[k1.Length + k2.Length];

		Kdf.GenerateBytes(k, 0, k.Length);
		Array.Copy(k, 0, k1, 0, k1.Length);
		Array.Copy(k, k1.Length, k2, 0, k2.Length);
	}

	protected virtual byte[] DecryptBlock(
		byte[] inEnc,
		int inOff,
		int inLen) {
		byte[] m;
		byte[] k1, k2;
		int len = 0;

		// Ensure that the length of the input is greater than the MAC in bytes
		if (inLen < (V.Length + Mac.GetMacSize())) {
			throw new InvalidCipherTextException("Length of input must be greater than the MAC and V combined");
		}

		// note order is important: set up keys, do simple encryptions, check mac, do final encryption.
		if (Cipher == null) {
			// Streaming mode.
			k1 = new byte[inLen - V.Length - Mac.GetMacSize()];
			k2 = new byte[_param.MacKeySize / 8];
			var k = new byte[k1.Length + k2.Length];

			Kdf.GenerateBytes(k, 0, k.Length);

			if (V.Length != 0) {
				Array.Copy(k, 0, k2, 0, k2.Length);
				Array.Copy(k, k2.Length, k1, 0, k1.Length);
			} else {
				Array.Copy(k, 0, k1, 0, k1.Length);
				Array.Copy(k, k1.Length, k2, 0, k2.Length);
			}

			// process the message
			m = new byte[k1.Length];

			for (int i = 0; i != k1.Length; i++) {
				m[i] = (byte)(inEnc[inOff + V.Length + i] ^ k1[i]);
			}
		} else {
			// Block cipher mode.

			SetupBlockCipherAndMacKeyBytes(out k1, out k2);

			ICipherParameters cp = new KeyParameter(k1);

			// If IV provide use it to initialize the cipher
			if (Iv != null) {
				cp = new ParametersWithIV(cp, Iv);
			}

			Cipher.Init(false, cp);

			m = new byte[Cipher.GetOutputSize(inLen - V.Length - Mac.GetMacSize())];

			// do initial processing
			len = Cipher.ProcessBytes(inEnc, inOff + V.Length, inLen - V.Length - Mac.GetMacSize(), m, 0);
		}

		// Convert the length of the encoding vector into a byte array.
		byte[] p2 = _param.GetEncodingV();
		byte[] l2 = null;
		if (V.Length != 0) {
			l2 = GetLengthTag(p2);
		}

		// Verify the MAC.
		int end = inOff + inLen;
		byte[] t1 = Arrays.CopyOfRange(inEnc, end - Mac.GetMacSize(), end);

		byte[] t2 = new byte[t1.Length];
		Mac.Init(new KeyParameter(k2));
		Mac.BlockUpdate(inEnc, inOff + V.Length, inLen - V.Length - t2.Length);

		SimilarMacCompute(p2, l2, t2);

		if (!Arrays.ConstantTimeAreEqual(t1, t2)) {
			throw new InvalidCipherTextException("invalid MAC");
		}

		if (Cipher == null) {
			return m;
		} else {
			len += Cipher.DoFinal(m, len);

			return Arrays.CopyOfRange(m, 0, len);
		}
	}

	protected virtual byte[] EncryptBlock(
		byte[] @in,
		int inOff,
		int inLen) {
		byte[] c;
		byte[] k1, k2;
		int len;

		if (Cipher == null) {
			// Streaming mode.
			k1 = new byte[inLen];
			k2 = new byte[_param.MacKeySize / 8];
			byte[] k = new byte[k1.Length + k2.Length];

			Kdf.GenerateBytes(k, 0, k.Length);

			if (V.Length != 0) {
				Array.Copy(k, 0, k2, 0, k2.Length);
				Array.Copy(k, k2.Length, k1, 0, k1.Length);
			} else {
				Array.Copy(k, 0, k1, 0, k1.Length);
				Array.Copy(k, inLen, k2, 0, k2.Length);
			}

			c = new byte[inLen];

			for (int i = 0; i != inLen; i++) {
				c[i] = (byte)(@in[inOff + i] ^ k1[i]);
			}

			len = inLen;
		} else {
			// Block cipher mode.

			SetupBlockCipherAndMacKeyBytes(out k1, out k2);


			// If iv provided use it to initialise the cipher
			if (Iv != null) {
				Cipher.Init(true, new ParametersWithIV(new KeyParameter(k1), Iv));
			} else {
				Cipher.Init(true, new KeyParameter(k1));
			}

			c = new byte[Cipher.GetOutputSize(inLen)];
			len = Cipher.ProcessBytes(@in, inOff, inLen, c, 0);
			len += Cipher.DoFinal(c, len);
		}


		// Convert the length of the encoding vector into a byte array.
		byte[] p2 = _param.GetEncodingV();
		byte[] l2 = null;
		if (V.Length != 0) {
			l2 = GetLengthTag(p2);
		}


		// Apply the MAC.
		byte[] T = new byte[Mac.GetMacSize()];

		Mac.Init(new KeyParameter(k2));
		Mac.BlockUpdate(c, 0, c.Length);
		SimilarMacCompute(p2, l2, T);


		// Output the triple (V,C,T).
		byte[] output = new byte[V.Length + len + T.Length];
		Array.Copy(V, 0, output, 0, V.Length);
		Array.Copy(c, 0, output, V.Length, len);
		Array.Copy(T, 0, output, V.Length + len, T.Length);
		return output;
	}

	/**
        * set up for use with stream mode, where the key derivation function
        * is used to provide a stream of bytes to xor with the message.
        *
        * @param agree the key agreement used as the basis for the encryption
        * @param kdf the key derivation function used for byte generation
        * @param mac the message authentication code generator for the message
        */
	public CustomIesEngine(
		IBasicAgreement agree,
		IDerivationFunction kdf,
		IMac mac) {
		Agree = agree;
		Kdf = kdf;
		Mac = mac;
		Cipher = null;
	}

	/**
        * set up for use in conjunction with a block cipher to handle the
        * message.
        *
        * @param agree the key agreement used as the basis for the encryption
        * @param kdf the key derivation function used for byte generation
        * @param mac the message authentication code generator for the message
        * @param cipher the cipher to used for encrypting the message
        */
	public CustomIesEngine(
		IBasicAgreement agree,
		IDerivationFunction kdf,
		IMac mac,
		BufferedBlockCipher cipher) {
		Agree = agree;
		Kdf = kdf;
		Mac = mac;
		Cipher = cipher;
	}

	/**
        * Initialise the encryptor.
        *
        * @param forEncryption whether or not this is encryption/decryption.
        * @param privParam our private key parameters
        * @param pubParam the recipient's/sender's public key parameters
        * @param param encoding and derivation parameters.
        */
	public virtual void Init(
		bool forEncryption,
		ICipherParameters privParameters,
		ICipherParameters pubParameters,
		ICipherParameters @params) {
		ForEncryption = forEncryption;
		PrivParam = privParameters;
		PubParam = pubParameters;
		V = new byte[0];

		ExtractParams(@params);
	}

	/**
        * Initialise the decryptor.
        *
        * @param publicKey      the recipient's/sender's public key parameters
        * @param params         encoding and derivation parameters, may be wrapped to include an IV for an underlying block cipher.
        * @param ephemeralKeyPairGenerator             the ephemeral key pair generator to use.
        */
	public void Init(AsymmetricKeyParameter publicKey, ICipherParameters @params,
	                 EphemeralKeyPairGenerator ephemeralKeyPairGenerator) {
		ForEncryption = true;
		PubParam = publicKey;
		KeyPairGenerator = ephemeralKeyPairGenerator;

		ExtractParams(@params);
	}

	/**
         * Initialise the encryptor.
         *
         * @param privateKey      the recipient's private key.
         * @param params          encoding and derivation parameters, may be wrapped to include an IV for an underlying block cipher.
         * @param publicKeyParser the parser for reading the ephemeral public key.
         */
	public void Init(AsymmetricKeyParameter privateKey, ICipherParameters @params, IKeyParser publicKeyParser) {
		ForEncryption = false;
		PrivParam = privateKey;
		KeyParser = publicKeyParser;

		ExtractParams(@params);
	}

	public BufferedBlockCipher GetCipher() {
		return Cipher;
	}

	public IMac GetMac() {
		return Mac;
	}

	public virtual byte[] ProcessBlock(
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
				MemoryStream bIn = new MemoryStream(@in, inOff, inLen) { Position = 0 };
				try {
					PubParam = KeyParser.ReadKey(bIn);
				} catch (IOException e) {
					throw new InvalidCipherTextException("unable to recover ephemeral public key: " + e.Message, e);
				} catch (ArgumentException e) {
					throw new InvalidCipherTextException("unable to recover ephemeral public key: " + e.Message, e);
				}

				int encLength = (inLen - (int)(bIn.Length - bIn.Position));
				V = Arrays.CopyOfRange(@in, inOff, inOff + encLength);
			}
		}

		// Compute the common value and convert to byte array. 
		Agree.Init(PrivParam);
		BigInteger z = Agree.CalculateAgreement(PubParam);
		byte[] bigZ = BigIntegers.AsUnsignedByteArray(Agree.GetFieldSize(), z);

		// Create input to KDF.  
		if (V.Length != 0) {
			byte[] vz = Arrays.Concatenate(V, bigZ);
			Arrays.Fill(bigZ, 0);
			bigZ = vz;
		}

		try {
			// Initialise the KDF.
			KdfParameters kdfParam = new KdfParameters(bigZ, _param.GetDerivationV());
			Kdf.Init(kdfParam);

			return ForEncryption
				? EncryptBlock(@in, inOff, inLen)
				: DecryptBlock(@in, inOff, inLen);
		} finally {
			Arrays.Fill(bigZ, 0);
		}
	}
}
