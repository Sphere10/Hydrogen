// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Plugins;

/// <summary>
/// App - contains one or more app blocks.
/// </summary>
public class App : IApp {
	/// <summary>
	/// Initialize a new instance of the <see cref="App"/> class.
	/// </summary>
	/// <param name="route"></param>
	/// <param name="name"></param>
	/// <param name="icon"></param>
	/// <param name="appBlocks"></param>
	public App(string route, string name, string icon, IEnumerable<IAppBlock> appBlocks) {
		Route = route ?? throw new ArgumentNullException(nameof(route));
		Name = name ?? throw new ArgumentNullException(nameof(name));
		AppBlocks = appBlocks ?? throw new ArgumentNullException(nameof(appBlocks));
		Icon = icon ?? throw new ArgumentNullException(nameof(icon));
	}

	/// <summary>
	/// Gets the routable page url for this app
	/// </summary>
	public string Route { get; }

	/// <summary>
	/// Gets the name of the item, useful for displaying in menus or headings.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the app blocks that are part of this 
	/// </summary>
	public IEnumerable<IAppBlock> AppBlocks { get; }

	/// <summary>
	/// Gets the icon font-awesome ccs classes for this app block.
	/// </summary>
	public string Icon { get; }
}
