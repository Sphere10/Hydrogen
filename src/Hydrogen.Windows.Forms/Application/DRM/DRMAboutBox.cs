//-----------------------------------------------------------------------
// <copyright file="ProductAboutBox.cs" company="Sphere 10 Software">
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
using Hydrogen.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public partial class DRMAboutBox : ProductAboutBox {

	public DRMAboutBox() {
		InitializeComponent();
	}

	protected override void PopulatePrimingData() {
		base.PopulatePrimingData();
		SetLicenseMessage();
	}

	private void SetLicenseMessage() {
		var productLicenseEnforcer = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseEnforcer>();
		productLicenseEnforcer.CalculateRights(out var nag);
		_expirationControl.Text = nag;
	}

	private void _changeProductKeyButton_Click(object sender, EventArgs e) {
		try {
			DRMProductActivationForm form = new DRMProductActivationForm();
			form.ShowDialog();
			SetLicenseMessage();
		} catch (Exception error) {
			var uiservices = HydrogenFramework.Instance.ServiceProvider.GetService<IUserInterfaceServices>();
			uiservices.ReportError(error);
		}
	}



}
