using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.ViewModels;
using VelocityNET.Presentation.Blazor.WidgetsGallery.ViewModels;

namespace VelocityNET.Presentation.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //Find and register these from assemblies with conventional namespace
            builder.Services.AddTransient<AppViewModel>();
            builder.Services.AddTransient<PagedGridExampleViewModel>();
            builder.Services.AddTransient<IndexViewModel>();

            await builder.Build().RunAsync();
        }
    }
}
