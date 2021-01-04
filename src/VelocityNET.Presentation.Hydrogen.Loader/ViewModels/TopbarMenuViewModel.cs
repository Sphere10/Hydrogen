using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;
using VelocityNET.Presentation.Hydrogen.Plugins;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{
    /// <summary>
    /// View model for topbar menu
    /// </summary>
    public class TopbarMenuViewModel : ComponentViewModelBase
    {
        /// <summary>
        /// Gets the app manager
        /// </summary>
        private IAppManager AppManager { get; }
        
        /// <summary>
        /// Gets the default menu items
        /// </summary>
        private IEnumerable<MenuItem> DefaultMenuItems { get; }
        
        /// <summary>
        /// Gets the list of menu items.
        /// </summary>
        public List<MenuItem> MenuItems { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopbarMenuViewModel"/> class.
        /// </summary>
        /// <param name="appManager"> app manager</param>
        public TopbarMenuViewModel(IAppManager appManager)
        {
            AppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
            DefaultMenuItems = new MenuItem[]
            {
                new("File", "/", new List<MenuItem>()
                {
                    new("Open", "/",new List<MenuItem>()),
                    new("Print", "/", new List<MenuItem>())
                }),
                new("Help", "/", new List<MenuItem>())
            };

            MenuItems = new List<MenuItem>(DefaultMenuItems);
            
            AppManager.AppBlockPageSelected += AppManagerOnAppBlockPageSelected;
        }

        /// <summary>
        /// Handles selection of app, updates menu with new app's menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppManagerOnAppBlockPageSelected(object? sender, AppBlockPageSelectedEventArgs e)
        {
            IEnumerable<MenuItem> newItems = DefaultMenuItems.Merge(e.AppBlockPage.MenuItems);
            
            MenuItems.Clear();
            MenuItems.AddRange(newItems);
            StateHasChangedDelegate?.Invoke();
        }
    }
}