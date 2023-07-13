// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms.Misc;

class ToolTipEx : ToolTip {
	public ToolTipEx() {
		this.OwnerDraw = true;
		this.Popup += new PopupEventHandler(this.OnPopup);
		this.Draw += new DrawToolTipEventHandler(this.OnDraw);
	}

	private void OnPopup(object sender, PopupEventArgs e) // use this event to set the size of the tool tip
	{
		e.ToolTipSize = new Size(200, 100);
	}

	private void OnDraw(object sender, DrawToolTipEventArgs e) // use this event to customise the tool tip
	{
		Graphics g = e.Graphics;

		LinearGradientBrush b = new LinearGradientBrush(e.Bounds,
			Color.GreenYellow,
			Color.MintCream,
			45f);

		g.FillRectangle(b, e.Bounds);

		g.DrawRectangle(new Pen(Brushes.Red, 1),
			new Rectangle(e.Bounds.X,
				e.Bounds.Y,
				e.Bounds.Width - 1,
				e.Bounds.Height - 1));

		g.DrawString(e.ToolTipText,
			new Font(e.Font, FontStyle.Bold),
			Brushes.Silver,
			new PointF(e.Bounds.X + 6, e.Bounds.Y + 6)); // shadow layer
		g.DrawString(e.ToolTipText,
			new Font(e.Font, FontStyle.Bold),
			Brushes.Black,
			new PointF(e.Bounds.X + 5, e.Bounds.Y + 5)); // top layer

		b.Dispose();
	}
}
