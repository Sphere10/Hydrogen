//-----------------------------------------------------------------------
// <copyright file="ConnectionBarTestForm.cs" company="Sphere 10 Software">
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
using Hydrogen.Data;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
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
                    result.Success ? "Success" : result.ErrorMessages.ToParagraphCase(), "Connection Test", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
        }
	}
}
