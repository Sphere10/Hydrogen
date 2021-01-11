using System;

namespace VelocityNET.Presentation.Hydrogen.Models
{
    public class Server
    {
        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        public string Name { get; set; } 

        /// <summary>
        /// Gets or sets the node URI.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets whether this is the default node.
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}