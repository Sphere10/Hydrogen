using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.Plugins
{
    /// <summary>
    /// App manager
    /// </summary>
    public interface IAppManager
    {
        /// <summary>
        /// Gets the available apps.
        /// </summary>
        IEnumerable<IApp> Apps { get; }
        
        /// <summary>
        /// Select a new app by name, making this app active.
        /// </summary>
        /// <param name="name"> name of app to select</param>
        void SelectApp(string name);
    }
}