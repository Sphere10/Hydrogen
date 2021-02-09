using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Sphere10.Hydrogen.Presentation.Loader.Plugins;
using Sphere10.Hydrogen.Presentation.Loader.Tests.PluginManagerTests;

namespace Sphere10.Hydrogen.Presentation.Loader.Tests.NavigationTests {

    public class DefaultPluginManagerTests {
        [Test]
        public void PluginManagerLoadCorrectPlugins() {
            IPluginLocator locator = new TestPluginLocator();
            IPluginManager manager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());

            Assert.AreEqual(1, manager.Plugins.Count());
        }

        [Test]
        public void PluginManagerAddsPluginServices() {
            IPluginLocator locator = new TestPluginLocator();
            IPluginManager manager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());

            var collection = new ServiceCollection();
            manager.ConfigureServices(collection);

            Assert.AreEqual(1, collection.Count);
        }
    }
}