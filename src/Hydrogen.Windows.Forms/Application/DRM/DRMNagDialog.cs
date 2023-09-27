// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;


namespace Hydrogen.Windows.Forms;

public partial class DRMNagDialog : ApplicationForm, INagDialog {

	public DRMNagDialog() {
		InitializeComponent();
	}

	public string NagMessage {
		get => _expirationControl.Text;
		set => _expirationControl.Text = value;
	}

	protected override void PopulatePrimingData() {
		base.PopulatePrimingData();
		SetLicenseMessage();
	}

	private void SetLicenseMessage() {
		var productLicenseEnforcer = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseEnforcer>();
		productLicenseEnforcer.CalculateRights(out var nag);
		NagMessage = nag;
	}

	private void ShowActivationForm() {
		DRMProductActivationForm form = new DRMProductActivationForm();
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
		var websiteLauncher = HydrogenFramework.Instance.ServiceProvider.GetService<IWebsiteLauncher>();
		websiteLauncher.LaunchProductPurchaseWebsite();
	}


}
