using Hydrogen.Web.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class IHostBuilderExtensions {
	public static IHostBuilder UseHydrogenFramework(this IHostBuilder hostBuilder) 
		=> hostBuilder.ConfigureServices(x => x.AddHydrogenFramework());
	
}
