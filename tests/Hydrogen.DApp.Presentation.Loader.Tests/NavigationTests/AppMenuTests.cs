﻿using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Loader.Plugins;
using Hydrogen.DApp.Presentation.Loader.Tests.PluginManagerTests;
using Hydrogen.DApp.Presentation.Loader.ViewModels;

namespace Hydrogen.DApp.Presentation.Loader.Tests.NavigationTests {

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

            Assert.AreSame(appManager.Apps, appsMenuViewModel.Apps);
            Assert.AreSame(appManager.SelectedApp, appsMenuViewModel.SelectedApp);
            Assert.AreSame(appManager.SelectedApp?.AppBlocks, blockMenuViewModel.AppBlocks);
        }
    }
}