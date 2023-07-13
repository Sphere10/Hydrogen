// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;

namespace Hydrogen.Utils.ExplorerBarDesigner;

public partial class ExportForm : Form {
	public ExportForm() {
		InitializeComponent();
	}

	public static void ShowDialog(IWin32Window parent, string text) {
		ExportForm form = new ExportForm();
		form.textBox1.Text = text;
		form.ShowDialog(parent);
	}
}
