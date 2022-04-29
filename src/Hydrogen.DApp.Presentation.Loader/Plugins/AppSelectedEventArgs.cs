using System;
using Sphere10.Hydrogen.Presentation.Plugins;

namespace Sphere10.Hydrogen.Presentation.Loader.Plugins
{
    /// <summary>
    /// Args for app selected event.
    /// </summary>
    public class AppSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the newly selected app.
        /// </summary>
        public IApp SelectedApp { get; }

        public AppSelectedEventArgs(IApp selectedApp)
        {
            SelectedApp = selectedApp;
        }
    }
}