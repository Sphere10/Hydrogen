//-----------------------------------------------------------------------
// <copyright file="DRMProductActivationForm.cs" company="Sphere 10 Software">
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
using Hydrogen.Application;
using Hydrogen.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public sealed partial class DRMProductActivationForm : ApplicationForm {

	public DRMProductActivationForm() {
		InitializeComponent();
	}



	private async void _activateButton_Click(object sender, EventArgs e) {
		try {
			var userInterfaceServices = HydrogenFramework.Instance.ServiceProvider.GetService<IUserInterfaceServices>();
			var productLicenseActivator = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseActivator>();
			var productLicenseProvider = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseProvider>();
			var productLicenseEnforcer = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseEnforcer>();

			using (_loadingCircle.BeginAnimationScope(this, _applicationBanner)) {
				await productLicenseActivator.ActivateLicense(_licenseKeyTextBox.Text);
			}
			userInterfaceServices.ReportInfo(
				"Activation Success",
				"Your software is now activated."
			);

			DialogResult = DialogResult.OK;
			Close();
		} catch (Exception error) {
			var userInterfaceServices = HydrogenFramework.Instance.ServiceProvider.GetService<IUserInterfaceServices>();
			userInterfaceServices.ReportError(
				 "Activation Error",
				 $"Your key is invalid. Please enter correct key. {error.ToDisplayString()}"
			 );
		}
	}

	private void _cancelButton_Click(object sender, EventArgs e) {
		DialogResult = DialogResult.Cancel;
		Close();
	}

}
