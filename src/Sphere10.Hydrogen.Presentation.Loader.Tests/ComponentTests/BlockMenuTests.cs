using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Sphere10.Hydrogen.Presentation.Loader.Components;
using Sphere10.Hydrogen.Presentation.Loader.Plugins;
using Sphere10.Hydrogen.Presentation.Loader.ViewModels;

namespace Sphere10.Hydrogen.Presentation.Loader.Tests.ComponentTests {

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