using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Loader.Components;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;
using VelocityNET.Presentation.Hydrogen.Loader.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.Tests.ComponentTests {

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