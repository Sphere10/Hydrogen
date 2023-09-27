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

public static class SizeExtensions {

	public static Size Clip(this Size measureSize, Size minSize, Size maxSize) {
		if (minSize.Width > 0 && measureSize.Width < minSize.Width)
			measureSize.Width = minSize.Width;
		if (minSize.Height > 0 && measureSize.Height < minSize.Height)
			measureSize.Height = minSize.Height;

		if (maxSize.Width > 0 && measureSize.Width > maxSize.Width)
			measureSize.Width = maxSize.Width;
		if (maxSize.Height > 0 && measureSize.Height > maxSize.Height)
			measureSize.Height = maxSize.Height;

		return measureSize;
	}

	public static Size ShrinkBy(this Size orgSize, Point point) {
		var x = orgSize.Width - point.X;
		var y = orgSize.Height - point.Y;
		return new Size(x, y);
	}
	public static Size ExpandBy(this Size orgSize, Point point) {
		var x = orgSize.Width + point.X;
		var y = orgSize.Height + point.Y;
		return new Size(x, y);
	}

	public static Size ExpandBy(this Size orgSize, Size size) {
		var x = orgSize.Width + size.Width;
		var y = orgSize.Height + size.Height;
		return new Size(x, y);
	}

	public static Size ShrinkBy(this Size orgSize, Size size) {
		var x = orgSize.Width - size.Width;
		var y = orgSize.Height - size.Height;
		return new Size(x, y);
	}

	public static Size ScaleBy(this Size size, double scale) => new((int)Math.Round(size.Width * scale), (int)Math.Round(size.Height * scale));

	public static SizeF ToSizeF(this Size size) {
		return new SizeF(size.Width, size.Height);
	}

}
