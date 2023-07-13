// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Web.AspNetCore;

// https://stackoverflow.com/questions/38571032/how-to-get-httpcontext-current-in-asp-net-core
public static class HttpContextEx {
	private static Microsoft.AspNetCore.Http.IHttpContextAccessor m_httpContextAccessor;

	public static void Configure(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor) {
		m_httpContextAccessor = httpContextAccessor;
	}

	public static Microsoft.AspNetCore.Http.HttpContext Current {
		get { return m_httpContextAccessor.HttpContext; }
	}
}
