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
/// Application block
/// </summary>
public class AppBlock : IAppBlock {
	/// <summary>
	/// Initializes a new instance of the <see cref="AppBlock"/> class.
	/// </summary>
	/// <param name="name"> name</param>
	/// <param name="appBlockPages"> pages</param>
	/// <param name="icon"> icon</param>
	public AppBlock(string name, string icon, IEnumerable<IAppBlockPage> appBlockPages) {
		Name = name ?? throw new ArgumentNullException(nameof(name));
		AppBlockPages = appBlockPages ?? throw new ArgumentNullException(nameof(appBlockPages));
		Icon = icon ?? throw new ArgumentNullException(nameof(icon));
	}

	/// <summary>
	/// Gets the name of the item, useful for displaying in menus or headings.
	/// </summary>
	public string Name { get; }

	/// <inheritdoc />
	public IEnumerable<IAppBlockPage> AppBlockPages { get; }

	/// <summary>
	/// Gets the icon font-awesome ccs classes for this app block.
	/// </summary>
	public string Icon { get; }
}
