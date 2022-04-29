//-----------------------------------------------------------------------
// <copyright file="ScreenA.cs" company="Sphere 10 Software">
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
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester
{
    public partial class ScreenA : ApplicationScreen
    {
        public ScreenA() {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            radioButton1.Checked = true;
        }
    }
}

