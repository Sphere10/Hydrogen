using System;
using Hydrogen.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceProviderNamedRegistrationsExtensions {
		
	public static T GetNamedService<T>(this IServiceProvider servicerProvider, string name) where T : class {
		var namedLookup = servicerProvider.GetService<INamedLookup<T>>();
		return namedLookup?[name];
	}

}
