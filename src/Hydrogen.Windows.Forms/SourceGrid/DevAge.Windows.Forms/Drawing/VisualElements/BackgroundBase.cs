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

public interface IBackground : IVisualElement {
	RectangleF GetBackgroundContentRectangle(MeasureHelper measure, RectangleF backGroundArea);

	SizeF GetBackgroundExtent(MeasureHelper measure, SizeF contentSize);
}


/// <summary>
/// The background visualelement can be used as a background for a objectStream element.
/// Override the OnDraw, GetBackgroundContentRectangle and GetBackgroundExtent to draw a custom background.
/// </summary>
[Serializable]
public abstract class BackgroundBase : VisualElementBase, IBackground {

	#region Constructor

	/// <summary>
	/// Default constructor
	/// </summary>
	public BackgroundBase() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public BackgroundBase(BackgroundBase other)
		: base(other) {
	}

	#endregion


	/// <summary>
	/// Calculate the client area where the content can be drawed, usually removing the area used by the background, for example removing a border.
	/// </summary>
	/// <returns></returns>
	public virtual RectangleF GetBackgroundContentRectangle(MeasureHelper measure, RectangleF backGroundArea) {
		return backGroundArea;
	}

	/// <summary>
	/// Calculate the total area used by the backgound and the content, adding the background area to the content area.
	/// </summary>
	/// <returns></returns>
	public virtual SizeF GetBackgroundExtent(MeasureHelper measure, SizeF contentSize) {
		return contentSize;
	}

	protected override void OnDraw(GraphicsCache graphics, RectangleF area) {
	}

	protected override SizeF OnMeasureContent(MeasureHelper measure, SizeF maxSize) {
		SizeF clienSize = SizeF.Empty;

		return GetBackgroundExtent(measure, clienSize);
	}
}
