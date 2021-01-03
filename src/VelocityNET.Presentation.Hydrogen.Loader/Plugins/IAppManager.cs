using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader.Plugins
{
    /// <summary>
    /// App manager
    /// </summary>
    public interface IAppManager
    {
        /// <summary>
        /// Raised when an app is selected
        /// </summary>
        event EventHandler<AppSelectedEventArgs> AppSelected;
        
        /// <summary>
        /// Gets the available apps.
        /// </summary>
        IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Gets or sets the selected app.
        /// </summary>
        IApp? SelectedApp { get; }
    }
}