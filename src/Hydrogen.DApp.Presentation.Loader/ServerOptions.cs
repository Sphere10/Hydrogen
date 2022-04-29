using System;
using System.Collections.Generic;

namespace Sphere10.Hydrogen.Presentation.Loader
{
    /// <summary>
    /// Data source options
    /// </summary>
    public class DataSourceOptions
    {
        /// <summary>
        /// Gets or sets the configured servers.
        /// </summary>
        public List<Uri> Servers { get; } = new ();
    }
}