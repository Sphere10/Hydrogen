//-----------------------------------------------------------------------
// <copyright file="IconExtensions.cs" company="Sphere 10 Software">
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
using System.Drawing;


namespace Sphere10.Framework {

	public static class IconExtensions {

		public static Bitmap ToBitmap(this Icon icon, int width, int height) {
			using (var properIcon = new Icon(icon, Math.Max(width, height), Math.Max(width, height))) {
				return properIcon.ToBitmap().ResizeAndDispose(new Size(width, height), ResizeMethod.AspectFitPadded);
			}
		}

	}
}
