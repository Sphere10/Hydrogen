using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Sphere10.Framework;

namespace Sphere10.Framework.CryptoEx {
	public static class HexEncoding {
		public const string CharSet = "0123456789abcdef";
		public const string NibblePattern = @"[0-9a-f]";
		public const string BytePattern = NibblePattern + "{2}";
		public const string SubStringPattern = "(?:" + BytePattern + ")*";
		public const string StringPattern = SubStringPattern + "$";

		private static readonly Regex HexStringRegex;

		static HexEncoding() {
			HexStringRegex = new Regex(StringPattern);
		}

		public static bool IsValid(string hexString) {
			return HexStringRegex.IsMatch(hexString);
		}

		public static byte[] Decode(string hexString) {
			if (!TryDecode(hexString, out var result))
				throw new ArgumentException("Invalid hex-formatted string", nameof(hexString));
			return result;
		}

		public static bool TryDecode(string hexString, out byte[] result) {
			result = null;
			if (!IsValid(hexString))
				return false;
			if (hexString.StartsWith("0x"))
				hexString = hexString.Substring(2);
			result = hexString.ToHexByteArray();
			return true;
		}

		public static string Encode(byte[] bytes) {
			return bytes.ToHexString(true).ToLowerInvariant();
		}
	}
}