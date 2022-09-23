//-----------------------------------------------------------------------
// <copyright file="ProductActivationForm.cs" company="Sphere 10 Software">
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

    public sealed partial class ProductActivationForm : ApplicationForm {
        private bool _cancelled;

        public ProductActivationForm() : base() {
            InitializeComponent();
            Cancelled = false;
        }

        public bool Cancelled { get; set; }

        private void _activateButton_Click(object sender, EventArgs e) {
			try {
				WinFormsApplicationServices.RegisterLicenseKey(_licenseKeyTextBox.Text);
				WinFormsApplicationServices.ReportInfo(
					"Activation Success",
					"Your software is now activated."
				);
				DialogResult = DialogResult.OK;
				Close();
			} catch {
				WinFormsApplicationServices.ReportError(
					 "Activation Error",
					 "Your key is invalid. Please enter correct key."
				 );
			}
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            Cancelled = true;
            Close();
        }

    }
}
