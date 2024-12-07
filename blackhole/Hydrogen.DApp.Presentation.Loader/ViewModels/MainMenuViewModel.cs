// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Hydrogen.DApp.Presentation.Loader.Plugins;
using Hydrogen.DApp.Presentation.Plugins;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Loader.ViewModels;

/// <summary>
/// View model for topbar menu
/// </summary>
public class MainMenuViewModel : ComponentViewModelBase {
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
	/// Initializes a new instance of the <see cref="MainMenuViewModel"/> class.
	/// </summary>
	/// <param name="appManager"> app manager</param>
	public MainMenuViewModel(IAppManager appManager) {
		AppManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
		DefaultMenuItems = new MenuItem[] {
			new MenuItem("File", "/", iconPath: "fa-list"),
			new("Help", "/", new List<MenuItem>(), "fa-info")
		};

		MenuItems = new List<MenuItem>(DefaultMenuItems);

		AppManager.AppBlockPageSelected += AppManagerOnAppBlockPageSelected;

		if (AppManager.SelectedPage is not null) {
			IEnumerable<MenuItem> newItems = DefaultMenuItems.Merge(AppManager.SelectedPage.MenuItems);
			MenuItems.Clear();
			MenuItems.AddRange(newItems);
		}
	}

	/// <summary>
	/// Handles selection of app, updates menu with new app's menu items.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void AppManagerOnAppBlockPageSelected(object? sender, AppBlockPageSelectedEventArgs e) {
		IEnumerable<MenuItem> newItems = DefaultMenuItems.Merge(e.AppBlockPage.MenuItems);

		MenuItems.Clear();
		MenuItems.AddRange(newItems);
		StateHasChangedDelegate?.Invoke();
	}
}
