// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

/// <summary>
/// App manager
/// </summary>
public class DefaultAppManager : IAppManager, IDisposable {
	/// <summary>
	/// Raised when an app is selected
	/// </summary>
	public event EventHandler<AppSelectedEventArgs>? AppSelected;

	/// <summary>
	/// Raised when an app block page is selected
	/// </summary>
	public event EventHandler<AppBlockPageSelectedEventArgs>? AppBlockPageSelected;

	/// <summary>
	/// Gets the available apps.
	/// </summary>
	public IEnumerable<IApp> Apps { get; }

	/// <summary>
	/// Gets or sets the selected app.
	/// </summary>
	public IApp? SelectedApp { get; private set; }

	/// <summary>
	/// Gets the selected app block page
	/// </summary>
	public IAppBlockPage? SelectedPage { get; private set; }

	/// <summary>
	/// Gets the navigation manager
	/// </summary>
	private NavigationManager NavigationManager { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DefaultAppManager"/> class.
	/// </summary>
	/// <param name="pluginManager"></param>
	/// <param name="navigationManager"></param>
	public DefaultAppManager(IPluginManager pluginManager, NavigationManager navigationManager) {
		Apps = pluginManager.Plugins.SelectMany(x => x.Apps) ??
		       throw new ArgumentNullException(nameof(navigationManager));
		NavigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));

		NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;

		SelectedApp = Apps.SingleOrDefault(x => x.Route ==
		                                        NavigationManager
			                                        .ToBaseRelativePathWithSlash(NavigationManager.Uri)
			                                        .ToAppPathFromBaseRelativePath());

		SelectedPage = SelectedApp?.AppBlocks
			.SelectMany(x => x.AppBlockPages)
			.SingleOrDefault(x => x.Route == NavigationManager.ToBaseRelativePathWithSlash(NavigationManager.Uri)
				.TrimQueryParameters());
	}

	/// <summary>
	/// Handles the location changed event from nav manager. 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e) {
		SelectedApp = Apps.SingleOrDefault(x => x.Route ==
		                                        NavigationManager
			                                        .ToBaseRelativePathWithSlash(NavigationManager.Uri)
			                                        .ToAppPathFromBaseRelativePath());

		SelectedPage = SelectedApp?.AppBlocks
			.SelectMany(x => x.AppBlockPages)
			.SingleOrDefault(x => x.Route == NavigationManager.ToBaseRelativePathWithSlash(NavigationManager.Uri)
				.TrimQueryParameters());

		if (SelectedApp is not null) {
			AppSelected?.Invoke(this, new AppSelectedEventArgs(SelectedApp));
		}

		if (SelectedPage is not null) {
			AppBlockPageSelected?.Invoke(this, new AppBlockPageSelectedEventArgs(SelectedPage));
		}
	}

	/// <summary>
	/// Dispose.
	/// </summary>
	public void Dispose() {
		NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
		GC.SuppressFinalize(this);
	}
}
