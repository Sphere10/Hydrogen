//-----------------------------------------------------------------------
// <copyright file="DRMNagDialog.cs" company="Sphere 10 Software">
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
using Microsoft.Extensions.DependencyInjection;


namespace Hydrogen.Windows.Forms;

public partial class DRMNagDialog : ApplicationForm, INagDialog
{

	public DRMNagDialog()
	{
		InitializeComponent();
	}

	public string NagMessage
	{
		get => _expirationControl.Text;
		set => _expirationControl.Text = value;
	}

	protected override void PopulatePrimingData()
	{
		base.PopulatePrimingData();
		SetLicenseMessage();
	}

	private void SetLicenseMessage()
	{
		var productLicenseEnforcer = HydrogenFramework.Instance.ServiceProvider.GetService<IProductLicenseEnforcer>();
		productLicenseEnforcer.CalculateRights(out var nag);
		NagMessage = nag;
	}

	private void ShowActivationForm()
	{
		DRMProductActivationForm form = new DRMProductActivationForm();
		if (form.ShowDialog() == DialogResult.OK)
		{
			Close();
		}
	}

	private void _enterKeyButton_Click(object sender, EventArgs e)
	{
		ShowActivationForm();
	}

	private void _closeButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void _buyNowLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		var websiteLauncher = HydrogenFramework.Instance.ServiceProvider.GetService<IWebsiteLauncher>();
		websiteLauncher.LaunchProductPurchaseWebsite();
	}


}
