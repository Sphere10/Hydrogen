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
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {


    public static class Text {
        private static readonly char[] TokenTrimDelimitters;

        static Text() {
            TokenTrimDelimitters = new[] { '{', '}' };
        }

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

        public static string FormatEx(string formatString, params object[] formatArgs) {
            return FormatEx(formatString, DefaultTokenResolver, formatArgs);
        }

        public static string FormatEx(string formatString, Func<string, string> userTokenResolver, params object[] formatArgs) {
            if (formatString == null)
                throw new ArgumentNullException("formatString");

            var splits = new Stack<string>(formatString.SplitAndKeep(TokenTrimDelimitters, StringSplitOptions.RemoveEmptyEntries).Reverse());
            var resolver = userTokenResolver ?? DefaultTokenResolver;
            var resultBuiler = new StringBuilder();
            var currentFormatItemBuilder = new StringBuilder();
            var inFormatItem = false;
            while (splits.Count > 0) {
                var split = splits.Pop();
                switch (split) {
                    case "{":
                        if (splits.Count > 0 && splits.Peek() == "{") {
                            // Escaped {{
                            splits.Pop();
                            if (inFormatItem)
                                currentFormatItemBuilder.Append("{");
                            else
                                resultBuiler.Append("{");
                            continue;
                        }

                        if (inFormatItem) {
                            // illegal
                            throw new FormatException("Invalid format string");
                        }
                        inFormatItem = true;
                        break;
                    case "}":
                        if (inFormatItem) {
                            // end of format item, process and add to string
                            resultBuiler.Append(ResolveFormatItem(currentFormatItemBuilder.ToString(), resolver, formatArgs));
                            inFormatItem = false;
                            currentFormatItemBuilder.Clear();
                        } else if (splits.Count > 0 && splits.Peek() == "}") {
                            // Escaped }}
                            splits.Pop();
                            resultBuiler.Append("}");
                        } else {
                            // illegal format string
                            throw new FormatException("Incorrect format string");
                        }
                        break;
                    default:
                        if (inFormatItem)
                            currentFormatItemBuilder.Append(split);
                        else
                            resultBuiler.Append(split);
                        break;
                }
            }
            if (inFormatItem)
                throw new FormatException("Incorrect format string");

            return resultBuiler.ToString();
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

        private static string ResolveFormatItem(string token, Func<string, string> resolver, params object[] formatArgs) {
            int formatIndex;
            string formatOptions;
            if (IsStandardFormatIndex(token, out formatIndex, out formatOptions)) {
                if (formatIndex >= formatArgs.Length)
                    throw new ArgumentOutOfRangeException("formatArgs", String.Format("Insufficient format arguments"));

                return String.Format("{0" + (formatOptions ?? String.Empty) + "}", formatArgs[formatIndex]);
            }
            return resolver(token) ?? DefaultTokenResolver(token);
        }

        private static bool IsStandardFormatIndex(string token, out int number, out string formatOptions) {
            var numberString = new string(token.TakeWhile(Char.IsDigit).ToArray());
            if (numberString.Length > 0) {
                number = Int32.Parse(numberString);
                formatOptions = token.Substring(numberString.Length);
                return true;
            }
            number = 0;
            formatOptions = null;
            return false;
        }

        private static string DefaultTokenResolver(string token) {
            return token;
        }

    }
}
