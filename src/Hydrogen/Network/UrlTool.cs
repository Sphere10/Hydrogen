//-----------------------------------------------------------------------
// <copyright file="UrlTool.cs" company="Sphere 10 Software">
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
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools {


	public static class Url {
		static readonly Regex WordDelimiters = new Regex(@"[\s��_]", RegexOptions.Compiled); // white space, em-dash, en-dash, underscore
		static readonly Regex InvalidChars = new Regex(@"[^a-z0-9\-]", RegexOptions.Compiled); // characters that are not valid
		static readonly Regex MultipleHyphens = new Regex(@"-{2,}", RegexOptions.Compiled); // multiple hyphens

		public static string ToUrlSlug(string value) {
			// convert to lower case
			value = value.ToLowerInvariant();

			// remove diacritics (accents)
			value = value.RemoveDiacritics();

			// ensure all word delimiters are hyphens
			value = WordDelimiters.Replace(value, "-");

			// strip out invalid characters
			value = InvalidChars.Replace(value, "");

			// replace multiple hyphens (-) with a single hyphen
			value = MultipleHyphens.Replace(value, "-");

			// trim hyphens (-) from ends
			return value.Trim('-');
		}

		public static string CodeNameForUrl(string s) {
			return s.ToCamelCase(false, true, '-');
		}

		public static string EncodeUrl(string s) {
			return EncodeUrl(s, null);
		}

		public static string EncodeUrl(string @string, char[] ignore) {
			var sb = new StringBuilder();
			var len = @string.Length;
			for (var i = 0; i < len; i++) {
				var c = @string[i];
				if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || (ignore != null && System.Array.IndexOf(ignore, c) != -1))
					sb.Append(c);
				else if (c == ' ')
					sb.Append('+');
				else 
					sb.AppendFormat("%{0:X2}", (int)c);
			}
			return sb.ToString();
		}

		public static string DecodeUrl(string @string) {
			StringBuilder result = new StringBuilder();
			int i, x = @string.Length;
			for (i = 0; i < x; i++) {
				if (@string[i] == '+')
					result.Append(' ');
				else if (@string[i] == '%' && (i + 1 < x && Char.IsDigit(@string[i + 1]))) {
					byte b = Convert.ToByte(@string.Substring(i + 1, 2), 16);
					byte[] bytes = new byte[1];
					bytes[0] = b;
					result.Append(Tools.Text.AsciiToString(bytes));
					i += 2;
				} else
					result.Append(@string[i]);
			}
			return result.ToString();

		}

		public static Dictionary<string, string> ParseQueryString(string queryString) {
			var result = new Dictionary<string, string>();
			var parts = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var s in parts) {
				string[] keyval = s.Split('=');
				if (keyval.Length != 2)
					throw new SoftwareException("Malfomed url '{0}'", queryString);
				result.Add(DecodeUrl(keyval[0]), DecodeUrl(keyval[1]));

			}
			return result;
		}

		public static string ExtractVideoIdFromYouTubeUrl(string url) {
			return ExtractVideoIdFromYouTubeUrl(url, true);
		}

		public static string ExtractVideoIdFromYouTubeUrl(string url, bool allowIdOnly) {
			Guard.ArgumentNotNullOrWhitespace(url, nameof(url));

			return url.GetRegexMatch(@"youtube.com/watch\?v=(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
				   url.GetRegexMatch(@"youtu.be/watch\?v=(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
				   url.GetRegexMatch(@"youtu.be/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
				   url.GetRegexMatch(@"youtube.com/embed/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
				   url.GetRegexMatch(@"youtube.com/v/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
				   (allowIdOnly ? url.GetRegexMatch(@"^(?<videoid>[a-zA-Z0-9_-]+)$", "videoid") : null);
		}

		public static string AppendQueryStringToUrl(string url, NameValueCollection queryParams) {
			if (url == null)
				url = String.Empty;

			string queryString = String.Join("&", System.Array.ConvertAll(queryParams.AllKeys, key =>
																						queryParams[key] == null
																							? Uri.EscapeUriString(key)
																							: String.Format("{0}={1}", Uri.EscapeUriString(key), Uri.EscapeUriString(queryParams[key]))));

			if (!String.IsNullOrEmpty(queryString))
				url = String.Format("{0}{1}{2}", url, (url.Contains("?") ? "&" : "?"), queryString);

			return url;
		}

		public static string Combine(string url1, string url2) {
			Guard.ArgumentNotNullOrWhitespace(url1, nameof(url1));
			Guard.ArgumentNotNullOrWhitespace(url2, nameof(url2));
			Guard.Argument(!url2.StartsWithAny(StringComparison.InvariantCultureIgnoreCase, "http:", "https:"), nameof(url2), "Cannot be appended since contains protocol");
			return url1.TrimEnd('/') + '/' + url2.TrimStart('/');
		}

		public static string ToQueryString(Dictionary<string, string> data) {
			StringBuilder sb = new StringBuilder();
			int pos = 0;
			foreach (string key in data.Keys) {
				if (pos > 0) sb.Append("&");
				sb.Append(EncodeUrl(key));
				sb.Append("=");
				sb.Append(EncodeUrl(data[key]));
				pos++;
			}
			return sb.ToString();
		}

		public static bool TryParse(string url, out string protocol, out int port, out string host, out string path, out string queryString) {
			var uri = new Uri(url);
			if (!uri.IsAbsoluteUri) {
				protocol = host = queryString = path = null;
				port = 0;
				return false;
			}
			protocol = uri.Scheme;
			port = uri.Port;
			host = uri.Host;
			path = uri.AbsolutePath;
			queryString = uri.Query;
			return true;
		}

		public static (string protocol, int port, string host, string path, string queryString) Parse(string url) {
			if (!TryParse(url, out var protocol, out var port, out var host, out var path, out var queryString))
				throw new InvalidOperationException($"Unable to parse '{url}'");
			return (protocol, port, host, path, queryString);
		}

	}
}
