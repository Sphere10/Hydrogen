//-----------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="Sphere 10 Software">
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
using Microsoft.AspNetCore.Http;

namespace Sphere10.Framework.Web.AspNetCore {
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
	}

}
