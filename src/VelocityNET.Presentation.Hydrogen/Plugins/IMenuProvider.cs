using System.Collections.Generic;

namespace VelocityNET.Presentation.Hydrogen.Plugins {
    public interface IMenuProvider {
        /// <summary>
        /// Gets this apps menu items.
        /// </summary>
        IEnumerable<MenuItem> MenuItems { get; }
    }
}