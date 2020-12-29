using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.WidgetGallery.Widgets
{
    /// <summary>
    /// Entity Grid
    /// </summary>
    public class EntityGrid : IAppBlockPage
    {
        /// <summary>
        /// Gets the routable page url for this app
        /// </summary>
        public string Route { get; } = "/widgets/entity-grid";

        /// <summary>
        /// Gets the name of the item, useful for displaying in menus or headings.
        /// </summary>
        public string Name { get; } = "Entity Grid";
    }
}