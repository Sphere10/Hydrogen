// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class CBACSVConverterScreen : ApplicationScreen {
	public CBACSVConverterScreen() {
		InitializeComponent();
	}

	private void _convertButton_Click(object sender, EventArgs e) {
		_outputTextBox.Text = (
			from line in _CBACSVTextBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
			let tokens = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
			let token1 = tokens[0]
			let token2 = tokens[1]
			let token3 = tokens[2]
			let token4 = tokens[3]
			let dateparts = token1.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
			let day = int.Parse(dateparts[0])
			let month = int.Parse(dateparts[1])
			let year = int.Parse(dateparts[2])
			select string.Format("{0:0000}-{1:00}-{2:00},{3},{4},{5}", year, month, day, token2, token3, token4)
		).ToDelimittedString(Environment.NewLine);
	}
}
