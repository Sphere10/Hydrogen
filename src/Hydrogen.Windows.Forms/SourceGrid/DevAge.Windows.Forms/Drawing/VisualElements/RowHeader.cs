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

[Serializable]
public class RowHeader : RowHeaderBase {

	#region Constuctor

	/// <summary>
	/// Default constructor
	/// </summary>
	public RowHeader() {
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="other"></param>
	public RowHeader(RowHeader other)
		: base(other) {
		mBackground = (Header)other.mBackground.Clone();
	}

	#endregion

	/// <summary>
	/// Clone
	/// </summary>
	/// <returns></returns>
	public override object Clone() {
		return new RowHeader(this);
	}

	#region Properties

	public override ControlDrawStyle Style {
		get { return base.Style; }
		set {
			base.Style = value;
			mBackground.Style = value;
		}
	}

	/// <summary>
	/// Back Color
	/// </summary>
	public Color BackColor {
		get { return mBackground.BackColor; }
		set { mBackground.BackColor = value; }
	}

	/// <summary>
	/// Draw mode for the header. Default is Linear.
	/// </summary>
	public BackgroundColorStyle BackgroundColorStyle {
		get { return mBackground.BackgroundColorStyle; }
		set { mBackground.BackgroundColorStyle = value; }
	}

	/// <summary>
	/// Border
	/// </summary>
	public RectangleBorder Border {
		get { return mBackground.Border; }
		set { mBackground.Border = value; }
	}

	#endregion

	private Header mBackground = new Header(0);
	protected override void OnDraw(GraphicsCache graphics, RectangleF area) {
		base.OnDraw(graphics, area);

		mBackground.Draw(graphics, area);
	}

	public override RectangleF GetBackgroundContentRectangle(MeasureHelper measure, RectangleF backGroundArea) {
		backGroundArea = base.GetBackgroundContentRectangle(measure, backGroundArea);

		return mBackground.GetBackgroundContentRectangle(measure, backGroundArea);
	}

	public override SizeF GetBackgroundExtent(MeasureHelper measure, SizeF contentSize) {
		contentSize = mBackground.GetBackgroundExtent(measure, contentSize);

		return base.GetBackgroundExtent(measure, contentSize);
	}

}
