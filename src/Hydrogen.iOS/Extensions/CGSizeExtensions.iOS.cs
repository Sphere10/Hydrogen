//-----------------------------------------------------------------------
// <copyright file="CGSizeExtensions.iOS.cs" company="Sphere 10 Software">
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

    public static class CGSizeExtensions {

#region Conversions

        public static Size ToSize(this CGSize size) {
            return new Size((int)size.Width, (int)size.Height);
        }

        public static SizeF ToSizeF(this CGSize size) {
            return new SizeF((float)size.Width, (float)size.Height);
        }

#endregion

        public static CGSize Clip(this CGSize measureSize, CGSize minSize, CGSize maxSize) {
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

        public static CGSize ShrinkBy(this CGSize orgSize, CGPoint point) {
            var x = orgSize.Width - point.X;
            var y = orgSize.Height - point.Y;
            return new CGSize(x, y);
        }
        public static CGSize ExpandBy(this CGSize orgSize, CGPoint point) {
            var x = orgSize.Width + point.X;
            var y = orgSize.Height + point.Y;
            return new CGSize(x, y);
        }

        public static CGSize ExpandBy(this CGSize orgSize, CGSize size) {
            var x = orgSize.Width + size.Width;
            var y = orgSize.Height + size.Height;
            return new CGSize(x, y);
        }

        public static CGSize ShrinkBy(this CGSize orgSize, CGSize size) {
            var x = orgSize.Width - size.Width;
            var y = orgSize.Height - size.Height;
            return new CGSize(x, y);
        }

    }
}
