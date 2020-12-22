using System.Collections.Generic;
using System.Reflection;
using VelocityNET.Presentation.Blazor.WidgetsGallery;

namespace VelocityNET.Presentation.Blazor
{
    public class AppViewModel
    {
        public IEnumerable<Assembly> RoutingAssemblies { get; }

        public AppViewModel()
        {
            RoutingAssemblies = new List<Assembly>()
            {
               Assembly.GetAssembly(typeof(Index))
            };
        }
    }
}