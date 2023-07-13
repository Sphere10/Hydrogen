// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Reflection;


namespace Hydrogen.Windows;

[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
public static partial class WinAPI {

	#region Structs

	/// <summary>
	/// The RECT structure defines the coordinates of the upper-left 
	/// and lower-right corners of a rectangle
	/// </summary>
	[Serializable(),
	 StructLayout(LayoutKind.Sequential)]
	public struct RECT {
		/// <summary>
		/// Specifies the x-coordinate of the upper-Left corner of the RECT
		/// </summary>
		public int Left;

		/// <summary>
		/// Specifies the y-coordinate of the upper-Left corner of the RECT
		/// </summary>
		public int Top;

		/// <summary>
		/// Specifies the x-coordinate of the lower-Right corner of the RECT
		/// </summary>
		public int Right;

		/// <summary>
		/// Specifies the y-coordinate of the lower-Right corner of the RECT
		/// </summary>
		public int Bottom;


		/// <summary>
		/// Creates a new RECT struct with the specified location and size
		/// </summary>
		/// <param name="Left">The x-coordinate of the upper-Left corner of the RECT</param>
		/// <param name="Top">The y-coordinate of the upper-Left corner of the RECT</param>
		/// <param name="Right">The x-coordinate of the lower-Right corner of the RECT</param>
		/// <param name="Bottom">The y-coordinate of the lower-Right corner of the RECT</param>
		public RECT(int Left, int Top, int Right, int Bottom) {
			this.Left = Left;
			this.Top = Top;
			this.Right = Right;
			this.Bottom = Bottom;
		}

		/// <summary>
		/// Creates a new RECT struct from the specified Rectangle
		/// </summary>
		/// <param name="rect">The Rectangle to create the RECT from</param>
		/// <returns>A RECT struct with the same location and size as 
		/// the specified Rectangle</returns>
		public static RECT FromRectangle(Rectangle rect) {
			return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}


		/// <summary>
		/// Creates a new RECT struct with the specified location and size
		/// </summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the RECT</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the RECT</param>
		/// <param name="width">The width of the RECT</param>
		/// <param name="height">The height of the RECT</param>
		/// <returns>A RECT struct with the specified location and size</returns>
		public static RECT FromXYWH(int x, int y, int width, int height) {
			return new RECT(x, y, x + width, y + height);
		}


		/// <summary>
		/// Returns a Rectangle with the same location and size as the RECT
		/// </summary>
		/// <returns>A Rectangle with the same location and size as the RECT</returns>
		public Rectangle ToRectangle() {
			return new Rectangle(this.Left, this.Top, this.Right - this.Left, this.Bottom - this.Top);
		}
	}


	[StructLayout(LayoutKind.Sequential)]
	public struct SIZE {
		public Int32 cx;
		public Int32 cy;

		public SIZE(Int32 cx, Int32 cy) {
			this.cx = cx;
			this.cy = cy;
		}
	}


	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private struct ARGB {
		public byte Blue;
		public byte Green;
		public byte Red;
		public byte Alpha;
	}


	public enum WindowMessageFlags : uint {
		WM_NCHITTEST = 132,
		HTTRANSPARENT = 0xFFFFFFFF,
		HTCLIENT = 1,
		HTCAPTION = 2,
		WM_NCMOUSEMOVE = 160,
		WM_NCLBUTTONDOWN = 161,
		WM_NCLBUTTONUP = 162,
		WM_NCLBUTTONDBLCLK = 163,
		WM_WINDOWPOSCHANGING = 70,
		WM_ENTERSIZEMOVE = 561,
		WM_EXITSIZEMOVE = 562,
		WM_SYSCOMMAND = 274,
		WM_PAINT = 15,
		HWND_TOP = 0,
		SC_MINIMIZE = 61472,
		SC_RESTORE = 61728,
		SC_MAXIMIZE = 61488,
		WM_SIZE = 5,
		WM_ACTIVATE = 6,
		WM_SETFOCUS = 7,
		WM_SETCURSOR = 32,
		WM_MOUSEMOVE = 0x0200,
		WM_LBUTTONDOWN = 0x0201,
		WM_LBUTTONUP = 0x0202,
		WM_LBUTTONDBLCLK = 0x0203,
		WM_RBUTTONDOWN = 0x0204,
		WM_RBUTTONUP = 0x0205,
		WM_RBUTTONDBLCLK = 0x0206,
		WM_MBUTTONDOWN = 0x0207,
		WM_MBUTTONUP = 0x0208,
		WM_MBUTTONDBLCLK = 0x0209,
		WM_MOUSEWHEEL = 0x020A,
		WM_XBUTTONDOWN = 0x020B,
		WM_XBUTTONUP = 0x020C,
		WM_XBUTTONDBLCLK = 0x020D,
		WM_MOUSELEAVE = 0x02A3,
		WM_WINDOWPOSCHANGED = 0x0047,
		WM_NCACTIVATE = 0X0086,
		GWL_WNDPROC = 0xFFFFFFFC,
		GWL_EXSTYLE = 0xFFFFFFEC,

		/// <summary>
		/// The WM_PRINT message is sent to a window to request that it draw 
		/// itself in the specified device context, most commonly in a printer 
		/// device context
		/// </summary>
		WM_PRINT = 791,

		/// <summary>
		/// The WM_PRINTCLIENT message is sent to a window to request that it draw 
		/// its client area in the specified device context, most commonly in a 
		/// printer device context
		/// </summary>
		WM_PRINTCLIENT = 792,

		ECM_FIRST = 0x1500,
		EM_SETCUEBANNER = ECM_FIRST + 1,
		EM_GETCUEBANNER = ECM_FIRST + 2,
		EM_SHOWBALLOONTIP = (ECM_FIRST + 3),
		EM_HIDEBALLOONTIP = (ECM_FIRST + 4),

		EM_SETMARGINS = 211

	}


	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPOS {
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public WindowPosFlags flags;
	};


	[Flags]
	public enum WindowPosFlags : uint {
		NONE = 0x0000,
		SWP_NOSIZE = 0x0001,
		SWP_NOMOVE = 0x0002,
		SWP_NOZORDER = 0x0004,
		SWP_NOREDRAW = 0x0008,
		SWP_NOACTIVATE = 0x0010,
		SWP_FRAMECHANGED = 0x0020,
		SWP_SHOWWINDOW = 0x0040,
		SWP_HIDEWINDOW = 0x0080,
		SWP_NOCOPYBITS = 0x0100,
		SWP_NOOWNERZORDER = 0x0200,
		SWP_NOSENDCHANGING = 0x0400,
		SWP_DEFERERASE = 0x2000,
		SWP_ASYNCWINDOWPOS = 0x4000,
		SWP_CUSTOMFLAG = 0x8000
	};


	public enum WindowStyles {
		WS_EX_LAYERED = 0x00080000
	}


	[StructLayout(LayoutKind.Sequential)]
	public struct LARGE_INTEGER {
		public int lowpart;
		public int highpart;
	}


	/// <summary>
	/// The POINT structure defines the x- and y- coordinates of a point
	/// </summary>
	[Serializable(),
	 StructLayout(LayoutKind.Sequential)]
	public struct POINT {
		/// <summary>
		/// Specifies the x-coordinate of the point
		/// </summary>
		public int x;

		/// <summary>
		/// Specifies the y-coordinate of the point
		/// </summary>
		public int y;


		/// <summary>
		/// Creates a new RECT struct with the specified x and y coordinates
		/// </summary>
		/// <param name="x">The x-coordinate of the point</param>
		/// <param name="y">The y-coordinate of the point</param>
		public POINT(int x, int y) {
			this.x = x;
			this.y = y;
		}


		/// <summary>
		/// Creates a new POINT struct from the specified Point
		/// </summary>
		/// <param name="p">The Point to create the POINT from</param>
		/// <returns>A POINT struct with the same x and y coordinates as 
		/// the specified Point</returns>
		public static POINT FromPoint(Point p) {
			return new POINT(p.X, p.Y);
		}


		/// <summary>
		/// Returns a Point with the same x and y coordinates as the POINT
		/// </summary>
		/// <returns>A Point with the same x and y coordinates as the POINT</returns>
		public Point ToPoint() {
			return new Point(this.x, this.y);
		}
	}


	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public struct ICONFILE {
		/// <summary>
		/// 
		/// </summary>
		public short reserved;

		/// <summary>
		/// 
		/// </summary>
		public short resourceType;

		/// <summary>
		/// 
		/// </summary>
		public short iconCount;

		/// <summary>
		/// 
		/// </summary>
		public ICONENTRY entries;
	}


	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ICONENTRY {
		/// <summary>
		/// 
		/// </summary>
		public byte width;

		/// <summary>
		/// 
		/// </summary>
		public byte height;

		/// <summary>
		/// 
		/// </summary>
		public byte numColors;

		/// <summary>
		/// 
		/// </summary>
		public byte reserved;

		/// <summary>
		/// 
		/// </summary>
		public short numPlanes;

		/// <summary>
		/// 
		/// </summary>
		public short bitsPerPixel;

		/// <summary>
		/// 
		/// </summary>
		public int dataSize;

		/// <summary>
		/// 
		/// </summary>
		public int dataOffset;
	}

	#endregion

	/// <summary>
	/// The WmPrintFlags enemeration contains flags that may be sent 
	/// when a WM_PRINT or WM_PRINTCLIENT message is recieved
	/// </summary>
	//[Flags]
	//public enum WmPrintFlags {
	//    /// <summary>
	//    /// Draws the window only if it is visible
	//    /// </summary>
	//    PRF_CHECKVISIBLE = 1,

	//    /// <summary>
	//    /// Draws the nonclient area of the window
	//    /// </summary>
	//    PRF_NONCLIENT = 2,

	//    /// <summary>
	//    /// Draws the client area of the window
	//    /// </summary>
	//    PRF_CLIENT = 4,

	//    /// <summary>
	//    /// Erases the background before drawing the window
	//    /// </summary>
	//    PRF_ERASEBKGND = 8,

	//    /// <summary>
	//    /// Draws all visible children windows
	//    /// </summary>
	//    PRF_CHILDREN = 16,

	//    /// <summary>
	//    /// Draws all owned windows
	//    /// </summary>
	//    PRF_OWNED = 32
	//}


	/// <summary>
	/// The SetErrorModeFlags enemeration contains flags that control 
	/// whether the system will handle the specified types of serious errors, 
	/// or whether the process will handle them
	/// </summary>
	//[Flags]
	//public enum SetErrorModeFlags {
	//    /// <summary>
	//    /// Use the system default, which is to display all error dialog boxes
	//    /// </summary>
	//    SEM_DEFAULT = 0,

	//    /// <summary>
	//    /// The system does not display the critical-error-handler message box. 
	//    /// Instead, the system sends the error to the calling process
	//    /// </summary>
	//    SEM_FAILCRITICALERRORS = 1,

	//    /// <summary>
	//    /// The system does not display the general-protection-fault message box. 
	//    /// This flag should only be set by debugging applications that handle 
	//    /// general protection (GP) faults themselves with an exception handler
	//    /// </summary>
	//    SEM_NOGPFAULTERRORBOX = 2,

	//    /// <summary>
	//    /// After this value is set for a process, subsequent attempts to clear 
	//    /// the value are ignored. 64-bit Windows:  The system automatically fixes 
	//    /// memory alignment faults and makes them invisible to the application. 
	//    /// It does this for the calling process and any descendant processes
	//    /// </summary>
	//    SEM_NOALIGNMENTFAULTEXCEPT = 4,

	//    /// <summary>
	//    /// The system does not display a message box when it fails to find a 
	//    /// file. Instead, the error is returned to the calling process
	//    /// </summary>
	//    SEM_NOOPENFILEERRORBOX = 32768
	//}


	//[Serializable]
	//public enum ShareFileAccess : uint {
	//    All = 0x10000000,
	//    Execute = 0x20000000,
	//    Write = 0x40000000,
	//    Read = 0x80000000,
	//};
}
