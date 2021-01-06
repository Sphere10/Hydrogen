using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader
{

    public class MainAppPlugin : Plugin
    {
        public override IEnumerable<IApp> Apps { get; } = new List<IApp>
        {
            new VelocityNET.Presentation.Hydrogen.Plugins.App("/", "Home", "fa-home",
                new[]
                {
                    new AppBlock("Home", "fa-home", new[]
                    {
                        new AppBlockPage("/", "Index", "fa-home",
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