using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
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
        /// Initializes the plugin manager.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the plugin service collection
        /// </summary>
        IServiceCollection ServiceCollection { get; }
    }
}