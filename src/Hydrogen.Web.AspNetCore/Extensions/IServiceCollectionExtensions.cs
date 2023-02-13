using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions {
	public static IServiceCollection AddHydrogenFramework(this IServiceCollection serviceCollection) {
		HydrogenFramework.Instance.RegisterModules(serviceCollection);
		return serviceCollection;
	}

	public static IServiceCollection AddHydrogenLogger(this IServiceCollection serviceCollection, Hydrogen.ILogger logger) {
		return serviceCollection.AddTransient<ILoggerProvider>( _ => new HydrogenLoggerProvider(logger));
	}

}
