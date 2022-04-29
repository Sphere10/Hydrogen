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
