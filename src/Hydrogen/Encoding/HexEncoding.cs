using System;
using System.Text.RegularExpressions;

namespace Hydrogen {
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
			result = null;
            if (!IsValid(hexString, allow_0x_prefix))
				return false;
            if (allow_0x_prefix && hexString.Length == 3 && hexString == "0x0")
                result = new byte[0]; // 0x0
            else
                result = hexString.ToHexByteArray();
			return true;
		}

		public static string Encode(byte[] bytes, bool prefix_0x = false) {
			return bytes.ToHexString(!prefix_0x).ToLowerInvariant();
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
}