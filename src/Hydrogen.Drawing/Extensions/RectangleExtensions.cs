// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Hydrogen;

public static class RectangleExtensions {

	#region Positions & Dimensions

	public static Point TopLeft(this Rectangle rectangle) {
		return new Point(rectangle.Left, rectangle.Top);
	}

	public static Point TopRight(this Rectangle rectangle) {
		return new Point(rectangle.Right, rectangle.Top);
	}

	public static Point TopCenter(this Rectangle rect) {
		return new Point(rect.CenterX(), rect.Top);
	}

	public static Point BottomLeft(this Rectangle rectangle) {
		return new Point(rectangle.Left, rectangle.Bottom);
	}

	public static Point BottomCenter(this Rectangle rect) {
		return new Point(rect.CenterX(), rect.Bottom);
	}

	public static Point BottomRight(this Rectangle rectangle) {
		return new Point(rectangle.Right, rectangle.Bottom);
	}

	public static int CenterX(this Rectangle rect) {
		return rect.Left + rect.Width / 2;
	}

	public static int CenterY(this Rectangle rect) {
		return rect.Top + rect.Height / 2;
	}

	public static Point CenterLeft(this Rectangle rect) {
		return new Point(rect.Left, rect.CenterY());
	}

	public static Point CenterCenter(this Rectangle rect) {
		return new Point(rect.CenterX(), rect.CenterY());
	}

	public static Point CenterRight(this Rectangle rect) {
		return new Point(rect.Right, rect.CenterY());
	}

	public static int AbsoluteWidth(this Rectangle orgRect) {
		return orgRect.X + orgRect.Width;
	}
	public static int AbsoluteHeight(this Rectangle orgRect) {
		return orgRect.Y + orgRect.Height;
	}

	#endregion

	#region Intersections

	public static Rectangle IntersectWith(this Rectangle rect, int x, int y, int width, int height) {
		return rect.IntersectWith(new Rectangle(x, y, width, height));
	}

	public static Rectangle IntersectWith(this Rectangle rect, Rectangle other) {
		return Rectangle.Intersect(rect, other);
	}

	#endregion

	#region Setting Position & Dimensions

	public static Rectangle SetLocation(this Rectangle orgRect, Point point) {
		orgRect.Location = point;
		return orgRect;
	}

	public static Rectangle SetLocation(this Rectangle orgRect, int x, int y) {
		orgRect.Location = new Point(x, y);
		return orgRect;
	}
	public static Rectangle SetLocation(this Rectangle orgRect, Size point) {
		orgRect.Location = new Point(point.Width, point.Height);
		return orgRect;
	}

	public static Rectangle SetSize(this Rectangle orgRect, Size size) {
		orgRect.Size = size;
		return orgRect;
	}

	public static Rectangle SetSize(this Rectangle orgRect, int width, int height) {
		orgRect.Size = new Size(width, height);
		;
		return orgRect;
	}

	public static Rectangle SetHeight(this Rectangle orgRect, int height) {
		orgRect.Height = height;
		return orgRect;
	}

	public static Rectangle SetWidth(this Rectangle orgRect, int width) {
		orgRect.Width = width;
		return orgRect;
	}

	#endregion

	#region Translations & Expansions

	public static Rectangle Translate(this Rectangle orgRect, int x, int y) {
		orgRect.X += x;
		orgRect.Y += y;
		return orgRect;
	}

	public static Rectangle Translate(this Rectangle orgRect, Point point) {
		orgRect.Location = orgRect.Location.Add(point);
		return orgRect;
	}

	public static Rectangle TranslateNegative(this Rectangle orgRect, int x, int y) {
		orgRect.X -= x;
		orgRect.Y -= y;
		return orgRect;
	}

	public static Rectangle TranslateNegative(this Rectangle orgRect, Point point) {
		orgRect.Location = orgRect.Location.Subtract(point);
		return orgRect;
	}

	public static Rectangle ExpandBy(this Rectangle orgRect, Size size) {
		orgRect.Size = orgRect.Size.ExpandBy(size);
		return orgRect;
	}

	public static Rectangle ExpandBy(this Rectangle orgRect, int width, int height) {
		orgRect.Width += width;
		orgRect.Height += height;
		return orgRect;
	}

	public static Rectangle ShrinkBy(this Rectangle orgRect, int width, int height) {
		orgRect.Width -= width;
		orgRect.Height -= height;
		return orgRect;
	}

	public static Rectangle ShrinkBy(this Rectangle orgRect, Size size) {
		orgRect.Size = orgRect.Size.ShrinkBy(size);
		return orgRect;
	}

	public static Rectangle TranslateAndExpandBy(this Rectangle orgRect, Point point, Size size) {
		orgRect.Location = orgRect.Location.Add(point);
		orgRect.Size = orgRect.Size.ExpandBy(size);
		return orgRect;
	}

	public static Rectangle TranslateAndExpandBy(this Rectangle orgRect, int x, int y, int width, int height) {
		orgRect.X += x;
		orgRect.Y += y;
		orgRect.Width += width;
		orgRect.Height += height;
		return orgRect;
	}

	public static Rectangle TranslateNegativeAndShrinkBy(this Rectangle orgRect, int x, int y, int width, int height) {
		orgRect.X -= x;
		orgRect.Y -= y;
		orgRect.Width -= width;
		orgRect.Height -= height;
		return orgRect;
	}

	public static Rectangle TranslateNegativeAndShrinkBy(this Rectangle orgRect, Point point, Size size) {
		orgRect.Location = orgRect.Location.Subtract(point);
		orgRect.Size = orgRect.Size.ShrinkBy(size);
		return orgRect;
	}

	public static Rectangle AsWideRectangle(this Rectangle rect) {
		bool isLong = rect.Height > rect.Width;
		return new Rectangle(
			rect.X,
			rect.Y,
			isLong ? rect.Height : rect.Width,
			isLong ? rect.Width : rect.Height
		);
	}

	public static Rectangle AsLongRectangle(this Rectangle rect) {
		bool isWide = rect.Width > rect.Height;
		return new Rectangle(
			rect.X,
			rect.Y,
			isWide ? rect.Height : rect.Width,
			isWide ? rect.Width : rect.Height
		);
	}

	#endregion

	#region Conversions

	public static RectangleF ToRectangleF(this Rectangle rectangle) {
		return new RectangleF(rectangle.Location.ToPointF(), rectangle.Size.ToSizeF());
	}

	#endregion

	#region Misc

	public static GraphicsPath GetRoundPath(this Rectangle r, int depth) {
		GraphicsPath graphPath = new GraphicsPath();
		graphPath.AddArc(r.X, r.Y, depth, depth, 180, 90);
		graphPath.AddArc(r.X + r.Width - depth, r.Y, depth, depth, 270, 90);
		graphPath.AddArc(r.X + r.Width - depth, r.Y + r.Height - depth, depth, depth, 0, 90);
		graphPath.AddArc(r.X, r.Y + r.Height - depth, depth, depth, 90, 90);
		graphPath.AddLine(r.X, r.Y + r.Height - depth, r.X, r.Y + depth / 2);
		return graphPath;
	}

	#endregion

}
