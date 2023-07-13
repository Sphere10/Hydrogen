// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows;

public static partial class WinAPI {

	public static class GDI32 {

		/// <summary>
		///     Specifies a raster-operation code. These codes define how the color data for the
		///     source rectangle is to be combined with the color data for the destination
		///     rectangle to achieve the final color.
		/// </summary>
		public enum TernaryRasterOperations : uint {
			// ReSharper disable InconsistentNaming
			/// <summary>dest = source</summary>
			SRCCOPY = 0x00CC0020,

			/// <summary>dest = source OR dest</summary>
			SRCPAINT = 0x00EE0086,

			/// <summary>dest = source AND dest</summary>
			SRCAND = 0x008800C6,

			/// <summary>dest = source XOR dest</summary>
			SRCINVERT = 0x00660046,

			/// <summary>dest = source AND (NOT dest)</summary>
			SRCERASE = 0x00440328,

			/// <summary>dest = (NOT source)</summary>
			NOTSRCCOPY = 0x00330008,

			/// <summary>dest = (NOT src) AND (NOT dest)</summary>
			NOTSRCERASE = 0x001100A6,

			/// <summary>dest = (source AND pattern)</summary>
			MERGECOPY = 0x00C000CA,

			/// <summary>dest = (NOT source) OR dest</summary>
			MERGEPAINT = 0x00BB0226,

			/// <summary>dest = pattern</summary>
			PATCOPY = 0x00F00021,

			/// <summary>dest = DPSnoo</summary>
			PATPAINT = 0x00FB0A09,

			/// <summary>dest = pattern XOR dest</summary>
			PATINVERT = 0x005A0049,

			/// <summary>dest = (NOT dest)</summary>
			DSTINVERT = 0x00550009,

			/// <summary>dest = BLACK</summary>
			BLACKNESS = 0x00000042,

			/// <summary>dest = WHITE</summary>
			WHITENESS = 0x00FF0062,

			/// <summary>
			/// Capture window as seen on screen.  This includes layered windows 
			/// such as WPF windows with AllowsTransparency="true"
			/// </summary>
			CAPTUREBLT = 0x40000000
			// ReSharper restore InconsistentNaming
		}


		[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

		[DllImport("gdi32.dll")]
		public static extern uint SetBkColor(IntPtr hdc, uint crColor);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateSolidBrush(uint crColor);


		[DllImport("gdi32.dll")]
		public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);


		[DllImport("gdi32.dll")]
		public static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask, int yMask, uint dwRop);

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);


		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		[DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteDC([In] IntPtr hdc);

		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GdiFlush();

		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		[DllImport("gdi32.dll")]
		public static extern int SetBkMode(IntPtr hdc, int iBkMode);

		[DllImport("gdi32.dll")]
		public static extern uint SetTextColor(IntPtr hdc, int crColor);

	}

}
