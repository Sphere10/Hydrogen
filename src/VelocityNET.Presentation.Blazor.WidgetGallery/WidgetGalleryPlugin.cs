using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.WidgetGallery.Widgets.Services;

namespace VelocityNET.Presentation.Blazor.WidgetGallery
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