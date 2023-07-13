// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class EnterTextDialog : Form {
	public EnterTextDialog() : this(null) {
	}

	public EnterTextDialog(string prefill) {
		InitializeComponent();
		if (prefill != null)
			_textBox.Text = prefill;
	}

	public string Instructions {
		get { return _userInstructionLabel.Text; }
		set { _userInstructionLabel.Text = value; }
	}

	public string UserInput { get; set; }

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);
		UserInput = _textBox.Text;
	}

	public static bool Show(IWin32Window owner, string title, string text, out string userInput, string prefill = null) {
		var form = new EnterTextDialog(prefill) {
			Text = title,
			Instructions = text,
		};
		var result = form.ShowDialog();
		userInput = form.UserInput;
		return result == DialogResult.OK;
	}
}
