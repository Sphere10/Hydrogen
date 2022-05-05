﻿namespace Hydrogen.DApp.Presentation.Plugins {

    public interface INamedItem {
        /// <summary>
        /// Gets the name of the item, useful for displaying in menus or headings.
        /// </summary>
        public string Name { get; }
    }

}