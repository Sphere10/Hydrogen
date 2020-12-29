using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.ViewModels;

namespace VelocityNET.Presentation.Blazor
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("app");
            
            builder.Services.AddViewModelsFromAssembly(typeof(Program).Assembly);
            builder.ConfigureContainer(new PluginServiceProviderFactory());

            await builder.Build().RunAsync();
        }
    }

}