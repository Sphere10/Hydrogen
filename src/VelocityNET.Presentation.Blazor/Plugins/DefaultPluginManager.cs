using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
{
    /// <summary>
    /// Default plugin managers
    /// </summary>
    public class DefaultPluginManager : IPluginManager
    {
        /// <summary>
        /// Gets the plugin locator
        /// </summary>
        private IPluginLocator PluginLocator { get; }
        
        /// <summary>
        /// Gets the navigation manager
        /// </summary>
        private NavigationManager NavigationManager { get; }

        /// <summary>
        /// Gets the available loaded plugins.
        /// </summary>
        public IEnumerable<IPlugin> Plugins { get; private set; } = new List<IPlugin>();

        /// <summary>
        /// Gets the plugin service collection.
        /// </summary>
        public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPluginManager"/> class.
        /// </summary>
        /// <param name="pluginLocator"> plugin locator</param>
        /// <param name="navigationManager"> navigation manager</param>
        public DefaultPluginManager(IPluginLocator pluginLocator, NavigationManager navigationManager)
        {
            PluginLocator = pluginLocator;
            NavigationManager = navigationManager;
        }

        public void Initialize()
        {
            IEnumerable<Type> types= PluginLocator.LocatePlugins();

            Plugins = types.Select(Activator.CreateInstance)
                .Cast<IPlugin>();
            
            foreach (IPlugin plugin in Plugins)
            {
                plugin.ConfigureServices(ServiceCollection);
            }
        }
    }
}