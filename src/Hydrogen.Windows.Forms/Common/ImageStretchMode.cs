//-----------------------------------------------------------------------
// <copyright file="ImageStretchMode.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrogen.Windows.Forms {
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
}
