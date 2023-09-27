// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Loader.Plugins;
using Hydrogen.DApp.Presentation.Loader.Tests.PluginManagerTests;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Tests.NavigationTests;

public class DefaultAppManagerTests {
	[Test]
	public void AppManagerLoadsPluginApps() {
		TestPlugin expected = new TestPlugin();

		IPluginLocator locator = new TestPluginLocator();
		IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
		IAppManager appManager = new DefaultAppManager(pluginManager, new TestNavigationManager());

		Assert.AreEqual(expected.Apps.Count(), appManager.Apps.Count());
	}

	[Test]
	public void AppManagerSelectsDefaultAppOrNone() {
		TestPlugin expected = new TestPlugin();

		IPluginLocator locator = new TestPluginLocator();
		IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
		IAppManager appManager = new DefaultAppManager(pluginManager, new TestNavigationManager());

		Assert.NotNull(appManager.SelectedApp);

		Assert.AreEqual(expected.Apps.First().Name, appManager.SelectedApp.Name);
	}

	[Test]
	public void AppManagerNoSelectedAppOnBadNav() {
		var nav = new TestNavigationManager();

		IPluginLocator locator = new TestPluginLocator();
		IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
		IAppManager appManager = new DefaultAppManager(pluginManager, nav);

		nav.NavigateTo(nav.Uri + "unknown");

		Assert.Null(appManager.SelectedApp);
	}

	[Test]
	public void NavToApp() {
		var nav = new TestNavigationManager();

		IPluginLocator locator = new TestPluginLocator();
		IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
		IAppManager appManager = new DefaultAppManager(pluginManager, nav);

		var app = appManager.Apps.First(x => x.Name != appManager.SelectedApp?.Name);

		nav.NavigateTo(app.Route);

		Assert.NotNull(appManager.SelectedApp);
		Assert.AreEqual(app.Name, appManager.SelectedApp.Name);
	}

	[Test]
	public void NavToAppPage() {
		var nav = new TestNavigationManager();

		IPluginLocator locator = new TestPluginLocator();
		IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
		IAppManager appManager = new DefaultAppManager(pluginManager, nav);

		IApp app = appManager.Apps.First(x => x.Name != appManager.SelectedApp?.Name);
		IAppBlockPage page = app.AppBlocks.First().AppBlockPages.First();

		nav.NavigateTo(page.Route);

		Assert.NotNull(appManager.SelectedApp);
		Assert.NotNull(appManager.SelectedPage);
		Assert.AreEqual(app.Name, appManager.SelectedApp.Name);
		Assert.AreEqual(page.Name, appManager.SelectedPage.Name);
	}
}
