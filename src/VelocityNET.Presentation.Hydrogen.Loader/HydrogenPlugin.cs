using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader
{

    public class HydrogenPlugin : Plugin
    {
        public override IEnumerable<IApp> Apps { get; } = new List<IApp>
        {
            new VelocityNET.Presentation.Hydrogen.Plugins.App("/", "Hydrogen", "./img/heading-solid.svg",
                new[]
                {
                    new AppBlock("Hydrogen", "fa-link", new []
                    {
                        new AppBlockPage("/", "Home", "fa-home"),
                        new AppBlockPage("/servers", "Servers", "fa-cogs",
                        Enumerable.Empty<MenuItem>())
                    })
                })
        };

        protected override void ConfigureServicesInternal(IServiceCollection serviceCollection)
        {
            serviceCollection.AddViewModelsFromAssembly(Assembly.Load("VelocityNET.Presentation.Hydrogen"));
        }
    }

}