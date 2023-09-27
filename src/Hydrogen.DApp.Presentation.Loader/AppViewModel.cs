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
using System.Reflection;
using Hydrogen.DApp.Presentation.Loader.Plugins;

namespace Hydrogen.DApp.Presentation.Loader;

public class AppViewModel {
	public IEnumerable<Assembly> RoutingAssemblies { get; }

	public AppViewModel(IPluginLocator pluginLocator) {
		if (pluginLocator == null)
			throw new ArgumentNullException(nameof(pluginLocator));

		RoutingAssemblies = pluginLocator.LocatePlugins().Select(x => x.Assembly)
			.Where(x => x.FullName != typeof(Program).Assembly.FullName)
			.Distinct();
	}
}
