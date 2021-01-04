using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;
using VelocityNET.Presentation.Hydrogen.Loader.Tests.PluginManagerTests;

namespace VelocityNET.Presentation.Hydrogen.Loader.Tests.NavigationTests
{

    public class DefaultAppManagerTests
    {
        [Test]
        public void AppManagerLoadsPluginApps()
        {
            TestPlugin expected = new TestPlugin();

            IPluginLocator locator = new TestPluginLocator();
            IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
            IAppManager appManager = new DefaultAppManager(pluginManager, new TestNavigationManager());

            Assert.AreEqual(expected.Apps.Count(), appManager.Apps.Count());
        }

        [Test]
        public void AppManagerSelectsDefaultAppOrNone()
        {
            TestPlugin expected = new TestPlugin();

            IPluginLocator locator = new TestPluginLocator();
            IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
            IAppManager appManager = new DefaultAppManager(pluginManager, new TestNavigationManager());

            Assert.NotNull(appManager.SelectedApp);
            Assert.AreEqual(expected.Apps.First().Name, appManager.SelectedApp.Name);
        }

        [Test]
        public void AppManagerNoSelectedAppOnBadNav()
        {
            var nav = new TestNavigationManager();

            IPluginLocator locator = new TestPluginLocator();
            IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
            IAppManager appManager = new DefaultAppManager(pluginManager, nav);

            nav.NavigateTo(nav.Uri + "unknown");

            Assert.Null(appManager.SelectedApp);
        }

        [Test]
        public void NavToApp()
        {
            var nav = new TestNavigationManager();

            IPluginLocator locator = new TestPluginLocator();
            IPluginManager pluginManager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
            IAppManager appManager = new DefaultAppManager(pluginManager, nav);

            var app = appManager.Apps.First(x => x.Name != appManager.SelectedApp?.Name);

            nav.NavigateTo(app.Route);

            Assert.NotNull(appManager.SelectedApp);
            Assert.AreEqual(app.Name, appManager.SelectedApp.Name);
        }
    }
}