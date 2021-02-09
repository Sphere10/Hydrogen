using System;
using System.Collections.Generic;

namespace VelocityNET.Presentation.Hydrogen.Plugins {

    /// <summary>
    /// Menu item view model
    /// </summary>
    public class MenuItem {
        /// <summary>
        /// Gets the menu heading
        /// </summary>
        public string Heading { get; }

        /// <summary>
        /// Gets the child menu items.
        /// </summary>
        public List<MenuItem> Children { get; }

        /// <summary>
        /// Gets the route / path that this menu item should navigate to.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets the icon image path for this menu item.
        /// </summary>
        public string? IconPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="route"></param>
        /// <param name="children"></param>
        /// <param name="iconPath"></param>
        public MenuItem(string heading, string route, List<MenuItem> children, string? iconPath = null) {
            Heading = heading ?? throw new ArgumentNullException(nameof(heading));
            Children = children ?? throw new ArgumentNullException(nameof(children));
            Route = route ?? throw new ArgumentNullException(nameof(route));

            IconPath = iconPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="route"></param>
        /// <param name="iconPath"></param>
        public MenuItem(string heading, string route, string? iconPath = null) : this(heading, route, new List<MenuItem>(), iconPath) {
        }
    }

}