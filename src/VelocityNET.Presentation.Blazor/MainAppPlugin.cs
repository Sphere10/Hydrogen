using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor
{
    public class MainAppPlugin : IPlugin
    {
        public IEnumerable<IApp> Apps { get; } = new List<IApp>
        {
            new VelocityNET.Presentation.Blazor.Shared.Plugins.App("/", "Home", new List<IAppBlock>())
        };
    }
}