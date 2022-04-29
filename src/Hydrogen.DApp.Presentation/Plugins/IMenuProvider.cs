using System.Collections.Generic;

namespace Sphere10.Hydrogen.Presentation.Plugins {
    public interface IMenuProvider {
        /// <summary>
        /// Gets this apps menu items.
        /// </summary>
        IEnumerable<MenuItem> MenuItems { get; }
    }
}