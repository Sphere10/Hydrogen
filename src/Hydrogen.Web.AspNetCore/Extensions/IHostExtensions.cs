using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;

namespace Microsoft.Extensions.Hosting;
public static class IHostExtensions {
	public static IHost StartHydrogenFramework(this IHost host) {
		HydrogenFramework.Instance.SetAspNetCoreHost(host);
		HydrogenFramework.Instance.StartFramework(host.Services);
		return host;
	}

}

