// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Hydrogen;

public class HexEncoding {
	public const string CharSet = "0123456789abcdefABCDEF";
	public const string NibblePattern = @"[0-9a-fA-F]";
	public const string BytePattern = NibblePattern + "{2}";
	public const string SubStringPattern = "(?:" + BytePattern + ")+";
	public const string StringPattern = "^" + SubStringPattern + "$";

	private static readonly Regex HexStringRegex;

	static HexEncoding() {
		HexStringRegex = new Regex(StringPattern);
	}

	public static bool IsValid(string hexString, bool allow_0x_prefix = true) {
		if (hexString == null)
			return false;
		if (hexString == string.Empty)
			return true;

		if (allow_0x_prefix && hexString.StartsWith("0x")) {
			if (hexString.Length == 3 && hexString[2] == '0')
				return true; // allow 0x0
			hexString = hexString.Substring(2);
		}
		return HexStringRegex.IsMatch(hexString);
	}

	public static byte[] Decode(string hexString, bool allow_0x_prefix = true) {
		if (!TryDecode(hexString, out var result, allow_0x_prefix))
			throw new FormatException("Invalid hex-formatted string");
		return result;
	}

	public static bool TryDecode(string hexString, out byte[] result, bool allow_0x_prefix = true) {
		Guard.ArgumentNotNull(hexString, nameof(hexString));
		result = Array.Empty<byte>();
		if (hexString == string.Empty) {
			return true;
		}
		if (!IsValid(hexString, allow_0x_prefix))
			return false;

		var startsWith0x = hexString.StartsWith("0x");
		if (allow_0x_prefix && hexString.Length == 3 && startsWith0x && hexString[2] == '0') {
			// is 0x0
			return true;
		}

		var offset = 0;
		if (startsWith0x)
			offset = 2;

		if ((hexString.Length - offset) % 2 != 0)
			throw new FormatException("Hex-formatted string has odd number of nibbles");

		var numberBytes = (hexString.Length - offset) / 2;

		var bytes = new byte[numberBytes];
		for (var i = 0; i < numberBytes; i++)
			bytes[i] = Convert.ToByte(new string(new char[2] { hexString[offset + 2 * i], hexString[offset + 2 * i + 1] }), 16);

		result = bytes;
		return true;
	}

	public static string Encode(byte[] bytes, bool prefix_0x = false) {
		Guard.ArgumentNotNull(bytes, nameof(bytes));

		if (bytes.Length == 0)
			return prefix_0x ? "0x0" : string.Empty;

		var hexBuilder = new StringBuilder(bytes.Length * 2);
		if (prefix_0x)
			hexBuilder.Append("0x");

		foreach (var @byte in bytes)
			hexBuilder.AppendFormat("{0:x2}", @byte);

		return hexBuilder.ToString();

	}

	public static int ByteLength(string hexString) {
		int numNibble;
		if (hexString.StartsWith("0x")) {
			if (hexString.Length == 3 && hexString[2] == '0')
				return 0; // 0x0 is code for null
			numNibble = (hexString.Length - 2);
		} else {
			numNibble = hexString.Length;
		}
		if (numNibble % 2 != 0)
			throw new FormatException("Invalid hex-formatted string");

		return numNibble / 2;
	}
}
