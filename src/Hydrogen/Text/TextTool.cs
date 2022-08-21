//-----------------------------------------------------------------------
// <copyright file="TextTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Hydrogen;
using static System.Net.Mime.MediaTypeNames;

// ReSharper disable CheckNamespace
namespace Tools {


	public static class Text {

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
            => StringFormatter.FormatEx(formatString, userTokenResolver, formatArgs);

		public static string FormatWithDictionary(string formatString, IDictionary<string, object> userTokenResolver, bool recursive, params object[] formatArgs)
			=> StringFormatter.FormatWithDictionary(formatString, userTokenResolver, recursive, formatArgs);

        public static bool IsValidHexString(IEnumerable<char> hexString) {
            return !hexString.Any(c => !(c >= '0' && c <= '9') && !(c >= 'a' && c <= 'f') && !(c >= 'A' && c <= 'F'));
        }

        public static bool HasNoLettersOrDigits(string value) {
            return value == null || value.All(c => !Char.IsLetterOrDigit(c));
        }

        public static bool TryNewRegex(string pattern, out Regex regex)
            => TryNewRegex(pattern, RegexOptions.None, 30, out regex);

		public static bool TryNewRegex(string pattern, RegexOptions options, int timeoutSec, out Regex regex) {
            regex = null;	
            try {
                regex = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(timeoutSec));
            } catch {
				return false;
			}
            return true;
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
            return Base62Converter.ToBase62String(bytes);
        }

        public static byte[] FromBase62(string text) {
            return Base62Converter.FromBase62String(text);
        }

        public static string ToBase32(string text) {
            return Base32Converter.ToBase32String(text);
        }

        public static byte[] FromBase32(string text) {
            return Base32Converter.FromBase32String(text);
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
            using (var sourceStream = new MemoryStream(ConvertToByteArray(text)))
            using (var destStream = new MemoryStream())
            using (var streamPipeline = new StreamPipeline(compressor, hasPassword ? encryptor : noop)) {
                streamPipeline.Run(sourceStream, destStream);
                return destStream.ToArray();
            }
        }

        public static string DecompressText<TSymmetricAlgorithm>(byte[] bytes, string password = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC) where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
            var hasPassword = !String.IsNullOrEmpty(password);
            Action<Stream, Stream> decryptor = (source, dest) => Streams.Decrypt<TSymmetricAlgorithm>(source, dest, password, null, paddingMode, cipherMode);
            Action<Stream, Stream> decompressor = Streams.GZipDecompress;
            Action<Stream, Stream> noop = (source, dest) => Streams.RouteStream(source, dest);
            using (var sourceStream = new MemoryStream(bytes))
            using (var destStream = new MemoryStream())
            using (var streamPipeline = new StreamPipeline(hasPassword ? decryptor : noop, decompressor)) {
                streamPipeline.Run(sourceStream, destStream);
                return ConvertToString(destStream.ToArray());
            }
        }

    }
}
