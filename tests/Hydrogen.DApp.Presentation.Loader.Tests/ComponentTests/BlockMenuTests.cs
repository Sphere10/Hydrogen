using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Loader.Components;
using Hydrogen.DApp.Presentation.Loader.Plugins;
using Hydrogen.DApp.Presentation.Loader.ViewModels;

namespace Hydrogen.DApp.Presentation.Loader.Tests.ComponentTests {

    public class Tests : Bunit.TestContext {
        [SetUp]
        public void Setup() {
            Services.AddTransient<BlockMenuViewModel>();
            Services.AddTransient<IAppManager, DefaultAppManager>();
            Services.AddTransient<IPluginManager, DefaultPluginManager>();
            Services.AddTransient<IPluginLocator, StaticPluginLocator>();

            Services.AddLogging();
        }

        [Test]
        public void BlockMenuRenders() {
            var component = RenderComponent<BlockMenu>();

            Assert.AreEqual(1, component.RenderCount);
            Assert.NotNull(component.Instance);
        }
    }
}