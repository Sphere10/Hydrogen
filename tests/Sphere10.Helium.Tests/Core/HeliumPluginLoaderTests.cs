using System.Diagnostics;
using NUnit.Framework;
using Sphere10.Framework;
using Sphere10.Helium.PluginFramework;

namespace Sphere10.Helium.Tests.Core
{
    /// <summary>
    /// These are Integration/Black-box tests.
    /// Meaning: it tests the inputs and outputs of all the methods in the class.
    /// It DOES NOT test inside the workings of methods because it is NOT needed for this class.
    /// </summary>
    [TestFixture]
    public class HeliumPluginLoaderTests
    {
        private IHeliumPluginLoader _heliumPluginLoader;

        [SetUp]
        public void SetupHeliumPluginLoader()
        {
            var relativePathsForPlugins = new[] {
                @"Sphere10.Helium.Tests\bin\Debug\net5.0\Sphere10.Helium.BlueService.dll"//,
                //@"Sphere10.Helium.Usage\bin\Debug\net5.0\Sphere10.Helium.Usage.dll",
                //@"Sphere10.Helium.TestPlugin1\bin\Debug\net5.0\Sphere10.Helium.TestPlugin1.dll",
                //@"Sphere10.Helium.TestPlugin2\bin\Debug\net5.0\Sphere10.Helium.TestPlugin2.dll",
                //@"Sphere10.Helium.TestPlugin3\bin\Debug\net5.0\Sphere10.Helium.TestPlugin3.dll"
            };

            _heliumPluginLoader = new HeliumPluginLoader(new ConsoleLogger());
            _heliumPluginLoader.LoadPlugins(relativePathsForPlugins);
        }

        [Test]
        public void AssembliesLoaded()
        {
            Debug.Assert(_heliumPluginLoader.PluginAssemblyHandlerList.Count == 5);
        }
    }
}
