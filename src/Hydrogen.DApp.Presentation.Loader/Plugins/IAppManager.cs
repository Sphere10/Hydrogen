using System;
using System.Collections.Generic;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins
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
        /// Raised when an app page is selected.
        /// </summary>
        event EventHandler<AppBlockPageSelectedEventArgs> AppBlockPageSelected;
        
        /// <summary>
        /// Gets the available apps.
        /// </summary>
        IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Gets or sets the selected app.
        /// </summary>
        IApp? SelectedApp { get; }

        /// <summary>
        /// Selected page
        /// </summary>
        IAppBlockPage? SelectedPage { get; }
    }
}