using System;
using System.Linq;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx; 

public class BigIntegerUtils {
	internal static byte[] BigIntegerToBytes(BigInteger b, int numBytes)
	{
		if (b == null)
		{
			return null;
		}
		byte[] bytes = new byte[numBytes];
		byte[] biBytes = b.ToByteArray();
		int start = (biBytes.Length == numBytes + 1) ? 1 : 0;
		int length = Math.Min(biBytes.Length, numBytes);
		Array.Copy(biBytes, start, bytes, numBytes - length, length);
		return bytes;
	}
	internal static byte[] BigIntegerToBytes(BigInteger num)
	{
		if (num.Equals(BigInteger.Zero))
			//Positive 0 is represented by a null-length vector
			return new byte[0];

		bool isPositive = true;
		if (num.CompareTo(BigInteger.Zero) < 0)
		{
			isPositive = false;
			num = num.Multiply(BigInteger.ValueOf(-1));
		}
		var array = num.ToByteArray();
		Array.Reverse(array);
		if (!isPositive)
			array[array.Length - 1] |= 0x80;
		return array;
	}

	// internal static BigInteger BytesToBigInteger(byte[] data)
	// {
	// 	if (data == null)
	// 		throw new ArgumentNullException(nameof(data));
	// 	if (data.Length == 0)
	// 		return BigInteger.Zero;
	// 	data = data.ToArray();
	// 	var positive = (data[data.Length - 1] & 0x80) == 0;
	// 	if (!positive)
	// 	{
	// 		data[data.Length - 1] &= unchecked((byte)~0x80);
	// 		Array.Reverse(data);
	// 		return new BigInteger(1, data).Negate();
	// 	}
	// 	return new BigInteger(1, data);
	// }
	internal static BigInteger BytesToBigInteger(byte[] data)
	{
		return new BigInteger(1, data);
	}
}
