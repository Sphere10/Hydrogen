// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Tests;

public class TestPlugin : Plugin {
	public TestPlugin() {
		Apps = new[] {
			new Hydrogen.DApp.Presentation.Plugins.App("/",
				"Home",
				"abc",
				new[] {
					new AppBlock("test",
						"abc",
						new[] {
							new AppBlockPage("/test",
								"test page",
								"abc",
								new[] {
									new MenuItem("Test Menu", "/app1/page1", new List<MenuItem>())
								})
						})
				}),
			new Hydrogen.DApp.Presentation.Plugins.App("/app1",
				"app1",
				"abc",
				new[] {
					new AppBlock("app1",
						"abc",
						new[] {
							new AppBlockPage("/app1/page1",
								"app1 page",
								"abc",
								new[] {
									new MenuItem("Test Menu", "/app1/page1", new List<MenuItem>())
								})
						})
				})
		};
	}

	public override IEnumerable<IApp> Apps { get; }

	protected override void ConfigureServicesInternal(IServiceCollection serviceCollection) {
		serviceCollection.AddTransient<TestViewModel>();
	}
}


internal class TestViewModel {
}
