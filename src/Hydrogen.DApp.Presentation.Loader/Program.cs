using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sphere10.Hydrogen.Presentation.Loader.Plugins;
using Sphere10.Hydrogen.Presentation.Loader.Services;
using Sphere10.Hydrogen.Presentation.Services;

namespace Sphere10.Hydrogen.Presentation.Loader
{

    public class Program
    {
        private static IConfiguration Configuration { get; set; } = null!;

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            Configuration = builder.Configuration;
            
            ConfigureServices(builder.Services);
            
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddViewModelsFromAssembly(typeof(Program).Assembly);

            serviceCollection.AddTransient(typeof(IWizardBuilder<>), typeof(DefaultWizardBuilder<>));
            serviceCollection.AddSingleton<IGenericEventAggregator, BasicGenericEventAggregator>();
            serviceCollection.AddSingleton<IModalService, ModalService>();
            serviceCollection.AddSingleton<INodeService, MockNodeService>();
            serviceCollection.AddSingleton<IEndpointManager, DefaultEndpointManager>();
            
            serviceCollection.AddBlazoredLocalStorage();

            serviceCollection.AddOptions();
            serviceCollection.Configure<DataSourceOptions>(Configuration.GetSection("DataSource"));
            
            InitializePlugins(serviceCollection);
        }

        /// <summary>
        /// Initializes plugin system.
        /// </summary>
        /// <param name="serviceCollection"> current service collection</param>
        private static void InitializePlugins(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IPluginLocator, StaticPluginLocator>();
            serviceCollection.AddSingleton<IAppManager, DefaultAppManager>();
            serviceCollection.AddSingleton<IPluginManager, DefaultPluginManager>();
            
            ServiceProvider provider = serviceCollection.BuildServiceProvider();
            IPluginManager manager = provider.GetRequiredService<IPluginManager>();
            
            manager.ConfigureServices(serviceCollection);
        }
    }
}