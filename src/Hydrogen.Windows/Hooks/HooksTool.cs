// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Windows;

public static class HooksTool {

	internal const int MaxExceptionsBeforeAbort = 20;

	public static void SimulateKeyboardDeviceAction(ScanCode scanCode, KeyState keyState) {
		var inputs = new WinAPI.USER32.INPUT[] {
			new WinAPI.USER32.INPUT {
				type = WinAPI.USER32.INPUT_TYPE.INPUT_KEYBOARD,
				di = new WinAPI.USER32.DEVICEINPUTUNION {
					ki = {
						wScan = scanCode,
						dwFlags = WinAPI.USER32.KEYEVENTF.SCANCODE | (keyState == KeyState.Up ? WinAPI.USER32.KEYEVENTF.KEYUP : 0)
					}
				}
			}
		};

		if (WinAPI.USER32.SendInput((uint)inputs.Length, inputs, WinAPI.USER32.INPUT.Size) == 0) {
			throw new Exception("Failed to emulate keyboard event");
		}

	}

	public static void SimulateMouseDeviceAction(MouseButton button, MouseButtonState state, int x, int y) {
		var inputs = new List<WinAPI.USER32.INPUT>();


		if (button == MouseButton.None) {
			inputs.Add(
				new WinAPI.USER32.INPUT {
					type = WinAPI.USER32.INPUT_TYPE.INPUT_MOUSE,
					di = new WinAPI.USER32.DEVICEINPUTUNION {
						mi = {
							dx = x,
							dy = y,
							dwFlags = WinAPI.USER32.MOUSEEVENTF.MOVE_NOCOALESCE
						}
					}
				}
			);
		} else {

			WinAPI.USER32.MOUSEEVENTF buttonFlag;
			switch (button) {
				case MouseButton.Left:
					buttonFlag = state == MouseButtonState.Down ? WinAPI.USER32.MOUSEEVENTF.LEFTDOWN : WinAPI.USER32.MOUSEEVENTF.LEFTUP;
					break;
				case MouseButton.Middle:
					buttonFlag = state == MouseButtonState.Down ? WinAPI.USER32.MOUSEEVENTF.MIDDLEDOWN : WinAPI.USER32.MOUSEEVENTF.MIDDLEUP;
					break;
				case MouseButton.Right:
					buttonFlag = state == MouseButtonState.Down ? WinAPI.USER32.MOUSEEVENTF.RIGHTDOWN : WinAPI.USER32.MOUSEEVENTF.RIGHTUP;
					break;
				default:
					buttonFlag = 0;
					break;
			}

			inputs.Add(
				new WinAPI.USER32.INPUT {
					type = WinAPI.USER32.INPUT_TYPE.INPUT_MOUSE,
					di = new WinAPI.USER32.DEVICEINPUTUNION {
						mi = {
							dx = x,
							dy = y,
							dwFlags = WinAPI.USER32.MOUSEEVENTF.ABSOLUTE | buttonFlag
						}
					}
				}
			);
		}


		if (WinAPI.USER32.SendInput((uint)inputs.Count, inputs.ToArray(), WinAPI.USER32.INPUT.Size) == 0) {
			throw new Exception("Failed to emulate mouse event");
		}
	}

}
