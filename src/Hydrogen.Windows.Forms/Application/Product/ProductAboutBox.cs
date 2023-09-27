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

public partial class ProductAboutBox : ApplicationForm, IAboutBox {

	public ProductAboutBox() {
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		if (!Tools.Runtime.IsDesignMode) {
			Text = StringFormatter.FormatEx(this.Text);
			_label1.Text = StringFormatter.FormatEx(_label1.Text);
			_label4.Text = StringFormatter.FormatEx(_label4.Text);
			_label2.Text = StringFormatter.FormatEx(_label2.Text);
			_label3.Text = StringFormatter.FormatEx(_label3.Text);
			_link1.Text = StringFormatter.FormatEx(_link1.Text).TrimStart("https://").TrimStart("http://");
			_label9.Text = StringFormatter.FormatEx(_label9.Text);
			_label10.Text = StringFormatter.FormatEx(_label10.Text);
			_label11.Text = StringFormatter.FormatEx(_label11.Text);
			_label12.Text = StringFormatter.FormatEx(_label12.Text);
			_companyNumberLabel.Text = StringFormatter.FormatEx(_companyNumberLabel.Text);
		}
	}

	private void _productLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
		var url = StringFormatter.FormatEx("{CompanyUrl}");
		var websiteLauncher = HydrogenFramework.Instance.ServiceProvider.GetService<IWebsiteLauncher>();
		websiteLauncher.LaunchWebsite(url);
	}

}
