// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Hydrogen;

public static class StringExtensions {
	private static readonly char[] InvalidFilePathChars = ("*?/\\:" + Path.GetInvalidPathChars().ToDelimittedString(string.Empty)).ToCharArray();
	private static readonly Regex AlphabetCheckRegex = new Regex("^[a-zA-Z0-9]*$");

	#region General

	public static string Truncate(this string value, int maxLength, string truncationSuffix = "…") {
		return value?.Length > maxLength
			? value.Substring(0, maxLength) + truncationSuffix
			: value;
	}
	public static int CountSubstring(this string text, string value, StringComparison comparison = StringComparison.Ordinal) {
		var count = 0;
		var minIndex = text.IndexOf(value, 0, comparison);
		while (minIndex != -1) {
			minIndex = text.IndexOf(value, minIndex + value.Length, comparison);
			count++;
		}
		return count;
	}

	public static string ToUnixPath(this string path)
		=> path.Replace('\\', '/');

	public static bool StartsWithAny(this string @string, params string[] strings)
		=> @string.StartsWithAny(StringComparison.InvariantCulture, strings);

	public static bool StartsWithAny(this string @string, StringComparison comparison, params string[] strings)
		=> strings.Any(x => @string.StartsWith(x, comparison));

	public static bool IsNullOrEmpty(this string text)
		=> String.IsNullOrEmpty(text);

	public static bool IsNullOrWhiteSpace(this string text)
		=> String.IsNullOrWhiteSpace(text);

	public static string ToNullWhenEmpty(this string text)
		=> text.ToValueWhenNullOrEmpty(null);

	public static string ToNullWhenWhitespace(this string text)
		=> text.ToValueWhenNullOrWhitespace(null);

	public static string ToValueWhenNullOrEmpty(this string @string, string value)
		=> string.IsNullOrEmpty(@string) ? value : @string;

	public static string ToValueWhenNullOrWhitespace(this string @string, string value)
		=> string.IsNullOrWhiteSpace(@string) ? value : @string;

	public static string Repeat(this string @string, int times) {
		var stringBuilder = new StringBuilder(@string.Length * times);
		for (var i = 0; i < times; i++)
			stringBuilder.Append(@string);
		return stringBuilder.ToString();
	}

	public static IEnumerable<int> IndexOfAll(this string sourceString, string subString) {
		return Regex.Matches(sourceString, Regex.Escape(subString)).Cast<Match>().Select(m => m.Index);
	}

	public static IEnumerable<string> SplitAndKeep(this string @string, char[] delims, StringSplitOptions options = StringSplitOptions.None) {
		var start = 0;
		var index = 0;

		while ((index = @string.IndexOfAny(delims, start)) != -1) {
			index = Interlocked.Exchange(ref start, index + 1);

			if (start - index - 1 > 0 || !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
				yield return @string.Substring(index, start - index - 1);

			yield return @string.Substring(start - 1, 1);
		}

		if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries)) {
			if (start < @string.Length) {
				yield return @string.Substring(start);
			}
		} else {
			yield return @string.Substring(start);
		}
	}

	public static string Clip(this string text, int maxLength, string cap = "...") {
		if (string.IsNullOrEmpty(text))
			return text;

		if (text.Length <= maxLength) {
			return text;
		}
		if (text.Length < cap.Length || maxLength < cap.Length)
			return text.Substring(0, maxLength);
		return text.Substring(0, maxLength - cap.Length) + cap;
	}

	public static bool ContainsAnySubstrings(this string text, params string[] substrings) {
		return substrings.Any(text.Contains);
	}

	public static bool IsAlphabetic(this string text) {
		return AlphabetCheckRegex.IsMatch(text);
	}

	public static string Tabbify(this string text, int tabs = 1) {
		var tabbedText = new StringBuilder();
		var tabBuilder = new StringBuilder();
		for (var i = 0; i < tabs; i++)
			tabBuilder.Append("\t");

		var tabSpace = tabBuilder.ToString();

		foreach (var line in GetLines(text))
			tabbedText.Append(String.Format("{0}{1}{2}", tabSpace, line, Environment.NewLine));

		return tabbedText.ToString();
	}

	public static string RemoveNonAlphaNumeric(this string @string) {
		var sb = new StringBuilder();
		for (int i = 0; i < @string.Length; i++) {
			char c = @string[i];
			if (Char.IsLetterOrDigit(c))
				sb.Append(c);
		}
		return sb.ToString();
	}

	public static string RemoveWhitespace(this string @string) {
		var sb = new StringBuilder();
		for (int i = 0; i < @string.Length; i++) {
			char c = @string[i];
			if (!char.IsWhiteSpace(c))
				sb.Append(c);
		}
		return sb.ToString();
	}

	public static byte[] ToAsciiByteArray(this string asciiString) => asciiString.ToByteArray(Encoding.ASCII);

	public static byte[] ToByteArray(this string asciiString, Encoding encoding) {
		return encoding.GetBytes(asciiString);
	}


	public static byte[] ToHexByteArray(this string hexString) => HexEncoding.Decode(hexString);


	public static IEnumerable<string> GetLines(this string str, bool removeEmptyLines = false) {
		return str.Split(
			new[] { "\r\n", "\r", "\n" },
			removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None
		);
	}

	public static string ChompStart(this string inputString, params string[] delimiters) {
		if (inputString == string.Empty)
			return string.Empty;

		var sb = new StringBuilder(inputString);
		foreach (var delimitter in delimiters)
			sb.Replace(delimitter, string.Empty, 0, 1);
		return sb.ToString();
	}

	public static string ChompStart(this string inputString, params char[] delimiters)
		=> ChompStart(inputString, delimiters.Select(x => x.ToString()).ToArray());


	public static string ChompEnd(this string inputString, params string[] delimiters) {
		if (inputString == string.Empty)
			return string.Empty;

		var sb = new StringBuilder(inputString);
		foreach (var delimitter in delimiters) {
			var startIX = sb.Length - delimitter.Length;
			if (startIX >= 0)
				sb.Replace(delimitter, string.Empty, startIX, sb.Length - startIX);
		}
		return sb.ToString();
	}

	public static string ChompEnd(this string inputString, params char[] delimiters)
		=> ChompEnd(inputString, delimiters.Select(x => x.ToString()).ToArray());


	public static string Chomp(this string inputString, params string[] delimiters) {
		if (inputString == string.Empty)
			return string.Empty;

		var sb = new StringBuilder(inputString);
		foreach (var delimitter in delimiters) {
			sb.Replace(delimitter, string.Empty, 0, 1);
			var startIX = sb.Length - delimitter.Length;
			if (startIX >= 0)
				sb.Replace(delimitter, string.Empty, startIX, sb.Length - startIX);
		}
		return sb.ToString();
	}

	public static string Chomp(this string inputString, params char[] delimiters)
		=> Chomp(inputString, delimiters.Select(x => x.ToString()).ToArray());


	public static string TrimWithCapture(this string value, out string trimmedStart, out string trimmedEnd) {
		// Slow implementation here
		trimmedStart = new string(value.TakeWhile(char.IsWhiteSpace).ToArray());
		trimmedEnd = new string(value.Reverse().TakeWhile(char.IsWhiteSpace).Reverse().ToArray());
		return value.Trim();
	}

	public static string ReplaceMany(this string inputString, params (string Match, string Replacement)[] replacements) {
		var sb = new StringBuilder(inputString);
		foreach (var replacement in replacements)
			sb.Replace(replacement.Match, replacement.Replacement);
		return sb.ToString();
	}

	static public string EncapsulateWith(this string value, string prePostFix) {
		return value.EncapsulateWith(prePostFix, prePostFix);
	}

	static public string EncapsulateWith(this string value, string preFix, string postFix) {
		if (!value.StartsWith(preFix))
			value = preFix + value;

		if (!value.EndsWith(postFix))
			value = value + postFix;

		return value;
	}

	public static string FormatWith(this string _string, params object[] _params) {
		if (_params == null || !_params.Any())
			return _string;

		return String.Format(_string, _params);
	}

	public static string FormatWithDictionary(this string _string, IDictionary<string, string> userTokenResolver, bool recursive = false)
		=> StringFormatter.FormatWithDictionary(_string, userTokenResolver.ToDictionary(x => x.Key, x => (object)x.Value), recursive);


	public static string FormatWithDictionary(this string _string, IDictionary<string, object> userTokenResolver, bool recursive = false)
		=> StringFormatter.FormatWithDictionary(_string, userTokenResolver, recursive);

	/// <summary>
	/// Parse a string into an enumeration
	/// </summary>
	/// <typeparam name="TEnum">The Enumeration type to cast to</typeparam>
	/// <param name="source"></param>
	/// <returns></returns>
	public static TEnum ParseEnum<TEnum>(this string source) {
		Type t = typeof(TEnum);

		if (!t.IsEnum)
			throw new ArgumentException("TEnum must be a valid Enumeration", "TEnum");

		return (TEnum)Enum.Parse(t, source);
	}

	public static string AsAmendmentIf(this string text, bool condition)
		=> condition ? $" {text}" : string.Empty;

	public static string AsAmendmentIfNotNull(this string text)
		=> AsAmendmentIf(text, text != null);

	public static string AsAmendmentIfNotNullOrEmpty(this string text)
		=> AsAmendmentIf(text, string.IsNullOrEmpty(text));

	public static string AsAmendmentIfNotNullOrWhitespace(this string text)
		=> AsAmendmentIf(text, string.IsNullOrWhiteSpace(text));

	public static string EscapeBraces(this string text) {
		Debug.Assert(text != null);
		return text.Replace("{", "{{").Replace("}", "}}");
	}

	public static string TrimToLength(this string @string, int len, string append = "") {
		if (@string == null || @string.Length <= len)
			return @string;

		return @string.Substring(0, len) + append;
	}

	public static string TrimWordsToLength(this string @string, int len, string append = "") {
		if (null == @string || @string.Length <= len)
			return @string;

		var match = new Regex(@"[^\s]\s", RegexOptions.RightToLeft).Match(@string, len - append.Length + 1);
		return match.Success
			? @string.Substring(0, match.Index + 1) + append
			: @string.Substring(0, len - append.Length) + append;
	}


	public static string MakeStartWith(this string @string, string startsWith, bool caseSensitive = true) {
		var cmpString = caseSensitive ? @string : @string.ToUpperInvariant();
		var cmpStartsWith = caseSensitive ? @startsWith : @startsWith.ToUpperInvariant();

		if (cmpString.StartsWith(cmpStartsWith))
			return @string;
		return startsWith + @string;
	}

	public static string MakeEndWith(this string @string, string endsWith, bool caseSensitive = true) {
		var cmpString = caseSensitive ? @string : @string.ToUpperInvariant();
		var cmpEndsWith = caseSensitive ? endsWith : endsWith.ToUpperInvariant();

		if (cmpString.EndsWith(cmpEndsWith))
			return @string;
		return @string + endsWith;
	}


	public static string TrimStart(this string @string, string substring, bool caseSensitive = true) {
		var cmpString = caseSensitive ? @string : @string.ToUpperInvariant();
		var cmpSubstring = caseSensitive ? substring : substring.ToUpperInvariant();

		if (cmpString.StartsWith(cmpSubstring))
			return @string.Substring(substring.Length);

		return @string;
	}

	public static string TrimEnd(this string @string, string substring, bool caseSensitive = true) {
		var cmpString = caseSensitive ? @string : @string.ToUpperInvariant();
		var cmpSubstring = caseSensitive ? substring : substring.ToUpperInvariant();

		if (cmpString.EndsWith(cmpSubstring))
			return @string.Substring(0, @string.Length - substring.Length);
		return @string;
	}

	public static string ToBase64(this string str) {
		byte[] encbuff = Encoding.UTF8.GetBytes(str);
		return Convert.ToBase64String(encbuff);
	}

	public static string FromBase64(this string str) {
		byte[] decbuff = Convert.FromBase64String(str);
		return Encoding.UTF8.GetString(decbuff);
	}

	public static string GetRegexMatch(this string input, string pattern, string group) {
		var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
		return match.Groups[@group].Success ? match.Groups[@group].Value : null;
	}

	#endregion

	#region Paths

	public static string ToCleanPathString(this string path) {
		path = path.Trim();
		if (path.Length == 3 && path[2] == '\\') {
#warning its drive so dont trim back slash from it Make this code better.
			return path;
		} else {
			return path.Trim().TrimEnd(Path.DirectorySeparatorChar);
		}
	}

	static public string EscapeCSV(this string value, string delimiter = ",") {
		var needsQuotes = value.Contains(delimiter);
		var isQuoted = value.StartsWith("\"") && !value.StartsWith("\"\"") && value.EndsWith("\"") && !value.EndsWith("\"\""); // doesn't handle """"text"""" scenarios
		value = value.Replace("\"", "\"\"");
		if (needsQuotes && !isQuoted)
			value = $"\"{value}\"";
		return value;
	}

	public static bool ToBool(this string value) {
		bool retval = false;
		switch (value.ToUpper()) {
			case "CHECKED":
			case "1":
			case "Y":
			case "YES":
			case "OK":
			case "TRUE":
			case "GRANTED":
			case "PERMISSION GRANTED":
			case "APPROVED":
				retval = true;
				break;
		}
		return retval;
	}

	#endregion

	#region Escaping / Unescaping

	public static string Escape(this string str, char escapeSymbol, params char[] escapedChars) {
		if (str == null)
			throw new ArgumentNullException(nameof(str));
		var result = string.Empty;
		var reader = new StringReader(str);
		char? peek;
		while ((peek = reader.PeekChar()) != null) {
			if (peek == escapeSymbol) {
				result += reader.ReadChar(); // append escape symbol					
				var next = reader.PeekChar();
				if (next == null) {
					// end of string, last char was escape symbol
					if (escapedChars.Contains(escapeSymbol)) {
						// need to escape it 
						result += $"{escapeSymbol}";
					}
				} else if (escapedChars.Contains((char)next)) {
					// is an escape sequence, append next char
					result += reader.ReadChar();
				} else {
					// is an invalid escape sequence
					if (escapedChars.Contains(escapeSymbol)) {
						// need to escape symbol, since it's an escaped char
						result += $"{escapeSymbol}";
					}
				}
			} else if (escapedChars.Contains((char)peek)) {
				// char needs escaping
				result += $"{escapeSymbol}{reader.ReadChar()}";
			} else {
				// normal char
				result += reader.ReadChar();
			}
		}
		return result;
	}

	public static string Unescape(this string str, char escapeSymbol, params char[] escapedChars) {
		if (str == null)
			throw new ArgumentNullException(nameof(str));
		var result = string.Empty;
		var reader = new StringReader(str);
		char? peek;
		while ((peek = reader.PeekChar()) != null) {
			if (peek == escapeSymbol) {
				reader.ReadChar(); // omit the escape symbol
				peek = reader.PeekChar();
				if (peek == null) {
					// last character was the escape symbol, so include it
					result += escapeSymbol;
					break;
				}
				if (!escapedChars.Contains((char)peek)) {
					// was not an escaped char, so include the escape symbol
					result += escapeSymbol;
					continue;
				}
			}
			// include the char (or escaped char)
			result += reader.ReadChar();
		}
		return result;
	}

	static public string UrlEncoded(this string str) {
		return Tools.Url.EncodeUrl(str);
	}

	static public string UrlDecoded(this string str) {
		return Tools.Url.DecodeUrl(str);
	}

	public static string EscapeJavascriptString(this string value) {
		return value.Replace("'", "\\'").Replace("\"", "\\\"");
	}

	public static string EscapeJavaScript(this string s) {
		StringBuilder sb = new StringBuilder(s);
		sb.Replace("\\", "\\\\");
		sb.Replace("\"", "\\\"");
		sb.Replace("\'", "\\'");
		sb.Replace("\t", "\\t");
		sb.Replace("\r", "\\r");
		sb.Replace("\n", "\\n");
		return sb.ToString();
	}

	/// <summary>
	/// Escapes a string safely for javascript interpretation, avoiding HTML embedding and character encoding issues by
	/// unicode-escaping all characters not guaranteed to be safe.
	/// </summary>
	public static string EscapeHtmlJavacript(this string @string) {
		/*
		 * Allowed are printable US-ASCII characters, minus Javascript-unsafe, minus XML/HTML-unsafe.
		 * 
		 * Skipped as javascript-unsafe:  " ' \
		 * Skipped as XML/HTML-unsafe:    Space & " ' < >
		 */
		if (null == @string) {
			return null;
		} else {
			return Regex.Replace(@string,
				@"[^\u0021\u0023-\u0025\u0028-\u003B\u003D\u003F-\u005B\u005D-\u007E]",
				new MatchEvaluator(delegate(Match match) {
					byte[] bb = Encoding.Unicode.GetBytes(match.Value);
					return String.Format(@"\u{0}{1}", bb[1].ToString("x2"), bb[0].ToString("x2"));
				}));
		}
	}

	public static string ReplaceNewLinesWithBR(this string @string) {
		return @string.Replace("\n", "<br/>");
	}

	public static string RemoveAnchorTag(this string @string) => Tools.Url.StripAnchorTag(@string);

	public static Dictionary<string, string> ParseQueryString(this string encdata) {
		return Tools.Url.ParseQueryString(encdata);
	}

	static public string EscapeSQL(this string unsafeString) {
		return unsafeString.Replace("'", "''");
	}

	#endregion

	#region Casing

	public static string ToSentenceCase(this string value) {
		return ParagraphBuilder.StringToSentenceCase(value);
	}

	/// <summary>
	/// Every word starts with capital.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static string ToTitleCase(this string value) {
		string[] parts = value.Split(new string[] { " " }, StringSplitOptions.None);
		StringBuilder builder = new StringBuilder();
		foreach (string part in parts) {
			builder.Append(part.ToSentenceCase());
			builder.Append(" ");
		}
		return builder.ToString();
	}

	public static string ToCamelCase(this string value)
		=> Tools.Text.ToCasing(TextCasing.CamelCase, value);

	public static string RemoveCamelCase(this string value) {
		StringBuilder retval = new StringBuilder();
		char lastChar = value[0];
		retval.Append(Char.ToUpper(lastChar));
		for (int i = 1; i < value.Length; i++) {
			char currChar = value[i];
			if (Char.IsLower(lastChar) && Char.IsUpper(currChar)) {
				retval.Append(" ");
			}
			retval.Append(currChar);
			lastChar = currChar;
		}
		return retval.ToString();
	}

	public static string ToCasing(this string value, TextCasing style, string text, FirstCharacterPolicy firstCharacterPolicy = FirstCharacterPolicy.Anything, string prefixIfPolicyInvalid = null) 
		=> Tools.Text.ToCasing(style, text, firstCharacterPolicy, prefixIfPolicyInvalid);

	/// <summary>
	/// Parses a camel cased or pascal cased string and returns a new 
	/// string with spaces between the words in the string.
	/// </summary>
	/// <example>
	/// The string "PascalCasing" will return an array with two 
	/// elements, "Pascal" and "Casing".
	/// </example>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string SplitUpperCaseToString(this string source) {
		return String.Join(" ", SplitUpperCase(source));
	}

	/// <summary>
	/// Parses a camel cased or pascal cased string and returns an array 
	/// of the words within the string.
	/// </summary>
	/// <example>
	/// The string "PascalCasing" will return an array with two 
	/// elements, "Pascal" and "Casing".
	/// </example>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string[] SplitUpperCase(this string source) {
		if (source == null)
			return new string[] { }; //Return empty array.

		if (source.Length == 0)
			return new string[] { "" };

		var words = new List<string>();
		int wordStartIndex = 0;

		char[] letters = source.ToCharArray();

		// Skip the first letter. we don't care what case it is.
		for (int i = 1; i < letters.Length; i++) {
			if (Char.IsUpper(letters[i])) {
				if (i + 1 < letters.Length && !Char.IsUpper(letters[i + 1])) {
					//Grab everything before the current index.
					words.Add(new String(letters, wordStartIndex, i - wordStartIndex));
					wordStartIndex = i;
				}
			}
		}

		//We need to have the last word.
		words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

		//Copy to a string array.
		string[] wordArray = new string[words.Count];
		words.CopyTo(wordArray, 0);
		return wordArray;
	}

	public static string GetLeafDirectory(this string path) {
		return path.Split(Path.DirectorySeparatorChar).Last();
	}

	public static bool IsUNCPath(this string path) {
		Uri uri = new Uri(path);
		return uri.IsUnc;
	}

	public static string GetUNCHost(this string path) {
		Debug.Assert(IsUNCPath(path));
		var uri = new Uri(path);
		return Uri.UnescapeDataString(uri.Host);
	}

	/// See: http://www.siao2.com/2007/05/14/2629747.aspx
	public static string RemoveDiacritics(this string @string) {
		var stFormD = @string.Normalize(NormalizationForm.FormD);
		var sb = new StringBuilder();

		for (int ich = 0; ich < stFormD.Length; ich++) {
			var uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
			if (uc != UnicodeCategory.NonSpacingMark) {
				sb.Append(stFormD[ich]);
			}
		}

		return (sb.ToString().Normalize(NormalizationForm.FormC));
	}

	#endregion

	#region Parse Many

	public static T1 ParseMany<T1>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return ParseToken<T1>(tokens, 0);
	}

	public static Tuple<T1, T2> ParseMany<T1, T2>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return Tuple.Create(ParseToken<T1>(tokens, 0), ParseToken<T2>(tokens, 1));
	}

	public static Tuple<T1, T2, T3> ParseMany<T1, T2, T3>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return Tuple.Create(ParseToken<T1>(tokens, 0), ParseToken<T2>(tokens, 1), ParseToken<T3>(tokens, 2));
	}

	public static Tuple<T1, T2, T3, T4> ParseMany<T1, T2, T3, T4>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return Tuple.Create(ParseToken<T1>(tokens, 0), ParseToken<T2>(tokens, 1), ParseToken<T3>(tokens, 2), ParseToken<T4>(tokens, 3));
	}

	public static Tuple<T1, T2, T3, T4, T5> ParseMany<T1, T2, T3, T4, T5>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return Tuple.Create(ParseToken<T1>(tokens, 0), ParseToken<T2>(tokens, 1), ParseToken<T3>(tokens, 2), ParseToken<T4>(tokens, 3), ParseToken<T5>(tokens, 4));
	}

	public static Tuple<T1, T2, T3, T4, T5, T6> ParseMany<T1, T2, T3, T4, T5, T6>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return Tuple.Create(ParseToken<T1>(tokens, 0), ParseToken<T2>(tokens, 1), ParseToken<T3>(tokens, 2), ParseToken<T4>(tokens, 3), ParseToken<T5>(tokens, 4), ParseToken<T6>(tokens, 5));
	}

	public static Tuple<T1, T2, T3, T4, T5, T6, T7> ParseMany<T1, T2, T3, T4, T5, T6, T7>(this string stringValue, params string[] delimitters) {
		var tokens = Tokenize(stringValue, delimitters);
		return Tuple.Create(ParseToken<T1>(tokens, 0), ParseToken<T2>(tokens, 1), ParseToken<T3>(tokens, 2), ParseToken<T4>(tokens, 3), ParseToken<T5>(tokens, 4), ParseToken<T6>(tokens, 5), ParseToken<T7>(tokens, 6));
	}

	private static T1 ParseToken<T1>(string[] tokens, int index) {
		if (index >= tokens.Length)
			throw new SoftwareException($"No {typeof(T1).Name} found in token {index}");
		return Parse<T1>(tokens[index]);
	}

	private static string[] Tokenize(string stringValue, params string[] delimitters) {
		return stringValue.Split(delimitters, StringSplitOptions.None);
	}

	private static T Parse<T>(string token) {
		T val;
		if (typeof(T) == typeof(string)) {
			val = (T)(object)token;
		} else {
			val = Tools.Parser.Parse<T>(token);
		}
		return val;
	}

	#endregion

}
