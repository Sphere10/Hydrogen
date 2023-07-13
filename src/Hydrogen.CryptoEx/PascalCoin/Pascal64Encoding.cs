// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;
using System.Text.RegularExpressions;

namespace Hydrogen.CryptoEx.PascalCoin;

public static class Pascal64Encoding {
	public const char EscapeChar = '\\';
	public const string CharSet = @"abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-+{}[]_:""`|<>,.?/~";
	public const string CharSetStart = @"abcdefghijklmnopqrstuvwxyz!@#$%^&*()-+{}[]_:""`|<>,.?/~";
	public const string CharSetEscaped = @"(){}[]:""<>";
	public const string CharSetUnescaped = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*-+_`|,.?/~";
	public const string StartCharPattern = @"(a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|!|@|#|\$|%|\^|&|\*|\\\(|\\\)|-|\+|\\\{|\\\}|\\\[|\\]|_|\\:|\\""|`|\||\\<|\\>|,|\.|\?|/|~)";
	public const string NextCharPattern = @"(a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|0|1|2|3|4|5|6|7|8|9|!|@|#|\$|%|\^|&|\*|\\\(|\\\)|-|\+|\\\{|\\\}|\\\[|\\]|_|\\:|\\""|`|\||\\<|\\>|,|\.|\?|/|~)";
	public const string StringPattern = StartCharPattern + NextCharPattern + "*";
	public const string StringOnlyPattern = StringPattern + "$";
	private static readonly Regex EscapedRegex;

	static Pascal64Encoding() {
		EscapedRegex = new Regex(StringOnlyPattern);
	}

	public static bool IsValidUnescaped(string unescapedPascal64String) {
		return
			3 <= unescapedPascal64String.Length && unescapedPascal64String.Length <= 64 &&
			StartCharPattern.Contains(unescapedPascal64String[0]) &&
			unescapedPascal64String.All(c => CharSet.Contains(c));
	}

	public static bool IsValidEscaped(string escapedPascal64String) {
		return EscapedRegex.IsMatch(escapedPascal64String);
	}

	public static string Escape(string pascal64String) {
		return pascal64String.Escape(EscapeChar, CharSetEscaped.ToCharArray());
	}

	public static string Unescape(string pascal64String) {
		return pascal64String.Unescape(EscapeChar, CharSetEscaped.ToCharArray());
	}

}
