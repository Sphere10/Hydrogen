using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
{

    /// <summary>
    /// App manager
    /// </summary>
    public class DefaultAppManager : IAppManager, IDisposable
    {
        /// <summary>
        /// Gets the available apps.
        /// </summary>
        public IEnumerable<IApp> Apps { get; }
        
        /// <summary>
        /// Gets or sets the selected app.
        /// </summary>
        public IApp SelectedApp { get; private set; }

        private NavigationManager NavigationManager { get; }
        
        /// <summary>
        /// Gets the messenger.
        /// </summary>
        private IMessenger Messenger { get; }

        public DefaultAppManager(IPluginManager pluginManager, NavigationManager navigationManager, IMessenger messenger)
        {
            Apps = pluginManager.Plugins.SelectMany(x => x.Apps);
            NavigationManager = navigationManager;
            Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;

            SelectedApp = Apps.Single(x => x.Route == NavigationManager.ToBaseRelativePathWithSlash(NavigationManager.Uri));
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            string relativePath = NavigationManager.ToBaseRelativePathWithSlash(e.Location);
            
            SelectedApp = Apps.FirstOrDefault(x => x.Route.StartsWith(relativePath)) ??
                throw new InvalidOperationException($"No app found after navigating to {relativePath}");
        }

        public void SelectApp(string name)
        {
            if (SelectedApp.Name != name)
            {
                IApp app = Apps.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
                    throw new InvalidOperationException($"No app with name {name} found");
            
                NavigationManager.NavigateTo(app.Route);
                Messenger.Send(new GenericMessage<IApp>(app));
            }
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
        }
    }
}