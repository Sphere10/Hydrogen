using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
{
    /// <inheritdoc />
    public abstract class PluginServicesLoaderBase : IPluginServicesLoader
    {
        /// <summary>
        /// Loads plugins and their services into given service collection.
        /// </summary>
        /// <param name="serviceCollection"> services collection to load plugin services into.</param>
        /// <returns> types implementing <see cref="IPlugin"/></returns>
        public abstract IServiceCollection RegisterPluginTypes(IServiceCollection serviceCollection);
        
        /// <summary>
        /// Given a plugin assembly, locates the <see cref="IPluginServiceCollection"/> implementations and
        /// loads their <see cref="ServiceDescriptor"/> records into <paramref name="serviceCollection"/>
        /// </summary>
        /// <param name="pluginAssembly"> plugin type</param>
        /// <param name="serviceCollection"> service collection to add plugin to.</param>
        /// <returns> populated service collection</returns>
        protected IServiceCollection RegisterPluginServiceCollection(Assembly pluginAssembly,
            IServiceCollection serviceCollection)
        {
            var types =
                pluginAssembly.ExportedTypes.Where(x => x.IsAssignableTo(typeof(IPluginServiceCollection)));

            try
            {
                IEnumerable<IPluginServiceCollection> pluginServiceCollections = types
                    .Select(Activator.CreateInstance)
                    .Cast<IPluginServiceCollection>();

                foreach (ServiceDescriptor serviceDescriptor in pluginServiceCollections.SelectMany(x => x))
                {
                    serviceCollection.Add(serviceDescriptor);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return serviceCollection;
        }
    }

}