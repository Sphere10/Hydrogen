using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.WidgetsGallery;

namespace VelocityNET.Presentation.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddTransient<AppViewModel>();
            builder.Services.AddTransient<MainLayoutViewModel>();
            builder.Services.AddTransient<NavMenuViewModel>();

            builder.Services.AddTransient<IndexViewModel>();

            await builder.Build().RunAsync();
        }
    }
}
