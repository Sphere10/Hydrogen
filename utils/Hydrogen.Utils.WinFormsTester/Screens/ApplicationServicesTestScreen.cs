// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

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
