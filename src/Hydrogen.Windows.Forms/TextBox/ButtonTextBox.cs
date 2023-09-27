// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class ButtonTextBox : TextBox {
	private readonly Button _button;

	public event EventHandler ButtonClick {
		add { _button.Click += value; }
		remove { _button.Click -= value; }
	}

	public ButtonTextBox() {
		_button = new Button { Cursor = Cursors.Default };
		Controls.Add(_button);
		_button.SizeChanged += (o, e) => OnResize(e);
		FitButton();
	}

	public Button Button {
		get { return _button; }
	}

	protected override void OnResize(EventArgs e) {
		base.OnResize(e);
		FitButton();
	}

	private void FitButton() {
		_button.Size = new Size(_button.Width, this.ClientSize.Height + 2);
		_button.Location = new Point(this.ClientSize.Width - _button.Width, -1);
		// Send EM_SETMARGINS to prevent text from disappearing underneath the button
		WinAPI.USER32.SendMessage(this.Handle, WinAPI.WindowMessageFlags.EM_SETMARGINS, (IntPtr)2, (IntPtr)(_button.Width << 16));
	}

}
