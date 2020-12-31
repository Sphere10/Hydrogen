using System;
using System.Collections.Generic;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    /// <summary>
    /// App - contains one or more app blocks.
    /// </summary>
    public class App : IApp
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="name"></param>
        /// <param name="appBlocks"></param>
        /// <param name="icon"></param>
        public App(string route, string name, string icon, IEnumerable<IAppBlock> appBlocks, IEnumerable<MenuItem> menuItems)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AppBlocks = appBlocks ?? throw new ArgumentNullException(nameof(appBlocks));
            MenuItems = menuItems;
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        }

        /// <summary>
        /// Gets the routable page url for this app
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets the name of the item, useful for displaying in menus or headings.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the app blocks that are part of this 
        /// </summary>
        public IEnumerable<IAppBlock> AppBlocks { get; }

        /// <summary>
        /// Gets this apps menu items.
        /// </summary>
        public IEnumerable<MenuItem> MenuItems { get; }

        /// <summary>
        /// Gets the icon font-awesome ccs classes for this app block.
        /// </summary>
        public string Icon { get; }
    }
}