using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor
{
    public class MainAppPlugin : Plugin
    {
        public override IEnumerable<IApp> Apps { get; } = new List<IApp>
        {
            new VelocityNET.Presentation.Blazor.Shared.Plugins.App("/", "Home", new List<IAppBlock>())
        };

        protected override void ConfigureServicesInternal(IServiceCollection serviceCollection)
        {
        }
    }
}