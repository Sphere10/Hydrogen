using System.Collections.Generic;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    /// <summary>
    /// VelocityNET application plugin. VelocityNET client application will locate implementations of this
    /// interface and 
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the applications this plugin provides.
        /// </summary>
        public IEnumerable<IApp> Apps { get; }
    }
}