// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.DApp.Presentation.Plugins;

/// <summary>
/// Plugin base
/// </summary>
public abstract class Plugin : IPlugin {
	/// <summary>
	/// Gets the applications this plugin provides.
	/// </summary>
	public abstract IEnumerable<IApp> Apps { get; }

	/// <summary>
	/// Configure this plugin's services. Automatically configures view model services.
	/// </summary>
	/// <param name="serviceCollection"> services</param>
	public IServiceCollection ConfigureServices(IServiceCollection serviceCollection) {
		serviceCollection.AddViewModelsFromAssembly(GetType().Assembly);
		ConfigureServicesInternal(serviceCollection);

		return serviceCollection;
	}

	/// <summary>
	/// Extension point for plugins to configure required services.
	/// </summary>
	/// <param name="serviceCollection"> service collection</param>
	protected abstract void ConfigureServicesInternal(IServiceCollection serviceCollection);
}
