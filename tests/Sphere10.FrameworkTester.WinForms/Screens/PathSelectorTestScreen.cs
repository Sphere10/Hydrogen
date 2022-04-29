//-----------------------------------------------------------------------
// <copyright file="PathSelectorTestForm.cs" company="Sphere 10 Software">
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
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
    public partial class PathSelectorTestScreen : ApplicationScreen {
        public PathSelectorTestScreen() {
            InitializeComponent();
        }

        private void pathSelectorControl1_PathChanged() {
            try {
                DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl1.Path, "OK");
            } catch (Exception error) {
                ExceptionDialog.Show(this, error);
            }
        }

        private void pathSelectorControl2_PathChanged() {
            try {
                DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl2.Path, "OK");
            } catch (Exception error) {
                ExceptionDialog.Show(this, error);
            }
        }

        private void pathSelectorControl3_PathChanged() {
            try {
                DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl3.Path, "OK");
            } catch (Exception error) {
                ExceptionDialog.Show(this, error);
            }
        }

        private void pathSelectorControl4_PathChanged() {
            try {
                DialogEx.Show(this, SystemIconType.None, "Result", pathSelectorControl4.Path, "OK");
            } catch (Exception error) {
                ExceptionDialog.Show(this, error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            pathSelectorControl1.ForcePathExists =
                pathSelectorControl2.ForcePathExists =
                    pathSelectorControl3.ForcePathExists =
                        pathSelectorControl4.ForcePathExists = checkBox1.Checked;
        }

    }
}
