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

public sealed class WindowsKeyboardHook : BaseKeyboardHook, IComponent {
	public new event EventHandler Disposed;

	public WindowsKeyboardHook() : this(new PollDrivenActiveApplicationMonitor()) {
	}

	public WindowsKeyboardHook(IActiveApplicationMonitor activeApplicationMonitor)
		: base(activeApplicationMonitor) {
		_hKeyboardHook = IntPtr.Zero;
		_keyboardHookProcedure = null;
	}


	private IntPtr _hKeyboardHook;
	private WinAPI.USER32.HookProc _keyboardHookProcedure;


	public override void InstallHook() {
		lock (_syncObject) {
			if (Status != DeviceHookStatus.Uninstalled && !Disposing) {
				throw new SoftwareException("Keyboard hook has already been installed. Current Status is '{0}'", Status);
			}
			if (_hKeyboardHook == IntPtr.Zero) {
				using (var process = Process.GetCurrentProcess()) {
					//// Keep the delegate as a field to prevent garbage collection
					_keyboardHookProcedure = KeyboardHookProc;
					_hKeyboardHook = WinAPI.USER32.SetWindowsHookEx(
						WinAPI.USER32.HookType.WH_KEYBOARD_LL,
						_keyboardHookProcedure,
						//Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0])   // NOTE: 2021-08-03 removed since .NET 4+ and Core 3+ use new approach https://www.programmersought.com/article/51595908521/
						WinAPI.KERNEL32.GetModuleHandle(null),
						0
					);

					// If SetWindowsHookEx fails.
					if (_hKeyboardHook == IntPtr.Zero && !Disposing) {
						throw new WindowsException(WinAPI.KERNEL32.GetLastError(),
							"Windows did not allow the application to install a keyboard hook.");
					}
				}
			}
			Status = DeviceHookStatus.Disabled;
		}
	}

	public override void UninstallHook() {
		lock (_syncObject) {
			if (Status == DeviceHookStatus.Uninstalled && !Disposing) {
				throw new SoftwareException("Keyboard hook has not installed. Current Status is '{0}'", Status);
			}
			if (_hKeyboardHook != IntPtr.Zero) {
				if (!WinAPI.USER32.UnhookWindowsHookEx(_hKeyboardHook) && !Disposing) {
					throw new WindowsException(WinAPI.KERNEL32.GetLastError(),
						"Windows did not allow the application to uninstall keyboard hook.");
				}
			}
			_hKeyboardHook = IntPtr.Zero;
			_keyboardHookProcedure = null;
			Status = DeviceHookStatus.Uninstalled;
		}
	}

	private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
		var keyboardMessage = (WinAPI.USER32.WM)wParam;
		bool intercept = false;
		if (Status == DeviceHookStatus.Active) {
			ThreadPriority oldPriority = Thread.CurrentThread.Priority;
			try {
				Thread.CurrentThread.Priority = ThreadPriority.Highest;
				// it was ok and someone listens to events
				if (nCode >= 0) {
					var keyData = (WinAPI.USER32.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(WinAPI.USER32.KeyboardHookStruct));

					// Determine key 
					var virtualKey = (VirtualKey)keyData.vkCode;
					var key = Tools.WinTool.VirtualKeyToKey(virtualKey);
					var scanCode = (ScanCode)keyData.scanCode;
					var asciiChar = DetermineASCIIKeyPressed(keyData);

					// Determine key state
					var keyState = KeyState.Unknown;
					if (keyboardMessage == WinAPI.USER32.WM.KEYDOWN || keyboardMessage == WinAPI.USER32.WM.SYSKEYDOWN) {
						keyState = KeyState.Down;
					} else if (keyboardMessage == WinAPI.USER32.WM.KEYUP || keyboardMessage == WinAPI.USER32.WM.SYSKEYUP) {
						keyState = KeyState.Up;
					}

					intercept = ShouldIntercept?.Invoke(key, keyState) ?? false;

					// Base method will process rest of logic
					base.ContinueOutsideCriticalExecutionContext(
						() =>
							base.ProcessKeyEvent(
								asciiChar,
								key,
								scanCode,
								keyState,
								virtualKey.HasFlag(VirtualKey.Shift),
								virtualKey.HasFlag(VirtualKey.Control),
								virtualKey.HasFlag(VirtualKey.Alt)
							)
					);
				}
			} finally {
				Thread.CurrentThread.Priority = oldPriority;
			}
		}

		if (intercept) {
			return new IntPtr(-1);
		}
		return WinAPI.USER32.CallNextHookEx(_hKeyboardHook, nCode, wParam, lParam);
	}


	private char DetermineASCIIKeyPressed(WinAPI.USER32.KeyboardHookStruct keyHookStruct) {
		var retval = KeyEvent.UnknownASCIICharacter;
		var keyState = new byte[256];
		WinAPI.USER32.GetKeyboardState(keyState);
		var inBuffer = new byte[2];
		if (WinAPI.USER32.ToAscii(
			    keyHookStruct.vkCode,
			    keyHookStruct.scanCode,
			    keyState,
			    inBuffer,
			    keyHookStruct.flags
		    ) == 1) {
			retval = (char)inBuffer[0];
		}
		return retval;
	}

	public ISite Site { get; set; }
}
