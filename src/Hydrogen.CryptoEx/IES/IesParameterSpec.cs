// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx.IES;

/**
    * Parameter spec for an integrated encryptor, as in IEEE P1363a
    */
public class IesParameterSpec : IAlgorithmParameterSpec {
	private readonly byte[] _derivation;
	private readonly byte[] _encoding;
	private readonly int _macKeySize;
	private readonly int _cipherKeySize;
	private readonly byte[] _nonce;
	private readonly bool _usePointCompression;


	/**
         * Set the IES engine parameters.
         *
         * @param derivation    the optional derivation vector for the KDF.
         * @param encoding      the optional encoding vector for the KDF.
         * @param macKeySize    the key size (in bits) for the MAC.
         * @param cipherKeySize the key size (in bits) for the block cipher.
         * @param nonce         an IV to use initialising the block cipher.
         */
	public IesParameterSpec(
		byte[] derivation,
		byte[] encoding,
		int macKeySize,
		int cipherKeySize,
		byte[] nonce) : this(derivation, encoding, macKeySize, cipherKeySize, nonce, false) {
	}

	/**
         * Set the IES engine parameters.
         *
         * @param derivation    the optional derivation vector for the KDF.
         * @param encoding      the optional encoding vector for the KDF.
         * @param macKeySize    the key size (in bits) for the MAC.
         * @param cipherKeySize the key size (in bits) for the block cipher.
         * @param nonce         an IV to use initialising the block cipher.
         * @param usePointCompression whether to use EC point compression or not (false by default)
         */
	public IesParameterSpec(
		byte[] derivation,
		byte[] encoding,
		int macKeySize,
		int cipherKeySize = -1,
		byte[] nonce = null,
		bool usePointCompression = false) {
		if (derivation != null) {
			_derivation = new byte[derivation.Length];
			Array.Copy(derivation, 0, _derivation, 0, derivation.Length);
		} else {
			_derivation = null;
		}

		if (encoding != null) {
			_encoding = new byte[encoding.Length];
			Array.Copy(encoding, 0, _encoding, 0, encoding.Length);
		} else {
			_encoding = null;
		}

		_macKeySize = macKeySize;
		_cipherKeySize = cipherKeySize;
		_nonce = Arrays.Clone(nonce);
		_usePointCompression = usePointCompression;
	}

	/**
         * return the derivation vector.
         */
	public byte[] GetDerivationV() {
		return Arrays.Clone(_derivation);
	}

	/**
         * return the encoding vector.
         */
	public byte[] GetEncodingV() {
		return Arrays.Clone(_encoding);
	}

	/**
         * return the key size in bits for the MAC used with the message
         */
	public int GetMacKeySize() {
		return _macKeySize;
	}

	/**
         * return the key size in bits for the block cipher used with the message
         */
	public int GetCipherKeySize() {
		return _cipherKeySize;
	}

	/**
         * Return the nonce (IV) value to be associated with message.
         *
         * @return block cipher IV for message.
         */
	public byte[] GetNonce() {
		return Arrays.Clone(_nonce);
	}

	/**
         * Return the 'point compression' flag.
         *
         * @return the point compression flag
         */
	public bool GetPointCompression() {
		return _usePointCompression;
	}
}
