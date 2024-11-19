// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Url {
	static readonly Regex WordDelimiters = new Regex(@"[\s—–_]", RegexOptions.Compiled); // white space, em-dash, en-dash, underscore
	static readonly Regex InvalidChars = new Regex(@"[^a-z0-9\-]", RegexOptions.Compiled); // characters that are not valid
	static readonly Regex MultipleHyphens = new Regex(@"-{2,}", RegexOptions.Compiled); // multiple hyphens
	static readonly Regex AsciiLetters = new Regex(@"[a-zA-Z]", RegexOptions.Compiled); // characters that are not valid

	public static string StripAnchorTag(string url) {
		var ix = url.LastIndexOf('#');
		if (ix != -1) {
			url = url.Substring(0, ix);
		}
		return url;
	}

	public static IEnumerable<string> CalculateBreadcrumbFromPath(string urlPath) {
		var urlParts = Tools.Url.StripAnchorTag(urlPath.TrimStart("/")).Split('/').ToArray();
		for (var i = urlParts.Length; i > 0; i--) {
			yield return Tools.Url.Combine(urlParts.Take(i));
		}
	}

	public static string ToHtml4DOMObjectID(string text, string prefixIfRequired = "obj_") {
		var slug = ToUrlSlug(text);
		if (slug.Length == 0 || !AsciiLetters.IsMatch(slug[0].ToString()))
			slug = prefixIfRequired + slug;
		return slug;
	}

	public static string ToUrlSlug(string value) {
		// convert to lower case
		value = value.ToLowerInvariant();

		// remove diacritics (accents)
		value = value.RemoveDiacritics();

		// convert amperstands to n
		value = value.Replace('&', 'n');

		// remove single/double quotes
		value = value.Replace("'", string.Empty);
		value = value.Replace("\"", string.Empty);

		// ensure all word delimiters are hyphens
		value = WordDelimiters.Replace(value, "-");

		// strip out invalid characters
		value = InvalidChars.Replace(value, "");

		// replace multiple hyphens (-) with a single hyphen
		value = MultipleHyphens.Replace(value, "-");

		// trim hyphens (-) from ends
		return value.Trim('-');
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

	public static bool IsVideoSharingUrl(string url)
		=> IsVideoSharingUrl(url, out _, out _);

	public static bool IsVideoSharingUrl(string url, out VideoSharingPlatform platform, out string videoID) {
		if (TryParseYouTubeUrl(url, false, out videoID)) {
			platform = VideoSharingPlatform.YouTube;
			return true;
		}

		if (TryParseRumbleUrl(url, false, out videoID)) {
			platform = VideoSharingPlatform.Rumble;
			return true;
		}

		if (TryParseBitChuteUrl(url, false, out videoID)) {
			platform = VideoSharingPlatform.BitChute;
			return true;
		}			

		if (TryParseVimeoUrl(url, out videoID)) {
			platform = VideoSharingPlatform.Vimeo;
			return true;
		}			
		platform = 0;
		videoID = null;
		return false;
	}

	public static bool IsYouTubeUrl(string url)
		=> TryParseYouTubeUrl(url, out _);

	public static bool TryParseYouTubeUrl(string url, out string videoID)
		=> TryParseYouTubeUrl(url, true, out videoID);

	public static bool TryParseYouTubeUrl(string url, bool allowIdOnly, out string videoID) {
		Guard.ArgumentNotNullOrWhitespace(url, nameof(url));

		videoID = url.GetRegexMatch(@"youtube.com/watch\?v=(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
		          url.GetRegexMatch(@"youtu.be/watch\?v=(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
		          url.GetRegexMatch(@"youtu.be/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
		          url.GetRegexMatch(@"youtube.com/embed/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
		          url.GetRegexMatch(@"youtube.com/v/(?<videoid>[a-zA-Z0-9_-]+)", "videoid") ??
		          (allowIdOnly ? url.GetRegexMatch(@"^(?<videoid>[a-zA-Z0-9_-]+)$", "videoid") : null);

		return videoID != null;
	}

	public static bool IsRumbleUrl(string url)
		=> TryParseRumbleUrl(url, out _);

	public static bool TryParseRumbleUrl(string url, out string videoID) 
		=> TryParseRumbleUrl(url, true, out videoID);

	public static bool TryParseRumbleUrl(string url, bool allowIdOnly, out string videoID) {
		Guard.ArgumentNotNullOrWhitespace(url, nameof(url));
		videoID = url.GetRegexMatch(@"rumble.com/embed/(?<videoid>[a-zA-Z0-9]+)", "videoid") ??
		          url.GetRegexMatch(@"rumble.com/(?<videoid>v[a-zA-Z0-9]+)-", "videoid") ??
		          (allowIdOnly ? url.GetRegexMatch(@"^(?<videoid>[a-zA-Z0-9]+)$", "videoid") : null);

		return videoID != null;
	}

	public static bool IsVimeoUrl(string url)
		=> TryParseVimeoUrl(url, out _);

	public static bool TryParseVimeoUrl(string url, out string videoID) {
		Guard.ArgumentNotNullOrWhitespace(url, nameof(url));
		videoID = url.GetRegexMatch(@"player.vimeo.com/video/(?<videoid>[0-9_-]+)", "videoid");
		return videoID != null;
	}

	public static bool IsBitChuteUrl(string url)
		=> TryParseVimeoUrl(url, out _);

	public static bool TryParseBitChuteUrl(string url, out string videoID)
		=> TryParseBitChuteUrl(url, true, out videoID);

	public static bool TryParseBitChuteUrl(string url, bool allowIdOnly, out string videoID) {
		Guard.ArgumentNotNullOrWhitespace(url, nameof(url));

		videoID = url.GetRegexMatch(@"bitchute.com/video/(?<videoid>[a-zA-Z0-9]+)", "videoid") ??
		          url.GetRegexMatch(@"bitchute.com/embed/(?<videoid>[a-zA-Z0-9]+)", "videoid") ??
		          (allowIdOnly ? url.GetRegexMatch(@"^(?<videoid>[a-zA-Z0-9]+)$", "videoid") : null);

		return videoID != null;
	}

	public static string AppendQueryStringToUrl(string url, NameValueCollection queryParams) {
		if (url == null)
			url = String.Empty;

		string queryString = String.Join("&",
			System.Array.ConvertAll(queryParams.AllKeys,
				key =>
					queryParams[key] == null
						? Uri.EscapeUriString(key)
						: String.Format("{0}={1}", Uri.EscapeUriString(key), Uri.EscapeUriString(queryParams[key]))));

		if (!String.IsNullOrEmpty(queryString))
			url = String.Format("{0}{1}{2}", url, (url.Contains("?") ? "&" : "?"), queryString);

		return url;
	}

	public static string AddQueryString(string uri, string name, string value) {
		Guard.ArgumentNotNull(uri, nameof(uri));
		Guard.ArgumentNotNull(name, nameof(name));
		Guard.ArgumentNotNull(value, nameof(value));
		return AddQueryString(uri, new[] { new KeyValuePair<string, string>(name, value) });
	}

	public static string AddQueryString(string uri, IDictionary<string, string> queryParams) {
		Guard.ArgumentNotNull(uri, nameof(uri));
		Guard.ArgumentNotNull(queryParams, nameof(queryParams));
		return AddQueryString(uri, (IEnumerable<KeyValuePair<string, string>>)queryParams);
	}

	private static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, string>> queryParams) {
		Guard.ArgumentNotNull(uri, nameof(uri));
		Guard.ArgumentNotNull(queryParams, nameof(queryParams));

		queryParams = RemoveEmptyValueQueryParams(queryParams);

		var anchorIndex = uri.IndexOf('#');
		var uriToBeAppended = uri;
		var anchorText = "";

		if (anchorIndex != -1) {
			anchorText = uri.Substring(anchorIndex);
			uriToBeAppended = uri.Substring(0, anchorIndex);
		}

		var queryIndex = uriToBeAppended.IndexOf('?');
		var hasQuery = queryIndex != -1;

		var sb = new StringBuilder();
		sb.Append(uriToBeAppended);
		foreach (var parameter in queryParams) {
			sb.Append(hasQuery ? '&' : '?');
			sb.Append(WebUtility.UrlEncode(parameter.Key));
			sb.Append('=');
			sb.Append(WebUtility.UrlEncode(parameter.Value));
			hasQuery = true;
		}

		sb.Append(anchorText);
		return sb.ToString();
	}

	private static IEnumerable<KeyValuePair<string, string>> RemoveEmptyValueQueryParams(IEnumerable<KeyValuePair<string, string>> queryParams) {
		return queryParams.Where(x => !string.IsNullOrWhiteSpace(x.Value));
	}

	public static string Combine(string url1, string url2) {
		//Guard.ArgumentNotNullOrWhitespace(url1, nameof(url1));
		if (string.IsNullOrWhiteSpace(url2))
			return url1;
		Guard.Argument(!url2.StartsWithAny(StringComparison.InvariantCultureIgnoreCase, "http:", "https:"), nameof(url2), "Cannot be appended since contains protocol");
		return url1.TrimEnd('/') + '/' + url2.TrimStart('/');
	}

	public static string Combine(IEnumerable<string> urlParts) {
		var (head, tail) = urlParts.SplitTail();
		return Combine(head, tail.Select(x => x.Trim('/')).ToDelimittedString("/"));
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

	public static bool TryParse(string url, out string protocol, out int? port, out string host, out string path, out string queryString) {
		if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)) {
			protocol = host = path = queryString = null;
			port = -1;
			return false;
		}
		if (!uri.IsAbsoluteUri) {
			protocol = host = queryString = path = null;
			port = 0;
			return false;
		}
		
		protocol = uri.Scheme;
		port = SpecifiedPortExplicitly() ? uri.Port : null;
		host = uri.Host;
		path = uri.AbsolutePath;
		queryString = uri.Query;
		return true;

		bool SpecifiedPortExplicitly() {
			var hostStart = url.IndexOf(uri.Host, StringComparison.Ordinal);
			var portColonStart = hostStart + uri.Host.Length;
			if (portColonStart < url.Length)
				return url[portColonStart] == ':';
			return false;
		}
	}

	public static (string protocol, int? port, string host, string path, string queryString) Parse(string url) {
		if (!TryParse(url, out var protocol, out var port, out var host, out var path, out var queryString))
			throw new InvalidOperationException($"Unable to parse '{url}'");
		return (protocol, port, host, path, queryString);
	}

}
