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
using System.Drawing.Imaging;

namespace Hydrogen.Windows.Forms;

/// <para>Your PerPixel form should inherit this class</para>
/// <author><name>Rui Godinho Lopes</name><email>rui@ruilopes.com</email></author>
public class PerPixelAlphaForm : Form {
	public PerPixelAlphaForm() {
		// This form should not have a border or else Windows will clip it.
		FormBorderStyle = FormBorderStyle.None;
	}


	/// <para>Changes the current bitmap.</para>
	public void SetBitmap(Bitmap bitmap) {
		SetBitmap(bitmap, 255);
	}


	/// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
	public void SetBitmap(Bitmap bitmap, byte opacity) {
		if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
			throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

		// The ideia of this is very simple,
		// 1. Create a compatible DC with screen;
		// 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
		// 3. Call the UpdateLayeredWindow.

		IntPtr screenDc = WinAPI.USER32.GetDC(IntPtr.Zero);
		IntPtr memDc = WinAPI.GDI32.CreateCompatibleDC(screenDc);
		IntPtr hBitmap = IntPtr.Zero;
		IntPtr oldBitmap = IntPtr.Zero;

		try {
			hBitmap = bitmap.GetHbitmap(Color.FromArgb(0)); // grab a GDI handle from this GDI+ bitmap
			oldBitmap = WinAPI.GDI32.SelectObject(memDc, hBitmap);

			Size size = new Size(bitmap.Width, bitmap.Height);
			Point pointSource = new Point(0, 0);
			Point topPos = new Point(Left, Top);
			WinAPI.USER32.BLENDFUNCTION blend = new WinAPI.USER32.BLENDFUNCTION();
			blend.BlendOp = (byte)WinAPI.USER32.BlendOps.AC_SRC_OVER;
			blend.BlendFlags = 0;
			blend.SourceConstantAlpha = opacity;
			blend.AlphaFormat = (byte)WinAPI.USER32.BlendOps.AC_SRC_ALPHA;

			WinAPI.USER32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, WinAPI.USER32.BlendFlags.ULW_ALPHA);
		} finally {
			WinAPI.USER32.ReleaseDC(IntPtr.Zero, screenDc);
			if (hBitmap != IntPtr.Zero) {
				WinAPI.GDI32.SelectObject(memDc, oldBitmap);
				//Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
				WinAPI.GDI32.DeleteObject(hBitmap);
			}
			WinAPI.GDI32.DeleteDC(memDc);
		}
	}


	protected override CreateParams CreateParams {
		get {
			CreateParams cp = base.CreateParams;
			cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
			return cp;
		}
	}
}
