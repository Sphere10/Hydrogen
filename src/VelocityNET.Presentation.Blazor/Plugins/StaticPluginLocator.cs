using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.WidgetGallery;

namespace VelocityNET.Presentation.Blazor.Plugins
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
            return new[] {typeof(MainAppPlugin), typeof(WidgetGalleryPlugin)};
        }
    }
}