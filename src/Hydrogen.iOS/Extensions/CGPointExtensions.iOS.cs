//-----------------------------------------------------------------------
// <copyright file="CGPointExtensions.iOS.cs" company="Sphere 10 Software">
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
using CoreGraphics;


namespace Hydrogen {

    public static class CGPointExtensions {

        public static Point ToPoint(this CGPoint point) {
            return new Point((int)point.X, (int)point.Y);
        }

        public static PointF ToPointF(this CGPoint point) {
            return new PointF((float)point.X, (float)point.Y);
        }

        public static float DistanceTo(this CGPoint source, CGPoint dest) {
            return (float)Math.Sqrt(Math.Pow(dest.X - source.X, 2.0) + Math.Pow(dest.Y - source.Y, 2.0));
        }

        public static CGPoint Subtract(this CGPoint orgPoint, CGPoint point) {
            var x = orgPoint.X - point.X;
            var y = orgPoint.Y - point.Y;
            return new CGPoint(x, y);
        }
        public static CGPoint Add(this CGPoint orgPoint, CGPoint point) {
            var x = orgPoint.X + point.X;
            var y = orgPoint.Y + point.Y;
            return new CGPoint(x, y);
        }

        public static CGPoint Add(this CGPoint orgPoint, CGSize size) {
            var x = orgPoint.X + size.Width;
            var y = orgPoint.Y + size.Height;
            return new CGPoint(x, y);
        }
    }
}
