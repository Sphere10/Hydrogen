using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.JSInterop;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader
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