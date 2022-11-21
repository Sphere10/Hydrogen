using Microsoft.Extensions.Logging;

namespace Hydrogen.Web.AspNetCore;

public static class ILoggingBuilderExtensions {
	public static ILoggingBuilder AddHydrogenLogger(this ILoggingBuilder builder, Hydrogen.ILogger logger) {
		builder.Services.AddHydrogenLogger(logger); 
		return builder;
	}
}