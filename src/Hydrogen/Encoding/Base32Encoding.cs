// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Text;

namespace Hydrogen;

public static class Base32Encoding {

	// the valid chars for the encoding
	private static readonly string ValidChars = "QAZ2WSX3" + "EDC4RFV5" + "TGB6YHN7" + "UJM8K9LP";

	public static string Encode(string asciiString) {
		return Encode(asciiString.ToAsciiByteArray());
	}


	/// <summary>
	/// Converts an array of bytes to a Base32-k string.
	/// </summary>
	public static string Encode(ReadOnlySpan<byte> bytes) {
		var sb = new StringBuilder(); // holds the base32 chars
		var hi = 5;
		var currentByte = 0;
		while (currentByte < bytes.Length) {
			// do we need to use the next byte?
			byte index;
			switch (hi) {
				case > 8: {
					// get the last piece from the current byte, shift it to the right
					// and increment the byte counter
					index = (byte)(bytes[currentByte++] >> (hi - 5));
					if (currentByte != bytes.Length) {
						// if we are not at the end, get the first piece from
						// the next byte, clear it and shift it to the left
						index = (byte)(((byte)(bytes[currentByte] << (16 - hi)) >> 3) | index);
					}
					hi -= 3;
					break;
				}
				case 8:
					index = (byte)(bytes[currentByte++] >> 3);
					hi -= 3;
					break;
				default:
					// simply get the stuff from the current byte
					index = (byte)((byte)(bytes[currentByte] << (8 - hi)) >> 3);
					hi += 5;
					break;
			}
			sb.Append(ValidChars[index]);
		}
		return sb.ToString();
	}

	/// <summary>
	/// Converts a Base32-k string into an array of bytes.
	/// </summary>
	/// <exception cref="System.ArgumentException">
	/// Input string <paramref name="s">s</paramref> contains invalid Base32-k characters.
	/// </exception>
	public static byte[] Decode(string str) {
		if (str == string.Empty)
			return Array.Empty<byte>();

		var numBytes = str.Length * 5 / 8;
		var bytes = new byte[numBytes];

		// all UPPERCASE chars
		str = str.ToUpper();

		if (str.Length < 3) {
			bytes[0] = (byte)(ValidChars.IndexOf(str[0]) | ValidChars.IndexOf(str[1]) << 5);
			return bytes;
		}

		var bitBuffer = (ValidChars.IndexOf(str[0]) | ValidChars.IndexOf(str[1]) << 5);
		var bitsInBuffer = 10;
		var currentCharIndex = 2;
		for (var i = 0; i < bytes.Length; i++) {
			bytes[i] = (byte)bitBuffer;
			bitBuffer >>= 8;
			bitsInBuffer -= 8;
			while (bitsInBuffer < 8 && currentCharIndex < str.Length) {
				bitBuffer |= ValidChars.IndexOf(str[currentCharIndex++]) << bitsInBuffer;
				bitsInBuffer += 5;
			}
		}

		return bytes;
	}

	public static bool IsValid(string base32String) => !string.IsNullOrEmpty(base32String) && base32String.ToUpper().All(ValidChars.Contains);
}
