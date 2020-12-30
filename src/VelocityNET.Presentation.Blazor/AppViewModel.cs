using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor
{

    public class AppViewModel
    {
        public IEnumerable<Assembly> RoutingAssemblies { get; }

        public AppViewModel(IPluginLocator pluginLocator)
        {
            if (pluginLocator == null)
                throw new ArgumentNullException(nameof(pluginLocator));

            RoutingAssemblies = pluginLocator.LocatePlugins().Select(x => x.Assembly)
                .Where(x => x.FullName != typeof(Program).Assembly.FullName)
                .Distinct();
        }
    }

}