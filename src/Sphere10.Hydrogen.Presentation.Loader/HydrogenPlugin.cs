using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sphere10.Hydrogen.Presentation.Plugins;

namespace Sphere10.Hydrogen.Presentation.Loader
{

    public class HydrogenPlugin : Plugin
    {
        public override IEnumerable<IApp> Apps { get; } = new List<IApp>
        {
            new Sphere10.Hydrogen.Presentation.Plugins.App("/", "Hydrogen", "./img/heading-solid.svg",
                new[]
                {
                    new AppBlock("Hydrogen", "fa-link", new[]
                    {
                        new AppBlockPage("/", "Home", "fa-home", new List<MenuItem>
                        {
                            new("File", "#", new List<MenuItem>
                            {
                                new("Print", "#", "fa-print")
                            }, "fa-list")
                        }),
                        new AppBlockPage("/servers", "Servers", "fa-cogs")
                    })
                })
        };

        protected override void ConfigureServicesInternal(IServiceCollection serviceCollection)
        {
            serviceCollection.AddViewModelsFromAssembly(Assembly.Load("Sphere10.Hydrogen.Presentation"));
        }
    }

}