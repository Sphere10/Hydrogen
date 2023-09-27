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

public static class PointFExtensions {

	public static float DistanceTo(this PointF source, PointF dest) {
		return (float)Math.Sqrt(Math.Pow(dest.X - source.X, 2.0) + Math.Pow(dest.Y - source.Y, 2.0));
	}

	public static PointF Subtract(this PointF orgPoint, PointF point) {
		var x = orgPoint.X - point.X;
		var y = orgPoint.Y - point.Y;
		return new PointF(x, y);
	}
	public static PointF Add(this PointF orgPoint, PointF point) {
		var x = orgPoint.X + point.X;
		var y = orgPoint.Y + point.Y;
		return new PointF(x, y);
	}

	public static PointF Add(this PointF orgPoint, Size size) {
		var x = orgPoint.X + size.Width;
		var y = orgPoint.Y + size.Height;
		return new PointF(x, y);
	}

	public static PointF Add(this PointF orgPoint, SizeF size) {
		var x = orgPoint.X + size.Width;
		var y = orgPoint.Y + size.Height;
		return new PointF(x, y);
	}

	public static Point ToPoint(this PointF point) {
		return new Point((int)Math.Ceiling(point.X), (int)Math.Ceiling(point.Y));
	}

}
