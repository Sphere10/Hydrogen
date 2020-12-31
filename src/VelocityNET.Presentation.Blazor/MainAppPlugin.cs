using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor
{

    public class MainAppPlugin : Plugin
    {
        public override IEnumerable<IApp> Apps { get; } = new List<IApp>
        {
            new VelocityNET.Presentation.Blazor.Shared.Plugins.App("/", "Home", "fa-home",
                new[]
                {
                    new AppBlock("Home", "fa-home", new[]
                    {
                        new AppBlockPage("/", "Index", "fa-home")
                    })
                },
                Enumerable.Empty<MenuItem>()
            )
        };

        protected override void ConfigureServicesInternal(IServiceCollection serviceCollection)
        {
        }
    }

}