// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hydrogen.Web.AspNetCore;

public static class HttpRequestExtensions {

	public static Uri GetUri(this HttpRequest request) {
		var uriBuilder = new UriBuilder {
			Scheme = request.Scheme,
			Host = request.Host.Host,
			Port = request.Host.Port.GetValueOrDefault(80),
			Path = request.Path.ToString(),
			Query = request.QueryString.ToString()
		};
		return uriBuilder.Uri;
	}

	public static string GetParameter(this HttpRequest request, Enum param) {
		return request.GetParameter<string>(param);
	}

	public static bool ContainsParameter(this HttpRequest request, Enum param) {
		PageParameterProcessor processor = new PageParameterProcessor(request);
		return processor.ContainsParameter(param);
	}

	public static T GetParameter<T>(this HttpRequest request, Enum param) {
		PageParameterProcessor processor = new PageParameterProcessor(request);
		return processor.GetParameter<T>(param);
	}

	public static async Task<string> ToStringAsync(this HttpRequest request) {
		var requestString = new StringBuilder();
		requestString.Append($"{request.Method} {request.Path}{request.QueryString} HTTP/{request.Protocol}\n");
		foreach (var header in request.Headers)
			requestString.Append($"{header.Key}: {header.Value}\n");
		requestString.Append("\n");
		if (!request.Body.CanRead)
			return requestString.ToString();
		// Leave the body stream open so it's still available for further processing.
		using var reader = new StreamReader(request.Body, leaveOpen: true);
		var body = await reader.ReadToEndAsync();
		// Reset the request body stream position so the next middleware can read it.
		requestString.Append(body);
		return requestString.ToString();
	}
}
