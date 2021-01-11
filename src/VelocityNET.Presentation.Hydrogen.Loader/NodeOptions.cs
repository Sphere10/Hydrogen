using System.Collections.Generic;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Loader
{
    /// <summary>
    /// Data source options
    /// </summary>
    public class DataSourceOptions
    {
        /// <summary>
        /// Gets or sets the configured servers.
        /// </summary>
        public List<Server> Servers { get; } = new ();
    }
}