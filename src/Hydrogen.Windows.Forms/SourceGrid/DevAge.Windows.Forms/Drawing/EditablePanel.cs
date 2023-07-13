// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace DevAge.Drawing.VisualElements;

[Serializable]
public class EditablePanel : EditablePanelBase {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public EditablePanel() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public EditablePanel(EditablePanel other)
		: base(other) {
	}

	#endregion

	/// <summary>
	/// Clone
	/// </summary>
	/// <returns></returns>
	public override object Clone() {
		return new EditablePanel(this);
	}

	public override void Draw(GraphicsCache graphics, RectangleF area) {
		if (BorderStyle == BorderStyle.System)
			ControlPaint.DrawBorder3D(graphics.Graphics, Rectangle.Round(area), Border3DStyle.Flat, Border3DSide.All);
	}

	private RectangleBorder mEquivalentPadding = new RectangleBorder(new BorderLine(Color.Empty, 2));

	public override System.Drawing.RectangleF GetContentRectangle(System.Drawing.RectangleF backGroundArea) {
		if (BorderStyle == BorderStyle.System)
			return mEquivalentPadding.GetContentRectangle(backGroundArea);
		else
			return backGroundArea;
	}

	public override System.Drawing.SizeF GetExtent(System.Drawing.SizeF contentSize) {
		if (BorderStyle == BorderStyle.System)
			return mEquivalentPadding.GetExtent(contentSize);
		else
			return contentSize;
	}

	public override RectanglePartType GetPointPartType(RectangleF area, PointF point, out float distanceFromBorder) {
		if (BorderStyle == BorderStyle.System)
			return mEquivalentPadding.GetPointPartType(area, point, out distanceFromBorder);
		else {
			distanceFromBorder = 0;
			return RectanglePartType.ContentArea;
		}
	}
}
