using System;
using System.Collections.Generic;
using Hydrogen.DApp.Presentation.Plugins;
using Hydrogen.DApp.Presentation.WidgetGallery;

namespace Hydrogen.DApp.Presentation.Loader.Plugins
{

    /// <summary>
    /// Static plugin locator - knows the plugins and has direct references available.
    /// </summary>
    public class StaticPluginLocator : IPluginLocator
    {
        /// <summary>
        /// Locate plugins.
        /// </summary>
        /// <returns> <see cref="IPlugin"/> implementing plugin types.</returns>
        public IEnumerable<Type> LocatePlugins()
        {
            return new[] {typeof(HydrogenPlugin), typeof(WidgetGalleryPlugin)};
        }
    }
}