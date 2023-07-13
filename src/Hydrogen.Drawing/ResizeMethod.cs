// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public enum ResizeMethod {
	/// <summary>
	/// Image width and height will be independently scaled to fit the requested size. The resulting image size will always match the requested size.
	/// </summary>
	Stretch = 1,

	/// <summary>
	/// Image width and height will be equally stretched to best-fit the requested size and the source image pannedaccording alignment parameter. The resulting size will always match the requested size.
	/// </summary>
	AspectFill,

	/// <summary>
	/// Image width and height will be equally stretched to best-fit the requested size. Unfilled parts will be colored with the given padding color. The alignment parameter determines how the best-fit size will be placed within the requested size. The resulting image size will always match the requested size.
	/// </summary>
	AspectFitPadded,

	/// <summary>
	/// Image width and height will be proportionally scaled to best-fit the requested size. The resulting image size may not equal the requested size.
	/// </summary>
	AspectFit,
}
