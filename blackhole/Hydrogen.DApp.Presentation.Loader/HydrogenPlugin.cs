// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader;

public class HydrogenPlugin : Plugin {
	public override IEnumerable<IApp> Apps { get; } = new List<IApp> {
		new Hydrogen.DApp.Presentation.Plugins.App("/",
			"Hydrogen",
			"./img/heading-solid.svg",
			new[] {
				new AppBlock("Hydrogen",
					"fa-link",
					new[] {
						new AppBlockPage("/",
							"Home",
							"fa-home",
							new List<MenuItem> {
								new("File",
									"#",
									new List<MenuItem> {
										new("Print", "#", "fa-print")
									},
									"fa-list")
							}),
						new AppBlockPage("/servers", "Servers", "fa-cogs")
					})
			})
	};

	protected override void ConfigureServicesInternal(IServiceCollection serviceCollection) {
		serviceCollection.AddViewModelsFromAssembly(Assembly.Load("Hydrogen.DApp.Presentation"));
	}
}
