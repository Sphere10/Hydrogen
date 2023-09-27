// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Math;

namespace Hydrogen.CryptoEx;

public static class BigIntegerUtils {
	public static byte[] BigIntegerToBytes(BigInteger b, int numBytes) {
		if (b == null) {
			return null;
		}
		var bytes = new byte[numBytes];
		var biBytes = b.ToByteArray();
		var start = (biBytes.Length == numBytes + 1) ? 1 : 0;
		var length = Math.Min(biBytes.Length, numBytes);
		Array.Copy(biBytes, start, bytes, numBytes - length, length);
		return bytes;
	}

	public static BigInteger BytesToBigIntegerPositive(byte[] data) {
		return data == null ? null : new BigInteger(1, data);
	}

}
