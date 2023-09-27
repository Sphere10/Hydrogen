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
/// App block page.
/// </summary>
public class AppBlockPage : IAppBlockPage {
	/// <summary>
	/// Initializes a new instance of the <see cref="AppBlockPage"/> class.
	/// </summary>
	/// <param name="route"> route - the relative path from app to navigate to.</param>
	/// <param name="name"> page name</param>
	/// <param name="icon"></param>
	/// <param name="menuItems"></param>
	public AppBlockPage(string route, string name, string icon, IEnumerable<MenuItem> menuItems) {
		Route = route ?? throw new ArgumentNullException(nameof(route));
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Icon = icon ?? throw new ArgumentNullException(nameof(icon));
		MenuItems = menuItems ?? throw new ArgumentNullException(nameof(menuItems));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AppBlockPage"/> class.
	/// </summary>
	/// <param name="route"> route - the relative path from app to navigate to.</param>
	/// <param name="name"> page name</param>
	/// <param name="icon"></param>
	public AppBlockPage(string route, string name, string icon) : this(route, name, icon, new List<MenuItem>()) {
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
	/// Gets the icon font-awesome ccs classes for this app block.
	/// </summary>
	public string Icon { get; }

	/// <summary>
	/// Gets the menu items
	/// </summary>
	public IEnumerable<MenuItem> MenuItems { get; }
}
