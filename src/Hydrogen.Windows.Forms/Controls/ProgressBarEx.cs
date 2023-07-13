// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class ProgressBarEx : ProgressBar {
	//Property to set to decide whether to print a % or Text
	public ProgressBarDisplayText DisplayStyle { get; set; }

	//Property to hold the custom text
	public String CustomText { get; set; }

	public ProgressBarEx() {
		// Modify the ControlStyles flags
		//http://msdn.microsoft.com/en-us/library/system.windows.forms.controlstyles.aspx
		base.DoubleBuffered = true;
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
	}


	protected override void OnPaint(PaintEventArgs e) {
		Rectangle rect = ClientRectangle;
		Graphics g = e.Graphics;

		ProgressBarRenderer.DrawHorizontalBar(g, rect);
		rect.Inflate(-3, -3);
		if (Value > 0) {
			// As we doing this ourselves we need to draw the chunks on the progress bar
			Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
			ProgressBarRenderer.DrawHorizontalChunks(g, clip);
		}

		// Set the Display text (Either a % amount or our custom text
		string text = DisplayStyle == ProgressBarDisplayText.Percentage ? Value.ToString() + '%' : CustomText;


		using (Font f = SystemFonts.DefaultFont) {

			SizeF len = g.MeasureString(text, f);
			// Calculate the location of the text (the middle of progress bar)
			// Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));
			Point location = new Point(Convert.ToInt32((Width / 2) - len.Width / 2), Convert.ToInt32((Height / 2) - len.Height / 2));
			// The commented-out code will centre the text into the highlighted area only. This will centre the text regardless of the highlighted area.
			// Draw the custom text
			g.DrawString(text, f, Brushes.Black, location);
		}
	}
}
