// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Loader.Plugins;
using Hydrogen.DApp.Presentation.Loader.Tests.PluginManagerTests;
using Hydrogen.DApp.Presentation.Loader.ViewModels;
using NUnit.Framework.Legacy;

namespace Hydrogen.DApp.Presentation.Loader.Tests.NavigationTests;

public class AppMenuTests {
	[Test]
	public void AppMenuInitializedWithApps() {
		IPluginLocator locator = new TestPluginLocator();
		IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
		var navigationManager = new TestNavigationManager();
		IAppManager appManager = new DefaultAppManager(pluginManager, navigationManager);
		AppsMenuViewModel appsMenuViewModel = new AppsMenuViewModel(appManager);
		BlockMenuViewModel blockMenuViewModel = new BlockMenuViewModel(appManager);

		navigationManager.NavigateTo("/");

		ClassicAssert.AreSame(appManager.Apps, appsMenuViewModel.Apps);
		ClassicAssert.AreSame(appManager.SelectedApp, appsMenuViewModel.SelectedApp);
		ClassicAssert.AreSame(appManager.SelectedApp?.AppBlocks, blockMenuViewModel.AppBlocks);
	}
}
