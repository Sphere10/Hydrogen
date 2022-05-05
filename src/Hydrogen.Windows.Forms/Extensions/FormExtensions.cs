//-----------------------------------------------------------------------
// <copyright file="FormExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using Hydrogen.Windows;


namespace Hydrogen {

	public static class FormExtensions {

		public static void ShowDialog<T>(this Form parentForm) where T : Form, new() {
			parentForm.InvokeEx(
				() => {
					T form = new T();
					if (parentForm.WindowState == FormWindowState.Minimized) {
						form.StartPosition = FormStartPosition.CenterScreen;
					}
					form.ShowDialog(parentForm);
				}
			);
		}

		public static void ShowInactiveTopmost(this Form frm) {
			WinAPI.USER32.ShowWindow(frm.Handle, WinAPI.USER32.ShowWindowCommands.ShowNoActivate);
            WinAPI.USER32.SetWindowPos(frm.Handle, WinAPI.USER32.HWND_TOPMOST, frm.Left, frm.Top, frm.Width, frm.Height, WinAPI.USER32.SetWindowPosFlags.SWP_NOACTIVATE);
		}


	}
}
