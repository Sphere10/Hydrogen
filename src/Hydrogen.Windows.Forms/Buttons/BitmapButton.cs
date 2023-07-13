// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// Taken from http://www.codeproject.com/KB/cpp/bitmapbutton.aspx.
/// The button state images are taken from the Image property. The state
/// images are horizontally tiled in the following order:
///  up, down, focused, mouse-over, disabled 
/// </summary>
public class BitmapButton : Button {
	enum btnState {
		BUTTON_UP = 0,
		BUTTON_DOWN = 1,
		BUTTON_FOCUSED = 2,
		BUTTON_MOUSE_ENTER = 3,
		BUTTON_DISABLED = 4,
	}


	btnState imgState = btnState.BUTTON_UP;
	bool mouseEnter = false;

	public BitmapButton() {
		// enable double buffering.  Must be done by a derived class
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

		// initialize event handlers
		Paint += new PaintEventHandler(BitmapButton_Paint);
		MouseDown += new MouseEventHandler(BitmapButton_MouseDown);
		MouseUp += new MouseEventHandler(BitmapButton_MouseUp);
		GotFocus += new EventHandler(BitmapButton_GotFocus);
		LostFocus += new EventHandler(BitmapButton_LostFocus);
		MouseEnter += new EventHandler(BitmapButton_MouseEnter);
		MouseLeave += new EventHandler(BitmapButton_MouseLeave);
		KeyDown += new KeyEventHandler(BitmapButton_KeyDown);
		KeyUp += new KeyEventHandler(BitmapButton_KeyUp);
		EnabledChanged += new EventHandler(BitmapButton_EnabledChanged);
	}

	private void BitmapButton_Paint(object sender, PaintEventArgs e) {
		Graphics gr = e.Graphics;
		if (Image != null) {
			int indexWidth = Size.Width * (int)imgState;

			if (Image.Width > indexWidth) {
				gr.DrawImage(Image, 0, 0, new Rectangle(new Point(indexWidth, 0), Size), GraphicsUnit.Pixel);
			} else {
				gr.DrawImage(Image, 0, 0, new Rectangle(new Point(0, 0), new Size(Size.Width, Size.Height)), GraphicsUnit.Pixel);
			}
		}
	}

	private void BitmapButton_MouseDown(object sender, MouseEventArgs e) {
		imgState = btnState.BUTTON_DOWN;
		Invalidate();
	}

	private void BitmapButton_MouseUp(object sender, MouseEventArgs e) {
		imgState = btnState.BUTTON_FOCUSED;
		Invalidate();
	}

	private void BitmapButton_GotFocus(object sender, EventArgs e) {
		imgState = btnState.BUTTON_FOCUSED;
		Invalidate();
	}

	private void BitmapButton_LostFocus(object sender, EventArgs e) {
		if (mouseEnter) {
			imgState = btnState.BUTTON_MOUSE_ENTER;
		} else {
			imgState = btnState.BUTTON_UP;
		}
		Invalidate();
	}

	private void BitmapButton_MouseEnter(object sender, EventArgs e) {
		// only show mouse enter if doesn't have focus
		if (imgState == btnState.BUTTON_UP) {
			imgState = btnState.BUTTON_MOUSE_ENTER;
		}
		mouseEnter = true;
		Invalidate();
	}

	private void BitmapButton_MouseLeave(object sender, EventArgs e) {
		// only restore state if doesn't have focus
		if (imgState != btnState.BUTTON_FOCUSED) {
			imgState = btnState.BUTTON_UP;
		}
		mouseEnter = false;
		Invalidate();
	}

	private void BitmapButton_KeyDown(object sender, KeyEventArgs e) {
		if (e.KeyData == Keys.Space) {
			imgState = btnState.BUTTON_DOWN;
			Invalidate();
		}
	}

	private void BitmapButton_KeyUp(object sender, KeyEventArgs e) {
		if (e.KeyData == Keys.Space) {
			// still has focus
			imgState = btnState.BUTTON_FOCUSED;
			Invalidate();
		}
	}

	private void BitmapButton_EnabledChanged(object sender, EventArgs e) {
		if (Enabled) {
			imgState = btnState.BUTTON_UP;
		} else {
			imgState = btnState.BUTTON_DISABLED;
		}
		Invalidate();
	}
}
