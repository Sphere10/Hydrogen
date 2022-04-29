//-----------------------------------------------------------------------
// <copyright file="CGRectExtensions.iOS.cs" company="Sphere 10 Software">
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
using Hydrogen;

namespace Hydrogen {

    public static class CGRectExtensions {

#region Conversions

        public static Rectangle ToRectangle(this CGRect rect) {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public static RectangleF ToRectangleF(this CGRect rect) {
            return new RectangleF((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

#endregion

#region Positions & Dimensions

        public static CGPoint TopLeft(this CGRect rectangle) {
            return new CGPoint(rectangle.Left, rectangle.Top);
        }

        public static CGPoint TopRight(this CGRect rectangle) {
            return new CGPoint(rectangle.Right, rectangle.Top);
        }

        public static CGPoint TopCenter(this CGRect rectangle) {
            return new CGPoint(rectangle.CenterX(), rectangle.Top);
        }

        public static CGPoint BottomLeft(this CGRect rectangle) {
            return new CGPoint(rectangle.Left, rectangle.Bottom);
        }

        public static CGPoint BottomCenter(this CGRect rectangle) {
            return new CGPoint(rectangle.CenterX(), rectangle.Bottom);
        }

        public static CGPoint BottomRight(this CGRect rectangle) {
            return new CGPoint(rectangle.Right, rectangle.Bottom);
        }

        public static nfloat CenterX(this CGRect rectangle) {
            return rectangle.Left + rectangle.Width / 2;
        }

        public static nfloat CenterY(this CGRect rectangle) {
            return rectangle.Top + rectangle.Height / 2;
        }

        public static CGPoint CenterLeft(this CGRect rectangle) {
            return new CGPoint(rectangle.Left, rectangle.CenterY());
        }

        public static CGPoint CenterCenter(this CGRect rectangle) {
            return new CGPoint(rectangle.CenterX(), rectangle.CenterY());
        }

        public static CGPoint CenterRight(this CGRect rectangle) {
            return new CGPoint(rectangle.Right, rectangle.CenterY());
        }

        public static nfloat AbsoluteWidth(this CGRect rectangle) {
            return rectangle.X + rectangle.Width;
        }
        public static nfloat AbsoluteHeight(this CGRect rectangle) {
            return rectangle.Y + rectangle.Height;
        }

#endregion

#region Intersections

        public static CGRect IntersectWith(this CGRect rectangle, nfloat x, nfloat y, nfloat width, nfloat height) {
            return rectangle.IntersectWith(new CGRect(x, y, width, height));
        }

        public static CGRect IntersectWith(this CGRect rectangle, CGRect other) {
            return CGRect.Intersect(rectangle, other);
        }

#endregion

#region Setting Position & Dimensions

        public static CGRect SetLocation(this CGRect rectangle, CGPoint point) {
            rectangle.Location = point;
            return rectangle;
        }

        public static CGRect SetLocation(this CGRect rectangle, nfloat x, nfloat y) {
            rectangle.Location = new CGPoint(x, y);
            return rectangle;
        }
        public static CGRect SetLocation(this CGRect rectangle, CGSize pointSize) {
            rectangle.Location = new CGPoint(pointSize.Width, pointSize.Height);
            return rectangle;
        }

        public static CGRect SetSize(this CGRect rectangle, CGSize size) {
            rectangle.Size = size;
            return rectangle;
        }

        public static CGRect SetSize(this CGRect rectangle, nfloat width, nfloat height) {
            rectangle.Size = new CGSize(width, height);            ;
            return rectangle;
        }

        public static CGRect SetHeight(this CGRect orgRect, nfloat height) {
            orgRect.Height = height;
            return orgRect;
        }

        public static CGRect SetWidth(this CGRect orgRect, nfloat width) {
            orgRect.Width = width;
            return orgRect;
        }

#endregion

#region Translations & Expansions

        public static CGRect Translate(this CGRect rectangle, nfloat x, nfloat y) {
            rectangle.X += x;
            rectangle.Y += y;
            return rectangle;
        }

        public static CGRect Translate(this CGRect rectangle, CGPoint point) {
            rectangle.Location = rectangle.Location.Add(point);
            return rectangle;
        }

        public static CGRect TranslateNegative(this CGRect rectangle, nfloat x, nfloat y) {
            rectangle.X -= x;
            rectangle.Y -= y;
            return rectangle;
        }

        public static CGRect TranslateNegative(this CGRect rectangle, CGPoint point) {
            rectangle.Location = rectangle.Location.Subtract(point);
            return rectangle;
        }

        public static CGRect ExpandBy(this CGRect rectangle, CGSize size) {
            rectangle.Size = rectangle.Size.ExpandBy(size);
            return rectangle;
        }

        public static CGRect ExpandBy(this CGRect rectangle, nfloat width, nfloat height) {
            rectangle.Width += width;
            rectangle.Height += height;
            return rectangle;
        }

        public static CGRect ShrinkBy(this CGRect rectangle, nfloat width, nfloat height) {
            rectangle.Width -= width;
            rectangle.Height -= height;
            return rectangle;
        }

        public static CGRect ShrinkBy(this CGRect rectangle, CGSize size) {
            rectangle.Size = rectangle.Size.ShrinkBy(size);
            return rectangle;
        }

        public static CGRect TranslateAndExpandBy(this CGRect rectangle, CGPoint point, CGSize size) {
            rectangle.Location = rectangle.Location.Add(point);
            rectangle.Size = rectangle.Size.ExpandBy(size);
            return rectangle;
        }

        public static CGRect TranslateAndExpandBy(this CGRect rectangle, nfloat x, nfloat y, nfloat width, nfloat height) {
            rectangle.X += x;
            rectangle.Y += y;
            rectangle.Width += width;
            rectangle.Height += height;
            return rectangle;
        }

        public static CGRect TranslateNegativeAndShrinkBy(this CGRect rectangle, nfloat x, nfloat y, nfloat width, nfloat height) {
            rectangle.X -= x;
            rectangle.Y -= y;
            rectangle.Width -= width;
            rectangle.Height -= height;
            return rectangle;
        }

        public static CGRect TranslateNegativeAndShrinkBy(this CGRect rectangle, CGPoint point, CGSize size) {
            rectangle.Location = rectangle.Location.Subtract(point);
            rectangle.Size = rectangle.Size.ShrinkBy(size);
            return rectangle;
        }

        public static CGRect AsWideRect(this CGRect rect) {
            bool isLong = rect.Height > rect.Width;
            return new CGRect(
                rect.X,
                rect.Y,
                isLong ? rect.Height : rect.Width,
                isLong ? rect.Width : rect.Height
            );
        }

        public static CGRect AsLongRect(this CGRect rect) {
            bool isWide = rect.Width > rect.Height;
            return new CGRect(
                rect.X,
                rect.Y,
                isWide ? rect.Height : rect.Width,
                isWide ? rect.Width : rect.Height
            );
        }

#endregion
    }

}
