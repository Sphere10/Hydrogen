using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sphere10.Hydrogen.Presentation.Plugins {
    /// <summary>
    /// VelocityNET application plugin. VelocityNET client application will locate implementations of this
    /// interface and 
    /// </summary>
    public interface IPlugin {
        /// <summary>
        /// Gets the applications this plugin provides.
        /// </summary>
        IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Configure the service collection with this plugin's services.
        /// </summary>
        /// <param name="serviceCollection"> services</param>
        IServiceCollection ConfigureServices(IServiceCollection serviceCollection);
    }
}