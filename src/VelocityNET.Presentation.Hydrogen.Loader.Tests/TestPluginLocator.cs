using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;

// ReSharper disable once CheckNamespace
namespace VelocityNET.Presentation.Hydrogen.Loader.Tests.PluginManagerTests
{

    internal class TestPluginLocator : IPluginLocator
    {
        public IEnumerable<Type> LocatePlugins()
        {
            return new[] {typeof(TestPlugin)};
        }
    }

}