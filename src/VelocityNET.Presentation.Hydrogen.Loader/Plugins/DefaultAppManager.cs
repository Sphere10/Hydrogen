using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader.Plugins
{
    /// <summary>
    /// App manager
    /// </summary>
    public class DefaultAppManager : IAppManager, IDisposable
    {
        /// <summary>
        /// Raised when an app is selected
        /// </summary>
        public event EventHandler<AppSelectedEventArgs>? AppSelected;

        /// <summary>
        /// Gets the available apps.
        /// </summary>
        public IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Gets or sets the selected app.
        /// </summary>
        public IApp? SelectedApp { get; private set; }

        /// <summary>
        /// Gets the navigation manager
        /// </summary>
        private NavigationManager NavigationManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppManager"/> class.
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="navigationManager"></param>
        public DefaultAppManager(IPluginManager pluginManager, NavigationManager navigationManager)
        {
            Apps = pluginManager.Plugins.SelectMany(x => x.Apps) ??
                throw new ArgumentNullException(nameof(navigationManager));
            NavigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));

            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;

            SelectedApp = Apps.SingleOrDefault(x => x.Route ==
                NavigationManager
                    .ToBaseRelativePathWithSlash(NavigationManager.Uri)
                    .ToAppPathFromBaseRelativePath());
        }

        /// <summary>
        /// Handles the location changed event from nav manager. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            string relativePath = NavigationManager.ToBaseRelativePathWithSlash(e.Location);
            string appSegment = relativePath.ToAppPathFromBaseRelativePath();

            SelectedApp = Apps.FirstOrDefault(x => x.Route.StartsWith(appSegment));

            if (SelectedApp is not null)
            {
                AppSelected?.Invoke(this, new AppSelectedEventArgs(SelectedApp));
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
            GC.SuppressFinalize(this);
        }
    }
}