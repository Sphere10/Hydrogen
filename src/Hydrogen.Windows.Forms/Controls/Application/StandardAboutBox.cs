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
using Sphere10.Framework.Application;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {

    public partial class StandardAboutBox : ApplicationForm, IAboutBox {

        public StandardAboutBox()  {
            InitializeComponent();
        }

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (!Tools.Runtime.IsDesignMode) {
				Text = ApplicationServices.ProcessString(this.Text);
				_label1.Text = ApplicationServices.ProcessString(_label1.Text);
				_label4.Text = ApplicationServices.ProcessString(_label4.Text);
				_label2.Text = ApplicationServices.ProcessString(_label2.Text);
				_label3.Text = ApplicationServices.ProcessString(_label3.Text);
				_link1.Text = ApplicationServices.ProcessString(_link1.Text);
				_label9.Text = ApplicationServices.ProcessString(_label9.Text);
				_label10.Text = ApplicationServices.ProcessString(_label10.Text);
				_label11.Text = ApplicationServices.ProcessString(_label11.Text);
				_label12.Text = ApplicationServices.ProcessString(_label12.Text);
                _companyNumberLabel.Text = ApplicationServices.ProcessString(_companyNumberLabel.Text);
			}
		}

        private void _productLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			ApplicationServices.LaunchWebsite(_link1.Text);
        }

        private void _enterNewProductKey_Click(object sender, EventArgs e) {
            ProductActivationForm form = new ProductActivationForm();
            form.ShowDialog();
            _productExpirationDetailsControl.RefreshText();
			ApplicationServices.ApplyLicense(false);
        }



    }
}
