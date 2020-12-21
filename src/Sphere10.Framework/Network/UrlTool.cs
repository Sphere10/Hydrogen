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
using System.Linq;
using System.Text;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {
		public static class Url {

			public static string CodeNameForUrl(string s) {
				return s.ToCamelCase(false, true, '-');
			}

			public static string EncodeUrlData(string s) {
				return EncodeUrl(s, null);
			}
			public static string DecodeUrlData(string @string) {
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

			public static string EncodeUrl(string @string, char[] ignore) {
				StringBuilder sb = new StringBuilder();
				int i, x = @string.Length;
				char c;
				for (i = 0; i < x; i++) {
					c = @string[i];
					if (Char.IsLetterOrDigit(c) || c == '_' || c == '-' || (ignore != null && System.Array.IndexOf(ignore, c) != -1))
						sb.Append(c);
					else if (c == ' ')
						sb.Append('+');
					else {
						//string conv = c.ToString();
						//byte[] b = Encoding.ASCII.GetBytes(conv);
						sb.AppendFormat("%{0:X2}", (int)c);
					}
				}

				return sb.ToString();
			}

			public static Dictionary<string, string> ParseQueryString(string queryString) {
                var result = new Dictionary<string, string>();
				var parts = queryString.Split(new [] {'&'}, StringSplitOptions.RemoveEmptyEntries);
				foreach (var s in parts) {
					string[] keyval = s.Split('=');
					if (keyval.Length != 2)
						throw new SoftwareException("Malfomed url '{0}'", queryString);
					result.Add(DecodeUrlData(keyval[0]), DecodeUrlData(keyval[1]));

				}
				return result;
			}

			public static string ExtractVideoIdFromYouTubeUrl(string url) {
				return ExtractVideoIdFromYouTubeUrl(url, true);
			}

			public static string ExtractVideoIdFromYouTubeUrl(string url, bool allowIdOnly) {
				if (String.IsNullOrEmpty(url)) {
					return null;
				}

				return url.GetRegexMatch(@"youtube.com/watch\?v=(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
					   url.GetRegexMatch(@"youtu.be/watch\?v=(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
					   url.GetRegexMatch(@"youtu.be/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
					   url.GetRegexMatch(@"youtube.com/embed/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
					   url.GetRegexMatch(@"youtube.com/v/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
					   (allowIdOnly ? url.GetRegexMatch(@"^(?<videoid>[a-zA-Z0-9_-]+)$", "videoid") : null);
			}

#if !__WP8__
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
#endif

			public static string UrlCombine(string url1, string url2) {
				if (url2.StartsWith("http:") || url2.StartsWith("https:"))
					return url2;

				return url1.TrimEnd('/') + '/' + url2.TrimStart('/');
			}


            public static string ConvertToQueryString(Dictionary<string, string> data)
            {
				StringBuilder sb = new StringBuilder();
				int pos = 0;
				foreach (string key in data.Keys) {
					if (pos > 0) sb.Append("&");
					sb.Append(EncodeUrlData(key));
					sb.Append("=");
					sb.Append(EncodeUrlData(data[key]));
					pos++;
				}
				return sb.ToString();
			}

		}
	}

