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

public static class PointExtensions {

	public static float DistanceTo(this Point source, Point dest) {
		return (float)Math.Sqrt(Math.Pow(dest.X - source.X, 2.0) + Math.Pow(dest.Y - source.Y, 2.0));
	}

	public static Point Subtract(this Point orgPoint, Point point) {
		var x = orgPoint.X - point.X;
		var y = orgPoint.Y - point.Y;
		return new Point(x, y);
	}
	public static Point Add(this Point orgPoint, Point point) {
		var x = orgPoint.X + point.X;
		var y = orgPoint.Y + point.Y;
		return new Point(x, y);
	}

	public static Point Add(this Point orgPoint, Size size) {
		var x = orgPoint.X + size.Width;
		var y = orgPoint.Y + size.Height;
		return new Point(x, y);
	}

	public static Point Add(this Point orgPoint, SizeF size) {
		var x = orgPoint.X + size.Width;
		var y = orgPoint.Y + size.Height;
		return new Point((int)Math.Round(x, 0), (int)Math.Round(y, 0));
	}

	public static PointF ToPointF(this Point point) {
		return new PointF(point.X, point.Y);
	}


}
