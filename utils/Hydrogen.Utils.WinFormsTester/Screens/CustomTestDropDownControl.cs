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

namespace Hydrogen.Utils.WinFormsTester;

public partial class UserControl1 : UserControl {
	public UserControl1() {
		InitializeComponent();
	}

	private void UserControl1_SizeChanged(object sender, EventArgs e) {
		// Calculate size of checked list box.
		checkedListBox1.Size = new Size(DisplayRectangle.Width - checkedListBox1.Left - 5,
			DisplayRectangle.Height - checkedListBox1.Top - button1.Height - 10);
	}
}
