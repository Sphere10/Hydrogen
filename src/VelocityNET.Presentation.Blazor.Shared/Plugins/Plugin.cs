﻿using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    /// <summary>
    /// Plugin base
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        /// <summary>
        /// Gets the applications this plugin provides.
        /// </summary>
        public abstract IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Configure this plugin's services. Automatically configures view model services.
        /// </summary>
        /// <param name="serviceCollection"> services</param>
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddViewModelsFromAssembly(GetType().Assembly);
            ConfigureServicesInternal(serviceCollection);
        }

        /// <summary>
        /// Extension point for plugins to configure required services.
        /// </summary>
        /// <param name="serviceCollection"> service collection</param>
        protected abstract void ConfigureServicesInternal(IServiceCollection serviceCollection);
    }
}