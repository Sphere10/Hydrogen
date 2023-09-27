// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;

namespace Hydrogen.Windows;

public class AttachToCommandPromptScope : IDisposable {

	public AttachToCommandPromptScope() {
		var foregroundWindowHWND = WinAPI.USER32.GetForegroundWindow();
		WinAPI.USER32.GetWindowThreadProcessId(foregroundWindowHWND, out var shellProcessID);
		var shell = Process.GetProcessById((int)shellProcessID);

		if (!shell.ProcessName.ToUpperInvariant().IsIn("CMD", "POWERSHELL", "POWERSHELL_ISE"))
			return;

		// Attach to command shell
		WinAPI.KERNEL32.AttachConsole(shell.Id);

		// Clear the current line of command prompt
		var promptLength = Console.CursorLeft;
		Console.CursorLeft = 0;
		Console.Write(new string(Tools.Array.Gen(promptLength, ' ')));
		Console.CursorLeft = 0;
		Console.CursorTop -= 1;

		IsAttached = true;
		Shell = shell;
	}

	public bool IsAttached { get; }

	public Process Shell { get; }

	public void Dispose() {
		if (IsAttached) {
			WinAPI.KERNEL32.FreeConsole();
			WinAPI.USER32.PostMessage(Shell.MainWindowHandle, (int)WinAPI.USER32.WM.KEYDOWN, (int)VirtualKey.Return, 0);
		}
	}
}
