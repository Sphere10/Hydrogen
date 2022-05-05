//-----------------------------------------------------------------------
// <copyright file="CBACSVConverterForm.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.WinFormsTester
{
    public partial class CBACSVConverterScreen : ApplicationScreen {
        public CBACSVConverterScreen()
        {
            InitializeComponent();
        }

        private void _convertButton_Click(object sender, EventArgs e)
        {
            _outputTextBox.Text = (
                from line in _CBACSVTextBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                let tokens = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                let token1 = tokens[0]
                let token2 = tokens[1]
                let token3 = tokens[2]
                let token4 = tokens[3]
                let dateparts = token1.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
                let day = int.Parse(dateparts[0])
                let month = int.Parse(dateparts[1])
                let year = int.Parse(dateparts[2])
                select string.Format("{0:0000}-{1:00}-{2:00},{3},{4},{5}", year, month, day, token2, token3, token4)
            ).ToDelimittedString(Environment.NewLine);
        }
    }
}
