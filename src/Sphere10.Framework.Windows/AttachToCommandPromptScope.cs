//-----------------------------------------------------------------------
// <copyright file="SessionEndingHandlerTask.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Sphere10.Framework.Windows;
using Microsoft.Win32;
using System.Diagnostics;

namespace Sphere10.Framework.Windows {

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
                WinAPI.USER32.PostMessage(Shell.MainWindowHandle, (int)WinAPI.USER32.WM.KEYDOWN,(int)VirtualKey.Return, 0);
            }
        }
    }
}


