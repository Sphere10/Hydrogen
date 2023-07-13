// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Presentation.Plugins;

/// <summary>
/// Implementers of this interface have a font-awesome css icon to be displayed in lists etc.
/// </summary>
public interface IIconItem {
	/// <summary>
	/// Gets the icon font-awesome ccs classes for this app block.
	/// </summary>
	public string Icon { get; }
}
