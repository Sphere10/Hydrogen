using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Plugins {
    public interface IMenuProvider {
        /// <summary>
        /// Gets this apps menu items.
        /// </summary>
        IEnumerable<MenuItem> MenuItems { get; }
    }
}