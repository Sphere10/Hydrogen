// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Hydrogen.Web.AspNetCore;

public static class UrlExtensions {
	public static string Content(this UrlHelper urlHelper, string contentPath, bool toAbsolute = false) {
		var path = urlHelper.Content(contentPath);
		var url = new Uri(HttpContextEx.Current.Request.GetUri(), path);

		return toAbsolute ? url.AbsoluteUri : path;
	}
}
