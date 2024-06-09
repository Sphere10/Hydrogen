// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen;

namespace Tools.Web;

public static class Downloader {

	public static void DownloadFile(string url, string destPath, out string mimeType, bool verifySSLCert = true) {
		using var httpClientHandler = new HttpClientHandler();
		if (!verifySSLCert)
			httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
		using var client = new HttpClient(httpClientHandler);
		client.DefaultRequestHeaders.Clear();
		client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "none");
		var response = client.GetAsync(url).ResultSafe();
		mimeType = response.Content.Headers.ContentType.MediaType;
		var stream = response.Content.ReadAsStreamAsync().ResultSafe();
		using var fileStream = new FileStream(destPath, FileMode.OpenOrCreate);
		stream.CopyToAsync(fileStream);
	}

	public static async Task<string> DownloadFileAsync(string url, string destPath, bool verifySSLCert = true, CancellationToken cancellationToken = default) {
		using var httpClientHandler = new HttpClientHandler();
		if (!verifySSLCert)
			httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
		using var client = new HttpClient(httpClientHandler);
		client.DefaultRequestHeaders.Clear();
		client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "none");
		var response = await client.GetAsync(url, cancellationToken);
		var mimeType = response.Content.Headers.ContentType.MediaType;
		await using var stream = await response.Content.ReadAsStreamAsync().WithCancellationToken(cancellationToken);
		await using var fileStream = new FileStream(destPath, FileMode.OpenOrCreate);
		await stream.CopyToAsync(fileStream, cancellationToken);
		return mimeType;
	}

	public static bool TryParseFilenameFromUrl(string url, out string filename) {
		if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)) {
			filename = default;
			return false;
		}

		filename = Path.GetFileName(uri.LocalPath);
		return true;
	}

	public static string ParseFilenameFromUrl(string url) {
		if (!TryParseFilenameFromUrl(url, out var filename))
			throw new FormatException($"Url did not have a detectable filename. Url: {url}");
		return filename;
	}
}
