// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class PathSelectorTestScreen : ApplicationScreen {
	public PathSelectorTestScreen() {
		InitializeComponent();
	}

	private void pathSelectorControl1_PathChanged() {
		try {
			DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl1.Path, "OK");
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void pathSelectorControl2_PathChanged() {
		try {
			DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl2.Path, "OK");
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void pathSelectorControl3_PathChanged() {
		try {
			DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl3.Path, "OK");
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void pathSelectorControl4_PathChanged() {
		try {
			DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl4.Path, "OK");
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e) {
		pathSelectorControl1.ForcePathExists =
			pathSelectorControl2.ForcePathExists =
				pathSelectorControl3.ForcePathExists =
					pathSelectorControl4.ForcePathExists = checkBox1.Checked;
	}

}
