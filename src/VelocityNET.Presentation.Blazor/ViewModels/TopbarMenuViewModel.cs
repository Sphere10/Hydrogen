﻿using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.Shared.ViewModels;

namespace VelocityNET.Presentation.Blazor.ViewModels
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
            
            AppManager.AppSelected += AppManagerOnAppSelected;
        }

        /// <summary>
        /// Handles selection of app, updates menu with new app's menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppManagerOnAppSelected(object? sender, AppSelectedEventArgs e)
        {
            IEnumerable<MenuItem> newItems = DefaultMenuItems.Merge(e.SelectedApp.MenuItems);
            
            MenuItems.Clear();
            MenuItems.AddRange(newItems);
            StateHasChangedDelegate?.Invoke();
        }
    }
}