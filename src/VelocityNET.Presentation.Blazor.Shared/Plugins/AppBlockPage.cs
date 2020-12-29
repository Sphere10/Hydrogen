using System;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{
    /// <summary>
    /// App block page.
    /// </summary>
    public class AppBlockPage : IAppBlockPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppBlockPage"/> class.
        /// </summary>
        /// <param name="route"> route - the relative path from app to navigate to.</param>
        /// <param name="name"> page name</param>
        public AppBlockPage(string route, string name)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the routable page url for this app
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Gets the name of the item, useful for displaying in menus or headings.
        /// </summary>
        public string Name { get; }
    }

}