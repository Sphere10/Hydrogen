// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Diagnostics;

namespace Hydrogen.CryptoEx.Bitcoin;

// this implementation is based on https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp
public class BitcoinBase58Encoding {

	// All alphanumeric characters except for "0", "I", "O", and "l" 
	private static readonly char[] PszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
	private static readonly sbyte[] MapBase58 = new sbyte[256];

	static BitcoinBase58Encoding() {
		Array.Fill<sbyte>(MapBase58, -1);
		for (sbyte i = 0; i < PszBase58.Length; ++i) {
			MapBase58[PszBase58[i]] = i;
		}
	}

	private static bool IsSpace(char c) {
		switch (c) {
			case ' ':
			case '\f':
			case '\n':
			case '\r':
			case '\t':
			case '\v':
				return true;
		}
		return false;
	}

	public static bool IsValid(string base58String) {
		var isBase58Encoded = base58String.All(t => PszBase58.Contains(t));
		return isBase58Encoded && base58String.Length > 0;
	}


	public static byte[] Decode(string base58String) {
		if (!TryDecode(base58String, out var result))
			throw new ArgumentException("Invalid Base58-formatted string", nameof(base58String));
		return result;
	}

	public static bool TryDecode(string base58String, out byte[] result) {
		result = null;
		if (base58String == null)
			throw new ArgumentNullException(nameof(base58String));
		var psz = 0;
		var base58StringLength = base58String.Length;
		// Skip leading spaces.
		while (psz < base58StringLength && IsSpace(base58String[psz]))
			psz++;
		// Skip and count leading '1's.
		var zeroes = 0;
		var length = 0;
		while (psz < base58StringLength && base58String[psz] == '1') {
			zeroes++;
			psz++;
		}
		// Allocate enough space in big-endian base256 representation.
		var size = (base58StringLength - psz) * 733 / 1000 + 1; // log(58) / log(256), rounded up.
		var b256 = size <= 128 ? stackalloc byte[size] : new byte[size];
		// Process the characters.
		int it, i;
		while (psz < base58StringLength && !IsSpace(base58String[psz])) {
			// Decode base58 character
			int carry = MapBase58[(byte)base58String[psz]];
			if (carry == -1) // Invalid b58 character
				return false;
			it = size - 1;
			i = 0;
			while ((carry != 0 || i < length) && it >= 0) {

				carry += 58 * b256[it];
				b256[it] = (byte)(carry % 256);
				carry /= 256;
				i++;
				it--;
			}
			Debug.Assert(carry == 0);
			length = i;
			psz++;
		}
		// Skip trailing spaces.
		while (psz < base58StringLength && IsSpace(base58String[psz]))
			psz++;
		if (psz != base58String.Length)
			return false;
		// Skip leading zeroes in b256.
		it = size - length;
		// Copy result into output vector.
		result = new byte[zeroes + size - it];
		Array.Fill<byte>(result, 0, 0, zeroes);
		i = zeroes;
		while (it != size)
			result[i++] = b256[it++];
		return true;
	}

	public static string Encode(byte[] bytes) {
		if (bytes == null)
			throw new ArgumentNullException(nameof(bytes));
		var bytesLength = bytes.Length;
		// Skip & count leading zeroes.
		var zeroes = 0;
		var length = 0;
		var startIndex = 0;
		while (startIndex != bytesLength && bytes[startIndex] == 0) {
			startIndex++;
			zeroes++;
		}
		// Allocate enough space in big-endian base58 representation.
		var size = (bytesLength - startIndex) * 138 / 100 + 1; // log(256) / log(58), rounded up.
		var b58 = size <= 128 ? stackalloc byte[size] : new byte[size];
		// Process the bytes.
		int it, i;
		while (startIndex != bytesLength) {
			int carry = bytes[startIndex];
			// Apply "b58 = b58 * 256 + ch".
			it = size - 1;
			i = 0;
			while ((carry != 0 || i < length) && it >= 0) {
				{
					carry += 256 * b58[it];
					b58[it] = (byte)(carry % 58);
					carry /= 58;
					i++;
					it--;
				}
			}
			Debug.Assert(carry == 0);
			length = i;
			startIndex++;
		}
		// Skip leading zeroes in base58 result.
		it = size - length;
		while (it != size && b58[it] == 0)
			it++;
		// Translate the result into a string.
		var stringSize = zeroes + size - it;
		var str = stringSize <= 128 ? stackalloc char[stringSize] : new char[stringSize];
		str.Slice(0, zeroes).Fill('1');
		i = zeroes;
		while (it != size)
			str[i++] = PszBase58[b58[it++]];
		return new string(str);
	}
}
