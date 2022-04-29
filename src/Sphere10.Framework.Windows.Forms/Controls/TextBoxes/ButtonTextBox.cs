//-----------------------------------------------------------------------
// <copyright file="ButtonTextBox.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework.Windows;

namespace Sphere10.Framework.Windows.Forms {

	public class ButtonTextBox : TextBox {
		private readonly Button _button;

		public event EventHandler ButtonClick { add { _button.Click += value; } remove { _button.Click -= value; } }

		public ButtonTextBox() {
			_button = new Button {Cursor = Cursors.Default};
			Controls.Add(_button);	
			_button.SizeChanged += (o, e) => OnResize(e);
			FitButton();
		}

		public Button Button {
			get {
				return _button;
			}
		}
		
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			FitButton();
		}

		private void FitButton() {
			_button.Size = new Size(_button.Width, this.ClientSize.Height + 2);
			_button.Location = new Point(this.ClientSize.Width - _button.Width, -1);
			// Send EM_SETMARGINS to prevent text from disappearing underneath the button
		    WinAPI.USER32.SendMessage(this.Handle, WinAPI.WindowMessageFlags.EM_SETMARGINS, (IntPtr) 2, (IntPtr) (_button.Width << 16));
		}

	}
}
