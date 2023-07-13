// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Presentation.Plugins;

/// <summary>
/// Denotes an item that can be routed/navigated to.
/// </summary>
public interface IRoutablePage {
	/// <summary>
	/// Gets the routable page url for this app
	/// </summary>
	public string Route { get; }
}
