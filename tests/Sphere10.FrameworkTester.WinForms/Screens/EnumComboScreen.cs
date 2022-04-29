//-----------------------------------------------------------------------
// <copyright file="EnumComboForm.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
	public partial class EnumComboScreen : ApplicationScreen {
		public EnumComboScreen() {
			InitializeComponent();
			_enumComboBox.EnumType = typeof(TestEnum);
		}


		[Description("TEST!!!")]
		public enum TestEnum {
			[Description("With Description!")]
			WithDescription,

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
}
