using System;
using Microsoft.Extensions.DependencyInjection;

namespace VelocityNET.Presentation.Blazor.Plugins
{
    /// <summary>
    /// Plugin service provider factory. 
    /// </summary>
    public class PluginServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        /// <summary>
        /// Gets the services loader.
        /// </summary>
        private IPluginServicesLoader ServicesLoader { get; } = new StaticPluginServicesLoader();

        /// <summary>Creates a container builder from an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.</summary>
        /// <param name="services">The collection of services</param>
        /// <returns>A container builder that can be used to create an <see cref="T:System.IServiceProvider" />.</returns>
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        /// <summary>Creates an <see cref="T:System.IServiceProvider" /> from the container builder.</summary>
        /// <param name="containerBuilder">The container builder</param>
        /// <returns>An <see cref="T:System.IServiceProvider" /></returns>
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return ServicesLoader.RegisterPluginTypes(containerBuilder)
                .BuildServiceProvider();
        }
    }
}