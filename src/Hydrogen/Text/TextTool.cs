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
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Text {
	public static readonly Regex AsciiLetterRegex = new("[a-zA-Z]");

	public static string ToCasing(TextCasing style, string text, FirstCharacterPolicy firstCharacterPolicy = FirstCharacterPolicy.Anything, string prefixIfPolicyInvalid = null) {
		const string DefaultFirstChar = "_";
		const string DefaultLetterOnlyFirstChar = "v";
		const string DefaultDigitOnlyFirstChar = "0";

		if (firstCharacterPolicy != FirstCharacterPolicy.Anything && prefixIfPolicyInvalid == null) {
			if (firstCharacterPolicy.HasFlag(FirstCharacterPolicy.AllowUnderscore))
				prefixIfPolicyInvalid = DefaultFirstChar;
			else if (firstCharacterPolicy.HasFlag(FirstCharacterPolicy.AllowAsciiLetters))
				prefixIfPolicyInvalid = DefaultLetterOnlyFirstChar;
			else if (firstCharacterPolicy.HasFlag(FirstCharacterPolicy.AllowDigits))
				prefixIfPolicyInvalid = DefaultDigitOnlyFirstChar;
			else throw new ArgumentException("Unable to determine default first variable character based on policy", nameof(firstCharacterPolicy));
		}

		var separator = style switch {
			TextCasing.PascalCase => string.Empty,
			TextCasing.CamelCase => string.Empty,
			TextCasing.SnakeCase => "_",
			TextCasing.KebabCase => "-",
			_ => throw new NotSupportedException(style.ToString())
		};

		text = Regex
			.Replace(text,
				// (Match any non punctuation) & then ignore any punctuation
				@"([^(\p{P}\s)]+)[\p{P}\s]*",
				match => {
					var word = match.Groups[1].Value;
					if (string.IsNullOrEmpty(word))
						return string.Empty;

					var firstChar = style switch {
						TextCasing.PascalCase => char.ToUpperInvariant(word[0]),
						TextCasing.CamelCase => char.ToLowerInvariant(word[0]),
						TextCasing.SnakeCase => char.ToUpperInvariant(word[0]),
						TextCasing.KebabCase => char.ToLowerInvariant(word[0]),
						_ => throw new NotSupportedException(style.ToString())
					};

					// Flip to PascalCase after first match in CamelCase
					if (style == TextCasing.CamelCase)
						style = TextCasing.PascalCase;

					var rest = word.Substring(1);
					if (!string.IsNullOrEmpty(rest)) {
						rest = style switch {
							TextCasing.PascalCase => PascalizeTail(firstChar, rest),
							TextCasing.CamelCase => PascalizeTail(firstChar, rest),
							TextCasing.SnakeCase => rest.ToUpperInvariant(),
							TextCasing.KebabCase => rest.ToLowerInvariant(),
							_ => throw new NotSupportedException(style.ToString())
						};
					}

					return $"{separator}{firstChar}{rest}";
				})
			.TrimStart(separator.ToCharArray());

		// apply first char policy
		if (text.Length > 0 && firstCharacterPolicy != FirstCharacterPolicy.Anything) {
			var firstLetter = text[0];
			var isAsciiLetter = IsAsciiLetter(firstLetter);
			var isDigit = char.IsDigit(firstLetter);
			var isUnderscore = firstLetter == '_';

			if ((!isUnderscore || !firstCharacterPolicy.HasFlag(FirstCharacterPolicy.AllowUnderscore)) &&
			    (!isAsciiLetter || !firstCharacterPolicy.HasFlag(FirstCharacterPolicy.AllowAsciiLetters)) &&
			    (!isDigit || !firstCharacterPolicy.HasFlag(FirstCharacterPolicy.AllowDigits)))
				text = prefixIfPolicyInvalid + text;
		}

		return text;

		string PascalizeTail(char startChar, string value) {
			// This processes the tail section of a string (same for Pascal or Camel case)
			// This starts as lower
			var sb = new StringBuilder();
			var inUpperCase = char.IsUpper(startChar);
			for (var i = 0; i < value.Length; i++) {
				var ch = value[i];
				switch (inUpperCase) {
					case false:
						if (char.IsUpper(ch))
							inUpperCase = true;
						else if (i == 0 && char.IsDigit(startChar) || i > 0 && char.IsDigit(value[i - 1]))
							ch = char.ToUpperInvariant(ch);

						sb.Append(ch);
						break;
					case true:
						if (char.IsLower(ch)) {
							inUpperCase = false;
						} else {
							ch = char.ToLowerInvariant(ch);
						}
						sb.Append(ch);
						break;
				}
			}
			return sb.ToString();
		}

	}

	public static bool IsAsciiLetter(char c) => AsciiLetterRegex.IsMatch(c.ToString());

	public static IEnumerable<string> FindSentencesWithText(string paragraph, string text) {
		/*
			From: https://stackoverflow.com/questions/27592440/extract-sentence-containing-a-word-from-text

			string text = @"Stack Overflow is a question and answer site for professional and enthusiast programmers. 
							It's built and run by you as part of the Stack Exchange network of Q&A sites.
							With your help, we're working together to build a library of detailed answers to 
							every question about programming.";

			Output: Stack Overflow is a question and answer site for professional and enthusiast programmers.

		 */
		var regex = new Regex($"[^.!?;]*({text})[^.?!;]*[.?!;]");

		var results = regex.Matches(paragraph);

		for (var i = 0; i < results.Count; i++)
			yield return results[i].Value.Trim();
	}


	public static IEnumerable<string> EnumerateSentences(string paragraph) {
		var regex = new Regex($"[^.!?;]*[.?!;]");

		var results = regex.Matches(paragraph);

		if (results.Count > 0) {
			for (var i = 0; i < results.Count; i++)
				yield return results[i].Value.Trim();
		} else {
			yield return paragraph;
		}

	}

	public static string FormatEx(string formatString, params object[] formatArgs)
		=> StringFormatter.FormatEx(formatString, formatArgs);

	public static string FormatEx(string formatString, Func<string, string> userTokenResolver, params object[] formatArgs)
		=> StringFormatter.FormatEx(formatString, userTokenResolver, true, formatArgs);

	public static string FormatWithDictionary(string formatString, IDictionary<string, string> userTokenResolver, bool recursive, params object[] formatArgs)
		=> FormatWithDictionary(formatString, userTokenResolver.ToDictionary(x => x.Key, x => x.Value as object), recursive, formatArgs);

	public static string FormatWithDictionary(string formatString, IDictionary<string, object> userTokenResolver, bool recursive, params object[] formatArgs)
		=> StringFormatter.FormatWithDictionary(formatString, userTokenResolver, recursive, formatArgs);

	public static bool HasNoLettersOrDigits(string value) {
		return value == null || value.All(c => !Char.IsLetterOrDigit(c));
	}


	/// <summary>
	/// Return a random string of the given length with the default character set of a-Z, 0-9
	/// </summary>
	public static string GenerateRandomString(int length) {
		return GenerateRandomString("abcdefghjkmnpqrtuvwxyzABCDEFGHJKMNPQRTUVWXYZ2346789", length);
	}

	/// <summary>
	/// Return a random string of the given length with the specified character set
	/// </summary>
	public static string GenerateRandomString(string characterSet, int length) {
		var random = Tools.Maths.RNG;
		StringBuilder sBuilder = new StringBuilder();
		for (int i = 0; i < length; i++)
			sBuilder.Append(characterSet[random.Next(0, characterSet.Length)]);

		return sBuilder.ToString();
	}

	public static string ToBase62(byte[] bytes) {
		return Base62Encoding.ToBase62String(bytes);
	}

	public static byte[] FromBase62(string text) {
		return Base62Encoding.FromBase62String(text);
	}

	public static string ToBase32(string text) {
		return Base32Encoding.Encode(text);
	}

	public static byte[] FromBase32(string text) {
		return Base32Encoding.Decode(text);
	}

	public static string AsciiToString(byte[] bytes) {
		StringBuilder sb = new StringBuilder(bytes.Length);
		foreach (byte b in bytes) {
			sb.Append(b <= 0x7f ? (char)b : '?');
		}
		return sb.ToString();
	}

	/// <summary>
	/// Converts a string to byte array
	/// </summary>
	/// <param name="input">The string</param>
	/// <returns>The byte array</returns>
	public static byte[] ConvertToByteArray(string input) {
		return input.Select(Convert.ToByte).ToArray();
	}

	/// <summary>
	/// Converts a byte array to a string
	/// </summary>
	/// <param name="bytes">the byte array</param>
	/// <returns>The string</returns>
	public static string ConvertToString(byte[] bytes) {
		return new string(bytes.Select(Convert.ToChar).ToArray());
	}

	public static byte[] CompressText(string text, string sharedSecret = null) {
		return CompressText<AesManaged>(text, sharedSecret);
	}

	public static string DecompressText(byte[] bytes, string sharedSecret = null) {
		return DecompressText<AesManaged>(bytes, sharedSecret);
	}

	public static byte[] CompressText<TSymmetricAlgorithm>(string text, string password = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC) where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		var hasPassword = !String.IsNullOrEmpty(password);
		Action<Stream, Stream> compressor = Streams.GZipCompress;
		Action<Stream, Stream> encryptor = (source, dest) => Streams.Encrypt<TSymmetricAlgorithm>(source, dest, password, null, paddingMode, cipherMode);
		Action<Stream, Stream> noop = (source, dest) => Streams.RouteStream(source, dest);
		using var sourceStream = new MemoryStream(ConvertToByteArray(text));
		using var destStream = new MemoryStream();
		using var streamPipeline = new StreamPipeline(compressor, hasPassword ? encryptor : noop);
		streamPipeline.Run(sourceStream, destStream);
		return destStream.ToArray();
	}

	public static string DecompressText<TSymmetricAlgorithm>(byte[] bytes, string password = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC) where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		var hasPassword = !String.IsNullOrEmpty(password);
		Action<Stream, Stream> decompressor = Streams.GZipDecompress;
		Action<Stream, Stream> decryptor = (source, dest) => Streams.Decrypt<TSymmetricAlgorithm>(source, dest, password, null, paddingMode, cipherMode);
		Action<Stream, Stream> noop = (source, dest) => Streams.RouteStream(source, dest);
		using var sourceStream = new MemoryStream(bytes);
		using var destStream = new MemoryStream();
		using var streamPipeline = new StreamPipeline(hasPassword ? decryptor : noop, decompressor);
		streamPipeline.Run(sourceStream, destStream);
		return ConvertToString(destStream.ToArray());
	}

}
