using System;
using System.Collections.Generic;

namespace VelocityNET.Presentation.Hydrogen.Plugins
{
/// <summary>
/// Menu item view model
/// </summary>
    public class MenuItem
    {
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
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="route"></param>
        /// <param name="children"></param>
        public MenuItem(string heading, string route, List<MenuItem> children)
        {
            Heading = heading ?? throw new ArgumentNullException(nameof(heading));
            Children = children ?? throw new ArgumentNullException(nameof(children));
            Route = route ?? throw new ArgumentNullException(nameof(route));
        }
    }
}