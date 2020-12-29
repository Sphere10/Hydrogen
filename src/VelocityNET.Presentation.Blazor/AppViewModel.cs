using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor
{

    public class AppViewModel
    {
        public IEnumerable<Assembly> RoutingAssemblies { get; }

        public AppViewModel(IEnumerable<IPlugin> plugins)
        {
            if (plugins == null)
            {
                throw new ArgumentNullException(nameof(plugins));
            }
            
            RoutingAssemblies = plugins.Select(x => x.GetType().Assembly)
                .Where(x => x.FullName != typeof(Program).Assembly.FullName)
                .Distinct();
        }
    }
}