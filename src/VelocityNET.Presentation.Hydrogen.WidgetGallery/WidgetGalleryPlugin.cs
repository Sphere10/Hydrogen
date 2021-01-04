using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Services;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery
{

    /// <summary>
    /// Widget gallery plugin
    /// </summary>
    public class WidgetGalleryPlugin : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetGalleryPlugin"/> class.
        /// </summary>
        public WidgetGalleryPlugin()
        {
            Apps = new List<IApp>
            {
                new App("/widget-gallery", "Widget Gallery", "fa-cog",
                    new[]
                    {
                        new AppBlock("Widgets",
                            "fa-cog",
                            new[]
                            {
                                new AppBlockPage("/widget-gallery/", "Index", "fa-chart-area",
                                    Enumerable.Empty<MenuItem>()),
                                new AppBlockPage("/widget-gallery/data-tables", "Data Tables", "fa-chart-area", new[]
                                {
                                    new MenuItem("Data tables", "/widget-gallery/data-tables", new List<MenuItem>())
                                })
                            }
                        )
                    })
            };
        }

        /// <summary>
        /// Gets this plugin's apps.
        /// </summary>
        public override IEnumerable<IApp> Apps { get; }

        /// <summary>
        /// Configures plugin's services. These are services for the pages provided by
        /// the apps in this plugin.
        /// </summary>
        /// <param name="serviceCollection"> service collection</param>
        protected override void ConfigureServicesInternal(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRandomNumberService, RandomNumberService>();
        }
    }

}