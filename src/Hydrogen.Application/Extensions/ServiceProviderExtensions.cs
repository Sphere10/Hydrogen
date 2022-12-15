using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application;

public static class ServiceProviderExtensions {
	public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T impl)  {
		var impls = serviceProvider.GetServices<T>().ToArray();
		impl = impls.LastOrDefault();
		return impls.Any();
	}

}
