using System.Collections.Generic;
using System.Reflection;
using VelocityNET.Presentation.Blazor.WidgetsGallery.Pages;

namespace VelocityNET.Presentation.Blazor
{
    public class AppViewModel
    {
        public IEnumerable<Assembly> RoutingAssemblies { get; }

        public AppViewModel()
        {
            
            ///Find these based on namespaces
            RoutingAssemblies = new List<Assembly>
            {
               Assembly.GetAssembly(typeof(PagedGridExample))
            };
        }
    }
}