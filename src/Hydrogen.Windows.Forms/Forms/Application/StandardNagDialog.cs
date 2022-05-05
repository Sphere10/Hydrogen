//-----------------------------------------------------------------------
// <copyright file="StandardNagDialog.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Windows.Forms;
using System.Reflection;



namespace Hydrogen.Windows.Forms {

    public partial class StandardNagDialog : ApplicationForm, INagDialog {

		public StandardNagDialog() {
            InitializeComponent();
        }

		public override void Refresh() {
			_nagMessageControl.RefreshText();
		}

        private void ShowActivationForm() {
            ProductActivationForm form = new ProductActivationForm();
			if (form.ShowDialog() == DialogResult.OK) {
				Close();
			}
        }

        private void _enterKeyButton_Click(object sender, EventArgs e) {
            ShowActivationForm();
        }

        private void _closeButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void _buyNowLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			ApplicationServices.LaunchProductPurchaseWebsite();
			//if (_buyNowLink.Tag != null && !string.IsNullOrEmpty(_buyNowLink.Tag.ToString())) {
			//    ApplicationServices.LaunchWebsite(_buyNowLink.Tag.ToString());
			//}
        }

		

    }
}
