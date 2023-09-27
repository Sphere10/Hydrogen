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

public static class RectangleFExtensions {

	#region Positions & Dimensions

	public static PointF TopLeft(this RectangleF rectangleF) {
		return new PointF(rectangleF.Left, rectangleF.Top);
	}

	public static PointF TopRight(this RectangleF rectangleF) {
		return new PointF(rectangleF.Right, rectangleF.Top);
	}

	public static PointF TopCenter(this RectangleF rectangleF) {
		return new PointF(rectangleF.CenterX(), rectangleF.Top);
	}

	public static PointF BottomLeft(this RectangleF rectangleF) {
		return new PointF(rectangleF.Left, rectangleF.Bottom);
	}

	public static PointF BottomCenter(this RectangleF rectangleF) {
		return new PointF(rectangleF.CenterX(), rectangleF.Bottom);
	}

	public static PointF BottomRight(this RectangleF rectangleF) {
		return new PointF(rectangleF.Right, rectangleF.Bottom);
	}

	public static float CenterX(this RectangleF rectangleF) {
		return rectangleF.Left + rectangleF.Width / 2;
	}

	public static float CenterY(this RectangleF rectangleF) {
		return rectangleF.Top + rectangleF.Height / 2;
	}

	public static PointF CenterLeft(this RectangleF rectangleF) {
		return new PointF(rectangleF.Left, rectangleF.CenterY());
	}

	public static PointF CenterCenter(this RectangleF rectangleF) {
		return new PointF(rectangleF.CenterX(), rectangleF.CenterY());
	}

	public static PointF CenterRight(this RectangleF rectangleF) {
		return new PointF(rectangleF.Right, rectangleF.CenterY());
	}

	public static float AbsoluteWidth(this RectangleF rectangleF) {
		return rectangleF.X + rectangleF.Width;
	}
	public static float AbsoluteHeight(this RectangleF rectangleF) {
		return rectangleF.Y + rectangleF.Height;
	}

	#endregion

	#region Intersections

	public static RectangleF IntersectWith(this RectangleF rectangleF, float x, float y, float width, float height) {
		return rectangleF.IntersectWith(new RectangleF(x, y, width, height));
	}

	public static RectangleF IntersectWith(this RectangleF rectangleF, RectangleF other) {
		return RectangleF.Intersect(rectangleF, other);
	}

	#endregion

	#region Setting Position & Dimensions

	public static RectangleF SetLocation(this RectangleF rectangleF, PointF pointF) {
		rectangleF.Location = pointF;
		return rectangleF;
	}

	public static RectangleF SetLocation(this RectangleF rectangleF, float x, float y) {
		rectangleF.Location = new PointF(x, y);
		return rectangleF;
	}
	public static RectangleF SetLocation(this RectangleF rectangleF, SizeF pointF) {
		rectangleF.Location = new PointF(pointF.Width, pointF.Height);
		return rectangleF;
	}

	public static RectangleF SetSizeF(this RectangleF rectangleF, SizeF sizeF) {
		rectangleF.Size = sizeF;
		return rectangleF;
	}

	public static RectangleF SetSizeF(this RectangleF rectangleF, float width, float height) {
		rectangleF.Size = new SizeF(width, height);
		;
		return rectangleF;
	}

	public static RectangleF SetHeight(this RectangleF rectangleF, float height) {
		rectangleF.Height = height;
		return rectangleF;
	}

	public static RectangleF SetWidth(this RectangleF rectangleF, float width) {
		rectangleF.Width = width;
		return rectangleF;
	}

	#endregion

	#region Translations & Expansions

	public static RectangleF Translate(this RectangleF rectangleF, float x, float y) {
		rectangleF.X += x;
		rectangleF.Y += y;
		return rectangleF;
	}

	public static RectangleF Translate(this RectangleF rectangleF, PointF PointF) {
		rectangleF.Location = rectangleF.Location.Add(PointF);
		return rectangleF;
	}

	public static RectangleF TranslateNegative(this RectangleF rectangleF, float x, float y) {
		rectangleF.X -= x;
		rectangleF.Y -= y;
		return rectangleF;
	}

	public static RectangleF TranslateNegative(this RectangleF rectangleF, PointF PointF) {
		rectangleF.Location = rectangleF.Location.Subtract(PointF);
		return rectangleF;
	}

	public static RectangleF ExpandBy(this RectangleF orgRect, SizeF SizeF) {
		orgRect.Size = orgRect.Size.ExpandBy(SizeF);
		return orgRect;
	}

	public static RectangleF ExpandBy(this RectangleF orgRect, float width, float height) {
		orgRect.Width += width;
		orgRect.Height += height;
		return orgRect;
	}

	public static RectangleF ShrinkBy(this RectangleF orgRect, float width, float height) {
		orgRect.Width -= width;
		orgRect.Height -= height;
		return orgRect;
	}

	public static RectangleF ShrinkBy(this RectangleF orgRect, SizeF SizeF) {
		orgRect.Size = orgRect.Size.ShrinkBy(SizeF);
		return orgRect;
	}

	public static RectangleF TranslateAndExpandBy(this RectangleF orgRect, PointF PointF, SizeF SizeF) {
		orgRect.Location = orgRect.Location.Add(PointF);
		orgRect.Size = orgRect.Size.ExpandBy(SizeF);
		return orgRect;
	}

	public static RectangleF TranslateAndExpandBy(this RectangleF orgRect, float x, float y, float width, float height) {
		orgRect.X += x;
		orgRect.Y += y;
		orgRect.Width += width;
		orgRect.Height += height;
		return orgRect;
	}

	public static RectangleF TranslateNegativeAndShrinkBy(this RectangleF orgRect, float x, float y, float width, float height) {
		orgRect.X -= x;
		orgRect.Y -= y;
		orgRect.Width -= width;
		orgRect.Height -= height;
		return orgRect;
	}

	public static RectangleF TranslateNegativeAndShrinkBy(this RectangleF orgRect, PointF PointF, SizeF SizeF) {
		orgRect.Location = orgRect.Location.Subtract(PointF);
		orgRect.Size = orgRect.Size.ShrinkBy(SizeF);
		return orgRect;
	}

	public static RectangleF AsWideRectangleF(this RectangleF rect) {
		bool isLong = rect.Height > rect.Width;
		return new RectangleF(
			rect.X,
			rect.Y,
			isLong ? rect.Height : rect.Width,
			isLong ? rect.Width : rect.Height
		);
	}

	public static RectangleF AsLongRectangleF(this RectangleF rect) {
		bool isWide = rect.Width > rect.Height;
		return new RectangleF(
			rect.X,
			rect.Y,
			isWide ? rect.Height : rect.Width,
			isWide ? rect.Width : rect.Height
		);
	}

	#endregion

	#region Conversions

	public static Rectangle ToRectangle(this RectangleF rectangle) {
		return new Rectangle(rectangle.Location.ToPoint(), rectangle.Size.ToSize());
	}

	#endregion

	#region Misc

	public static GraphicsPath GetRoundPath(this RectangleF r, float depth) {
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
