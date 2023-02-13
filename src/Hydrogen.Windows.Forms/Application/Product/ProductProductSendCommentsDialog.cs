//-----------------------------------------------------------------------
// <copyright file="ProductProductSendCommentsDialog.cs" company="Sphere 10 Software">
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
using System.Threading;
using Hydrogen;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms {

	public partial class ProductProductSendCommentsDialog : ApplicationForm, IProductSendCommentsDialog {

        public ProductProductSendCommentsDialog() {
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
				//ISphere10SoftwareService2 webService = HydrogenFramework.Instance.ServiceProvider.GetService<ISphere10SoftwareService2>();
				//            string email = _basicContactDetailsControl.ContactEmail;
				//            if (email.ToUpper() != "ANONYMOUS" && !Tools.Mail.IsValidEmail(email)) {
				//	WinFormsWinFormsApplicationProvider.ReportError("Malformed Email", "Please enter a correct email");
				//                close = false;
				//                return;
				//            }
				//            webService.SendComment(
				//	new ProductComment(
				//		_whoAreYouControl.UserType,
				//		email,
				//		WinFormsWinFormsApplicationProvider.ProductInformation,
				//		_commentTextBox.Text
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
}
