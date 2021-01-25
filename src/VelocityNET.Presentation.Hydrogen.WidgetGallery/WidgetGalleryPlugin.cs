using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Services;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Validators;

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
                new App("/widget-gallery", "Widget Gallery", "./img/boxes-solid.svg",
                    new[]
                    {
                        new AppBlock("Widgets",
                            "fa-cog",
                            new[]
                            {
                                new AppBlockPage("/widget-gallery", "Home", "fa fa-commenting-o")
                            }
                        ),
                        new AppBlock("Modals",
                            "fa-cog",
                            new[]
                            {
                                new AppBlockPage("/widget-gallery/modals", "Modals", "fa fa-commenting-o", new[]
                                {
                                    new MenuItem("File", "", new List<MenuItem>
                                    {
                                        new ("Modals", "/widget-gallery/modals")
                                    })
                                })
                            }
                        ),
                        new AppBlock("Tables",
                            "fa-cog",
                            new[]
                            {
                                new AppBlockPage("/widget-gallery/paging-table", "Paging Table", "fa-table", new[]
                                {
                                    new MenuItem("Data tables", "/widget-gallery/paging-table")
                                })
                            }
                        ),new AppBlock("Wizards",
                            "fa-cog",
                            new[]
                            {
                                new AppBlockPage("/widget-gallery/wizards", "Wizard page", "fa-table")
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
            serviceCollection.AddTransient<IValidator<NewWidgetModel>, NewWidgetModelValidator>();
        }
    }
}