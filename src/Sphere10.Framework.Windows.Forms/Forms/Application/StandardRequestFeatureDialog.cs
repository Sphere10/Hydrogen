//-----------------------------------------------------------------------
// <copyright file="StandardRequestFeatureDialog.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {

	public partial class StandardRequestFeatureDialog : ApplicationForm, IRequestFeatureDialog {

		public StandardRequestFeatureDialog() {
			InitializeComponent();
		}

		private void _cancelButton_Click(object sender, EventArgs e) {
			Close();
		}

		private void _sendButton_Click(object sender, EventArgs e) {
			bool close = true;
			try {
				throw new NotImplementedException();
				// HS: 2019-02-24 need updating
				//ISphere10SoftwareService2 softwareService = ComponentRegistry.Instance.Resolve<ISphere10SoftwareService2>();
				//string email = _basicContactDetailsControl.ContactEmail;
				//if (email.ToUpper() != "ANONYMOUS" && !Tools.Mail.IsValidEmail(email)) {
				//	ApplicationServices.ReportError("Malformed Email", "Please enter a correct email");
				//	close = false;
				//	return;
				//}
				//softwareService.RequestFeature(
				//	new FeatureRequest(
				//		_whoAreYouControl.UserType,
				//		email,
				//		ApplicationServices.ProductInformation,
				//		_featureTextBox.Text
				//	)
				//);
			} catch (Exception error) {
				ApplicationServices.ReportError(error);
			} finally {
				if (close) {
					Close();
				}
			}
		}
	}
}
