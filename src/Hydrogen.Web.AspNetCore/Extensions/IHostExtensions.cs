using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;
using System.Runtime.InteropServices;
using System;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace Microsoft.Extensions.Hosting;
public static class IHostExtensions {
	public static IHost StartHydrogenFramework(this IHost host, HydrogenFrameworkOptions options = HydrogenFrameworkOptions.Default) {
		HydrogenFramework.Instance.SetAspNetCoreHost(host);
		HydrogenFramework.Instance.StartFramework(host.Services, options);
		return host;
	}

}

