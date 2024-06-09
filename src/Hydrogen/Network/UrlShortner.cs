// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Hydrogen;

public static class UrlShortner {
	public static async Task<string> TinyUrlAsync(string url, string apiKey, string provider = "0_mk") {
		//string yourUrl = "http://your-site.com/your-url-for-minification";
		//string apikey = "YOUR-API-KEY-GOES-HERE";
		//string provider = "0_mk"; // see provider strings list in API docs
		var uriString = string.Format(
			"http://tiny-url.info/api/v1/create?url={0}&apikey={1}&provider={2}&format=text",
			url,
			apiKey,
			provider);

		var address = new Uri(uriString);
		var client = new System.Net.WebClient();
		return await client.DownloadStringTaskAsync(address);
	}
	public static string TinyUrl(string url, string apiKey, string provider = "0_mk") {
		//string yourUrl = "http://your-site.com/your-url-for-minification";
		//string apikey = "YOUR-API-KEY-GOES-HERE";
		//string provider = "0_mk"; // see provider strings list in API docs
		var uriString = string.Format(
			"http://tiny-url.info/api/v1/create?url={0}&apikey={1}&provider={2}&format=text",
			url,
			apiKey,
			provider);

		var address = new Uri(uriString);
		var client = new System.Net.WebClient();
		return client.DownloadString(address);
	}
	public static async Task<string> GoogleAsync(string url, string apiKey) {
		var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + apiKey);
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.Method = "POST";

		using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync())) {
			var json = "{\"longUrl\":\"" + url + "\",\"key\":\"" + apiKey + "\"}";
			await streamWriter.WriteAsync(json);
		}
		string responseJson = null;
		var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
		using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
			responseJson = await streamReader.ReadToEndAsync();
		}

		/* {
			 "kind": "urlshortener#url",
			 "id": "https://goo.gl/Akn82b",
			 "longUrl": "https://sphere10.com/"
			} */

		// Was in a rush TODO: parse nicer
		return
			responseJson
				.Replace("\n", string.Empty)
				.Replace("\r", string.Empty)
				.Replace("\t", string.Empty)
				.Replace(" ", string.Empty)
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1]
				.Replace("https:", "https^")
				.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]
				.Replace("https^", "https:")
				.Replace("\"", string.Empty)
				.Trim();

	}
	public static string Google(string url, string apiKey) {
		var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + apiKey);
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.Method = "POST";

		using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
			var json = "{\"longUrl\":\"" + url + "\",\"key\":\"" + apiKey + "\"}";
			streamWriter.Write(json);
		}
		string responseJson = null;
		var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
			responseJson = streamReader.ReadToEnd();
		}

		/* {
			 "kind": "urlshortener#url",
			 "id": "https://goo.gl/Akn82b",
			 "longUrl": "https://sphere10.com/"
			} */

		// Was in a rush TODO: parse nicer
		return
			responseJson
				.Replace("\n", string.Empty)
				.Replace("\r", string.Empty)
				.Replace("\t", string.Empty)
				.Replace(" ", string.Empty)
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1]
				.Replace("https:", "https^")
				.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]
				.Replace("https^", "https:")
				.Replace("\"", string.Empty)
				.Trim();
	}
}
