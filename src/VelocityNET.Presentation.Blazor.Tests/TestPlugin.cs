using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Tests
{
    /// <summary>
    /// Test plugin.
    /// </summary>
    public class TestPlugin : IPlugin
    {
        /// <summary>
        /// Gets the name of the item, useful for displaying in menus or headings.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the applications this plugin provides.
        /// </summary>
        public IEnumerable<IApp> Apps { get; } = new List<IApp>();
    }

}