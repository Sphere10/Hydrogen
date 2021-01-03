using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader.Plugins
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