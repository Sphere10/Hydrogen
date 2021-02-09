using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sphere10.Hydrogen.Presentation.Plugins;

namespace Sphere10.Hydrogen.Presentation.Loader.Plugins
{
    /// <summary>
    /// Manages plugins.
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Gets the currently available plugins
        /// </summary>
        IEnumerable<IPlugin> Plugins { get; }

        /// <summary>
        /// Configures the service collection with services from plugins.
        /// </summary>
        IServiceCollection ConfigureServices(IServiceCollection serviceCollection);
    }
}