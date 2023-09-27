﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// Based off: https://github.com/bazzilic/BigIntegerExt

using System;
using System.Numerics;

namespace Hydrogen;

public static class BigIntegerExtensions {

	// primes smaller than 2000 to test the generated prime number
	public static readonly int[] PrimesBelow2000 = {
		2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
		73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
		179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
		283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
		419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
		547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
		661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
		811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
		947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
		1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
		1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
		1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
		1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
		1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
		1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889, 1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
		1993, 1997, 1999
	};


	/// <summary>
	/// Returns the modulo inverse of this
	/// </summary>
	/// <param name="mod">Modulo</param>
	/// <returns>Modulo inverse of this</returns>
	public static BigInteger ModInverse(this BigInteger T, BigInteger mod) {
		BigInteger i = mod, v = 0, d = 1;

		while (T > 0) {
			BigInteger t = i / T, x = T;
			T = i % x;
			i = x;
			x = d;
			d = v - t * x;
			v = x;
		}

		v %= mod;
		if (v < 0)
			v = (v + mod) % mod;

		return v;
	}


	/// <summary>
	/// Returns the position of the most significant bit in the BigInteger
	/// </summary>
	/// <example>
	/// 1) The result is 1, if the value of BigInteger is 0...0000 0000
	/// 2) The result is 1, if the value of BigInteger is 0...0000 0001
	/// 3) The result is 2, if the value of BigInteger is 0...0000 0010
	/// 4) The result is 2, if the value of BigInteger is 0...0000 0011
	/// 5) The result is 5, if the value of BigInteger is 0...0001 0011
	/// </example>
	/// <returns></returns>
	public static int BitCount(this BigInteger T) {
		var data = T.ToByteArray();
		byte value = data[data.Length - 1];
		byte mask = 0x80;
		var bits = 8;

		while (bits > 0 && (value & mask) == 0) {
			bits--;
			mask >>= 1;
		}

		bits += (data.Length - 1) << 3;

		return bits == 0 ? 1 : bits;
	}


	/// <summary>
	/// Returns the specified amount of random bits generated using provided RNG
	/// </summary>
	/// <param name="bits"></param>
	/// <param name="rng"></param>
	public static BigInteger GenRandomBits(this BigInteger T, int bits, System.Security.Cryptography.RandomNumberGenerator rng) {
		if (bits <= 0)
			throw new ArithmeticException("Number of required bits is not valid.");

		var bytes = bits >> 3;
		var remBits = bits % 8;

		if (remBits != 0)
			bytes++;

		var data = new byte[bytes];

		rng.GetBytes(data);

		if (remBits != 0) {
			byte mask;

			if (bits != 1) {
				mask = (byte)(0x01 << (remBits - 1));
				data[bytes - 1] |= mask;
			}

			mask = (byte)(0xFF >> (8 - remBits));
			data[bytes - 1] &= mask;
		} else
			data[bytes - 1] |= 0x80;

		data[bytes - 1] &= 0x7F;

		return new BigInteger(data);
	}


	/// <summary>
	/// Generates a positive BigInteger that is probably prime (secured version)
	/// </summary>
	/// <param name="bits">Number of bits; has to be greater than 1</param>
	/// <param name="confidence">Number of chosen bases</param>
	/// <param name="rand">RNGCryptoServiceProvider object</param>
	/// <returns>A probably prime number</returns>
	public static BigInteger GenPseudoPrime(this BigInteger T, int bits, int confidence, System.Security.Cryptography.RandomNumberGenerator rand) {
		if (bits < 2)
			throw new ArgumentOutOfRangeException(nameof(bits), bits, "GenPseudoPrime can only generate prime numbers of 2 bits or more");

		var result = new BigInteger();
		var done = false;

		while (!done) {
			result = result.GenRandomBits(bits, rand);
			result |= 1; // make it odd

			// prime test
			done = result.IsProbablePrime(confidence);
		}

		return result;
	}


	/// <summary>
	/// Determines whether a number is probably prime using the Rabin-Miller's test
	/// </summary>
	/// <remarks>
	/// Before applying the test, the number is tested for divisibility by primes &lt; 2000
	/// </remarks>
	/// <param name="confidence">Number of chosen bases</param>
	/// <returns>True if this is probably prime</returns>
	public static bool IsProbablePrime(this BigInteger T, int confidence) {
		var thisVal = BigInteger.Abs(T);
		if (thisVal.IsZero || thisVal.IsOne) return false;

		var val = (Int32)BigInteger.Min(Int32.MaxValue, thisVal);

		// test for divisibility by primes < 2000
		for (var i = 0; i < PrimesBelow2000.Length; i++) {
			var divisor = PrimesBelow2000[i];

			if (divisor >= val)
				return true;

			var resultNum = BigInteger.Remainder(thisVal, divisor);
			if (resultNum == BigInteger.Zero)
				return false;
		}

		return thisVal.RabinMillerTest(confidence);
	}


	/// <summary>
	/// Probabilistic prime test based on Miller-Rabin's algorithm.
	/// Algorithm based on http://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.186-4.pdf (p. 72)
	/// This method REQUIRES that the BigInteger is positive
	/// </summary>
	/// <remarks>
	/// for any p &gt; 0 with p - 1 = 2^s * t
	///
	/// p is probably prime (strong pseudoprime) if for any a &lt; p,
	/// 1) a^t mod p = 1 or
	/// 2) a^((2^j)*t) mod p = p-1 for some 0 &lt;= j &lt;= s-1
	///
	/// Otherwise, p is composite.
	/// </remarks>
	/// <param name="confidence">Number of chosen bases</param>
	/// <returns>True if this is a strong pseudoprime to randomly chosen bases</returns>
	public static bool RabinMillerTest(this BigInteger w, int confidence) {
		var m = w - 1;
		var a = 0;

		while (m % 2 == 0) {
			m /= 2;
			a += 1;
		}

		// There is no built-in method for generating random BigInteger values.
		// Instead, random BigIntegers are constructed from randomly generated
		// byte arrays of the same length as the w.
		var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
		var wlen = w.BitCount();
		var b = BigInteger.Zero;

		for (var i = 0; i < confidence; i++) {
			do {
				b = b.GenRandomBits(wlen, rng);
			} while (b < 2 || b >= w - 1);

			var z = BigInteger.ModPow(b, m, w);
			if (z == 1 || z == w - 1)
				continue;

			for (var j = 1; j < a; j++) {
				z = BigInteger.ModPow(z, 2, w);
				if (z == 1)
					return false;
				if (z == w - 1)
					break;
			}

			if (z != w - 1)
				return false;
		}

		return true;
	}
}
