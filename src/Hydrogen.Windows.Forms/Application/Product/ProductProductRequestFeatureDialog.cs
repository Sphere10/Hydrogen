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

public partial class ProductProductRequestFeatureDialog : ApplicationForm, IProductRequestFeatureDialog {

	public ProductProductRequestFeatureDialog() {
		InitializeComponent();
		UserInterfaceServices = HydrogenFramework.Instance.ServiceProvider.GetService<IUserInterfaceServices>();
	}

	protected IUserInterfaceServices UserInterfaceServices { get; }

	private void _cancelButton_Click(object sender, EventArgs e) {
		Close();
	}

	private void _sendButton_Click(object sender, EventArgs e) {
		bool close = true;
		try {
			throw new NotImplementedException();
			// HS: 2019-02-24 need updating
			//ISphere10SoftwareService2 softwareService = HydrogenFramework.Instance.ServiceProvider.GetService<ISphere10SoftwareService2>();
			//string email = _basicContactDetailsControl.ContactEmail;
			//if (email.ToUpper() != "ANONYMOUS" && !Tools.Mail.IsValidEmail(email)) {
			//	WinFormsWinFormsApplicationProvider.ReportError("Malformed Email", "Please enter a correct email");
			//	close = false;
			//	return;
			//}
			//softwareService.RequestFeature(
			//	new FeatureRequest(
			//		_whoAreYouControl.UserType,
			//		email,
			//		WinFormsWinFormsApplicationProvider.ProductInformation,
			//		_featureTextBox.Text
			//	)
			//);
		} catch (Exception error) {
			UserInterfaceServices.ReportError(error);
		} finally {
			if (close) {
				Close();
			}
		}
	}
}
