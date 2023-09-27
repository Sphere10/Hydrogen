// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;

namespace DevAge.Drawing.VisualElements;

public interface IEditablePanel : IBorder {
	BorderStyle BorderStyle { get; set; }
}


[Serializable]
public abstract class EditablePanelBase : IEditablePanel {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public EditablePanelBase() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public EditablePanelBase(EditablePanelBase other) {
		BorderStyle = other.BorderStyle;
	}

	#endregion

	#region Properties

	private BorderStyle mBorderStyle = BorderStyle.System;

	public virtual BorderStyle BorderStyle {
		get { return mBorderStyle; }
		set { mBorderStyle = value; }
	}

	protected virtual bool ShouldSerializeBorderStyle() {
		return BorderStyle != BorderStyle.System;
	}

	#endregion

	public abstract RectangleF GetContentRectangle(RectangleF backGroundArea);

	public abstract SizeF GetExtent(SizeF contentSize);

	/// <summary>
	/// Draw the current VisualElement in the specified Graphics object.
	/// </summary>
	/// <param name="graphics"></param>
	/// <param name="area"></param>
	public abstract void Draw(GraphicsCache graphics, System.Drawing.RectangleF area);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="area"></param>
	/// <param name="point"></param>
	/// <param name="distanceFromBorder">Returns the distance of the specified point from the border rectangle. -1 if is not inside the border. Returns a positive value or 0 if inside the border. Consider always the distance from the outer border.</param>
	/// <returns></returns>
	public abstract RectanglePartType GetPointPartType(System.Drawing.RectangleF area,
	                                                   System.Drawing.PointF point,
	                                                   out float distanceFromBorder);

	public abstract object Clone();
}
