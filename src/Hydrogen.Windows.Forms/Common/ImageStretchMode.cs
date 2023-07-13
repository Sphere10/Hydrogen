// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Forms;

/// <summary>
/// Specifies how images should fill objects
/// </summary>
public enum ImageStretchMode {
	/// <summary>
	/// Use default settings
	/// </summary>
	Normal = 0,

	/// <summary>
	/// The image is transparent
	/// </summary>
	Transparent = 2,

	/// <summary>
	/// The image should be tiled
	/// </summary>
	Tile = 3,

	/// <summary>
	/// The image should be stretched to fit the objects width 
	/// </summary>
	Horizontal = 5,

	/// <summary>
	/// The image should be stretched to fill the object
	/// </summary>
	Stretch = 6,

	/// <summary>
	/// The image is stored in ARGB format
	/// </summary>
	ARGBImage = 7
}
