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


namespace Hydrogen.Windows.Forms.WinForms;

class LayeredWindow : Form {
	private Rectangle m_rect;

	public Point LayeredPos {
		get { return m_rect.Location; }
		set { m_rect.Location = value; }
	}

	public Size LayeredSize {
		get { return m_rect.Size; }
	}

	public LayeredWindow() {
		//We need to set this before the window is created, otherwise we
		//have to reset thw window styles using SetWindowLong because
		//the window will no longer be drawn
		this.ShowInTaskbar = false;

		this.FormBorderStyle = FormBorderStyle.None;
	}

	public void UpdateWindow(Bitmap image, byte opacity) {
		UpdateWindow(image, opacity, -1, -1, this.LayeredPos);
	}

	public void UpdateWindow(Bitmap image, byte opacity, int width, int height, Point pos) {
		if (image != null) {
			IntPtr hdcWindow = WinAPI.USER32.GetWindowDC(this.Handle);
			IntPtr hDC = WinAPI.GDI32.CreateCompatibleDC(hdcWindow);
			IntPtr hBitmap = image.GetHbitmap(Color.FromArgb(0));
			IntPtr hOld = WinAPI.GDI32.SelectObject(hDC, hBitmap);
			Size size = new Size(0, 0);
			Point zero = new Point(0, 0);

			if (width == -1 || height == -1) {
				//No width and height specified, use the size of the image
				size.Width = image.Width;
				size.Height = image.Height;
			} else {
				//Use whichever size is smallest, so that the image will
				//be clipped if necessary
				size.Width = Math.Min(image.Width, width);
				size.Height = Math.Min(image.Height, height);
			}
			m_rect.Size = size;
			m_rect.Location = pos;

			WinAPI.USER32.BLENDFUNCTION blend = new Windows.WinAPI.USER32.BLENDFUNCTION();
			blend.BlendOp = (byte)WinAPI.USER32.BlendOps.AC_SRC_OVER;
			blend.SourceConstantAlpha = opacity;
			blend.AlphaFormat = (byte)WinAPI.USER32.BlendOps.AC_SRC_ALPHA;
			blend.BlendFlags = (byte)WinAPI.USER32.BlendFlags.None;

			WinAPI.USER32.UpdateLayeredWindow(this.Handle, hdcWindow, ref pos, ref size, hDC, ref zero, 0, ref blend, WinAPI.USER32.BlendFlags.ULW_ALPHA);

			WinAPI.GDI32.SelectObject(hDC, hOld);
			WinAPI.GDI32.DeleteObject(hBitmap);
			WinAPI.GDI32.DeleteDC(hDC);
			WinAPI.USER32.ReleaseDC(this.Handle, hdcWindow);
		}
	}

	protected override CreateParams CreateParams {
		get {
			CreateParams cp = base.CreateParams;
			cp.ExStyle |= (int)WinAPI.WindowStyles.WS_EX_LAYERED;
			return cp;
		}
	}
}
