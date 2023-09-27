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
/// VelocityNET application plugin. VelocityNET client application will locate implementations of this
/// interface and 
/// </summary>
public interface IPlugin {
	/// <summary>
	/// Gets the applications this plugin provides.
	/// </summary>
	IEnumerable<IApp> Apps { get; }

	/// <summary>
	/// Configure the service collection with this plugin's services.
	/// </summary>
	/// <param name="serviceCollection"> services</param>
	IServiceCollection ConfigureServices(IServiceCollection serviceCollection);
}
