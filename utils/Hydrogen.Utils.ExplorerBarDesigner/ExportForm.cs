//-----------------------------------------------------------------------
// <copyright file="ExportForm.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Utils.ExplorerBarDesigner {
    public partial class ExportForm : Form {
        public ExportForm() {
            InitializeComponent();
        }

        public static void ShowDialog(IWin32Window parent, string text) {
            ExportForm form = new ExportForm();
            form.textBox1.Text = text;
            form.ShowDialog(parent);
        }
    }
}
