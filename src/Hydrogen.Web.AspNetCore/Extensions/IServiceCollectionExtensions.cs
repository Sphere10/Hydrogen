using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hydrogen.Web.AspNetCore;

public static class IServiceCollectionExtensions {
	public static IServiceCollection AddHydrogenLogger(this IServiceCollection serviceCollection, Hydrogen.ILogger logger) {
		return serviceCollection.AddSingleton<ILoggerProvider>( _ => new HydrogenLoggingProvider(logger));
	}
}
