//-----------------------------------------------------------------------
// <copyright file="UrlIDTestForm.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Misc;
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
    public partial class UrlIDTestScreen : ApplicationScreen {
        public UrlIDTestScreen() {
            InitializeComponent();
        }

        private async void _generateButton_Click(object sender, EventArgs e) {
            try {
                var writer = new TextBoxWriter(_textBoxEx);
                for (uint x = 1; x < 1000; x += 1) {
                    await writer.WriteLineAsync(UrlID.Generate(x));
                }

            } catch (Exception error) {
                ExceptionDialog.Show(error);
            }
        }
    }
}
