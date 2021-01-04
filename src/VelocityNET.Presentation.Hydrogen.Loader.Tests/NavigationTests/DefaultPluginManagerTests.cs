using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;
using VelocityNET.Presentation.Hydrogen.Loader.Tests.PluginManagerTests;

namespace VelocityNET.Presentation.Hydrogen.Loader.Tests.NavigationTests
{

    public class DefaultPluginManagerTests
    {
        [Test]
        public void PluginManagerLoadCorrectPlugins()
        {
            IPluginLocator locator = new TestPluginLocator();
            IPluginManager manager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());
            
            Assert.AreEqual(1, manager.Plugins.Count());
        }

        [Test]
        public void PluginManagerAddsPluginServices()
        {
            IPluginLocator locator = new TestPluginLocator();
            IPluginManager manager = new DefaultPluginManager(locator, new NullLogger<DefaultPluginManager>());

            var collection = new ServiceCollection();
            manager.ConfigureServices(collection);

            Assert.AreEqual(1, collection.Count);
        }
    }
}