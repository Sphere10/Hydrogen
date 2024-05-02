// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Hydrogen.CryptoEx.PascalCoin;

public class PascalCoinEciesKdfBytesGenerator : IDerivationFunction {
	private byte[] _shared;

	public PascalCoinEciesKdfBytesGenerator(IDigest digest) {
		Digest = digest;
		digest.Reset();
	}

	public IDigest Digest { get; }

	public virtual void Init(IDerivationParameters parameters) {
		KdfParameters kdfParameters = (KdfParameters)parameters;
		if (kdfParameters != null) {
			_shared = kdfParameters.GetSharedSecret();
		} else {
			throw new ArgumentException("KDF Parameters Required For KDF Generator");
		}
	}

	public virtual int GenerateBytes(byte[] output, int outOff, int length) {
		if ((output.Length - length) < outOff) {
			throw new DataLengthException("Output Buffer too Small");
		}

		int outLen = Digest.GetDigestSize();

		if (length > outLen) {
			throw new DataLengthException(
				"Specified Hash Cannot Produce Sufficient Data for the Specified Operation.");
		}

		byte[] temp = new byte[Digest.GetDigestSize()];

		Digest.BlockUpdate(_shared, 0, _shared.Length);
		Digest.DoFinal(temp, 0);

		Array.Copy(temp, 0, output, outOff, length);

		Digest.Reset();
		return length;
	}
	
}
