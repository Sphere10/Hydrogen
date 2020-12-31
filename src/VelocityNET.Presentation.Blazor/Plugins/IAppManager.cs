using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
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
        IApp SelectedApp { get; }
    }
}