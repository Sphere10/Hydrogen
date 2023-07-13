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
public class Button : ButtonBase {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public Button() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public Button(Button other) : base(other) {
	}

	#endregion

	/// <summary>
	/// Clone
	/// </summary>
	/// <returns></returns>
	public override object Clone() {
		return new Button(this);
	}

	protected override void OnDraw(GraphicsCache graphics, System.Drawing.RectangleF area) {
		base.OnDraw(graphics, area);

		ButtonState state;
		if (Style == ButtonStyle.Disabled)
			state = ButtonState.Inactive;
		else if (Style == ButtonStyle.Pressed)
			state = ButtonState.Pushed;
		else if (Style == ButtonStyle.Hot)
			state = ButtonState.Normal;
		else
			state = ButtonState.Normal;

		ControlPaint.DrawButton(graphics.Graphics, Rectangle.Round(area), state);

		if (Style == ButtonStyle.NormalDefault) {
			graphics.Graphics.DrawRectangle(Pens.Black, Rectangle.Round(area));
		}

		if (Style == ButtonStyle.Focus) {
			using (MeasureHelper measure = new MeasureHelper(graphics)) {
				ControlPaint.DrawFocusRectangle(graphics.Graphics, Rectangle.Round(GetBackgroundContentRectangle(measure, area)));
			}
		}
	}

	public override System.Drawing.RectangleF GetBackgroundContentRectangle(MeasureHelper measure, System.Drawing.RectangleF backGroundArea) {
		backGroundArea = base.GetBackgroundContentRectangle(measure, backGroundArea);

		if (backGroundArea.Width > 4) {
			backGroundArea.X += 2;
			backGroundArea.Width -= 4;
		}

		if (backGroundArea.Height > 4) {
			backGroundArea.Y += 2;
			backGroundArea.Height -= 4;
		}

		return backGroundArea;
	}

	public override System.Drawing.SizeF GetBackgroundExtent(MeasureHelper measure, System.Drawing.SizeF contentSize) {
		contentSize = new SizeF(contentSize.Width + 4, contentSize.Height + 4);

		return base.GetBackgroundExtent(measure, contentSize);
	}
}
