using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
{
    /// <summary>
    /// Finds available plugin types.
    /// </summary>
    public interface IPluginLocator
    {
        /// <summary>
        /// Locate plugins.
        /// </summary>
        /// <returns> <see cref="IPlugin"/> implementing plugin types.</returns>
        IEnumerable<Type> LocatePlugins();
    }
}