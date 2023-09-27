// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;


namespace Hydrogen;

public static class IconExtensions {

	public static Bitmap ToBitmap(this Icon icon, int width, int height) {
		using (var properIcon = new Icon(icon, Math.Max(width, height), Math.Max(width, height))) {
			return properIcon.ToBitmap().ResizeAndDispose(new Size(width, height), ResizeMethod.AspectFitPadded);
		}
	}

}
