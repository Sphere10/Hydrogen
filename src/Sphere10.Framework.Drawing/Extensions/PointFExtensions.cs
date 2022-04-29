//-----------------------------------------------------------------------
// <copyright file="PointFExtensions.cs" company="Sphere 10 Software">
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
}
