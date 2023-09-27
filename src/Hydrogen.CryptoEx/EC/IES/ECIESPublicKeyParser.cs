// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.IO;
using Hydrogen.CryptoEx.IES;

namespace Hydrogen.CryptoEx.EC.IES;

internal class ECIESPublicKeyParser : IKeyParser {
	private readonly ECDomainParameters _ecParams;

	public ECIESPublicKeyParser(ECDomainParameters ecParams) {
		_ecParams = ecParams;
	}

	public AsymmetricKeyParameter ReadKey(Stream stream) {
		byte[] v;
		int first = stream.ReadByte();

		// Decode the public ephemeral key
		switch (first) {
			case 0x00: // infinity
				throw new IOException("Sender's public key invalid.");

			case 0x02: // compressed
			case 0x03: // Byte length calculated as in ECPoint.getEncoded();
				v = new byte[1 + (_ecParams.Curve.FieldSize + 7) / 8];
				break;

			case 0x04: // uncompressed or
			case 0x06: // hybrid
			case 0x07: // Byte length calculated as in ECPoint.getEncoded();
				v = new byte[1 + 2 * ((_ecParams.Curve.FieldSize + 7) / 8)];
				break;

			default:
				throw new IOException("Sender's public key has invalid point encoding 0x" + first.ToString("X2"));
		}

		v[0] = (byte)first;
		Streams.ReadFully(stream, v, 1, v.Length - 1);

		return new ECPublicKeyParameters(_ecParams.Curve.DecodePoint(v), _ecParams);
	}
}
