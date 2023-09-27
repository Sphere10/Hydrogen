// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;

namespace Microsoft.Extensions.Hosting;

public static class IHostExtensions {
	public static IHost StartHydrogenFramework(this IHost host, HydrogenFrameworkOptions options = HydrogenFrameworkOptions.Default) {
		HydrogenFramework.Instance.SetAspNetCoreHost(host);
		HydrogenFramework.Instance.StartFramework(host.Services, options);
		return host;
	}

}
