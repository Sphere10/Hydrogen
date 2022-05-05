using System;
using System.Collections.Generic;
using Hydrogen.DApp.Presentation.Loader.Plugins;

// ReSharper disable once CheckNamespace
namespace Hydrogen.DApp.Presentation.Loader.Tests.PluginManagerTests {

    internal class TestPluginLocator : IPluginLocator {
        public IEnumerable<Type> LocatePlugins() {
            return new[] { typeof(TestPlugin) };
        }
    }

}