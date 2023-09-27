// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Application;
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
