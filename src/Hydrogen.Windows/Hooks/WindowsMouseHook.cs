// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Hydrogen.Windows;

public sealed class WindowsMouseHook : BaseMouseHook, IComponent {
	public new event EventHandler Disposed;
	public static readonly TimeSpan DefaultMovingStoppedInterval;
	private IntPtr _hMouseHook;
	private WinAPI.USER32.HookProc _mouseHookProcedure;

	static WindowsMouseHook() {
		DefaultMovingStoppedInterval = TimeSpan.FromMilliseconds(100);
	}

	public WindowsMouseHook()
		: this(new PollDrivenActiveApplicationMonitor()) {
	}

	public WindowsMouseHook(TimeSpan movingStoppedInterval)
		: this(new PollDrivenActiveApplicationMonitor(), movingStoppedInterval) {
	}

	public WindowsMouseHook(IActiveApplicationMonitor activeApplicationMonitor)
		: this(activeApplicationMonitor, DefaultMovingStoppedInterval) {
	}

	public WindowsMouseHook(IActiveApplicationMonitor activeApplicationMonitor, TimeSpan movingStoppedInterval)
		: base(activeApplicationMonitor, movingStoppedInterval) {
		_hMouseHook = IntPtr.Zero;
	}

	public override void InstallHook() {
		if (Status != DeviceHookStatus.Uninstalled && !Disposing) {
			throw new SoftwareException("Mouse hook has already been installed. Current Status is '{0}'", Status);
		}

		if (_hMouseHook == IntPtr.Zero) {
			using (Process process = Process.GetCurrentProcess()) {
				// Keep the delegate as a field to prevent garbage collection
				_mouseHookProcedure = new WinAPI.USER32.HookProc(MouseHookProc);
				_hMouseHook = WinAPI.USER32.SetWindowsHookEx(
					WinAPI.USER32.HookType.WH_MOUSE_LL,
					_mouseHookProcedure,
					//Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0])   // NOTE: 2021-08-03 removed since .NET 4+ and Core 3+ use new approach https://www.programmersought.com/article/51595908521/
					WinAPI.KERNEL32.GetModuleHandle(null),
					0
				);

				if (_hMouseHook == IntPtr.Zero && !Disposing) {
					throw new WindowsException(WinAPI.KERNEL32.GetLastError(), "Windows did not allow the application to install a mouse hook.");
				}
			}
		}
		Status = DeviceHookStatus.Disabled;
	}

	public override void UninstallHook() {
		if (Status == DeviceHookStatus.Uninstalled && !Disposing) {
			throw new SoftwareException("Mouse hook has not installed. Current Status is '{0}'", Status);
		}
		if (_hMouseHook != IntPtr.Zero) {
			if (!WinAPI.USER32.UnhookWindowsHookEx(_hMouseHook) && !Disposing) {
				throw new WindowsException(WinAPI.KERNEL32.GetLastError(), "Windows did not allow the application to uninstall mouse hook.");
			}
		}
		_hMouseHook = IntPtr.Zero;
		_mouseHookProcedure = null;
		Status = DeviceHookStatus.Uninstalled;
	}


	public override void Simulate(MouseButton button, MouseButtonState buttonState, int screenX, int screenY) {
		HooksTool.SimulateMouseDeviceAction(button, buttonState, screenX, screenY);
	}

	private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
		var mouseMessages = (WinAPI.USER32.WM)wParam;
		if (Status == DeviceHookStatus.Active) {
			var oldPriority = Thread.CurrentThread.Priority;
			try {
				Thread.CurrentThread.Priority = ThreadPriority.Highest;

				// Fetch the low-level struct
				var mouseHookStruct = (WinAPI.USER32.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.USER32.MSLLHOOKSTRUCT));

				// Determine button that was clicked
				if (nCode >= 0) {
					var clickType = MouseClickType.None;
					var buttonClicked = MouseButton.None;
					var buttonState = MouseButtonState.None;
					int wheelDelta = 0;

					if ((mouseMessages & WinAPI.USER32.WM.MOUSEWHEEL) == WinAPI.USER32.WM.MOUSEWHEEL) {
						wheelDelta = Tools.WinTool.GET_WHEEL_DELTA_WPARAM(mouseHookStruct.mouseData) * WinAPI.USER32.WHEEL_DELTA;
					} else {
						if ((mouseMessages & WinAPI.USER32.WM.LBUTTONDOWN) == WinAPI.USER32.WM.LBUTTONDOWN) {
							buttonClicked = MouseButton.Left;
							buttonState = MouseButtonState.Down;
							clickType = (mouseMessages & WinAPI.USER32.WM.LBUTTONDBLCLK) == WinAPI.USER32.WM.LBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}

						if ((mouseMessages & WinAPI.USER32.WM.LBUTTONUP) == WinAPI.USER32.WM.LBUTTONUP) {
							buttonClicked = MouseButton.Left;
							buttonState = MouseButtonState.Up;
							clickType = (mouseMessages & WinAPI.USER32.WM.LBUTTONDBLCLK) == WinAPI.USER32.WM.LBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}

						if ((mouseMessages & WinAPI.USER32.WM.RBUTTONDOWN) == WinAPI.USER32.WM.RBUTTONDOWN) {
							buttonClicked = MouseButton.Right;
							buttonState = MouseButtonState.Down;
							clickType = (mouseMessages & WinAPI.USER32.WM.RBUTTONDBLCLK) == WinAPI.USER32.WM.RBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}

						if ((mouseMessages & WinAPI.USER32.WM.RBUTTONUP) == WinAPI.USER32.WM.RBUTTONUP) {
							buttonClicked = MouseButton.Right;
							buttonState = MouseButtonState.Up;
							clickType = (mouseMessages & WinAPI.USER32.WM.RBUTTONDBLCLK) == WinAPI.USER32.WM.RBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}


						if ((mouseMessages & WinAPI.USER32.WM.MBUTTONDOWN) == WinAPI.USER32.WM.MBUTTONDOWN) {
							buttonClicked = MouseButton.Middle;
							buttonState = MouseButtonState.Down;
							clickType = (mouseMessages & WinAPI.USER32.WM.MBUTTONDBLCLK) == WinAPI.USER32.WM.MBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}
						if ((mouseMessages & WinAPI.USER32.WM.MBUTTONUP) == WinAPI.USER32.WM.MBUTTONUP) {
							buttonClicked = MouseButton.Middle;
							buttonState = MouseButtonState.Up;
							clickType = (mouseMessages & WinAPI.USER32.WM.MBUTTONDBLCLK) == WinAPI.USER32.WM.MBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}

						if ((mouseMessages & WinAPI.USER32.WM.XBUTTONDOWN) == WinAPI.USER32.WM.XBUTTONDOWN) {
							buttonClicked = (Tools.WinTool.HIWORD(mouseHookStruct.mouseData) & (ushort)WinAPI.USER32.MouseEventDataXButtons.XBUTTON1) == (ushort)WinAPI.USER32.MouseEventDataXButtons.XBUTTON1
								? MouseButton.XButton1
								: MouseButton.XButton2;
							buttonState = MouseButtonState.Down;
							clickType = (mouseMessages & WinAPI.USER32.WM.XBUTTONDBLCLK) == WinAPI.USER32.WM.XBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}

						if ((mouseMessages & WinAPI.USER32.WM.XBUTTONUP) == WinAPI.USER32.WM.XBUTTONUP) {
							buttonClicked = (Tools.WinTool.HIWORD(mouseHookStruct.mouseData) & (ushort)WinAPI.USER32.MouseEventDataXButtons.XBUTTON2) == (ushort)WinAPI.USER32.MouseEventDataXButtons.XBUTTON2
								? MouseButton.XButton1
								: MouseButton.XButton2;
							buttonState = MouseButtonState.Up;
							clickType = (mouseMessages & WinAPI.USER32.WM.XBUTTONDBLCLK) == WinAPI.USER32.WM.XBUTTONDBLCLK ? MouseClickType.Double : MouseClickType.Single;
						}

					}

					// Base method will process rest of logic
					ContinueOutsideCriticalExecutionContext(
						() => base.ProcessMouseActivity(mouseHookStruct.pt.x, mouseHookStruct.pt.y, buttonClicked, buttonState, clickType, wheelDelta)
					);
				}
			} finally {
				Thread.CurrentThread.Priority = oldPriority;
			}
		}

		return WinAPI.USER32.CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
	}


	public ISite Site { get; set; }

}
