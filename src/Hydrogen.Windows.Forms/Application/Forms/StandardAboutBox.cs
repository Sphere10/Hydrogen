//-----------------------------------------------------------------------
// <copyright file="StandardAboutBox.cs" company="Sphere 10 Software">
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
using System.Reflection;
using System.Windows.Forms;
using Hydrogen.Application;
using Hydrogen;

namespace Hydrogen.Windows.Forms {

    public partial class StandardAboutBox : ApplicationForm, IAboutBox {

        public StandardAboutBox()  {
            InitializeComponent();
        }

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (!Tools.Runtime.IsDesignMode) {
				Text = WinFormsApplicationServices.ProcessString(this.Text);
				_label1.Text = WinFormsApplicationServices.ProcessString(_label1.Text);
				_label4.Text = WinFormsApplicationServices.ProcessString(_label4.Text);
				_label2.Text = WinFormsApplicationServices.ProcessString(_label2.Text);
				_label3.Text = WinFormsApplicationServices.ProcessString(_label3.Text);
				_link1.Text = WinFormsApplicationServices.ProcessString(_link1.Text);
				_label9.Text = WinFormsApplicationServices.ProcessString(_label9.Text);
				_label10.Text = WinFormsApplicationServices.ProcessString(_label10.Text);
				_label11.Text = WinFormsApplicationServices.ProcessString(_label11.Text);
				_label12.Text = WinFormsApplicationServices.ProcessString(_label12.Text);
                _companyNumberLabel.Text = WinFormsApplicationServices.ProcessString(_companyNumberLabel.Text);
			}
		}

        private void _productLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			WinFormsApplicationServices.LaunchWebsite(_link1.Text);
        }

        private void _enterNewProductKey_Click(object sender, EventArgs e) {
            ProductActivationForm form = new ProductActivationForm();
            form.ShowDialog();
            _productExpirationDetailsControl.RefreshText();
			WinFormsApplicationServices.ApplyLicense(false);
        }



    }
}
