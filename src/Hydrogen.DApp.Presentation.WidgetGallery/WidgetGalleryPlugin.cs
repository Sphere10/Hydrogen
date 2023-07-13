// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen.DApp.Presentation.Plugins;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Services;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Validators;

namespace Hydrogen.DApp.Presentation.WidgetGallery;

/// <summary>
/// Widget gallery plugin
/// </summary>
public class WidgetGalleryPlugin : Plugin {
	/// <summary>
	/// Initializes a new instance of the <see cref="WidgetGalleryPlugin"/> class.
	/// </summary>
	public WidgetGalleryPlugin() {
		Apps = new List<IApp> {
			new App("/widget-gallery",
				"Widget Gallery",
				"./img/boxes-solid.svg",
				new[] {
					new AppBlock("Widgets",
						"fa-cog",
						new[] {
							new AppBlockPage("/widget-gallery", "Home", "fa fa-commenting-o")
						}
					),
					new AppBlock("Modals",
						"fa-info",
						new[] {
							new AppBlockPage("/widget-gallery/modals",
								"Modals",
								"fa fa-commenting-o",
								new[] {
									new MenuItem("File",
										"",
										new List<MenuItem> {
											new("Modals", "/widget-gallery/modals", "fa-info")
										})
								})
						}
					),
					new AppBlock("Tables",
						"fa-table",
						new[] {
							new AppBlockPage("/widget-gallery/tables",
								"Tables",
								"fa-table",
								new[] {
									new MenuItem("Table demo", "/widget-gallery/paging-table")
								})
						}
					),
					new AppBlock("Wizards",
						"fa-magic",
						new[] {
							new AppBlockPage("/widget-gallery/wizards", "Demo wizard", "fa-magic")
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
	protected override void ConfigureServicesInternal(IServiceCollection serviceCollection) {
		serviceCollection.AddTransient<IRandomNumberService, RandomNumberService>();
		serviceCollection.AddTransient<IValidator<NewWidgetModel>, NewWidgetModelValidator>();
	}
}
