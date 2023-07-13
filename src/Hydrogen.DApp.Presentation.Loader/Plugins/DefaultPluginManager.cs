// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

/// <summary>
/// Default plugin managers
/// </summary>
public class DefaultPluginManager : IPluginManager {
	/// <summary>
	/// Gets the plugin locator
	/// </summary>
	private IPluginLocator PluginLocator { get; }

	private ILogger<DefaultPluginManager> Logger { get; }

	/// <summary>
	/// Gets the available loaded plugins.
	/// </summary>
	public IEnumerable<IPlugin> Plugins { get; private set; } = new List<IPlugin>();

	/// <summary>
	/// Initializes a new instance of the <see cref="DefaultPluginManager"/> class.
	/// </summary>
	/// <param name="pluginLocator"> plugin locator</param>
	/// <param name="logger"> logger</param>
	public DefaultPluginManager(IPluginLocator pluginLocator, ILogger<DefaultPluginManager> logger) {
		PluginLocator = pluginLocator;
		Logger = logger;

		IEnumerable<Type> types = PluginLocator.LocatePlugins();

		Plugins = types.Select(Activator.CreateInstance)
			.Cast<IPlugin>();
	}

	/// <summary>
	/// Configures the service collection with services from plugins.
	/// </summary>
	public IServiceCollection ConfigureServices(IServiceCollection serviceCollection) {
		foreach (var plugin in Plugins) {
			serviceCollection = plugin.ConfigureServices(serviceCollection);
		}

		return serviceCollection;
	}
}
