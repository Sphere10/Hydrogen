// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Tools;

public static class WinForms {
	public struct IconInfo {
		public bool fIcon;
		public int xHotspot;
		public int yHotspot;
		public IntPtr hbmMask;
		public IntPtr hbmColor;
	}


	[DllImport("user32.dll")]
	public static extern IntPtr CreateIconIndirect(
		ref IconInfo icon);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetIconInfo(IntPtr hIcon,
	                                      ref IconInfo pIconInfo);

	public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot) {
		var tmp = new IconInfo();
		GetIconInfo(bmp.GetHicon(), ref tmp);
		tmp.xHotspot = xHotSpot;
		tmp.yHotspot = yHotSpot;
		tmp.fIcon = false;
		return new Cursor(CreateIconIndirect(ref tmp));
	}

	public static Cursor LoadRawCursor(byte[] bytes) {
		return new Cursor(new MemoryStream(bytes));
	}

}
