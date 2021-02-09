using System;
using System.Collections.Generic;
using Sphere10.Hydrogen.Presentation.Loader.Plugins;

// ReSharper disable once CheckNamespace
namespace Sphere10.Hydrogen.Presentation.Loader.Tests.PluginManagerTests {

    internal class TestPluginLocator : IPluginLocator {
        public IEnumerable<Type> LocatePlugins() {
            return new[] { typeof(TestPlugin) };
        }
    }

}