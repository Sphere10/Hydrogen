using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared;

namespace VelocityNET.Presentation.Blazor
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            ConfigureServices(builder.Services);
            
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddViewModelsFromAssembly(typeof(Program).Assembly);
            serviceCollection.AddTransient<IPluginLocator, StaticPluginLocator>();
            serviceCollection.AddSingleton<IPluginManager, DefaultPluginManager>();
            
            InitializePlugins(serviceCollection);
        }

        /// <summary>
        /// Initializes plugin system.
        /// </summary>
        /// <param name="serviceCollection"> current service collection</param>
        private static void InitializePlugins(IServiceCollection serviceCollection)
        {
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            
            IPluginManager manager = provider.GetRequiredService<IPluginManager>();
            manager.Initialize();

            foreach (ServiceDescriptor serviceDescriptor in manager.ServiceCollection)
            {
                serviceCollection.Add(serviceDescriptor);
            }
        }
    }
}