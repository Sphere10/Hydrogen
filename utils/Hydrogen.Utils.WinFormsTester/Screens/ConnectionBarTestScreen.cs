// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;
using Hydrogen.Data;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ConnectionBarTestScreen : ApplicationScreen {
	public ConnectionBarTestScreen() {
		InitializeComponent();
	}

	protected override void InitializeUIPrimingData() {
		_databaseConnectionBar.SelectedDBMSType = DBMSType.SQLServer;
	}

	private async void _testConnectionButton_Click(object sender, EventArgs e) {

		using (_loadingCircle.BeginAnimationScope()) {

			var result = await _databaseConnectionBar.TestConnection();
			_loadingCircle.StopAnimating();
			MessageBox.Show(this,
				result.IsSuccess ? "Success" : result.ErrorMessages.ToParagraphCase(),
				"Connection Test",
				MessageBoxButtons.OK,
				result.IsSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Error);
		}
	}
}
