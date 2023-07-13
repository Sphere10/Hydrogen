// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Hydrogen.CryptoEx.PascalCoin;

public class PascalBase58Encoding {
	public const string CharSet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
	public const string CharPattern = "[123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz]";
	public const string SubStringPattern = CharPattern + "+";
	public const string StringPattern = SubStringPattern + "$";
	private static Regex StringRegex;

	public PascalBase58Encoding() {
		StringRegex = new Regex(StringPattern);
	}

	public static bool IsValid(string base58String) {
		return StringRegex.IsMatch(base58String);
	}

	public static byte[] Decode(string base58String) {
		if (!TryDecode(base58String, out var result))
			throw new ArgumentException("Invalid Base58-formatted string", nameof(base58String));
		return result;
	}

	public static bool TryDecode(string base58String, out byte[] result) {
		result = null;
		BigInteger number = 0, radix = 1;
		for (var i = base58String.Length - 1; i >= 0; i--) {
			var j = CharSet.IndexOf(base58String[i]);
			if (j == -1) return false;
			number += j * radix;
			radix *= 58;
		}
		result = number
			.ToByteArray() // Converts to BigInteger hex-format (ends with sign byte)
			.Reverse()
			.Skip(1) // Skip sign byte
			.ToArray();
		return true;
	}

	public static string Encode(byte[] bytes) {
		var result = String.Empty;
		var number = new BigInteger(
			bytes
				.Reverse()
				.Concat((byte)1) // add sign byte post-fix					
				.ToArray()
		);
		while (number != 0) {
			number = BigInteger.DivRem(number, 58, out var remainder);
			result = CharSet[(int)remainder] + result;
		}
		return result;
	}
}
