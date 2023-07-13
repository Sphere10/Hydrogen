// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class BasicContactDetailsControl : UserControl {
	public BasicContactDetailsControl() {
		InitializeComponent();
	}

	private void EnableDisableControls() {
		_emailTextBox.Enabled = _emailButton.Checked;
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ContactIsAnonymous {
		get { return _anonymousButton.Checked; }
		set { _anonymousButton.Checked = value; }
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string ContactEmail {
		get {
			if (ContactIsAnonymous) {
				return "Anonymous";
			} else {
				return _emailTextBox.Text;
			}
		}
		set { _emailTextBox.Text = value; }
	}

	private void _emailButton_CheckedChanged(object sender, EventArgs e) {
		EnableDisableControls();
	}


}
