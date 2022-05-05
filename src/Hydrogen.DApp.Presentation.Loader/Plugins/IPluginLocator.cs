using System;
using System.Collections.Generic;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins
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