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

public static class SizeFExtensions {

	public static SizeF Clip(this SizeF measureSize, SizeF minSize, SizeF maxSize) {
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

	public static SizeF ShrinkBy(this SizeF orgSize, PointF point) {
		var x = orgSize.Width - point.X;
		var y = orgSize.Height - point.Y;
		return new SizeF(x, y);
	}

	public static SizeF ExpandBy(this SizeF orgSize, PointF point) {
		var x = orgSize.Width + point.X;
		var y = orgSize.Height + point.Y;
		return new SizeF(x, y);
	}

	public static SizeF ExpandBy(this SizeF orgSize, SizeF size) {
		var x = orgSize.Width + size.Width;
		var y = orgSize.Height + size.Height;
		return new SizeF(x, y);
	}

	public static SizeF ShrinkBy(this SizeF orgSize, SizeF size) {
		var x = orgSize.Width - size.Width;
		var y = orgSize.Height - size.Height;
		return new SizeF(x, y);
	}

	public static Size ScaleBy(this SizeF size, float scale) => new((int)Math.Round(size.Width * scale), (int)Math.Round(size.Height * scale));

	public static Size ToSize(this SizeF sizef) {
		return new Size((int)Math.Ceiling(sizef.Width), (int)Math.Ceiling(sizef.Height));
	}

}
