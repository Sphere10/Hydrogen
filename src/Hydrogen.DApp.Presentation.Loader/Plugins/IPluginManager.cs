// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen.DApp.Presentation.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Plugins;

/// <summary>
/// Manages plugins.
/// </summary>
public interface IPluginManager {
	/// <summary>
	/// Gets the currently available plugins
	/// </summary>
	IEnumerable<IPlugin> Plugins { get; }

	/// <summary>
	/// Configures the service collection with services from plugins.
	/// </summary>
	IServiceCollection ConfigureServices(IServiceCollection serviceCollection);
}
