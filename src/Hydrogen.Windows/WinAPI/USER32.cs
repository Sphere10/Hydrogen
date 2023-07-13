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
using System.Text;

namespace Hydrogen.Windows;

public static partial class WinAPI {

	public static class USER32 {
		// ReSharper disable InconsistentNaming

		#region Constants

		public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		public static readonly IntPtr HWND_TOP = new IntPtr(0);
		public const int WHEEL_DELTA = 120;

		#endregion

		#region Delegates

		public delegate IntPtr HookProc(Int32 code, IntPtr wParam, IntPtr lParam);


		public delegate IntPtr WndProcDelegate(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		#endregion

		#region Enums & Flags

		[Flags]
		public enum MouseEventDataXButtons : uint {

			Nothing = 0x00000000,
			XBUTTON1 = 0x00000001,
			XBUTTON2 = 0x00000002

		}


		[Flags]
		public enum MOUSEEVENTF : uint {
			ABSOLUTE = 0x8000,
			HWHEEL = 0x01000,
			MOVE = 0x0001,
			MOVE_NOCOALESCE = 0x2000,
			LEFTDOWN = 0x0002,
			LEFTUP = 0x0004,
			RIGHTDOWN = 0x0008,
			RIGHTUP = 0x0010,
			MIDDLEDOWN = 0x0020,
			MIDDLEUP = 0x0040,
			VIRTUALDESK = 0x4000,
			WHEEL = 0x0800,
			XDOWN = 0x0080,
			XUP = 0x0100
		}


		[Flags]
		public enum KEYEVENTF : uint {
			EXTENDEDKEY = 0x0001,
			KEYUP = 0x0002,
			SCANCODE = 0x0008,
			UNICODE = 0x0004
		}


		//public const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
		//public const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
		//public const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
		//public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
		//public const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
		//public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
		//public const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
		//public const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
		//public const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
		//public const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
		//public const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
		//public const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

		//public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
		//public const int KEYEVENTF_KEYUP = 0x0002;
		//public const int KEYEVENTF_UNICODE = 0x0004;
		//public const int KEYEVENTF_SCANCODE = 0x0008;


		public enum INPUT_TYPE : uint {
			INPUT_MOUSE = 0,
			INPUT_KEYBOARD = 1,
			INPUT_HARDWARE = 2
		}


		[Flags]
		public enum WM : uint {
			MOUSEMOVE = 0x0200,
			LBUTTONDOWN = 0x0201,
			RBUTTONDOWN = 0x0204,
			MBUTTONDOWN = 0x0207,
			XBUTTONDOWN = 0x020B,
			LBUTTONUP = 0x0202,
			RBUTTONUP = 0x0205,
			MBUTTONUP = 0x0208,
			XBUTTONUP = 0x020C,
			LBUTTONDBLCLK = 0x0203,
			RBUTTONDBLCLK = 0x0206,
			MBUTTONDBLCLK = 0x0209,
			XBUTTONDBLCLK = 0x020D,
			MOUSEWHEEL = 0x020A,
			KEYDOWN = 0x0100,
			KEYUP = 0x0101,
			SYSKEYDOWN = 0x0104,
			SYSKEYUP = 0x0105,
			QUERYENDSESSION = 0x11,
		}


		[Flags]
		public enum BlendOps : byte {
			AC_SRC_OVER = 0x00,
			AC_SRC_ALPHA = 0x01,
			AC_SRC_NO_PREMULT_ALPHA = 0x01,
			AC_SRC_NO_ALPHA = 0x02,
			AC_DST_NO_PREMULT_ALPHA = 0x10,
			AC_DST_NO_ALPHA = 0x20
		}


		[Flags]
		public enum BlendFlags : uint {
			None = 0x00,
			ULW_COLORKEY = 0x01,
			ULW_ALPHA = 0x02,
			ULW_OPAQUE = 0x04
		}


		public enum ShowWindowCommands {
			/// <summary>
			/// Hides the window and activates another window.
			/// </summary>
			Hide = 0,

			/// <summary>
			/// Activates and displays a window. If the window is minimized or 
			/// maximized, the system restores it to its original size and position.
			/// An application should specify this flag when displaying the window 
			/// for the first time.
			/// </summary>
			Normal = 1,

			/// <summary>
			/// Activates the window and displays it as a minimized window.
			/// </summary>
			ShowMinimized = 2,

			/// <summary>
			/// Maximizes the specified window.
			/// </summary>
			Maximize = 3, // is this the right value?

			/// <summary>
			/// Activates the window and displays it as a maximized window.
			/// </summary>       
			ShowMaximized = 3,

			/// <summary>
			/// Displays a window in its most recent size and position. This value 
			/// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
			/// the window is not activated.
			/// </summary>
			ShowNoActivate = 4,

			/// <summary>
			/// Activates the window and displays it in its current size and position. 
			/// </summary>
			Show = 5,

			/// <summary>
			/// Minimizes the specified window and activates the next top-level 
			/// window in the Z order.
			/// </summary>
			Minimize = 6,

			/// <summary>
			/// Displays the window as a minimized window. This value is similar to
			/// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
			/// window is not activated.
			/// </summary>
			ShowMinNoActive = 7,

			/// <summary>
			/// Displays the window in its current size and position. This value is 
			/// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
			/// window is not activated.
			/// </summary>
			ShowNA = 8,

			/// <summary>
			/// Activates and displays the window. If the window is minimized or 
			/// maximized, the system restores it to its original size and position. 
			/// An application should specify this flag when restoring a minimized window.
			/// </summary>
			Restore = 9,

			/// <summary>
			/// Sets the show state based on the SW_* value specified in the 
			/// STARTUPINFO structure passed to the CreateProcess function by the 
			/// program that started the application.
			/// </summary>
			ShowDefault = 10,

			/// <summary>
			///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
			/// that owns the window is not responding. This flag should only be 
			/// used when minimizing windows from a different thread.
			/// </summary>
			ForceMinimize = 11
		}


		[Flags]
		public enum DeferWindowPosCommands : uint {
			///<Summary>Draws a frame (defined in the window's class description) around the window.</Summary>
			SWP_DRAWFRAME = 0x0020,

			///<Summary>Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.</Summary>
			SWP_FRAMECHANGED = 0x0020,

			///<Summary>Hides the window.</Summary>
			SWP_HIDEWINDOW = 0x0080,

			///<Summary>Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).</Summary>
			SWP_NOACTIVATE = 0x0010,

			///<Summary>Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.</Summary>
			SWP_NOCOPYBITS = 0x0100,

			///<Summary>Retains the current position (ignores the x and y parameters).</Summary>
			SWP_NOMOVE = 0x0002,

			///<Summary>Does not change the owner window's position in the Z order.</Summary>
			SWP_NOOWNERZORDER = 0x0200,

			///<Summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</Summary>
			SWP_NOREDRAW = 0x0008,

			///<Summary>Same as the SWP_NOOWNERZORDER flag.</Summary>
			SWP_NOREPOSITION = 0x0200,

			///<Summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</Summary>
			SWP_NOSENDCHANGING = 0x0400,

			///<Summary>Retains the current size (ignores the cx and cy parameters).</Summary>
			SWP_NOSIZE = 0x0001,

			///<Summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</Summary>
			SWP_NOZORDER = 0x0004,

			///<Summary>Displays the window.</Summary>
			SWP_SHOWWINDOW = 0x0040
		}


		public enum GetWindow_Cmd : uint {
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4,
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6
		}


		public enum SystemCursor : int {
			IDC_APPSTARTING = 32650, //Standard arrow and small hourglass
			IDC_NORMAL = 32512, //Standard arrow
			IDC_CROSS = 32515, //Crosshair
			IDC_HAND = 32649, //Hand
			IDC_HELP = 32651, //Arrow and question mark
			IDC_IBEAM = 32513, //I-beam
			IDC_NO = 32648, //Slashed circle
			IDC_SIZEALL = 32646, //Four-pointed arrow pointing north, south, east, and west
			IDC_SIZENESW = 32643, //Double-pointed arrow pointing northeast and southwest
			IDC_SIZENS = 32645, //Double-pointed arrow pointing north and south
			IDC_SIZENWSE = 32642, //Double-pointed arrow pointing northwest and southeast
			IDC_SIZEWE = 32644, //Double-pointed arrow pointing west and east
			IDC_UP = 32516, //Vertical arrow
			IDC_WAIT = 32514 //Hourglass
		}


		[Flags]
		public enum SetWindowPosFlags : uint {

			/// <summary>
			///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
			/// </summary>
			SWP_ASYNCWINDOWPOS = 0x4000,

			/// <summary>
			///     Prevents generation of the WM_SYNCPAINT message.
			/// </summary>
			SWP_DEFERERASE = 0x2000,

			/// <summary>
			///     Draws a frame (defined in the window's class description) around the window.
			/// </summary>
			SWP_DRAWFRAME = 0x0020,

			/// <summary>
			///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
			/// </summary>
			SWP_FRAMECHANGED = 0x0020,

			/// <summary>
			///     Hides the window.
			/// </summary>
			SWP_HIDEWINDOW = 0x0080,

			/// <summary>
			///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
			/// </summary>
			SWP_NOACTIVATE = 0x0010,

			/// <summary>
			///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
			/// </summary>
			SWP_NOCOPYBITS = 0x0100,

			/// <summary>
			///     Retains the current position (ignores X and Y parameters).
			/// </summary>
			SWP_NOMOVE = 0x0002,

			/// <summary>
			///     Does not change the owner window's position in the Z order.
			/// </summary>
			SWP_NOOWNERZORDER = 0x0200,

			/// <summary>
			///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
			/// </summary>
			SWP_NOREDRAW = 0x0008,

			/// <summary>
			///     Same as the SWP_NOOWNERZORDER flag.
			/// </summary>
			SWP_NOREPOSITION = 0x0200,

			/// <summary>
			///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
			/// </summary>
			SWP_NOSENDCHANGING = 0x0400,

			/// <summary>
			///     Retains the current size (ignores the cx and cy parameters).
			/// </summary>
			SWP_NOSIZE = 0x0001,

			/// <summary>
			///     Retains the current Z order (ignores the hWndInsertAfter parameter).
			/// </summary>
			SWP_NOZORDER = 0x0004,

			/// <summary>
			///     Displays the window.
			/// </summary>
			SWP_SHOWWINDOW = 0x0040,

		}


		public enum HookType : int {
			WH_JOURNALRECORD = 0,
			WH_JOURNALPLAYBACK = 1,
			WH_KEYBOARD = 2,
			WH_GETMESSAGE = 3,
			WH_CALLWNDPROC = 4,
			WH_CBT = 5,
			WH_SYSMSGFILTER = 6,
			WH_MOUSE = 7,
			WH_HARDWARE = 8,
			WH_DEBUG = 9,
			WH_SHELL = 10,
			WH_FOREGROUNDIDLE = 11,
			WH_CALLWNDPROCRET = 12,
			WH_KEYBOARD_LL = 13,
			WH_MOUSE_LL = 14
		}


		public enum SystemMetric : int {
			/// <summary>
			///  Width of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
			/// </summary>
			SM_CXSCREEN = 0,

			/// <summary>
			/// Height of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
			/// </summary>
			SM_CYSCREEN = 1,

			/// <summary>
			/// Height of the arrow bitmap on a vertical scroll bar, in pixels.
			/// </summary>
			SM_CYVSCROLL = 20,

			/// <summary>
			/// Width of a vertical scroll bar, in pixels.
			/// </summary>
			SM_CXVSCROLL = 2,

			/// <summary>
			/// Height of a caption area, in pixels.
			/// </summary>
			SM_CYCAPTION = 4,

			/// <summary>
			/// Width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look.
			/// </summary>
			SM_CXBORDER = 5,

			/// <summary>
			/// Height of a window border, in pixels. This is equivalent to the SM_CYEDGE value for windows with the 3-D look. 
			/// </summary>
			SM_CYBORDER = 6,

			/// <summary>
			/// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border. 
			/// </summary>
			SM_CXDLGFRAME = 7,

			/// <summary>
			/// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border. 
			/// </summary>
			SM_CYDLGFRAME = 8,

			/// <summary>
			/// Height of the thumb box in a vertical scroll bar, in pixels
			/// </summary>
			SM_CYVTHUMB = 9,

			/// <summary>
			/// Width of the thumb box in a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CXHTHUMB = 10,

			/// <summary>
			/// Default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions specified by SM_CXICON and SM_CYICON
			/// </summary>
			SM_CXICON = 11,

			/// <summary>
			/// Default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions SM_CXICON and SM_CYICON.
			/// </summary>
			SM_CYICON = 12,

			/// <summary>
			/// Width of a cursor, in pixels. The system cannot create cursors of other sizes.
			/// </summary>
			SM_CXCURSOR = 13,

			/// <summary>
			/// Height of a cursor, in pixels. The system cannot create cursors of other sizes.
			/// </summary>
			SM_CYCURSOR = 14,

			/// <summary>
			/// Height of a single-line menu bar, in pixels.
			/// </summary>
			SM_CYMENU = 15,

			/// <summary>
			/// Width of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			SM_CXFULLSCREEN = 16,

			/// <summary>
			/// Height of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			SM_CYFULLSCREEN = 17,

			/// <summary>
			/// For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the screen, in pixels
			/// </summary>
			SM_CYKANJIWINDOW = 18,

			/// <summary>
			/// Nonzero if a mouse with a wheel is installed; zero otherwise
			/// </summary>
			SM_MOUSEWHEELPRESENT = 75,

			/// <summary>
			/// Height of a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CYHSCROLL = 3,

			/// <summary>
			/// Width of the arrow bitmap on a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CXHSCROLL = 21,

			/// <summary>
			/// Nonzero if the debug version of User.exe is installed; zero otherwise.
			/// </summary>
			SM_DEBUG = 22,

			/// <summary>
			/// Nonzero if the left and right mouse buttons are reversed; zero otherwise.
			/// </summary>
			SM_SWAPBUTTON = 23,

			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED1 = 24,

			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED2 = 25,

			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED3 = 26,

			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED4 = 27,

			/// <summary>
			/// Minimum width of a window, in pixels.
			/// </summary>
			SM_CXMIN = 28,

			/// <summary>
			/// Minimum height of a window, in pixels.
			/// </summary>
			SM_CYMIN = 29,

			/// <summary>
			/// Width of a button in a window's caption or title bar, in pixels.
			/// </summary>
			SM_CXSIZE = 30,

			/// <summary>
			/// Height of a button in a window's caption or title bar, in pixels.
			/// </summary>
			SM_CYSIZE = 31,

			/// <summary>
			/// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
			/// </summary>
			SM_CXFRAME = 32,

			/// <summary>
			/// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
			/// </summary>
			SM_CYFRAME = 33,

			/// <summary>
			/// Minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
			/// </summary>
			SM_CXMINTRACK = 34,

			/// <summary>
			/// Minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message
			/// </summary>
			SM_CYMINTRACK = 35,

			/// <summary>
			/// Width of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click
			/// </summary>
			SM_CXDOUBLECLK = 36,

			/// <summary>
			/// Height of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click. (The two clicks must also occur within a specified time.) 
			/// </summary>
			SM_CYDOUBLECLK = 37,

			/// <summary>
			/// Width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CXICON
			/// </summary>
			SM_CXICONSPACING = 38,

			/// <summary>
			/// Height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CYICON.
			/// </summary>
			SM_CYICONSPACING = 39,

			/// <summary>
			/// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; zero if the menus are left-aligned.
			/// </summary>
			SM_MENUDROPALIGNMENT = 40,

			/// <summary>
			/// Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
			/// </summary>
			SM_PENWINDOWS = 41,

			/// <summary>
			/// Nonzero if User32.dll supports DBCS; zero otherwise. (WinMe/95/98): Unicode
			/// </summary>
			SM_DBCSENABLED = 42,

			/// <summary>
			/// Number of buttons on mouse, or zero if no mouse is installed.
			/// </summary>
			SM_CMOUSEBUTTONS = 43,

			/// <summary>
			/// Identical Values Changed After Windows NT 4.0  
			/// </summary>
			SM_CXFIXEDFRAME = SM_CXDLGFRAME,

			/// <summary>
			/// Identical Values Changed After Windows NT 4.0
			/// </summary>
			SM_CYFIXEDFRAME = SM_CYDLGFRAME,

			/// <summary>
			/// Identical Values Changed After Windows NT 4.0
			/// </summary>
			SM_CXSIZEFRAME = SM_CXFRAME,

			/// <summary>
			/// Identical Values Changed After Windows NT 4.0
			/// </summary>
			SM_CYSIZEFRAME = SM_CYFRAME,

			/// <summary>
			/// Nonzero if security is present; zero otherwise.
			/// </summary>
			SM_SECURE = 44,

			/// <summary>
			/// Width of a 3-D border, in pixels. This is the 3-D counterpart of SM_CXBORDER
			/// </summary>
			SM_CXEDGE = 45,

			/// <summary>
			/// Height of a 3-D border, in pixels. This is the 3-D counterpart of SM_CYBORDER
			/// </summary>
			SM_CYEDGE = 46,

			/// <summary>
			/// Width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CXMINIMIZED.
			/// </summary>
			SM_CXMINSPACING = 47,

			/// <summary>
			/// Height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CYMINIMIZED.
			/// </summary>
			SM_CYMINSPACING = 48,

			/// <summary>
			/// Recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon view
			/// </summary>
			SM_CXSMICON = 49,

			/// <summary>
			/// Recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
			/// </summary>
			SM_CYSMICON = 50,

			/// <summary>
			/// Height of a small caption, in pixels
			/// </summary>
			SM_CYSMCAPTION = 51,

			/// <summary>
			/// Width of small caption buttons, in pixels.
			/// </summary>
			SM_CXSMSIZE = 52,

			/// <summary>
			/// Height of small caption buttons, in pixels.
			/// </summary>
			SM_CYSMSIZE = 53,

			/// <summary>
			/// Width of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
			/// </summary>
			SM_CXMENUSIZE = 54,

			/// <summary>
			/// Height of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
			/// </summary>
			SM_CYMENUSIZE = 55,

			/// <summary>
			/// Flags specifying how the system arranged minimized windows
			/// </summary>
			SM_ARRANGE = 56,

			/// <summary>
			/// Width of a minimized window, in pixels.
			/// </summary>
			SM_CXMINIMIZED = 57,

			/// <summary>
			/// Height of a minimized window, in pixels.
			/// </summary>
			SM_CYMINIMIZED = 58,

			/// <summary>
			/// Default maximum width of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
			/// </summary>
			SM_CXMAXTRACK = 59,

			/// <summary>
			/// Default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
			/// </summary>
			SM_CYMAXTRACK = 60,

			/// <summary>
			/// Default width, in pixels, of a maximized top-level window on the primary display monitor.
			/// </summary>
			SM_CXMAXIMIZED = 61,

			/// <summary>
			/// Default height, in pixels, of a maximized top-level window on the primary display monitor.
			/// </summary>
			SM_CYMAXIMIZED = 62,

			/// <summary>
			/// Least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for future use
			/// </summary>
			SM_NETWORK = 63,

			/// <summary>
			/// Value that specifies how the system was started: 0-normal, 1-failsafe, 2-failsafe /w net
			/// </summary>
			SM_CLEANBOOT = 67,

			/// <summary>
			/// Width of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins, in pixels. 
			/// </summary>
			SM_CXDRAG = 68,

			/// <summary>
			/// Height of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins. This value is in pixels. It allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
			/// </summary>
			SM_CYDRAG = 69,

			/// <summary>
			/// Nonzero if the user requires an application to present information visually in situations where it would otherwise present the information only in audible form; zero otherwise. 
			/// </summary>
			SM_SHOWSOUNDS = 70,

			/// <summary>
			/// Width of the default menu check-mark bitmap, in pixels.
			/// </summary>
			SM_CXMENUCHECK = 71,

			/// <summary>
			/// Height of the default menu check-mark bitmap, in pixels.
			/// </summary>
			SM_CYMENUCHECK = 72,

			/// <summary>
			/// Nonzero if the computer has a low-end (slow) processor; zero otherwise
			/// </summary>
			SM_SLOWMACHINE = 73,

			/// <summary>
			/// Nonzero if the system is enabled for Hebrew and Arabic languages, zero if not.
			/// </summary>
			SM_MIDEASTENABLED = 74,

			/// <summary>
			/// Nonzero if a mouse is installed; zero otherwise. This value is rarely zero, because of support for virtual mice and because some systems detect the presence of the port instead of the presence of a mouse.
			/// </summary>
			SM_MOUSEPRESENT = 19,

			/// <summary>
			/// Windows 2000 (v5.0+) Coordinate of the top of the virtual screen
			/// </summary>
			SM_XVIRTUALSCREEN = 76,

			/// <summary>
			/// Windows 2000 (v5.0+) Coordinate of the left of the virtual screen
			/// </summary>
			SM_YVIRTUALSCREEN = 77,

			/// <summary>
			/// Windows 2000 (v5.0+) Width of the virtual screen
			/// </summary>
			SM_CXVIRTUALSCREEN = 78,

			/// <summary>
			/// Windows 2000 (v5.0+) Height of the virtual screen
			/// </summary>
			SM_CYVIRTUALSCREEN = 79,

			/// <summary>
			/// Number of display monitors on the desktop
			/// </summary>
			SM_CMONITORS = 80,

			/// <summary>
			/// Windows XP (v5.1+) Nonzero if all the display monitors have the same color format, zero otherwise. Note that two displays can have the same bit depth, but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of bits, or those bits can be located in different places in a pixel's color value. 
			/// </summary>
			SM_SAMEDISPLAYFORMAT = 81,

			/// <summary>
			/// Windows XP (v5.1+) Nonzero if Input Method Manager/Input Method Editor features are enabled; zero otherwise
			/// </summary>
			SM_IMMENABLED = 82,

			/// <summary>
			/// Windows XP (v5.1+) Width of the left and right edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels. 
			/// </summary>
			SM_CXFOCUSBORDER = 83,

			/// <summary>
			/// Windows XP (v5.1+) Height of the top and bottom edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels. 
			/// </summary>
			SM_CYFOCUSBORDER = 84,

			/// <summary>
			/// Nonzero if the current operating system is the Windows XP Tablet PC edition, zero if not.
			/// </summary>
			SM_TABLETPC = 86,

			/// <summary>
			/// Nonzero if the current operating system is the Windows XP, Media Center Edition, zero if not.
			/// </summary>
			SM_MEDIACENTER = 87,

			/// <summary>
			/// Metrics Other
			/// </summary>
			SM_CMETRICS_OTHER = 76,

			/// <summary>
			/// Metrics Windows 2000
			/// </summary>
			SM_CMETRICS_2000 = 83,

			/// <summary>
			/// Metrics Windows NT
			/// </summary>
			SM_CMETRICS_NT = 88,

			/// <summary>
			/// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services client session, the return value is nonzero. If the calling process is associated with the Terminal Server console session, the return value is zero. The console session is not necessarily the physical console - see WTSGetActiveConsoleSessionId for more information. 
			/// </summary>
			SM_REMOTESESSION = 0x1000,

			/// <summary>
			/// Windows XP (v5.1+) Nonzero if the current session is shutting down; zero otherwise
			/// </summary>
			SM_SHUTTINGDOWN = 0x2000,

			/// <summary>
			/// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. Its value is nonzero if the current session is remotely controlled; zero otherwise
			/// </summary>
			SM_REMOTECONTROL = 0x2001,
		}


		[Flags]
		public enum DrawTextFlags : uint {
			// ReSharper disable InconsistentNaming
			/// <summary>
			/// Justifies the text to the top of the rectangle.
			/// </summary>
			DT_TOP = 0x00000000,

			/// <summary>
			/// Aligns text to the left.
			/// </summary>
			DT_LEFT = 0x00000000,

			/// <summary>
			/// Centers text horizontally in the rectangle
			/// </summary>
			DT_CENTER = 0x00000001,

			/// <summary>
			/// Aligns text to the right
			/// </summary>
			DT_RIGHT = 0x00000002,

			/// <summary>
			/// Centers text vertically. This value is used only with the DT_SINGLELINE value
			/// </summary>
			DT_VCENTER = 0x00000004,

			/// <summary>
			/// Justifies the text to the bottom of the rectangle. This value is used 
			/// only with the DT_SINGLELINE value
			/// </summary>
			DT_BOTTOM = 0x00000008,

			/// <summary>
			/// Breaks words. Lines are automatically broken between words if a word would 
			/// extend past the edge of the rectangle specified by the lpRect parameter. A 
			/// carriage return-line feed sequence also breaks the line. If this is not 
			/// specified, output is on one line
			/// </summary>
			DT_WORDBREAK = 0x00000010,

			/// <summary>
			/// Displays text on a single line only. Carriage returns and line feeds do not 
			/// break the line
			/// </summary>
			DT_SINGLELINE = 0x00000020,

			/// <summary>
			/// Expands tab characters. The default number of characters per tab is eight. 
			/// The DT_WORD_ELLIPSIS, DT_PATH_ELLIPSIS, and DT_END_ELLIPSIS values cannot be 
			/// used with the DT_EXPANDTABS value
			/// </summary>
			DT_EXPANDTABS = 0x00000040,

			/// <summary>
			/// Sets tab stops. Bits 15–8 (high-order byte of the low-order word) of the uFormat 
			/// parameter specify the number of characters for each tab. The default number of 
			/// characters per tab is eight. The DT_CALCRECT, DT_EXTERNALLEADING, DT_public, 
			/// DT_NOCLIP, and DT_NOPREFIX values cannot be used with the DT_TABSTOP value
			/// </summary>
			DT_TABSTOP = 0x00000080,

			/// <summary>
			/// Draws without clipping. DrawText is somewhat faster when DT_NOCLIP is used
			/// </summary>
			DT_NOCLIP = 0x00000100,

			/// <summary>
			/// Includes the font external leading in line height. Normally, external leading 
			/// is not included in the height of a line of text
			/// </summary>
			DT_EXTERNALLEADING = 0x00000200,

			/// <summary>
			/// Determines the width and height of the rectangle. If there are multiple lines 
			/// of text, DrawText uses the width of the rectangle pointed to by the lpRect 
			/// parameter and extends the base of the rectangle to bound the last line of text. 
			/// If the largest word is wider than the rectangle, the width is expanded. If the 
			/// text is less than the width of the rectangle, the width is reduced. If there is 
			/// only one line of text, DrawText modifies the right side of the rectangle so that 
			/// it bounds the last character in the line. In either case, DrawText returns the 
			/// height of the formatted text but does not draw the text
			/// </summary>
			DT_CALCRECT = 0x00000400,

			/// <summary>
			/// Turns off processing of prefix characters. Normally, DrawText interprets the 
			/// mnemonic-prefix character &amp; as a directive to underscore the character that 
			/// follows, and the mnemonic-prefix characters &amp;&amp; as a directive to print a 
			/// single &amp;. By specifying DT_NOPREFIX, this processing is turned off
			/// </summary>
			DT_NOPREFIX = 0x00000800,

			/// <summary>
			/// Uses the system font to calculate text metrics
			/// </summary>
			DT_public = 0x00001000,

			/// <summary>
			/// Duplicates the text-displaying characteristics of a multiline edit control. 
			/// Specifically, the average character width is calculated in the same manner as 
			/// for an edit control, and the function does not display a partially visible last 
			/// line
			/// </summary>
			DT_EDITCONTROL = 0x00002000,

			/// <summary>
			/// For displayed text, replaces characters in the middle of the string with ellipses 
			/// so that the result fits in the specified rectangle. If the string contains backslash 
			/// (\) characters, DT_PATH_ELLIPSIS preserves as much as possible of the text after 
			/// the last backslash. The string is not modified unless the DT_MODIFYSTRING flag is 
			/// specified
			/// </summary>
			DT_PATH_ELLIPSIS = 0x00004000,

			/// <summary>
			/// For displayed text, if the end of a string does not fit in the rectangle, it is 
			/// truncated and ellipses are added. If a word that is not at the end of the string 
			/// goes beyond the limits of the rectangle, it is truncated without ellipses. The 
			/// string is not modified unless the DT_MODIFYSTRING flag is specified
			/// </summary>
			DT_END_ELLIPSIS = 0x00008000,

			/// <summary>
			/// Modifies the specified string to match the displayed text. This value has no effect 
			/// unless DT_END_ELLIPSIS or DT_PATH_ELLIPSIS is specified
			/// </summary>
			DT_MODIFYSTRING = 0x00010000,

			/// <summary>
			/// Layout in right-to-left reading order for bi-directional text when the font selected 
			/// into the hdc is a Hebrew or Arabic font. The default reading order for all text is 
			/// left-to-right
			/// </summary>
			DT_RTLREADING = 0x00020000,

			/// <summary>
			/// Truncates any word that does not fit in the rectangle and adds ellipses
			/// </summary>
			DT_WORD_ELLIPSIS = 0x00040000
			// ReSharper restore InconsistentNaming
		}

		#endregion

		#region Structs

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;

			public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format) {
				BlendOp = op;
				BlendFlags = flags;
				SourceConstantAlpha = alpha;
				AlphaFormat = format;
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT {
			public int dx;
			public int dy;
			public MouseEventDataXButtons mouseData;
			public MOUSEEVENTF dwFlags;
			public uint time;
			public UIntPtr dwExtraInfo;
		}


		//[StructLayout(LayoutKind.Sequential)]
		//public struct KEYBOARDINPUT {
		//    public short wVk;
		//    public short wScan;
		//    public int dwFlags;
		//    public int time;
		//    public int dwExtraInfo;
		//}


		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBOARDINPUT {
			public VirtualKey wVk;
			public ScanCode wScan;
			public KEYEVENTF dwFlags;
			public int time;
			public UIntPtr dwExtraInfo;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT {
			public int uMsg;
			public short wParamL;
			public short wParamH;
		}


		[StructLayout(LayoutKind.Explicit)]
		public struct DEVICEINPUTUNION {
			[FieldOffset(0)] internal MOUSEINPUT mi;
			[FieldOffset(0)] internal KEYBOARDINPUT ki;
			[FieldOffset(0)] internal HARDWAREINPUT hi;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT {
			internal INPUT_TYPE type;

			// DEVICEINPUTUNION is used to avoid explicit field offset specifying, since changes between x32 and x64
			// e.g. in x32, would be FieldOffset(4) but in x64 would be FieldOffset(8). This approach works on both.
			internal DEVICEINPUTUNION di;

			public static int Size {
				get { return Marshal.SizeOf(typeof(INPUT)); }
			}
		}


		//[StructLayout(LayoutKind.Sequential)]
		//public struct MOUSEINPUT {
		//    public int dx;
		//    public int dy;
		//    public int mouseData;
		//    public int dwFlags;
		//    public int time;
		//    public int dwExtraInfo;
		//}


		//Declare wrapper managed MouseHookStruct class.
		[StructLayout(LayoutKind.Sequential)]
		public class MouseHookStruct {
			public POINT pt;
			public int mouseData;
			public int hwnd;
			public int wHitTestCode;
			public int dwExtraInfo;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct MSLLHOOKSTRUCT {
			public POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}


		//Declare wrapper managed KeyboardHookStruct class.
		[StructLayout(LayoutKind.Sequential)]
		public class KeyboardHookStruct {
			public uint vkCode; //Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
			public uint scanCode; // Specifies a hardware scan code for the key. 
			public uint flags; // Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
			public uint time; // Specifies the time stamp for this message.
			public IntPtr dwExtraInfo; // Specifies extra information associated with the message. 
		}

		#endregion

		#region USER32.DLL Functions

		[DllImport("User32.Dll", EntryPoint = "PostMessageA")]
		public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SwapMouseButton([param: MarshalAs(UnmanagedType.Bool)] bool fSwap);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, UInt32 crKey, [In] ref BLENDFUNCTION pblend, BlendFlags dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		public static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

		public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong) {
			return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
		}

		///  HOW TO DEAL WITH THIS IN 64bit?
		//[DllImport("user32")]
		//public static extern IntPtr SetWindowLong(IntPtr hWnd, uint nIndex, Windows.Win32.Win32WndProc newProc);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CallWindowProc(WndProcDelegate lpPrevWndFunc, IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LockWindowUpdate(IntPtr hWndLock);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowCommands nCmdShow);

		[DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
		public static extern long GetWindowLong32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
		public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) {
			return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : new IntPtr(GetWindowLong32(hWnd, nIndex));
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, [MarshalAs(UnmanagedType.U4)] DeferWindowPosCommands uFlags);


		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);


		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);


		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetCursor(IntPtr hCursor);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr LoadCursor(IntPtr hInstcance, SystemCursor hcur);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hwnd, [param: MarshalAs(UnmanagedType.U4)] WindowMessageFlags msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hwnd, [param: MarshalAs(UnmanagedType.U4)] WindowMessageFlags msg, IntPtr wParam, string lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, string lParam);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetCapture();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, UInt32 dwThreadId);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int ToAscii(UInt32 uVirtKey, UInt32 uScanCode, byte[] lpKeyState, byte[] lpwTransKey, UInt32 fuState);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int ToAscii(UInt32 uVirtKey, UInt32 uScanCode, byte[] lpKeyState, [Out] StringBuilder lpChar, UInt32 uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetKeyboardState(byte[] lpKeyState);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern UInt32 SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 lpdwProcessId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr LoadBitmap(IntPtr hInstance, string lpBitmapName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr LoadBitmap(IntPtr hInstance, int lpBitmapName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetSystemMetrics(SystemMetric smIndex);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Int32 ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern Int32 LoadString(IntPtr hInstance, UInt32 uID, StringBuilder lpBuffer, int nBufferMax);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CreateIconFromResourceEx(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion, int cxDesired, int cyDesired, uint uFlags);

		[DllImport("User32.dll", SetLastError = true)]
		public static extern unsafe IntPtr CreateIconFromResourceEx(byte* pbIconBits, uint cbIconBits, bool fIcon, int dwVersion, int csDesired, int cyDesired, uint flags);

		[DllImport("user32.dll")]
		public static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, DrawTextFlags uFormat);

		#endregion

		// ReSharper restore InconsistentNaming
	}

}
