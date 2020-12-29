using NUnit.Framework;
using VelocityNET.Presentation.Blazor.Plugins;

namespace VelocityNET.Presentation.Blazor.Tests.PluginTests
{

    public class PluginLoaderTests
    {
        [Test]
        public void LoadTestPlugin()
        {
            IPluginServicesLoader servicesLoader = new StaticPluginServicesLoader();
            // IEnumerable<Type> plugins = servicesLoader.GetPluginTypesFromAssembly(typeof(TestPlugin).Assembly);
            // Assert.AreEqual(typeof(TestPlugin),plugins.Single());
        }
    }

}