//-----------------------------------------------------------------------
// <copyright file="ApplicationServicesTester.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
	public partial class ApplicationServicesTestScreen : ApplicationScreen {
		public ApplicationServicesTestScreen() {
			InitializeComponent();
		}

		private void _callTestMethodButton_Click(object sender, EventArgs e) {
			try {
				throw new NotImplementedException();
				// HS: 2019-02-24 disabled due to .net standard upgrade
				//var services = new SoftwareService2WebServiceClient();
				//services.Test();
			} catch (Exception error) {
				MessageBox.Show(this, error.ToDisplayString());
			}
		}
	}
}
