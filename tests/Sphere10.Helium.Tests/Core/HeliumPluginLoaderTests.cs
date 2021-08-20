using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework;
using Sphere10.Helium.PluginFramework;

namespace Sphere10.Helium.Tests.Core
{
    /// <summary>
    /// These are Integration/Black-box tests.
    /// Meaning: it tests the inputs and outputs of all the methods in the class.
    /// It DOES NOT test inside the workings of methods because it is NOT needed for this class.
    /// Plugin = Assembly(dll) = .Net Project with Handlers and Sagas.
    /// </summary>
    [TestFixture]
    public class HeliumPluginLoaderTests
    {
        private const string TestPlugin1 = @"Sphere10.Helium.Tests\bin\Debug\net5.0\Sphere10.Helium.TestPlugin1.dll";
        private const string TestPlugin2 = @"Sphere10.Helium.Tests\bin\Debug\net5.0\Sphere10.Helium.TestPlugin2.dll";
        private const string TestPlugin3 = @"Sphere10.Helium.Tests\bin\Debug\net5.0\Sphere10.Helium.TestPlugin3.dll";

        private readonly string[] _relativePathsForPlugins = new[] {
            TestPlugin1,
            TestPlugin2,
            TestPlugin3
        };

        private IHeliumPluginLoader _heliumPluginLoader;
        private const int TotalAmountOfHandlersInPlugins = 15; //Manually count all the handlers in all the Plugins.
        private int _totalAmountOfPluginsAtStartUp; //Shall be 3.

        [SetUp]
        public void SetupHeliumPluginLoader()
        {
            _totalAmountOfPluginsAtStartUp = _relativePathsForPlugins.Length;

            _heliumPluginLoader = new HeliumPluginLoader(new ConsoleLogger()); //Class under test
            _heliumPluginLoader.LoadPlugins(_relativePathsForPlugins);
        }

        [Test]
        public void AssembliesLoaded()
        {
            var totalAmountOfPluginsLoaded = _heliumPluginLoader.PluginAssemblyHandlerList
                .GroupBy(x => x.RelativePath)
                .Count();

            Debug.Assert(totalAmountOfPluginsLoaded == _totalAmountOfPluginsAtStartUp);
        }

        [Test]
        public void AllHandlersLoaded()
        {
            Debug.Assert(_heliumPluginLoader.PluginAssemblyHandlerList.Count == TotalAmountOfHandlersInPlugins);
        }

        [Test]
        public void AllPluginsEnabledByDefault()
        {
            Debug.Assert(GetAmountOfEnabledPlugins() == _totalAmountOfPluginsAtStartUp);
        }

        [Test]
        public void DisablePlugin()
        {
            _heliumPluginLoader.DisablePlugin(new[] { TestPlugin2 });

            Debug.Assert(GetAmountOfEnabledPlugins() == _totalAmountOfPluginsAtStartUp - 1);
        }

        [Test]
        public void EnablePlugin()
        {
            _heliumPluginLoader.EnablePlugin(new[] { TestPlugin2 });

            Debug.Assert(GetAmountOfEnabledPlugins() == _totalAmountOfPluginsAtStartUp);
        }

        [Test]
        public void EnableAllPlugins()
        {
            _heliumPluginLoader.DisableAllPlugins();
            _heliumPluginLoader.EnableAllPlugins();

            Debug.Assert(GetAmountOfEnabledPlugins() == _totalAmountOfPluginsAtStartUp);
        }

        [Test]
        public void DisableAllPlugins()
        {
            _heliumPluginLoader.EnableAllPlugins();
            _heliumPluginLoader.DisableAllPlugins();
            
            Debug.Assert(GetAmountOfEnabledPlugins() == 0);
        }

        [Test]
        public void GetAllEnabledPlugins()
        {
            _heliumPluginLoader.EnableAllPlugins();

            var enabledPlugins = _heliumPluginLoader.GetEnabledPlugins();

            Debug.Assert(GetAmountOfEnabledPlugins() == enabledPlugins.Length);
        }

        [Test]
        public void GetAllDisabledPlugins()
        {
            _heliumPluginLoader.EnableAllPlugins();

            var disabledPlugins = _heliumPluginLoader.GetDisabledPlugins();

            Debug.Assert(disabledPlugins.Length == 0);
        }

        private int GetAmountOfEnabledPlugins()
        {
            var totalAmountOfEnabledPlugins = _heliumPluginLoader.PluginAssemblyHandlerList
                .Where(x => x.IsEnabled)
                .GroupBy(y => y.RelativePath)
                .Count();

            return totalAmountOfEnabledPlugins;
        }
    }
}
