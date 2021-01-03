using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader.Plugins
{
    /// <summary>
    /// Plugin loader
    /// </summary>
    public interface IPluginServicesLoader
    {
        /// <summary>
        /// Loads plugins and their services into given service collection.
        /// </summary>
        /// <param name="serviceCollection"> services collection to load plugin services into.</param>
        /// <returns> types implementing <see cref="IPlugin"/></returns>
        public IServiceCollection RegisterPluginTypes(IServiceCollection serviceCollection);
    }

}