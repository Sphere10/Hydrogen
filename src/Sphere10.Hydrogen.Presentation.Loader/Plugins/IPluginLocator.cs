using System;
using System.Collections.Generic;
using Sphere10.Hydrogen.Presentation.Plugins;

namespace Sphere10.Hydrogen.Presentation.Loader.Plugins
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