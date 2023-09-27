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
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class EnumComboScreen : ApplicationScreen {
	public EnumComboScreen() {
		InitializeComponent();
		_enumComboBox.EnumType = typeof(TestEnum);
	}


	[Description("TEST!!!")]
	public enum TestEnum {
		[Description("With Description!")] WithDescription,

		WithoutDescription
	}


	private void _setNullButton_Click(object sender, EventArgs e) {
		_enumComboBox.SelectedEnum = null;
	}

	private void _setEnumVal1Button_Click(object sender, EventArgs e) {
		_enumComboBox.SelectedEnum = TestEnum.WithDescription;
	}

	private void _setEnumValue2Button_Click(object sender, EventArgs e) {
		_enumComboBox.SelectedEnum = TestEnum.WithoutDescription;
	}

	private void _getValueButton_Click(object sender, EventArgs e) {
		var selectedValue = _enumComboBox.SelectedEnum;
		MessageBox.Show(this, selectedValue != null ? selectedValue.ToString() : "NULL");
	}
}
