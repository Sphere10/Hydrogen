using System;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.WidgetGallery;

namespace VelocityNET.Presentation.Blazor.Plugins
{

    /// <summary>
    /// A static plugin services loader - knows which plugins by direct references are being loaded, and loads them
    /// into the services container.
    /// </summary>
    public class StaticPluginServicesLoader : PluginServicesLoaderBase
    {
        /// <summary>
        /// Loads plugins and their services into given service collection.
        /// </summary>
        /// <param name="serviceCollection"> services collection to load plugin services into.</param>
        /// <returns> types implementing <see cref="IPlugin"/></returns>
        public override IServiceCollection RegisterPluginTypes(IServiceCollection serviceCollection)
        {
            Type[] knownPlugins = {typeof(MainAppPlugin), typeof(WidgetGalleryPlugin)};

            foreach (var type in knownPlugins)
            {
                serviceCollection.AddSingleton(typeof(IPlugin), type);
                RegisterPluginServiceCollection(type.Assembly, serviceCollection);
            }

            return serviceCollection;
        }
    }
}