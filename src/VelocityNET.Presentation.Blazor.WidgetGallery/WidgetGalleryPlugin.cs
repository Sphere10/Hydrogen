using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.WidgetGallery
{

    /// <summary>
    /// Widget gallery plugin
    /// </summary>
    public class WidgetGalleryPlugin : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetGalleryPlugin"/> class.
        /// </summary>
        public WidgetGalleryPlugin()
        {
            Apps = new List<IApp>
            {
                new App("/widget-gallery", "Widget Gallery",
                    new[]
                    {
                        new AppBlock("Widgets", 
                            new[]
                        {
                            new AppBlockPage("/widget-gallery/entity-grid", "Entity Grid")
                        })
                    })
            };
        }

        /// <summary>
        /// Gets the applications this plugin provides.
        /// </summary>
        public IEnumerable<IApp> Apps { get; }
    }

}